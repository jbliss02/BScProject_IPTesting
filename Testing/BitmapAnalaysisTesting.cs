using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IPConnect_Testing.Analysis;
using IPConnect_Testing.Images.Bitmaps;
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
            var x = "nmd.";
        }
    }
}
