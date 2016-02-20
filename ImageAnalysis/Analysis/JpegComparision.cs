using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageAnalysis.Images;
using ImageAnalysis.Images.Bitmaps;
using System.Drawing;

namespace ImageAnalysis.Analysis
{
    public class JpegComparision
    {
        JPEG img1;
        JPEG img2;

        BitmapWrapper bm1;
        BitmapWrapper bm2;

        public JpegComparision(string path1, string path2)
        {
            this.img1 = new JPEG(path1);
            this.img2 = new JPEG(path2);
            Setup();
        }

        public JpegComparision(JPEG img1, JPEG img2)
        {
            this.img1 = img1;
            this.img2 = img2;
            Setup();
        }

        private void Setup()
        {
            bm1 = img1.ReturnBitmapWrapper();
            bm2 = img2.ReturnBitmapWrapper();

            if (bm1.bitmap.Width != bm2.bitmap.Width || bm1.bitmap.Height != bm2.bitmap.Height) { throw new Exception("Images not the same size"); }
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
        /// 
        //TO DO - MOVE THIS INTO PIXEL MATRIX
        public PixelMatrix ReturnPixelMatrix()
        {
            //create an matrix, the same size as the images, to store any changes in pixels
            PixelMatrix matrix = new PixelMatrix();
            matrix.Columns = new List<PixelColumn>();

            for(int i = 0; i < bm1.bitmap.Width; i++)
            {
                PixelColumn column = new PixelColumn();
                column.Cells = new List<PixelCell>();

                for(int n = 0; n < bm1.bitmap.Height; n++)
                {
                    PixelCell cell = new PixelCell();
                    if(bm1.bitmap.GetPixel(i,n).Name != bm2.bitmap.GetPixel(i, n).Name)
                    {
                        cell.hasChanged = true;
                        cell.change = Int64.Parse(bm1.bitmap.GetPixel(i, n).Name, System.Globalization.NumberStyles.HexNumber) - Int64.Parse(bm2.bitmap.GetPixel(i, n).Name, System.Globalization.NumberStyles.HexNumber);
                       
                    }
                    else
                    {
                        cell.hasChanged = false;
                    }
                    column.Cells.Add(cell);

                }//height

                matrix.Columns.Add(column);

            }//width

           
            return matrix;

        }//DifferenceInPixels


        /// <summary>
        ///  Creates a new bitmap which is a copy of the Image being examied for motion
        ///  Any pixels that have changed from the source image has the colour changed
        /// </summary>

        /// <returns></returns>
        public Bitmap ColourPixelChanges(System.Drawing.Color color)
        {
            PixelMatrix matrix  = ReturnPixelMatrix();
            Console.WriteLine(matrix.SumChangedPixels);

            Bitmap bitmap = new Bitmap(bm2.bitmap.Width, bm2.bitmap.Height);

            for (int i = 0; i < bm2.bitmap.Width; i++)
            {
                for (int n = 0; n < bm2.bitmap.Height; n++)
                {
                    if(matrix.Columns[i].Cells[n].hasChanged)
                    {
                        bitmap.SetPixel(i, n, color);
                    }
                    else
                    {
                        int a = bm2.bitmap.GetPixel(i, n).A;
                        int r = bm2.bitmap.GetPixel(i, n).R;
                        int g = bm2.bitmap.GetPixel(i, n).G;
                        int b = bm2.bitmap.GetPixel(i, n).B;

                        bitmap.SetPixel(i, n, Color.FromArgb(a, r, g, b));
                    }

                }//height

            }//width

            return bitmap;

        }//ColourPixelChanges


    }
}
