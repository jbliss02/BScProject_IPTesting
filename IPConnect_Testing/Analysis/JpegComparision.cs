using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPConnect_Testing.Images;
using IPConnect_Testing.Images.Bitmaps;
using System.Drawing;

namespace IPConnect_Testing.Analysis
{
    public class JpegComparision
    {
        JPEG img1;
        JPEG img2;

        BitmapWrapper bm1;
        BitmapWrapper bm2;

        public JpegComparision(JPEG img1, JPEG img2)
        {
            this.img1 = img1;
            this.img2 = img2;
            bm1 = img1.ReturnBitmapWrapper();
            bm2 = img2.ReturnBitmapWrapper();

            if (bm1.bitmap.Width != bm2.bitmap.Width || bm1.bitmap.Height != bm2.bitmap.Width) { throw new Exception("Images not the same size"); }
        }

        public void DifferenceInPixelSum()
        {
            double sum1 = bm1.pixelAnalysis.SumRGB();
            double sum2 = bm2.pixelAnalysis.SumRGB();
            double diff = sum2 - sum1;

            Console.WriteLine(sum1);
            Console.WriteLine(sum2);
            Console.WriteLine(diff);
        }

        /// <summary>
        /// Compares each pixel between the images to determine whether the
        /// Colour has changed, returns a Pixel matrix which stores the changes 
        /// </summary>
        public PixelMatrix DifferenceInPixels()
        {
            //create an matrix, the same size as the images, to store any changes in pixels
            PixelMatrix matrix = new PixelMatrix();

            for(int i = 0; i < bm1.bitmap.Width; i++)
            {
                PixelColumn column = new PixelColumn();
                column.cells = new List<PixelCell>();

                for(int n = 0; n < bm2.bitmap.Height; n++)
                {
                    PixelCell cell = new PixelCell();
                    if(bm1.bitmap.GetPixel(i,n).Name == bm2.bitmap.GetPixel(i, n).Name) { cell.hasChanged = true; }
                    column.cells.Add(cell);

                }//height

                matrix.columns.Add(column);

            }//width

            return matrix;

        }//DifferenceInPixels


    }
}
