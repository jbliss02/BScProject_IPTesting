using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPConnect_Testing.Images;
using IPConnect_Testing.Analysis;
using IPConnect_Testing.Images.Bitmaps;
using IPConnect_Testing.Images.Jpeg;

namespace IPConnect_Testing.MotionSensor
{
    /// <summary>
    /// Base class that provides basic functionality to serve Motion Sensor approaches in category 2 (pixel analysis)
    /// </summary>
    public class MotionSensor_2
    {
        //Work queue
        public Queue<ByteWrapper> Images { get; set; } //images waiting to be processed

        //motion detected event
        public event MotionDetected motionDetected;
        public delegate void MotionDetected(ByteWrapper image, EventArgs e);

        //threshold setting
        public int ControlImageNumber { get; set; } = 30; //number of changes to use as the control (half the images as done in pairs)
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

        //logging and saving
        public String logfile { get; set; }
        protected int imagesReceived; //used to flush the queue of images
        protected int imagesChecked;
        protected int numberMotionFiles;

        public MotionSensor_2()
        {
            SetUp();
        }

        private void SetUp()
        {
            Images = new Queue<ByteWrapper>();
            
            ThresholdSet = false;
            sensitivity = 1;
        }

        /// <summary>
        /// Called when the object detects motion, creates the event
        /// </summary>
        /// <param name="image"></param>
        protected void OnMotion(ByteWrapper image1, ByteWrapper image2)
        {
            if(motionDetected != null)
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
                imagesReceived++;
                Images.Enqueue(img);
                SendForCompare();
            });
        }

        public void ImageCreated(ByteWrapper img, EventArgs e)
        {
            imagesReceived++;
            Images.Enqueue(img);
            SendForCompare();
        }

        /// <summary>
        /// Gets the next two images on the queue and sends for analysis
        /// </summary>
        private void SendForCompare()
        {
            //as there are multiple threads working the queue may have images removed by other threads
            //move this to lock functionality, rather than losing 
            if (Images.Count > 1)
            {
                //take images out of the queue, as this is async other methods may dequeue between the calls so be defensive
                ByteWrapper img1 = null;
                if (Images.Count > 0) { img1 = Images.Dequeue(); }
                ByteWrapper img2 = null;
                if (Images.Count > 0) { img2 = Images.Dequeue(); }

                if (img1 != null && img2 != null)
                {
                    Compare(img1, img2);
                    imagesChecked = imagesChecked + 2;
                }
            }
        }//SendForCompare

        public virtual void Compare(ByteWrapper img1, ByteWrapper img2) { } //will always be implemented in the sub class


    }
}
