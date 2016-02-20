using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageAnalysis.Analysis;
namespace Testing
{
    [TestClass]
    public class PixelMatrixTest
    {
        /// <summary>
        /// Checks that the grid is correctly set 
        /// </summary>
        [TestMethod]
        [TestCategory("PixelMatrix")]
        public void Grid()
        {
            PixelMatrix matrix = new PixelMatrix();
            matrix.GridSystemOn = true;
            matrix.Populate(@"F:\temp\MotionSensor\2.2\test_101.jpg", @"F:\temp\MotionSensor\2.2\test_128.jpg");

            Assert.IsNotNull(matrix.imageGrid.Columns);
            Assert.IsTrue(matrix.imageGrid.Columns.Count == 4);
            Assert.IsTrue(matrix.imageGrid.Columns[0].grids.Count == 4);
            Assert.IsTrue(matrix.imageGrid.Columns[1].grids.Count == 4);
            Assert.IsTrue(matrix.imageGrid.Columns[2].grids.Count == 4);
            Assert.IsTrue(matrix.imageGrid.Columns[3].grids.Count == 4);
        }

        /// <summary>
        /// Checks that the grid is correctly  populated
        /// </summary>
        [TestMethod]
        [TestCategory("PixelMatrix")]
        public void GridPopulation()
        {
            PixelMatrix matrix = new PixelMatrix();
            matrix.GridSystemOn = true;
            matrix.Populate(@"F:\temp\MotionSensor\2.2\test_101.jpg", @"F:\temp\MotionSensor\2.2\test_128.jpg");

            for(int i = 0; i < matrix.imageGrid.Columns.Count; i++)
            {
                for(int n = 0; n < matrix.imageGrid.Columns[i].grids.Count; n++)
                {
                    Assert.IsTrue(matrix.imageGrid.Columns[i].grids[n].change != 0);
                }                     
            }
        }//GridPopulation

    }
}
