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
        public void CaptureListTestXml()
        {

            CaptureMotionTesting cap = new CaptureMotionTesting();
            cap.captureId = "111";
            cap.startTime = DateTime.Now;
            cap.endTime = DateTime.Now.AddMinutes(1);          
            cap.detectedMovmentFrames = new List<int>();
            cap.detectedMovmentFrames.Add(1);
            cap.detectedMovmentFrames.Add(31);
            cap.detectedMovmentFrames.Add(78);
            cap.detectedMovmentFrames.Add(66);

            XmlDocument xmldoc = cap.MotionTestingXml();
            Assert.IsTrue(xmldoc.OuterXml.Count() > 0);

            cap = new CaptureMotionTesting();
            cap.captureId = "222";
            cap = new CaptureMotionTesting();
            cap.startTime = DateTime.Now;
            cap.endTime = DateTime.Now.AddMinutes(1);
            cap.detectedMovmentFrames = new List<int>();
            cap.detectedMovmentFrames.Add(8);
            cap.detectedMovmentFrames.Add(88);
            cap.detectedMovmentFrames.Add(98);
            cap.detectedMovmentFrames.Add(28);

            xmldoc = cap.MotionTestingXml();
            Assert.IsTrue(xmldoc.OuterXml.Count() > 0);


            cap = new CaptureMotionTesting();
            cap.captureId = "333";
            cap = new CaptureMotionTesting();
            cap.startTime = DateTime.Now;
            cap.endTime = DateTime.Now.AddMinutes(1);

            xmldoc = cap.MotionTestingXml();
            Assert.IsTrue(xmldoc.OuterXml.Count() > 0);


            cap = new CaptureMotionTesting();
            cap.captureId = "444";

            xmldoc = cap.MotionTestingXml();
            Assert.IsTrue(xmldoc.OuterXml.Count() > 0);

        }


    }
}
