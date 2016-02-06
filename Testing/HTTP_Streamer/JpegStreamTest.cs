using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTTP_Streamer.Controllers;
using HTTP_Streamer.Models;
using System.Collections.Generic;

namespace Testing
{
    [TestClass]
    public class JPegStreamTest
    {
        /// <summary>
        /// Tests that all files and settings are extracted from a directory correctly
        /// Ensures files with wrong extensions are not extracted
        /// </summary>
        [TestMethod]
        public void MpegSectionsTest()
        {
            List<JpegSection> sections = new JpegStream(-1, "FileTest").MpegSections();

            Assert.IsNotNull(sections);
            Assert.IsTrue(sections.Count == 3);
            Assert.IsNotNull(sections[0].imageFiles);
            Assert.IsTrue(sections[0].imageFiles.Count + sections[1].imageFiles.Count + sections[2].imageFiles.Count  == 12);
        }

        [TestMethod]
        public void MpegSectionsNegativeTest()
        {
            Exception ex = null;
            try
            {
                List<JpegSection> sections = new JpegStream(-1, "DoesntExist").MpegSections();
            }
            catch(Exception exc)
            {
                ex = exc;
            }

            Assert.IsNotNull(ex);

            ex = null;
            try
            {
                List<JpegSection> sections = new JpegStream(-99, "FileTest").MpegSections();
            }
            catch (Exception exc)
            {
                ex = exc;
            }

            Assert.IsNotNull(ex);

        }

        /// <summary>
        /// Tests where only certain frames are extracted
        /// </summary>
        [TestMethod]
        public void MpegPickFrames()
        {
            List<JpegSection> sections = new JpegStream(0, "201623194433607",222,1015).MpegSections();

            Assert.IsNotNull(sections);
            Assert.IsTrue(sections.Count == 2);
            Assert.IsNotNull(sections[0].imageFiles);
            Assert.IsTrue(sections[0].imageFiles.Count + sections[1].imageFiles.Count == 794);
        }

    }
}
