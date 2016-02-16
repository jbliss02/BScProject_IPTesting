using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using IPConnect_Testing.Images;
using IPConnect_Testing.Images.Bitmaps;
using IPConnect_Testing.Images.Jpeg;
using IPConnect_Testing.MotionSensor;
using IPConnect_Testing.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing.RumtimeAnalysis
{
    /// <summary>
    /// Tests the runtime of the motion detetcion algorithms with their various settings
    /// </summary>
    /// 
    [TestClass]
    public class MotionSensorRunTime
    {
        /// <summary>
        /// Looks at the time to analyse two images on PixelMatrix
        /// Various numbers of pixels are analysed
        /// </summary>
        [TestMethod]
        [TestCategory("Runtime analysis")]
        public void PixelSize()
        {
            try
            {
                List<int> dimensions = ReturnDimensions();
                Stopwatch sw = new Stopwatch();
                int count = 1;
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"f:\temp\runtime\analysis.txt", true))
                {
                    for (int i = 0; i < dimensions.Count; i++)
                    {
                        for(int n = 0; n < 10; n++)
                        {
                            BitmapWrapper bm1 = new BitmapWrapper(@"F:\temp\analysis\640x480\test_0.jpg");
                            BitmapWrapper bm2 = new BitmapWrapper(@"F:\temp\analysis\640x480\test_0.jpg");
                            sw.Restart();
                            PixelMatrix matrix = new PixelMatrix();
                            matrix.SearchWidth = dimensions[i];
                            matrix.Populate(bm1, bm2);
                            sw.Stop();
                            file.WriteLine(i + " - " + n + " - " + " no grid - " + sw.Elapsed.TotalMilliseconds);

                            sw.Restart();
                            bm1 = new BitmapWrapper(@"F:\temp\analysis\640x480\test_0.jpg");
                            bm2 = new BitmapWrapper(@"F:\temp\analysis\640x480\test_0.jpg");
                            matrix = new PixelMatrix();
                            matrix.SearchWidth = dimensions[i];
                            matrix.GridSystemOn = true;
                            matrix.Populate(bm1, bm2);
                            sw.Stop();
                            file.WriteLine(i + " - " + n + " - " + " grid - " + sw.Elapsed.TotalMilliseconds);
                        }
                    }
                }
                Assert.IsTrue(true);
            }
            catch(Exception ex)
            {
                Assert.IsTrue(false);
            }

        }

        [TestMethod]
        [TestCategory("Runtime analysis")]
        public void MotionSensorPixelSize()
        {
            try
            {
                List<int> dimensions = ReturnDimensions();
                Stopwatch sw = new Stopwatch();
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"f:\temp\runtime\motion_analysis.txt", true))
                {
                    for (int i = 0; i < dimensions.Count; i++)
                    {
                        for (int n = 0; n < 10; n++)
                        {
                            //images in memory when sent to the 
                            ByteWrapper image1 = ImageConvert.ReturnByteWrapper(@"F:\temp\analysis\640x480\test_0.jpg");
                            image1.sequenceNumber = i;
                            ByteWrapper image2 = ImageConvert.ReturnByteWrapper(@"F:\temp\analysis\640x480\test_0.jpg");
                            image2.sequenceNumber = i;

                            
                            MotionSensor_2a motion = new MotionSensor_2a();
                            motion.ThresholdSet = true;
                            sw.Restart();
                            motion.ImageCreatedAsync(image1, EventArgs.Empty);
                            motion.ImageCreatedAsync(image2, EventArgs.Empty);
                            sw.Stop();
                            file.WriteLine(i + " - " + n + " - " + " no grid - " + sw.Elapsed.TotalMilliseconds);

                            MotionSensor_2b motionb = new MotionSensor_2b();
                            motionb.ThresholdSet = true;
                            sw.Restart();
                            motionb.ImageCreatedAsync(image1, EventArgs.Empty);
                            motionb.ImageCreatedAsync(image2, EventArgs.Empty);
                            sw.Stop();
                            file.WriteLine(i + " - " + n + " - " + "  grid - " + sw.Elapsed.TotalMilliseconds);


                        }
                    }
                }
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false);
            }
        }


        /// <summary>
        /// returns various x by y sizes to fit certain number of pixels
        /// </summary>
        /// <returns></returns>
        private List<int> ReturnDimensions()
        {
            //width is 480 in every case for consistent fall in pixel numbers
            List<int> result = new List<int>();
            result.Add(640);
            result.Add(320);
            result.Add(160);
            result.Add(80);
            result.Add(40);
            result.Add(20);
            return result;
        }

    }
}
