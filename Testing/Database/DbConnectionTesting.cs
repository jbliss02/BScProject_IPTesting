using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ImageAnalysis;
using ImageAnalysis.Images;
using ImageAnalysis.Images.Bitmaps;
using ImageAnalysis.Images.Jpeg;
using ImageAnalysis.MotionSensor;
using ImageAnalysis.Analysis;
using IPConnect_Testing;
using IPConnect_Testing.Testing;
using ImageAnalysis.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing.Database
{
    [TestClass]
    public class DbConnectionTesting
    {
        [TestMethod]
        [TestCategory("Database")]
        public void TestConnection()
        {
            CaptureList captureList = new CaptureList();
            captureList.PopulateAllCaptures(false);
            Assert.IsTrue(captureList.list.Count > 0);
        }

        [TestMethod]
        [TestCategory("Database")]
        public void TestCaptureMovementExtraction()
        {
            CaptureList captureList = new CaptureList();
            captureList.PopulateAllCaptures(false);
            captureList.PopulateMovement();

            int nMovements = (from c in captureList.list
                              select c.movement.Count).Sum();

            Assert.IsTrue(nMovements > 0);

        }
    }
}
