using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IPConnect_Testing;
using System.IO;

namespace Testing
{
    [TestClass]
    public class ImageSaverTesting
    {
        [TestMethod]
        [TestCategory("Image Saving")]
        public void DirectoryCreation()
        {
            ImageSaver saver = new ImageSaver(1);
            Assert.IsTrue(Directory.Exists(saver.parentDirectory));
            Assert.IsTrue(Directory.Exists(saver.captureDirectory));

        }
    }
}
