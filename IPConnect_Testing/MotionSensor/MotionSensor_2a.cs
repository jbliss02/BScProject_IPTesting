using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using IPConnect_Testing.Images;
using IPConnect_Testing.Analysis;
using IPConnect_Testing.Images.Bitmaps;
using IPConnect_Testing.Images.Jpeg;

namespace IPConnect_Testing.MotionSensor
{
    /// <summary>
    /// 2a is an approach which takes a stream of images and compares the sum of pixel colours between adjacent images
    /// A control sample is taken initially which determines a threshold, beyond which the move in pixel colour totals
    /// is considered abnormal, and therefore classed as movement. 
    /// </summary>

    public class MotionSensor_2a : MotionSensor_2
    { 
        public override void Compare(ByteWrapper image1, ByteWrapper image2)
        {
            var bm1 = new BitmapWrapper(ImageConvert.ReturnBitmap(image1.bytes));
            var bm2 = new BitmapWrapper(ImageConvert.ReturnBitmap(image2.bytes));

            PixelMatrix matrix = new PixelMatrix(bm1, bm2);
            double sumChangedPixels = matrix.SumChangedPixels;

            //keep adding for threshold calculation, set the threshold, or monitor
            if(ThresholdSet)
            {
                //now scanning, compare the two images and see what the difference is
                if (matrix.SumChangedPixels > pixelChangeThreshold)
                {
                    OnMotion(image1, image2);
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

        }//Compare

        /// <summary>
        /// Called when the threshold is to be set, or re-set
        /// takes the current range of changes and calculates threshold
        /// based on these ranges, and the specified sensitivity
        /// </summary>
        private void SetThreshold()
        {
            double range = ((pixelChange.Max() - pixelChange.Min()) / pixelChange.Min()) * 100;
            double buffer = range * 2 * sensitivity;
            pixelChangeThreshold = pixelChange.Max() + buffer;
            ThresholdSet = true;
        }
    }
}
