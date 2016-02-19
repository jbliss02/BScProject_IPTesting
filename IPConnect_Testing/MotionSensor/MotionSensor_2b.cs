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
        public ImageGrid ThresholdImage{get; private set;} //the thresholds, per grid

        public override void Compare(ByteWrapper image1, ByteWrapper image2)
        {
            var bm1 = new BitmapWrapper(ImageConvert.ReturnBitmap(image1.bytes));
            var bm2 = new BitmapWrapper(ImageConvert.ReturnBitmap(image2.bytes));

            PixelMatrix matrix = new PixelMatrix();
            if (SearchHeight > 0) { matrix.SearchHeight = SearchHeight; }
            if (SearchWidth > 0) { matrix.SearchWidth = SearchWidth; }
            if (LinkCompare) { matrix.LinkCompare = true; }
            matrix.GridSystemOn = true;
            matrix.Populate(bm1, bm2);

            double sumChangedPixels = matrix.SumChangedPixels;

            //keep adding for threshold calculation, set the threshold, or monitor
            if (ThresholdSet)
            {
                //do the motion detection
                for(int i = 0; i < ThresholdImage.Columns.Count; i++)
                {
                    for(int n = 0; n < ThresholdImage.Columns[i].grids.Count; n++)
                    {
                        if(matrix.imageGrid.Columns[i].grids[n].change > ThresholdImage.Columns[i].grids[n].threshold)
                        {
                            OnMotion(image1, image2, ThresholdImage.GridNumber(i, n));
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

            Comparison = matrix.Comparator;

        }//Compare

        /// <summary>
        /// Creates a dummy threshold image, used for testing and benchmarking
        /// </summary>
        public void CreateDummyThreshold(int width, int height)
        {           
            ThresholdImage = new ImageGrid(width, height);
            for(int i = 0; i < width; i++)
            {
                ThresholdImage.Columns.Add(new GridColumn());

                for (int n = 0; n < height; n++)
                {
                    Random random = new Random();
                    double buffer = ReturnBuffer(random.Next(25000000));
                    ThresholdImage.Columns[i].grids.Add(new Grid());
                    ThresholdImage.Columns[i].grids[n].threshold = Math.Round(buffer, 0);
                }
            }
        }

        /// <summary>
        /// Called when the threshold is to be set, or re-set
        /// takes the current range of changes and calculates threshold
        /// based on these ranges, and the specified sensitivity
        /// sets the gridImages object as each grid has its own threshold
        /// </summary>
        private void SetThreshold()
        {
            //set the threshold image
            ThresholdImage = new ImageGrid(gridImages[0].Columns[0].grids.Count, gridImages[0].Columns.Count);

            //do the calculation grid by grid
            for(int i = 0; i < gridImages[0].Columns.Count; i++)
            {
                ThresholdImage.Columns.Add(new GridColumn());

                for(int n = 0; n < gridImages[0].Columns[i].grids.Count; n++)
                {
                    List<double> gridTotals = new List<double>();
                    for(int k = 0; k < gridImages.Count; k++)
                    {
                        gridTotals.Add(gridImages[k].Columns[i].grids[n].positiveChange);
                    }

                    double buffer = ReturnBuffer(ReturnMax(gridTotals));
                    ThresholdImage.Columns[i].grids.Add(new Grid());
                    ThresholdImage.Columns[i].grids[n].threshold = Math.Round(buffer, 0);
                }

            }//each grid column

            ThresholdSet = true;
        }

        private double ReturnBuffer(double max)
        {
            return max * 1.2 * sensitivity;
        }

        private double ReturnMax(List<double> list)
        {
            double result = list[0];
            for(int i = 1; i < list.Count; i++)
            {
                if(list[i] > result) { result = list[i]; }
            }

            return result;
        }

    }
}
