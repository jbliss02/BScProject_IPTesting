using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using IPConnect_Testing.Images;
using IPConnect_Testing.Images.Bitmaps;
using IPConnect_Testing.Streams;

namespace IPConnect_Testing.Analysis
{
    /// <summary>
    /// Compares two jpeg images
    /// </summary>
    public class JpegAnalysis
    {

        /// <summary>
        /// Returns the millseconds to turn a in memory JPEG into a Bitmap
        /// </summary>
        /// <returns></returns>
        public double MsToBitmap(JPEG jpeg)
        {
            jpeg.Bytes(); //gets the bytes into memory
            Stopwatch sw = new Stopwatch();
            sw.Start();
            BitmapWrapper wrapper = new BitmapWrapper(jpeg.ReturnBitmap());
            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }

        /// <summary>
        /// Returns the milliseconds to turn a in memory JPEG into a Bitmap,
        /// and total all the pixels within the bitmap
        /// </summary>
        /// <returns></returns>
        public double MsToBitmapAndSumPixels(JPEG jpeg)
        {
            jpeg.Bytes(); //gets the bytes into memory
            Stopwatch sw = new Stopwatch();
            sw.Start();
            BitmapWrapper wrapper = new BitmapWrapper(jpeg.ReturnBitmap());
            wrapper.pixelAnalysis.SumRGB();
            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }

        public double TotalPixels(JPEG jpeg)
        {
            jpeg.Bytes(); //gets the bytes into memory
            BitmapWrapper wrapper = new BitmapWrapper(jpeg.ReturnBitmap());
            return wrapper.bitmap.Height * wrapper.bitmap.Width;
        }

        /// <summary>
        /// Runs a number of single extractions from the IP camera
        /// Times the change to Bitmap and count of the pixels for
        /// These images
        /// </summary>
        public void RunBenchmarking(string logfile)
        {
            //extract the images
            List<String> saveLocations = new List<string>();
            string url = "http://192.168.0.2/axis-cgi/mjpg/video.cgi?resolution=";

            List<String[]> resolutions = ReturnResolutions();

            for(int i = 0; i < resolutions.Count; i++)
            {
                string actualurl = url + resolutions[i][0] + "x" + resolutions[i][1];

                //set up the extractor
                ImageExtractor imageExtractor = new ImageExtractor(actualurl, "root", "root");

                //set up the save file object
                ImageSaver imageSaver = new ImageSaver(0, 1);
                saveLocations.Add(imageSaver.CaptureDirectory);

                //create the validator 
                ImageValidator imageValidator = new ImageValidator();
                imageValidator.ListenForImages(imageExtractor);
                imageValidator.imageValidated += new ImageValidator.ImageValidatedEvent(imageSaver.ImageCreatedAsync);//subscribe to events from the validator

                imageExtractor.Run(true);

                System.Threading.Thread.Sleep(5000); //wait 5 seconds to let the async requests complete
            }

            //do the analysis
            for(int i = 0; i < saveLocations.Count; i++)
            {
                string header = resolutions[i][0] + "x" + resolutions[i][1];
                Console.WriteLine(Environment.NewLine + resolutions[i][0] + "x" + resolutions[i][1]);
                JPEG jpeg = new JPEG(saveLocations[i] + @"\1\test_0.jpg");

                string msToBitmap = MsToBitmap(jpeg).ToString();
                string msToBitmapAndSumPixels = MsToBitmapAndSumPixels(jpeg).ToString();
                string totalPixels = TotalPixels(jpeg).ToString();

                Console.WriteLine(msToBitmap);
                Console.WriteLine(msToBitmapAndSumPixels);
                Console.WriteLine(totalPixels);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(logfile, true))
                {
                    file.WriteLine(header);
                    file.WriteLine(msToBitmap);
                    file.WriteLine(msToBitmapAndSumPixels);
                    file.WriteLine(totalPixels);
                    file.WriteLine("------------------------------");
                }

            }

        }//RunBenchmarking

        private List<String[]> ReturnResolutions()
        {
            List<String[]> ret = new List<string[]>();
            ret.Add(new string[2] { "640", "480" });
            //ret.Add(new string[2] { "480", "360" });
            //ret.Add(new string[2] { "320", "240" });
            //ret.Add(new string[2] { "240", "180" });
            //ret.Add(new string[2] { "160", "120" });

            return ret;
        }
    }
}
