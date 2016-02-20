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
                        for (int n = 0; n < 200; n++)
                        {
                            BitmapWrapper bm1 = new BitmapWrapper(@"F:\temp\analysis\640x480\test_0.jpg");
                            BitmapWrapper bm2 = new BitmapWrapper(@"F:\temp\analysis\640x480\test_1.jpg");

                            sw.Restart();
                            PixelMatrix matrix = new PixelMatrix();
                            matrix.SearchWidth = dimensions[i];
                            matrix.GridSystemOn = true;
                            matrix.LinkCompare = true;
                            matrix.Populate(bm1, bm2);

                            sw.Stop();
                            file.WriteLine(i + " - " + n + " - " + " grid - " + sw.Elapsed.TotalMilliseconds);

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
        /// Test what happens when we do not search every pixel
        /// </summary>
        [TestMethod]
        [TestCategory("Runtime analysis")]
        public void PixelMatrix_skips()
        {
            try
            {
                List<int> dimensions = ReturnPixelJumps();

                Stopwatch sw = new Stopwatch();
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"f:\temp\runtime\pixelmatrixanalysis_jumps.txt", true))
                {
                    for (int i = 0; i < dimensions.Count; i++)
                    {
                        for (int n = 0; n < 200; n++)
                        {
                            BitmapWrapper bm1 = new BitmapWrapper(@"F:\temp\analysis\640x480\test_0.jpg");
                            BitmapWrapper bm2 = new BitmapWrapper(@"F:\temp\analysis\640x480\test_1.jpg");

                            sw.Restart();
                            PixelMatrix matrix = new PixelMatrix();
                            matrix.WidthSearchOffset = dimensions[i];
                            matrix.Populate(bm1, bm2);

                            sw.Stop();
                            file.WriteLine(i + " - " + n + " - " + " grid - " + sw.Elapsed.TotalMilliseconds);

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
        /// Runs the whole end to end motion sensor and logs the response times
        /// </summary>
        [TestMethod]
        [TestCategory("Runtime analysis")]
        public void MotionSensor2a()
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

        /// <summary>
        /// Runs analysis on MotionSensor2b
        /// </summary>
        [TestMethod]
        [TestCategory("Runtime analysis")]
        public void MotionSensor2b()
        {
            try
            {
                List<int> dimensions = ReturnDimensions();
                Stopwatch sw = new Stopwatch();
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"f:\temp\runtime\motion2b_analysis.txt", true))
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


                            MotionSensor_2b motion = new MotionSensor_2b();
                            motion.CreateDummyThreshold(4, 4);
                            sw.Restart();
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
                    for (int m = 0; m < dimensions.Count; m++)
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
                                for (int k = 0; k < matrix.Columns[n].Cells.Count; k++)
                                {
                                    if (matrix.Columns[n].Cells[k].change < min2)
                                    {
                                        min2 = matrix.Columns[n].Cells[k].change;
                                    }
                                }
                            }
                            sw.Stop();
                            file.WriteLine("ITERATION - min - " + m + " - " + sw.Elapsed.TotalMilliseconds);


                            sw.Restart();
                            double max2 = 0;
                            for (int n = 0; n < matrix.Columns.Count; n++)
                            {
                                for (int k = 0; k < matrix.Columns[n].Cells.Count; k++)
                                {
                                    if (matrix.Columns[n].Cells[k].change > max2)
                                    {
                                        max2 = matrix.Columns[n].Cells[k].change;
                                    }
                                }
                            }
                            sw.Stop();
                            file.WriteLine("ITERATION - max - " + m + " - " + sw.Elapsed.TotalMilliseconds);

                            sw.Restart();
                            double sum2 = 0;
                            for (int n = 0; n < matrix.Columns.Count; n++)
                            {
                                for (int k = 0; k < matrix.Columns[n].Cells.Count; k++)
                                {
                                    sum2 += matrix.Columns[n].Cells[k].change;
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
        /// Compares Matrix creation by Array and by list
        /// </summary>
        [TestMethod]
        [TestCategory("Runtime analysis")]
        public void ListTesting()
        {
            try
            {
                List<int> dimensions = ReturnDimensions();
                Stopwatch sw = new Stopwatch();

                BitmapWrapper bm1 = new BitmapWrapper(@"F:\temp\analysis\640x480\test_0.jpg");

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"f:\temp\runtime\listanalysis.txt", true))
                {
                    for (int i = 0; i < dimensions.Count; i++)
                    {
                        for (int n = 0; n < 200; n++)
                        {
                            //Array
                            sw.Restart();
                            PixelColumnArray[] array = new PixelColumnArray[bm1.bitmap.Width];
                            for(int w = 0; w < bm1.bitmap.Width; w++)
                            {
                                array[w] = new PixelColumnArray();
                                array[w].Cells = new PixelCellArray[bm1.bitmap.Height];

                                for (int h = 0; h < bm1.bitmap.Height; h++)
                                {
                                    array[w].Cells[h] = new PixelCellArray();
                                    array[w].Cells[h].colour = 11111;
                                }                                       
                            }
                            sw.Stop();
                            file.WriteLine(i + " - " + n + " - " + " array - " + sw.Elapsed.TotalMilliseconds);

                            //List
                            sw.Restart();
                            List<PixelColumn> list = new List<PixelColumn>();
                            for (int w = 0; w < bm1.bitmap.Width; w++)
                            {
                                PixelColumn column = new PixelColumn();

                                for (int h = 0; h < bm1.bitmap.Height; h++)
                                {
                                    PixelCell cell = new PixelCell();
                                    cell.colour = 11111;
                                    column.Cells.Add(cell);
                                }

                                list.Add(column);
                            }
                            sw.Stop();
                            file.WriteLine(i + " - " + n + " - " + " list - " + sw.Elapsed.TotalMilliseconds);

                        }//test iterations
                    }//dimensions
                }//write to file


                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        [TestCategory("Runtime analysis")]
        public void BitmapGetColour()
        {
            BitmapWrapper bm1 = new BitmapWrapper(@"F:\temp\analysis\640x480\test_0.jpg");

            Stopwatch sw = new Stopwatch();
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"f:\temp\runtime\getpixel.txt", true))
            {
                List<Int32> lst = ReturnPixelSizes();

                for (int k = 0; k < 10; k++)
                {
                    for(int n = 0; n < lst.Count; n++)
                    {
                        //sw.Start();
                        //for (int i = 0; i < lst[n]; i++)
                        //{

                        //}
                        //sw.Stop();
                        //file.WriteLine(sw.Elapsed.TotalMilliseconds);

                        sw.Restart();
                        for(int i = 0; i < lst[n]; i++)
                        {
                            Int64 x = Int64.Parse(bm1.bitmap.GetPixel(12, 59).Name, System.Globalization.NumberStyles.HexNumber);
                        }
                        sw.Stop();
                        file.WriteLine(k + " - " + lst[n] + " - "+ sw.Elapsed.TotalMilliseconds);
                    }
                }



            }



        }

        [TestMethod]
        [TestCategory("Runtime analysis")]
        public void Motion2aSubequent()
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


                            PixelMatrix dummy = new PixelMatrix();
                            dummy.LinkCompare = true;
                            dummy.SearchWidth = dimensions[i];
                            dummy.Populate(new BitmapWrapper(ImageConvert.ReturnBitmap(image1.bytes)), new BitmapWrapper(ImageConvert.ReturnBitmap(image2.bytes)));
                            

                            sw.Restart();
                            MotionSensor_2a motion = new MotionSensor_2a();
                            motion.ThresholdSet = true;
                            motion.LinkCompare = true;
                            motion.SearchWidth = dimensions[i];
                            motion.Comparison = dummy.Comparision;

                            motion.ImageCreated(image1, EventArgs.Empty);
                            motion.ImageCreated(image2, EventArgs.Empty);

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

        /// <summary>
        /// Checks that compare is being called the right number of times
        /// </summary>
        [TestMethod]
        [TestCategory("Runtime")]
        public void Motion2aMultipleCompare()
        {
            try
            {
                //images in memory when sent to the 
                ByteWrapper image1 = ImageConvert.ReturnByteWrapper(@"F:\temp\analysis\640x480\test_0.jpg");
                ByteWrapper image2 = ImageConvert.ReturnByteWrapper(@"F:\temp\analysis\640x480\test_1.jpg");

                PixelMatrix dummy = new PixelMatrix();
                dummy.LinkCompare = true;
                dummy.Populate(new BitmapWrapper(ImageConvert.ReturnBitmap(image1.bytes)), new BitmapWrapper(ImageConvert.ReturnBitmap(image2.bytes)));

                MotionSensor_2a motion = new MotionSensor_2a();
                motion.ThresholdSet = true;
                motion.LinkCompare = true;
                motion.Comparison = dummy.Comparision;

                motion.ImageCreated(image1, EventArgs.Empty);
                motion.ImageCreated(image2, EventArgs.Empty);

                motion.ImageCreated(image1, EventArgs.Empty);
                motion.ImageCreated(image2, EventArgs.Empty);
                  

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

        private List<int> ReturnPixelJumps()
        {
            //width is 480 in every case for consistent fall in pixel numbers
            List<int> result = new List<int>();
            result.Add(32);
            result.Add(16);
            result.Add(8);
            result.Add(4);
            result.Add(2);
            result.Add(1);
            return result;
        }

        private List<int> ReturnPixelSizes()
        {
            List<int> result = new List<int>();
            result.Add(307200);
            result.Add(153600);
            result.Add(76800);
            result.Add(38400);
            result.Add(19200);
            result.Add(9600);
            return result;
        }
    }
}
