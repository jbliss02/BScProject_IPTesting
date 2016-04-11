﻿using System;
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
        private int pixelJumpPerFrameJump; //set from the regulation formula. pixels jumped up to this value, then a frame is jumped
        private int backlogSpeedup; //when this number is exceeded everything is sped up, to decrease the backlog
        private int backlogSlowdown; //when this number is higher than backlog everything is slowed down, increasing accuracy

        //threshold setting
        public int ControlImageNumber { get; set; } //number of changes to use as the control (half the images as done in pairs)
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
            ControlImageNumber = ConfigurationManager.AppSettings["imagesInThreshold"].StringToInt() / 2;

            //backlog monitoring
            backlogCheckMs = ConfigurationManager.AppSettings["backlogCheckMs"].ToString().StringToInt();
            backlogSpeedup = ConfigurationManager.AppSettings["backlogSpeedup"].ToString().StringToInt();
            backlogSlowdown = ConfigurationManager.AppSettings["backlogSlowdown"].ToString().StringToInt();
            backlogTimer = new Stopwatch();
            backlog = new List<int>();
            SetRegulationParameters();
        }

        /// <summary>
        /// Extracts the regulation formula from config files and sets the pixelJumpPerFrameJump variable
        /// this drives the logic on what metric to increase / decrease when changing speed
        /// </summary>
        private void SetRegulationParameters()
        {
            string[] split = Regex.Split(ConfigurationManager.AppSettings["regulationFormula"], ":");

            if(split.Length != 2)
            {
                throw new Exception("regulationFormula was not in expected format");
            }

            int framesToSkip;
            int pixelsToSkip;

            if(split[0].Substring(split[0].Length - 2).ToUpper() == "P")
            {
                pixelsToSkip = split[0].Substring(0, split[0].Length - 2).StringToInt();
                framesToSkip = split[1].Substring(0, split[1].Length - 2).StringToInt();
            }
            else
            {
                framesToSkip = split[0].Substring(0, split[0].Length - 1).StringToInt();
                pixelsToSkip = split[1].Substring(0, split[1].Length - 1).StringToInt();
            }

            if(framesToSkip <= 0 || pixelsToSkip <= 0)
            {
                throw new Exception("regulationFormula was not in expected format");
            }

            pixelJumpPerFrameJump =  framesToSkip / pixelsToSkip;

        }//SetRegulationParameters

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

                LogRegulationSettings(backlogCount);

                if (backlogCount > backlogSpeedup) { Speedup(); }
                else if(backlogCount < backlogSlowdown) { Slowdown(); }  
                backlogTimer.Restart();

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
                if(settings.horizontalPixelsToSkip > 0 && settings.horizontalPixelsToSkip % pixelJumpPerFrameJump == 0)
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
                if (settings.horizontalPixelsToSkip > 0 && settings.horizontalPixelsToSkip % pixelJumpPerFrameJump == 0)
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
        private void LogRegulationSettings(int backlogCount)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(ConfigurationManager.AppSettings["regulationLogFile"], true))
            {
                file.WriteLine(backlogCount + "¬" + settings.framesToSkip + "¬" + settings.horizontalPixelsToSkip + "¬" + settings.verticalPixelsToSkip);
            }
        }

        public void Write(string st)
        {
            Console.WriteLine(DateTime.Now + " - " + st);
        }

    }
}
