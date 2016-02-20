using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageAnalysis;
using ImageAnalysis.Images;
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
            Assert.IsTrue(Directory.Exists(saver.ParentDirectory));
            Assert.IsTrue(Directory.Exists(saver.CaptureDirectory));

        }
    }
}
