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
    public class BitmapComparison
    {
        public void PixelSumComparision(string bmp1Path, string bmp2Path)
        {
            Bitmap bm1 = new Bitmap(bmp1Path);
            Bitmap bm2 = new Bitmap(bmp2Path);

            BitmapWrapper wrap1 = new BitmapWrapper(bm1);
            BitmapWrapper wrap2 = new BitmapWrapper(bm2);

            double sum1 = wrap1.pixelAnalysis.SumRGB();
            double sum2 = wrap2.pixelAnalysis.SumRGB();
            double diff = sum2 - sum1;

            Console.WriteLine(sum1);
            Console.WriteLine(sum2);
            Console.WriteLine(diff);
        }

    }
}
