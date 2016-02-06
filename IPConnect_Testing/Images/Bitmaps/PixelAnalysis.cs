using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace IPConnect_Testing.Images.Bitmaps
{
    public class PixelAnalysis
    {
        Bitmap bitmap;
        public PixelAnalysis(Bitmap bitmap){ this.bitmap = bitmap; }
        public Int64 totalPixelColors;
        
        /// <summary>
        /// Returns the hex value of all pixels in the bitmap
        /// </summary>
        public Int64 TotalPixels()
        {
            for (int i = 0; i < bitmap.Height; i++)
            {
                Color c = bitmap.GetPixel(1, i);
                totalPixelColors += c.Name.HexToInt();
            }

            return totalPixelColors;
        }
    }
}
