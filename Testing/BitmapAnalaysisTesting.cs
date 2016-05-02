using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageAnalysis.Analysis;
using ImageAnalysis.Images.Bitmaps;
using System.Drawing;

namespace Testing
{
    [TestClass]
    public class BitmapAnalaysisTesting
    {
        [TestMethod]
        [TestCategory("BitmapWrapper")]
        public void TotalPixelsVersusSumPixel()
        {
            BitmapWrapper bitmap = new BitmapWrapper(@"d:\temp\analysis\multiple_movement\test_266.jpg");

            Assert.IsFalse(bitmap.pixelAnalysis.SumPixelHex == bitmap.pixelAnalysis.totalPixelColors);
        }
    }
}
