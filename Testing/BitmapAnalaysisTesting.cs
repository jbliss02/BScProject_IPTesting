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
        public void TestMethod1()
        {
            BitmapWrapper bitmap = new BitmapWrapper(@"F:\temp\analysis\multiple_movement\test_266.jpg");

            Assert.IsTrue(bitmap.pixelAnalysis.SumPixelHex == bitmap.pixelAnalysis.totalPixelColors);
        }
    }
}
