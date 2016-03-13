using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
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
    public class MotionSensor_2
    {
        //Work queue
        public Queue<ByteWrapper> WorkQueue { get; set; } //images waiting to be processed
        object objLock = new object();
        int numberSkipped; //the number of frames that have been skipped in the current sequence

        //motion detected event
        public event MotionDetected motionDetected;
        public delegate void MotionDetected(ByteWrapper image, EventArgs e);

        //Backlog monitoring
        private int lastImageReceived;
        private List<int> backlog; //images received v images processed
        object backlogLock = new object(); //locks the backlog list so two processes cannot change at the same time
        private Stopwatch backlogTimer; //times when next to check the backlog
        private int backlogCheckMs; //check the backlog every so many milliseconds

        //threshold setting
        public int ControlImageNumber { get; set; } = 10; //number of changes to use as the control (half the images as done in pairs)
        public bool ThresholdSet { get; set; }

        public MotionSensorSettings settings { get; set; }

        public Logs.Logging logging;

        /// <summary>
        /// comparision object - returned from one capture, to be used in the next
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
            backlogCheckMs = ConfigurationManager.AppSettings["BacklogCheckMs"].ToString().StringToInt();
            backlogTimer = new Stopwatch();
            backlog = new List<int>();
         //   comparisionProcessed += new ComparisionProcessed(ComparisionProcessedEvent); //used for the backlog checking
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
            lastImageReceived = img.sequenceNumber;
            logging.imagesReceived++;

            AddToWorkQueue(img); //do this outside the anonymous method
            
            await Task.Run(() =>
            {                
                SendForCompareAsync(); 
            });         
        }

        public void ImageCreated(ByteWrapper img, EventArgs e)
        {
            lastImageReceived = img.sequenceNumber;
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
                lock (backlogLock)
                {
                    backlog.Add(lastImageReceived - image1.sequenceNumber);
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
                //take images out of the queue, as this is async other methods may dequeue between the calls so be defensive
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

        public virtual void Compare(ByteWrapper img1, ByteWrapper img2) { } //will always be implemented in the sub class

        /// <summary>
        /// Monitor's the backlog by comparing the number of received and processed images
        /// Sends requests to speed up or slow dow as appropriate
        /// </summary>
        private void MonitorWork()
        {
            if(!backlogTimer.IsRunning)
            {
                backlogTimer.Start();
            }
            else if(backlogTimer.Elapsed.TotalMilliseconds > backlogCheckMs)
            {
                backlogTimer.Stop();

                int backlogCount = 0;

                //lock the backlog collection whilst amending
                lock(backlogLock)
                {
                    if (backlog.Count > 1)
                    {
                        var backlogCopy = backlog;
                        backlogCount = (int)backlogCopy.Average();
                        backlog.Clear();

                    }
                }

                if (backlogCount > 50) { Speedup(); }
               // Write("Backlog was " + backlogCount);      
                backlogTimer.Restart();
           
            }
        }//MonitorWork

        /// <summary>
        /// Speeds the comparision process up by changing the settings
        /// </summary>
        private void Speedup()
        {
         //   Write("!!!!!!!!!!!!!!!!!!!! SPEEDING UP !!!!!!!!!!!!!!!");
          //  settings.framesToSkip = 10;
        }

        public void Write(string st)
        {
            Console.WriteLine(DateTime.Now + " - " + st);
        }

    }
}
