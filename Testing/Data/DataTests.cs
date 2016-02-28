﻿using System;
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
            XmlDocument doc = captureList.CaptureXml();
            Assert.IsTrue(doc.OuterXml.Count() > 0);
        }
    }
}