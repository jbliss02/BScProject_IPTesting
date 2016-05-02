using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using ImageAnalysis.Images;
using ImageAnalysis.Analysis;
using ImageAnalysis.Images.Bitmaps;
using ImageAnalysis.Images.Jpeg;
using Tools;

namespace ImageAnalysis.MotionSensor
{
    /// <summary>
    /// Base class that provides basic functionality to serve Motion Sensor approaches in category 2 (pixel analysis)
    /// </summary>
    public class MotionSensor_2 : IMotionSensor
    {
        //Work queue
        public Queue<ByteWrapper> WorkQueue { get; set; } //images waiting to be processed
        object objLock = new object();
        int numberSkipped; //the number of frames that have been skipped in the current sequence

        //motion detected event
        public event MotionDetected motionDetected;
        public delegate void MotionDetected(ByteWrapper image, EventArgs e);
       
        MotionSensorBacklog backlog; //Backlog monitoring

        //threshold setting
        public int ControlImageNumber { get; set; } //number of changes to use as the control (half the images as done in pairs)
        public bool ThresholdSet { get; set; }

        public MotionSensorSettings settings { get; set; }

        public Logs.Logging logging;

        /// <summary>
        /// comparision object - returned from one capture, to be used in the nextD:\bsc\project\IPConnect_Testing\ImageAnalysis\MotionSensor\IMotionSensor_2.cs
        /// </summary>
        public List<PixelColumn> Comparison;

        public MotionSensor_2()
        {
            SetUp();
        }

        private void SetUp()
        {
            WorkQueue = new Queue<ByteWrapper>();
            logging = new Logs.Logging();
            ThresholdSet = false;
            ControlImageNumber = ConfigurationManager.AppSettings["imagesInThreshold"].StringToInt() / 2;
            backlog = new MotionSensorBacklog();
        }

        /// <summary>
        /// Called when the object detects motion, creates the event
        /// </summary>
        /// <param name="image"></param>
        protected async void OnMotionAsync(ByteWrapper image1, ByteWrapper image2)
        {
            await Task.Run(() =>
            {
                if (motionDetected != null)
                {
                    motionDetected(image1, EventArgs.Empty);
                }
            });
        }

        /// <summary>
        /// Overload for the OnMotion procedure that includes an integer which defines
        /// the grid number in which motion was detected
        /// </summary>
        /// <param name="image1"></param>
        /// <param name="image2"></param>
        /// <param name="motionGrid"></param>
        protected void OnMotion(ByteWrapper image1, ByteWrapper image2, int motionGrid)
        {
            OnMotionAsync(image1, image2);
        }

        public async void ImageCreatedAsync(ByteWrapper img, EventArgs e)
        {
            backlog.lastImageReceived = img.sequenceNumber;
            logging.imagesReceived++;

            AddToWorkQueue(img); //do this outside the anonymous method
            
            await Task.Run(() =>
            {                
                SendForCompareAsync(); 
            });         
        }

        public void ImageCreated(ByteWrapper img, EventArgs e)
        {
            backlog.lastImageReceived = img.sequenceNumber;
            logging.imagesReceived++;
            AddToWorkQueue(img);
            SendForCompare();
        }

        /// <summary>
        /// Adds to the queue, unless frame should be skipped
        /// </summary>
        /// <param name="img"></param>
        private void AddToWorkQueue(ByteWrapper img)
        {
            if (this.settings.framesToSkip > 0)
            {
                if (numberSkipped == settings.framesToSkip)
                {
                    WorkQueue.Enqueue(img);
                    numberSkipped = 0;
                }
                else
                {
                    numberSkipped++;
                }
            }
            else
            {
                WorkQueue.Enqueue(img);
            }
        }

        /// <summary>
        /// Gets the next two images on the queue and sends for analysis, used when
        /// the system is running on an async basis
        /// </summary>
        private void SendForCompareAsync()
        {
            //as there are multiple threads working the queue may have images removed by other threads
            //move this to lock functionality, rather than losing   
            ByteWrapper image1 = null;
            ByteWrapper image2 = null;

            lock (objLock)
            {
                if (WorkQueue.Count > 1)
                {
                    image1 = WorkQueue.Dequeue();
                    image2 = WorkQueue.Dequeue();
                }
            }

            //send for comparision
            if (image1 != null && image2 != null)
            {
                Compare(image1, image2);
                logging.imagesChecked = logging.imagesChecked + 2;
                lock (backlog.backlogLock)
                {
                    backlog.backlog.Add(backlog.lastImageReceived - image1.sequenceNumber);
                }

                MonitorWork();
            }

        }//SendForCompareAsync

        /// <summary>
        /// Syncrohous version. Takes the oldest image, removes from list, and comapres with comparision object
        /// If no comparision object takes oldest image, removes from list, takes next oldest image but doesn't
        /// remove from the list
        /// </summary>
        private void SendForCompare()
        {
            if (WorkQueue.Count > 1)
            {
                ByteWrapper img1 = null;
                if (WorkQueue.Count > 0) { img1 = WorkQueue.Dequeue(); }
                ByteWrapper img2 = null;
                if (WorkQueue.Count > 0) { img2 = WorkQueue.Dequeue(); }

                if (img1 != null && img2 != null)
                {
                    Compare(img1, img2);
                    logging.imagesChecked = logging.imagesChecked + 2;
                }
            }
        }//SendForCompare

        public virtual void Compare(ByteWrapper img1, ByteWrapper img2) { } //will always be implemented in the derived class

        /// <summary>
        /// Monitor's the backlog by comparing the number of received and processed images
        /// Sends requests to speed up or slow dow as appropriate
        /// </summary>
        internal void MonitorWork()
        {
            if (!backlog.backlogTimer.IsRunning)
            {
                backlog.backlogTimer.Start();
            }
            else if (backlog.backlogTimer.Elapsed.TotalMilliseconds > backlog.backlogCheckMs)
            {
                backlog.backlogTimer.Stop();

                int backlogCount = 0;

                //lock the backlog collection whilst amending
                lock (backlog.backlogLock)
                {
                    if (backlog.backlog.Count > 1)
                    {
                        var backlogCopy = backlog.backlog;
                        backlogCount = (int)backlogCopy.Average();
                        backlog.backlog.Clear();

                    }
                }

                LogRegulationSettings(backlogCount);

                if (backlogCount > backlog.backlogSpeedup) { Speedup(); }
                else if (backlogCount < backlog.backlogSlowdown) { Slowdown(); }
                backlog.backlogTimer.Restart();

            }

        }//MonitorWork

        /// <summary>
        /// Speeds the comparision process up by changing the settings
        /// </summary>
        private void Speedup()
        {
            //if horizontal and vertical pixel jumps are not the same then increase the lower value
            if(settings.horizontalPixelsToSkip != settings.verticalPixelsToSkip)
            {
                if(settings.horizontalPixelsToSkip > settings.verticalPixelsToSkip) { settings.verticalPixelsToSkip = settings.horizontalPixelsToSkip; }
                else { settings.horizontalPixelsToSkip = settings.verticalPixelsToSkip; }
            }
            //decide whether to increase pixel jumps, or frames to skip
            else
            {
                if(settings.horizontalPixelsToSkip > 0 && settings.horizontalPixelsToSkip % backlog.pixelJumpPerFrameJump == 0)
                {
                    settings.framesToSkip++;
                }
                else
                {
                    settings.horizontalPixelsToSkip++;
                }
            }

        }//Speedup

        /// <summary>
        /// Slows down the comparision by amending the settings, makes the comparisions take longer, but are more accurate
        /// </summary>
        private void Slowdown()
        {
            //if horizontal and vertical pixel jumps are not the same then increase the lower value
            if (settings.horizontalPixelsToSkip != settings.verticalPixelsToSkip)
            {
                if (settings.horizontalPixelsToSkip < settings.verticalPixelsToSkip) { settings.verticalPixelsToSkip = settings.horizontalPixelsToSkip; }
                else { settings.horizontalPixelsToSkip = settings.verticalPixelsToSkip; }
            }
            //decide whether to decrease pixel jumps, or frames to skip
            else
            {
                if (settings.horizontalPixelsToSkip > 0 && settings.horizontalPixelsToSkip % backlog.pixelJumpPerFrameJump == 0)
                {
                    settings.framesToSkip--;
                }
                else
                {
                    settings.horizontalPixelsToSkip--;
                }
            }

        }//Speedup

        /// <summary>
        /// Writes the regulation settings to a file, for later analysis
        /// </summary>
        /// <param name="backlogCount"></param>
        internal void LogRegulationSettings(int backlogCount)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(ConfigurationManager.AppSettings["regulationLogFile"], true))
            {
                file.WriteLine(backlogCount + "¬" + settings.framesToSkip + "¬" + settings.horizontalPixelsToSkip + "¬" + settings.verticalPixelsToSkip);
            }
        }

    }
}
