using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTTP_Streamer.Controllers;
using HTTP_Streamer.Models;
using System.Collections.Generic;

namespace Testing
{
    [TestClass]
    public class MPegStreamTest
    {
        /// <summary>
        /// Tests that all files are extracted from a directory correctly.
        /// </summary>
        [TestMethod]
        public void GetFiles()
        {
            List<String> files = new MPegStream(-1, "FileTest").ImageFiles();

            Assert.IsNotNull(files);
            Assert.IsTrue(files.Count == 12);

        }
    }
}
