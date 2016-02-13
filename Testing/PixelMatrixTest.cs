using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IPConnect_Testing.Analysis;
namespace Testing
{
    [TestClass]
    public class PixelMatrixTest
    {
        /// <summary>
        /// Checks that the grid is correctly set and populated
        /// </summary>
        [TestMethod]
        [TestCategory("PixelMatrix")]
        public void Grid()
        {
            PixelMatrix matrix = new PixelMatrix();
            matrix.GridSystemOn = true;
            matrix.Populate(@"F:\temp\MotionSensor\2.2\test_101.jpg", @"F:\temp\MotionSensor\2.2\test_128.jpg");

            Assert.IsNotNull(matrix.GridColumns);
            Assert.IsTrue(matrix.GridColumns.Count == 4);
            Assert.IsTrue(matrix.GridColumns[0].grids.Count == 4);
            Assert.IsTrue(matrix.GridColumns[1].grids.Count == 4);
            Assert.IsTrue(matrix.GridColumns[2].grids.Count == 4);
            Assert.IsTrue(matrix.GridColumns[3].grids.Count == 4);
        }
    }
}
