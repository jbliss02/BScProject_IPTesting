using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using IPConnect_Testing;
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
        public void PixelMatrix()
        {
            try
            {
                List<int> dimensions = ReturnDimensions();
                Stopwatch sw = new Stopwatch();
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"f:\temp\runtime\pixelmatrixanalysis.txt", true))
                {
                    for (int i = 0; i < dimensions.Count; i++)
                    {
                        for(int n = 0; n < 200; n++)
                        {
                            BitmapWrapper bm1 = new BitmapWrapper(@"F:\temp\analysis\640x480\test_0.jpg");
                            BitmapWrapper bm2 = new BitmapWrapper(@"F:\temp\analysis\640x480\test_1.jpg");

                            sw.Restart();
                            PixelMatrix matrix = new PixelMatrix();
                            matrix.SearchWidth = dimensions[i];
                            matrix.Populate(bm1, bm2);
                            sw.Stop();
                            file.WriteLine(i + " - " + n + " - " + " no grid - " + sw.Elapsed.TotalMilliseconds);

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

        /// <summary>
        /// Runs the whole end to end motion sensor and logs the response times
        /// </summary>
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
                        for (int n = 0; n < 200; n++)
                        {
                            //images in memory when sent to the 
                            ByteWrapper image1 = ImageConvert.ReturnByteWrapper(@"F:\temp\analysis\640x480\test_0.jpg");
                            image1.sequenceNumber = i;
                            ByteWrapper image2 = ImageConvert.ReturnByteWrapper(@"F:\temp\analysis\640x480\test_1.jpg");
                            image2.sequenceNumber = i;

                            sw.Restart();
                            MotionSensor_2a motion = new MotionSensor_2a();
                            motion.ThresholdSet = true;
                            motion.SearchWidth = dimensions[i];
                           
                            motion.ImageCreated(image1, EventArgs.Empty);
                            motion.ImageCreated(image2, EventArgs.Empty);
                            sw.Stop();
                            file.WriteLine(i + " - " + n + " - " + " no grid - " + sw.Elapsed.TotalMilliseconds);

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

        [TestMethod]
        [TestCategory("Runtime analysis")]
        public void TestValidator()
        {
            ByteWrapper image1 = ImageConvert.ReturnByteWrapper(@"F:\temp\analysis\640x480\test_0.jpg");
            ImageValidator validator = new ImageValidator();
            try
            {
                List<int> dimensions = ReturnDimensions();
                Stopwatch sw = new Stopwatch();
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"f:\temp\runtime\validator_analysis.txt", true))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        sw.Restart();
                        validator.FileCreated(image1, EventArgs.Empty);
                        sw.Stop();
                        file.WriteLine(sw.Elapsed.TotalMilliseconds);
                    }
                }
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        [TestCategory("Runtime analysis")]
        public void LinqTesting()
        {
            try
            {
                List<int> dimensions = ReturnDimensions();
                Stopwatch sw = new Stopwatch();
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"f:\temp\runtime\linqanalysis.txt", true))
                {
                    for(int m = 0; m < dimensions.Count; m++)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            //do the minimum
                            BitmapWrapper bm1 = new BitmapWrapper(@"F:\temp\analysis\640x480\test_0.jpg");
                            BitmapWrapper bm2 = new BitmapWrapper(@"F:\temp\analysis\640x480\test_1.jpg");
                            PixelMatrix matrix = new PixelMatrix();
                            matrix.SearchWidth = dimensions[m];
                            matrix.Populate(bm1, bm2);

                            //LINQ
                            sw.Restart();
                            double min = matrix.MinChanged;
                            sw.Stop();
                            file.WriteLine("LINQ - min - " + m + " - " + sw.Elapsed.TotalMilliseconds);

                            sw.Restart();
                            double max = matrix.MaxChanged;
                            sw.Stop();
                            file.WriteLine("LINQ - max - " + m + " - " + sw.Elapsed.TotalMilliseconds);

                            sw.Restart();
                            double sum = matrix.SumChangedPixels;
                            sw.Stop();
                            file.WriteLine("LINQ - sum - " + m + " - " + sw.Elapsed.TotalMilliseconds);

                            //ITERATION
                            sw.Restart();
                            double min2 = 0;
                            for (int n = 0; n < matrix.Columns.Count; n++)
                            {
                                for (int k = 0; k < matrix.Columns[n].cells.Count; k++)
                                {
                                    if (matrix.Columns[n].cells[k].change < min2)
                                    {
                                        min2 = matrix.Columns[n].cells[k].change;
                                    }
                                }
                            }
                            sw.Stop();
                            file.WriteLine("ITERATION - min - " + m + " - " + sw.Elapsed.TotalMilliseconds);


                            sw.Restart();                           
                            double max2 = 0;
                            for (int n = 0; n < matrix.Columns.Count; n++)
                            {
                                for (int k = 0; k < matrix.Columns[n].cells.Count; k++)
                                {
                                    if (matrix.Columns[n].cells[k].change > max2)
                                    {
                                        max2 = matrix.Columns[n].cells[k].change;
                                    }
                                }
                            }
                            sw.Stop();
                            file.WriteLine("ITERATION - max - " + m + " - " + sw.Elapsed.TotalMilliseconds);

                            sw.Restart();
                            double sum2 = 0;                          
                            for (int n = 0; n < matrix.Columns.Count; n++)
                            {
                                for (int k = 0; k < matrix.Columns[n].cells.Count; k++)
                                {
                                    sum2 += matrix.Columns[n].cells[k].change;
                                }
                            }
                            sw.Stop();
                            file.WriteLine("ITERATION - sum - " + m + " - " + sw.Elapsed.TotalMilliseconds);

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
