using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace IPConnect_Testing.Images
{
    class PixelAnalysis
    {
        Bitmap bitmap;

        public PixelAnalysis(Bitmap bitmap){ this.bitmap = bitmap; }

        public Int64 totalPixelColors;

        public void TotalPixels()
        {
            for (int i = 0; i < bitmap.Height; i++)
            {
                Color c = bitmap.GetPixel(1, i);
                totalPixelColors += c.Name.HexToInt();
            }
        }
    }
}
