using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ExtensionMethods;
namespace IPConnect_Testing.Images.Bitmaps
{
    public class PixelAnalysis
    {
        Bitmap bitmap;
        public PixelAnalysis(Bitmap bitmap){ this.bitmap = bitmap; }
        public Int64 totalPixelColors;
        
        /// <summary>
        /// Returns the hex value of all RGB pixels in the bitmap
        /// </summary>
        public Int64 SumRGB()
        {
            for (int i = 0; i < bitmap.Height; i++)
            {
                for(int n = 0; n < bitmap.Width; n++)
                {
                    Color c = bitmap.GetPixel(n, i);

                    totalPixelColors += (c.R + c.G + c.B);

                  //  totalPixelColors += c.Name.HexToInt();
                }
            }

            return totalPixelColors;
        }
    }
}
