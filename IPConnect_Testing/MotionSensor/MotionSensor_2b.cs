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
   
    public class MotionSensor_2b : MotionSensor_2
    {
        private List<ImageGrid> gridImages = new List<ImageGrid>(); //a collection of images, in grid form
        public ImageGrid thresholdImage{get; private set;} //the thresholds, per grid

        public override void Compare(ByteWrapper image1, ByteWrapper image2)
        {
            var bm1 = new BitmapWrapper(ImageConvert.ReturnBitmap(image1.bytes));
            var bm2 = new BitmapWrapper(ImageConvert.ReturnBitmap(image2.bytes));

            PixelMatrix matrix = new PixelMatrix();
            matrix.GridSystemOn = true;
            matrix.Populate(bm1, bm2);

            double sumChangedPixels = matrix.SumChangedPixels;

            //keep adding for threshold calculation, set the threshold, or monitor
            if (ThresholdSet)
            {
                //do the motion detection
                for(int i = 0; i < thresholdImage.Columns.Count; i++)
                {
                    for(int n = 0; n < thresholdImage.Columns[i].grids.Count; n++)
                    {
                        if(matrix.imageGrid.Columns[i].grids[n].change > thresholdImage.Columns[i].grids[n].threshold)
                        {
                            OnMotion(image1, image2, thresholdImage.GridNumber(i, n));
                            return;
                        }
                    }
                }

            }
            else if (!ThresholdSet && gridImages.Count < ControlImageNumber)
            {
                gridImages.Add(matrix.imageGrid); //keep adding for the later threshold calculation
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
        /// sets the gridImages object as each grid has its own threshold
        /// </summary>
        private void SetThreshold()
        {
            //set the threshold image
            thresholdImage = new ImageGrid(gridImages[0].Columns[0].grids.Count, gridImages[0].Columns.Count);

            //do the calculation grid by grid
            for(int i = 0; i < gridImages[0].Columns.Count; i++)
            {
                thresholdImage.Columns.Add(new GridColumn());

                for(int n = 0; n < gridImages[0].Columns[i].grids.Count; n++)
                {
                    List<double> gridTotals = new List<double>();
                    for(int k = 0; k < gridImages.Count; k++)
                    {
                        gridTotals.Add(gridImages[k].Columns[i].grids[n].positiveChange);
                    }

                   // double range = (gridTotals.Max() - gridTotals.Min()) / gridTotals.Min() * 100;
                    double buffer = gridTotals.Max() * 1.2 * sensitivity;
                    thresholdImage.Columns[i].grids.Add(new Grid());
                    thresholdImage.Columns[i].grids[n].threshold = Math.Round(buffer, 0);
                }

            }//each grid column

            ThresholdSet = true;
        }
    }
}
