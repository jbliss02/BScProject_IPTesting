using System;
using IPConnect_Testing.Testing;
using IPConnect_Testing.Testing.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;

namespace Testing.TestHarness
{
    [TestClass]
    public class CaptureListTest
    {
        [TestMethod]
        [TestCategory("Test harness")]
        public void UpdateNumberFrames()
        {
            CaptureListTesting captureListTesting = new CaptureListTesting();

            captureListTesting.UpdateNumberFrames(0);

            //Assert.IsTrue(doc.InnerXml.Length > 20);

        }
    }
}
