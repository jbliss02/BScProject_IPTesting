using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Tools;

namespace ImageAnalysis.Images.Bitmaps
{
    public class PixelAnalysis
    {
        Bitmap bitmap;
        public PixelAnalysis(Bitmap bitmap){ this.bitmap = bitmap; }
        public double totalPixelColors { get { return SumRGB(); } }
        public double SumPixelHex { get { return sumPixelHex(); } }
        /// <summary>
        /// Returns the hex value of all RGB pixels in the bitmap
        /// </summary>
        public double SumRGB()
        {
            double result = 0;

            for (int i = 0; i < bitmap.Height; i++)
            {
                for(int n = 0; n < bitmap.Width; n++)
                {
                    Color c = bitmap.GetPixel(n, i);

                    result += (c.R + c.G + c.B);

                  //  totalPixelColors += c.Name.HexToInt();
                }
            }

            return result;
        }


        private double sumPixelHex()
        {
            double result = 0;

            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int n = 0; n < bitmap.Width; n++)
                {
                    Color c = bitmap.GetPixel(n, i);

                    result += Int64.Parse(bitmap.GetPixel(n, i).Name, System.Globalization.NumberStyles.HexNumber);

                    //  totalPixelColors += c.Name.HexToInt();
                }
            }

            return result;
        }
    }
}
