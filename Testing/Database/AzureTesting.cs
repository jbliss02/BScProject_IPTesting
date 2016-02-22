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
using IPConnect_Testing.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Testing.Database
{
    [TestClass]
    public class AzureTesting
    {
        [TestMethod]
        [TestCategory("Database")]
        public void TestConnection()
        {
            CaptureList captureList = new CaptureList();
            captureList.PopulateAllCaptures();
            var x = "j;";

        }
    }
}
