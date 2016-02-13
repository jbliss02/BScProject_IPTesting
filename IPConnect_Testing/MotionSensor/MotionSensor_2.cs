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
    public abstract class MotionSensor_2
    {
        //Work queue
        public Queue<ByteWrapper> images { get; set; } //images waiting to be processed

        //motion detected event
        public event MotionDetected motionDetected;
        public delegate void MotionDetected(ByteWrapper image, EventArgs e);

        //threshold setting
        public int ControlImageNumber { get; set; } = 10; //number of changes to use as the control (half the images as done in pairs)
        public bool ThresholdSet { get; set; }
        protected List<double> pixelChange; //holds a list of the difference between pixels of 2 images (used for setting threshold)
        protected double pixelChangeThreshold;
        protected double sensitivity = 1;

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
            images = new Queue<ByteWrapper>();
            pixelChange = new List<double>();
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

        public async void ImageCreated(ByteWrapper img, EventArgs e)
        {
            await Task.Run(() =>
            {
                imagesReceived++;
                images.Enqueue(img);
                SendForCompare();
            });
        }

        /// <summary>
        /// Gets the next two images on the queue and sends for analysis
        /// </summary>
        private void SendForCompare()
        {
            if (images.Count > 1)
            {
                //take images out of the queue, as this is async other methods may dequeue between the calls so be defensive
                ByteWrapper img1 = null;
                if (images.Count > 0) { img1 = images.Dequeue(); }
                ByteWrapper img2 = null;
                if (images.Count > 0) { img2 = images.Dequeue(); }

                if (img1 != null && img2 != null)
                {
                    Compare(img1, img2);
                    imagesChecked = imagesChecked + 2;
                }
            }
        }

        public virtual void Compare(ByteWrapper img1, ByteWrapper img2) { } //will always be implemented in the sub class


    }
}
