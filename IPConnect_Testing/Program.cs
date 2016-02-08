using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Media;
using System.Diagnostics;
using IPConnect_Testing.Images;
using IPConnect_Testing.Images.Bitmaps;
using IPConnect_Testing.Analysis;
using IPConnect_Testing.Streams;

namespace IPConnect_Testing
{

    /// <summary>
    /// This static calling program works as the BUS
    /// </summary>

    class Program
    {
        //static string url = "http://192.168.0.2/axis-cgi/mjpg/video.cgi?date=1&clock=1&resolution=135x180";
        static string url = "http://192.168.0.2/axis-cgi/mjpg/video.cgi?resolution=480x360";
        //static string url = "http://192.168.0.2/axis-cgi/mjpg/video.cgi";
        //static string url = "http://localhost:8080/api/Mpeg/stream";
        // static string url = "http://192.168.0.2/axis-cgi/mjpg/video.cgi?date=1&clock=1";
        //static string url = "http://localhost:9000/api/Mpeg/Stream?id=1";

        static string username = "root";
        static string password = "root";
        static List<Bitmap> bitmaps = new List<Bitmap>(); //the converted bitmap's which are looked at 
        static MotionSensor motionSensor;
        static ImageSaver imageSaver;

        static void Main(string[] args)
        {
            Write("IPConnect started");
           // RunPixelChanges_2();
            ExtractImages();
            Console.WriteLine("Finished");
            Console.ReadLine();

        }

        static void ExtractMjpegHeader()
        {
            MJPEG mjpeg = new MJPEG(@"F:\temp\mjpeg\combined_0_1.avi");

            Console.WriteLine(mjpeg.HeaderString());
            Console.ReadLine();

        }

        static void TestMjpegBoundaryBytes(string source)
        {
            List<byte[]> boundaryBytes = new MJPEG(source).JpegBoundaryBytes();

            for(int i = 0; i < boundaryBytes.Count; i++)
            {
                byte[] bytes = boundaryBytes[i];

                //test 1, ends with 2E 00 00
                string s = BitConverter.ToString(bytes);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"f:\temp\boundary extract.txt", true))
                {
                    file.WriteLine(s);
                }

                var x = "nd";
            }
            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        static void RunMotionSensor()
        {

            //System.Threading.Thread.Sleep(10000);

            Console.WriteLine(DateTime.Now + " - Started");
            SystemSounds.Exclamation.Play();

            ImageExtractor imageExtractor = new ImageExtractor(url, username, password);

            //create the validator and subs
            ImageValidator imageValidator = new ImageValidator();
            imageValidator.ListenForImages(imageExtractor);

            //subscribe to events from the validator (to and analyse)
            imageValidator.imageValidated += new ImageValidator.ImageValidatedEvent(ValidImageEventHandler);

            //sset up the save file object
            imageSaver = new ImageSaver(0, 1);

            //set up the montion sensor
            motionSensor = new MotionSensor(5);

            imageExtractor.Run();
        }

        static void ExtractImages()
        {
            //set up the extractor
            ImageExtractor imageExtractor = new ImageExtractor(url, username, password);
            imageExtractor.framerateBroadcast += new ImageExtractor.FramerateBroadcastEvent(FramerateBroadcastEventHandler);

            //create the validator 
            ImageValidator imageValidator = new ImageValidator();
            imageValidator.ListenForImages(imageExtractor);        
            imageValidator.imageValidated += new ImageValidator.ImageValidatedEvent(ValidImageEventHandler);//subscribe to events from the validator

            //set up the save file object
            imageSaver = new ImageSaver(0, 1);

            imageExtractor.Run();
        }

        /// <summary>
        /// Subscribes to a Image Validated event, then calls various other tied in methods
        /// </summary>
        /// <param name="img"></param>
        /// <param name="e"></param>
        static void ValidImageEventHandler(byte[] img, EventArgs e)
        {
            if(imageSaver != null)
            {
                Task saveImage = imageSaver.ImageCreatedAsync(img);
            }

           
            //Task streamAnalysis = streamAnalyser.ImageCreatedAsync(img, e);
            

            ////Extract the bitmap and do some testing - this needs to be moved to a thread so it is asyncronhous
            //bitmaps.Add(new ImageConverter().ReturnBitmap(img));
  
            //if(bitmaps.Count > 20 && motionSensor != null) {
            //    motionSensor.CheckForMotion(bitmaps);
            //    bitmaps = new List<Bitmap>();
            //}
        }

        static void FramerateBroadcastEventHandler(double framerate, EventArgs args)
        {
            if(imageSaver != null)
            {
                imageSaver.framerates.Add(framerate);
            }
        }

        private static UInt64 Hash()
        {
            //this needs to be changed and validated
            ulong hash = (UInt64)(int)DateTime.Now.Kind;
            return (hash << 62) | (UInt64) DateTime.Now.Ticks;
        }

        private static void fileInfo()
        {
            DateTime s = File.GetCreationTime(@"F:\temp\alarm\4469890.bmp");
        }

        private static void Write(String s)
        {
            Console.WriteLine(DateTime.Now + " - " + s);
        }

        private static void JpegAnalysis()
        {
            //create am


            JpegAnalysis jpegAnalysis = new Analysis.JpegAnalysis();

            //640x480
            Console.WriteLine(Environment.NewLine +  "640x480");
            JPEG jpeg = new JPEG(@"f:\temp\analysis\640x480\test_0.jpg");
            Console.WriteLine(jpegAnalysis.MsToBitmap(jpeg).ToString());
            Console.WriteLine(jpegAnalysis.MsToBitmapAndSumPixels(jpeg).ToString());

            //320x240
            Console.WriteLine(Environment.NewLine  + "320x240");
            jpeg = new JPEG(@"f:\temp\analysis\320x240\test_0.jpg");
            Console.WriteLine(jpegAnalysis.MsToBitmap(jpeg).ToString());
            Console.WriteLine(jpegAnalysis.MsToBitmapAndSumPixels(jpeg).ToString());

            //160x120
            Console.WriteLine(Environment.NewLine +  "160x120");
            jpeg = new JPEG(@"f:\temp\analysis\160x120\test_0.jpg");
            Console.WriteLine(jpegAnalysis.MsToBitmap(jpeg).ToString());
            Console.WriteLine(jpegAnalysis.MsToBitmapAndSumPixels(jpeg).ToString());

            //60x80
            Console.WriteLine(Environment.NewLine +  "60x80");
            jpeg = new JPEG(@"f:\temp\analysis\60x80\test_0.jpg");
            Console.WriteLine(jpegAnalysis.MsToBitmap(jpeg).ToString());
            Console.WriteLine(jpegAnalysis.MsToBitmapAndSumPixels(jpeg).ToString());

        }

        //runs method 2 on 2 images
        private static void RunPixelChanges_2()
        {
            //Takes two images, works out the pixel difference, and draws a red change map

            PixelMatrix matrix = new JpegComparision(@"f:\temp\analysis\movement\test_1.jpg", @"f:\temp\analysis\movement\test_2.jpg").ReturnPixelMatrix();

            matrix.SetReducedColumns();



            Bitmap changeImage = matrix.DrawPixelChanges();
           // matrix.DumpToText(@"f:\temp\analysis\movement\pixelchanges.txt");
            changeImage.Save(@"f:\temp\analysis\movement\pixelchanges_grey.bmp");
            matrix.DumpReducedToText(@"f:\temp\analysis\movement\reducedpixelchanges.txt");


            //// Bitmap bm = new JpegComparision(new JPEG(@"f:\temp\analysis\movement\test_474.jpg"), new JPEG(@"f:\temp\analysis\movement\test_506.jpg")).ColourPixelChanges(Color.IndianRed);
            //Bitmap bm = new JpegComparision(@"f:\temp\analysis\movement\test_1.jpg", @"f:\temp\analysis\movement\test_0.jpg").ColourPixelChanges(Color.IndianRed);
            //bm.Save(@"f:\temp\analysis\movement\pixelchanges2.bmp");

        }

        //runs method 2 on multiple images
        //takes the range difference across many images
        private static void RunPixelChanges_3()
        {
            //no movement
            List<String> ranges = new List<string>();
            var files = Directory.GetFiles(@"f:\temp\analysis\multiple_movement");

            for(int i = 1; i < files.Length; i++)
            {

            }

        }//RunPixelChanges_3

    }
}
