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
    public class CaptureMethodTesting
    {
        [TestMethod]
        [TestCategory("Data")]
        public void TestMethod1()
        {

            CaptureList capList = new CaptureList();
            capList.list = new List<Capture>();

            Capture cap = new Capture();
            cap.captureId = "111";
            cap.testing = new CaptureTesting();
            cap.testing.startTime = DateTime.Now;
            cap.testing.endTime = DateTime.Now.AddMinutes(1);
            cap.testing.detectedMovmentFrames = new List<int>();
            cap.testing.detectedMovmentFrames.Add(1);
            cap.testing.detectedMovmentFrames.Add(31);
            cap.testing.detectedMovmentFrames.Add(78);
            cap.testing.detectedMovmentFrames.Add(66);
            capList.list.Add(cap);

            cap = new Capture();
            cap.captureId = "222";
            cap.testing = new CaptureTesting();
            cap.testing.startTime = DateTime.Now;
            cap.testing.endTime = DateTime.Now.AddMinutes(1);
            cap.testing.detectedMovmentFrames = new List<int>();
            cap.testing.detectedMovmentFrames.Add(8);
            cap.testing.detectedMovmentFrames.Add(88);
            cap.testing.detectedMovmentFrames.Add(98);
            cap.testing.detectedMovmentFrames.Add(28);
            capList.list.Add(cap);

            cap = new Capture();
            cap.captureId = "333";
            cap.testing = new CaptureTesting();
            cap.testing.startTime = DateTime.Now;
            cap.testing.endTime = DateTime.Now.AddMinutes(1);
            capList.list.Add(cap);

            cap = new Capture();
            cap.captureId = "444";
            capList.list.Add(cap);

            XmlDocument xmldoc = capList.MotionTestingXml();
            Assert.IsTrue(xmldoc.OuterXml.Count() > 0);


        }
    }
}
