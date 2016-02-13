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
   /// 2b involves the creation of a pixel comparision matrix between
   /// two images. The analysis is then grouped in to, equal sized, grids. The grids summarise
   /// informaiton about the pixels within them
   /// </summary>
   /// 
    public class MotionSensor_2b : MotionSensor_2
    {
        private List<GridImage> gridImages; //a collection of images, in grid form
        private List<GridImage> thresholdImage; //the thresholds, per grid

        public override void Compare(ByteWrapper image1, ByteWrapper image2)
        {
            var bm1 = new BitmapWrapper(ImageConvert.ReturnBitmap(image1.bytes));
            var bm2 = new BitmapWrapper(ImageConvert.ReturnBitmap(image2.bytes));

            PixelMatrix matrix = new PixelMatrix(bm1, bm2);
            matrix.GridSystemOn = true;
            double sumChangedPixels = matrix.SumChangedPixels;

            //keep adding for threshold calculation, set the threshold, or monitor
            if (ThresholdSet)
            {
                //do the motion detection
            }
            else if (!ThresholdSet && gridImages.Count < ControlImageNumber)
            {
                //keep adding for the threshold
                gridImages.Add(new GridImage(matrix.GridColumns));
            }
            else
            {
                //enough images received to set the threshold and start monitoring
                SetThreshold(); 
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
