using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using ImageAnalysis.Images;
using ImageAnalysis.Analysis;
using ImageAnalysis.Images.Bitmaps;
using ImageAnalysis.Images.Jpeg;

namespace ImageAnalysis.MotionSensor
{
    /// <summary>
    /// 2a is an approach which takes a stream of images and compares the sum of pixel colours between adjacent images
    /// A control sample is taken initially which determines a threshold, beyond which the move in pixel colour totals
    /// is considered abnormal, and therefore classed as movement. 
    /// </summary>

    public class MotionSensor_2a : MotionSensor_2, IDisposable
    {
        private List<double> pixelChange; //holds a list of the difference between pixels of 2 images (used for setting threshold)
        public double pixelChangeThreshold;

        public MotionSensor_2a() : base() { pixelChange = new List<double>(); }

        public override void Compare(ByteWrapper image1, ByteWrapper image2)
        {
            var bm1 = new BitmapWrapper(ImageConvert.ReturnBitmap(image1.bytes));
            bm1.sequenceNumber = logging.imagesReceived - 2;
            var bm2 = new BitmapWrapper(ImageConvert.ReturnBitmap(image2.bytes));
            bm2.sequenceNumber = logging.imagesReceived - 1;

            PixelMatrix matrix = new PixelMatrix();
            matrix.LinkCompare = settings.linkCompare;
            if (settings.searchHeight > 0) { matrix.SearchHeight = settings.searchHeight; }
            if (settings.searchWidth > 0) {  matrix.SearchWidth = settings.searchWidth; }
            if (settings.horizontalPixelsToSkip > 0) { matrix.WidthSearchOffset = settings.horizontalPixelsToSkip + 1; }
            if (settings.verticalPixelsToSkip > 0) { matrix.WidthSearchOffset = settings.verticalPixelsToSkip + 1; }

            if (settings.linkCompare && Comparison != null)
            {
                matrix.Populate(Comparison, bm2);
            }
            else
            {
                matrix.Populate(bm1, bm2);
            }

            double sumChangedPixels = matrix.SumChangedPixelsPositive;

            //keep adding for threshold calculation, set the threshold, or monitor
            if(ThresholdSet)
            {
                //now scanning, compare the two images and see what the difference is
                if (sumChangedPixels > pixelChangeThreshold)
                {
                    OnMotionAsync(image1, image2);
                }
            }
            else if (!ThresholdSet && pixelChange.Count < ControlImageNumber)
            {
                pixelChange.Add(sumChangedPixels);
            }
            else  
            {
                SetThreshold(); //enough images received to set the threshold and start monitoring
            }

            Comparison = matrix.Comparator;

            if (logging.LoggingOn && logging.matrices != null) { logging.matrices.Add(matrix); }

            //clean up the memory
            matrix.Dispose();
            bm1.bitmap.Dispose();
            bm2.bitmap.Dispose();
            bm1 = null;
            bm2 = null;

            OnComparisionProcessed(image1.sequenceNumber, EventArgs.Empty);

        }//Compare
     
        /// <summary>
        /// Called when the threshold is to be set, or re-set
        /// takes the current range of changes and calculates threshold
        /// based on these ranges, and the specified sensitivity
        /// </summary>
        private void SetThreshold()
        {
            double max = pixelChange.Max();
            double min = pixelChange.Min();

            double range = ((max - min) / min) * 100; //percentage change
            // double buffer = (100 + (range * 1.5 * (double)settings.sensitivity)) / 100; //this is a percentage
            double buffer = (100 + (range * (double)settings.sensitivity)) / 100; //this is a percentage
            pixelChangeThreshold = max * buffer;
            ThresholdSet = true;
            if (logging.LoggingOn) { logging.threshold = pixelChangeThreshold; }
        }

        public void Dispose()
        {
            pixelChange = null;
            WorkQueue = null;
            settings = null;
        }
    }
}
