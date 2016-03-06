using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageAnalysis.Images;
using ImageAnalysis.Analysis;
using ImageAnalysis.Images.Bitmaps;
using ImageAnalysis.Images.Jpeg;

namespace ImageAnalysis.MotionSensor
{
    /// <summary>
    /// Base class that provides basic functionality to serve Motion Sensor approaches in category 2 (pixel analysis)
    /// </summary>
    public class MotionSensor_2
    {
        //Work queue
        public Queue<ByteWrapper> WorkQueue { get; set; } //images waiting to be processed
        private DateTime lastReceived;
        private DateTime lastProcessed;
        object objLock = new object();
        int numberSkipped; //the number of frames that have been skipped in the current sequence

        //motion detected event
        public event MotionDetected motionDetected;
        public delegate void MotionDetected(ByteWrapper image, EventArgs e);

        ////image analysed event (used to manage workflow)
        public event ComparisionProcessed comparisionProcessed;
        public delegate void ComparisionProcessed(int sequenceNumber, EventArgs e);

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
            settings = new MotionSensorSettings();
            comparisionProcessed += new ComparisionProcessed(ComparisionProcessedEvent);


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
            await Task.Run(() =>
            {
                Console.WriteLine("RECEIVED " + img.sequenceNumber);
                logging.imagesReceived++;
                AddToWorkQueue(img);
                SendForCompareAsync(); 
            });

        }

        public void ImageCreated(ByteWrapper img, EventArgs e)
        {
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
            lastReceived = DateTime.Now;
            if (settings.framesToSkip > 0)
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
        /// Gets the next two images on the queue and sends for analysis
        /// </summary>
        private async void SendForCompareAsync()
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
                Console.WriteLine("PROCESSED " + image1.sequenceNumber);
                //lastProcessed = DateTime.Now;
                //await Task.Run(() =>
                //{
                //    MonitorWork();
                //});
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

        protected virtual void OnComparisionProcessed(int sequenceNumber, EventArgs e)
        {
            ComparisionProcessed handler = comparisionProcessed;
            if (handler != null)
            {
                handler(sequenceNumber, e);
            }
        }

        private void ComparisionProcessedEvent(int sequenceNumber, EventArgs e)
        {
            Console.WriteLine("Processed " + sequenceNumber);
        }

        private async void MonitorWork()
        {
            await Task.Run(() =>
            {
                TimeSpan sp = lastProcessed - lastReceived;
                 
                Console.WriteLine("TIMESPAN IS " + sp.TotalSeconds);
            });
        }


    }
}
