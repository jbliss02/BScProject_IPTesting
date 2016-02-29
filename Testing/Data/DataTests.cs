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
using System.Xml;

namespace Testing.Data
{
    [TestClass]
    public class DataTests
    {
        [TestMethod]
        [TestCategory("Data")]
        public void CatpureXml()
        {
            CaptureList captureList = new CaptureList();
            captureList.PopulateAllCaptures(false);
            XmlDocument doc = captureList.SerialiseMe();
            Assert.IsTrue(doc.OuterXml.Count() > 0);
        }

        [TestMethod]
        [TestCategory("Data")]
        public void CaptureListTestXml()
        {

            CaptureTesting cap = new CaptureTesting();
            cap.captureId = "111";
            cap.detectionStartTime = DateTime.Now;
            cap.detectionEndTime = DateTime.Now.AddMinutes(1);
            cap.detectionMethod = "a";
            cap.detectedMovmentFrames = new List<int>();
            cap.detectedMovmentFrames.Add(1);
            cap.detectedMovmentFrames.Add(31);
            cap.detectedMovmentFrames.Add(78);
            cap.detectedMovmentFrames.Add(66);

            XmlDocument xmldoc = cap.SerialiseMe();
            Assert.IsTrue(xmldoc.OuterXml.Count() > 0);

            cap = new CaptureTesting();
            cap.captureId = "222";
            cap.detectionMethod = "a";
            cap.detectionStartTime = DateTime.Now;
            cap.detectionEndTime = DateTime.Now.AddMinutes(1);
            cap.detectedMovmentFrames = new List<int>();
            cap.detectedMovmentFrames.Add(8);
            cap.detectedMovmentFrames.Add(88);
            cap.detectedMovmentFrames.Add(98);
            cap.detectedMovmentFrames.Add(28);

            xmldoc = cap.SerialiseMe();
            Assert.IsTrue(xmldoc.OuterXml.Count() > 0);


            cap = new CaptureTesting();
            cap.captureId = "333";
            cap.detectionMethod = "a";
            cap.detectionStartTime = DateTime.Now;
            cap.detectionEndTime = DateTime.Now.AddMinutes(1);

            xmldoc = cap.SerialiseMe();
            Assert.IsTrue(xmldoc.OuterXml.Count() > 0);


            cap = new CaptureTesting();
            cap.captureId = "444";
            cap.detectionMethod = "a";
            xmldoc = cap.SerialiseMe();
            Assert.IsTrue(xmldoc.OuterXml.Count() > 0);

        }

        [TestMethod]
        [TestCategory("Data")]
        public void CatpureSettingsXml()
        {
            MotionSensorSettingsTest test = new MotionSensorSettingsTest();
            test.captureId = "111";
            test.sensitivity = 3.111;
            XmlDocument doc = test.SerialiseMe();
            Assert.IsTrue(doc.OuterXml.Count() > 0);
        }


    }
}
