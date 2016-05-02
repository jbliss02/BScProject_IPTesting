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
using ImageAnalysisDAL;
using System.Configuration;

namespace Testing.Database
{
    [TestClass]
    public class DbTesting
    {
       // public object ConfigurationManager { get; private set; }

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

        [TestMethod]
        [TestCategory("Database")]
        public void TestAddCapture()
        {
            ImageSaver imageSaver = new ImageSaver(@"d:\motion", "movement", 0);

            ICaptureDb db = new CaptureDb(ConfigurationManager.ConnectionStrings["LOCALDB"].ConnectionString);
            db.CreateCaptureSession(imageSaver.captureId, imageSaver.SaveDirectory);
            Assert.IsTrue(db.CaptureIdExists(imageSaver.captureId));

            Exception ex = null;
            try
            {
                db.CreateCaptureSession(imageSaver.captureId, imageSaver.SaveDirectory);
            }
            catch(Exception exc)
            {
                ex = exc;
            }

            Assert.IsNotNull(ex);

        }


    }
}
