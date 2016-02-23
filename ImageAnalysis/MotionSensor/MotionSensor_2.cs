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
        object objLock = new object();

        //motion detected event
        public event MotionDetected motionDetected;
        public delegate void MotionDetected(ByteWrapper image, EventArgs e);

        //threshold setting
        public int ControlImageNumber { get; set; } = 10; //number of changes to use as the control (half the images as done in pairs)
        public bool ThresholdSet { get; set; }
        protected double sensitivity = 1;

        /// <summary>
        /// The number of pixels to search horizontally. Defaults to the image width if not set
        /// </summary>
        public int SearchWidth { get; set; }
        /// <summary>
        /// The number of pixels to search vertically. Defaults to the image height if not set
        /// </summary>
        public int SearchHeight { get; set; }

        public Logs.Logging logging;

        /// <summary>
        /// Whether a compare action should try and share data with the next compare action
        /// only applicable when running syncrohously
        /// </summary>
        public bool LinkCompare;

        /// <summary>
        /// comparision object - returned from one capture, to be used in the next
        /// </summary>
        public List<PixelColumn> Comparison;

        /// <summary>
        /// Whether every image is processed twice (once as the comparator and once as the comparision
        /// or whether groups of 2 images are processed, so each image only gets processed once
        /// </summary>
        public bool SequentialComparision { get; set; }

        public MotionSensor_2()
        {
            SetUp();
        }

        private void SetUp()
        {
            WorkQueue = new Queue<ByteWrapper>();
            logging = new Logs.Logging();
            ThresholdSet = false;
            sensitivity = 1;
        }

        /// <summary>
        /// Called when the object detects motion, creates the event
        /// </summary>
        /// <param name="image"></param>
        protected void OnMotion(ByteWrapper image1, ByteWrapper image2)
        {
            if (motionDetected != null)
            {
                motionDetected(image1, EventArgs.Empty);
            }
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
            Console.WriteLine(image2.sequenceNumber + " movement in grid " + motionGrid);
            OnMotion(image1, image2);
        }

        public async void ImageCreatedAsync(ByteWrapper img, EventArgs e)
        {
            await Task.Run(() =>
            {
                logging.imagesReceived++;
                WorkQueue.Enqueue(img);
                SendForCompareAsync(); //need to create an async method
            });
        }

        public void ImageCreated(ByteWrapper img, EventArgs e)
        {
            logging.imagesReceived++;
            WorkQueue.Enqueue(img);
            SendForCompare();
        }

        /// <summary>
        /// Gets the next two images on the queue and sends for analysis
        /// </summary>
        private void SendForCompareAsync()
        {
            //as there are multiple threads working the queue may have images removed by other threads
            //move this to lock functionality, rather than losing        
            lock (objLock)
            {
                if (WorkQueue.Count > 1)
                {
                    ByteWrapper image1 = WorkQueue.Dequeue();
                    ByteWrapper image2 = WorkQueue.Dequeue();

                    if(image1 != null && image2 != null)
                    {
                        Console.WriteLine(image1.sequenceNumber);
                        Compare(image1, image2);
                    }

                }
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


    }
}
