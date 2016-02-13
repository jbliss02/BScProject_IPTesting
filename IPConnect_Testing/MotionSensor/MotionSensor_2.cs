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
        /// Called when the threshold is to be set, or re-set
        /// takes the current range of changes and calculates threshold
        /// based on these ranges, and the specified sensitivity
        /// </summary>
        protected void SetThreshold()
        {
            double range = ((pixelChange.Max() - pixelChange.Min()) / pixelChange.Min()) * 100;
            double buffer = range * 2 * sensitivity;
            pixelChangeThreshold = pixelChange.Max() + buffer;
            ThresholdSet = true;
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
        /// THIS NEEDS TO BE MOVED OUTSIDE OF THIS THREAD
        /// </summary>
        /// <param name="bitmapWrapper"></param>
        /// <param name="sequenceNumber"></param>
        /// <returns></returns>
        public async Task SaveImageAsync(BitmapWrapper bitmapWrapper, int sequenceNumber)
        {
            await Task.Run(() =>
            {
                bitmapWrapper.bitmap.Save(@"f:\temp\MotionSensor\2.1\movement\" + sequenceNumber + ".bmp");
            });
        }

        public async void ImageCreated(ByteWrapper img, EventArgs e)
        {
            await Task.Run(() =>
            {
                imagesReceived++;
                images.Enqueue(img);
                Compare();
            });
        }

        public virtual void Compare() { } //will always be implemented in the sub class


    }
}
