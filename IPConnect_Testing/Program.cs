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
using ImageAnalysis;
using ImageAnalysis.Images;
using ImageAnalysis.Images.Bitmaps;
using ImageAnalysis.Images.Jpeg;
using ImageAnalysis.Analysis;
using ImageAnalysis.Streams;
using ImageAnalysis.MotionSensor;
using ImageAnalysis.Data;
using IPConnect_Testing.Testing;
using IPConnect_Testing.Testing.DataObjects;
using Microsoft.Owin.Hosting;

namespace IPConnect_Testing
{

    /// <summary>
    /// This static calling program works as the BUS
    /// </summary>

    class Program
    {
        //static string url = "http://192.168.0.2/axis-cgi/mjpg/video.cgi?date=1&clock=1&resolution=135x180";
        static string url = "http://192.168.0.8/axis-cgi/mjpg/video.cgi?resolution=480x360";
        //static string url = "http://192.168.0.2/axis-cgi/mjpg/video.cgi";
        //static string url = "http://localhost:8080/api/Mpeg/stream";
        // static string url = "http://192.168.0.2/axis-cgi/mjpg/video.cgi?date=1&clock=1";
        //static string url = "http://localhost:9000/api/Mpeg/Stream?id=1";

        static string username = "root";
        static string password = "root";
        static List<Bitmap> bitmaps = new List<Bitmap>(); //the converted bitmap's which are looked at 

        static ImageSaver imageSaver;

        static void Main(string[] args)
        {
            Write("IPConnect_Testing started");
            Console.Beep(1000,250);
            //StartWebService();
            //RunMotionTests_2a();
            //TestAllCapturesForLag();
            TestAllCaptures();
            Write("IPConnect_Testing finished");
            Console.ReadLine();

        }

        static void StartWebService()
        {
            WebApp.Start<Startup>(url: "http://localhost:9001/");
        }


        static void TestAllCaptures()
        {
            MotionSettingTesting motion = new MotionSettingTesting();
            motion.TestAllCaptures_SequentialSettingChanges(MotionSensorTypes.Motion2a);
        }

        static void TestAllCapturesForLag()
        {
            MotionLagTesting lagTesting = new MotionLagTesting();
            lagTesting.TestCapture_BothSyncs(MotionSensorTypes.Motion2a, "201621317307840");
        }

        static void RunMotionTests_2a()
        {
            var motion = new Testing.MotionSensor2aTest();
            motion.settings = new MotionSensorSettingsTest();
            motion.settings.asynchronous = true;
            //motion.settings.framesToSkip = 20;
            //motion.settings.horizontalPixelsToSkip = 4;
            //motion.settings.verticalPixelsToSkip = 4;
            motion.Run(url, username, password);
            // motion.Run("2016220121715998");
        }



        static void RunMotionTests_2b()
        {
            var motion = new Testing.MotionSensor2aTest();
            motion.Run("2016220121312251");
        }

        static void RunMotionSensor2_a(string sessKey)
        {
            //set up the extractor
            string uri = "http://localhost:9000/api/jpeg/0/" + sessKey;

            ImageExtractor imageExtractor = new ImageExtractor(uri, username, password);
            imageExtractor.framerateBroadcast += new ImageExtractor.FramerateBroadcastEvent(FramerateBroadcastEventHandler);

            //set the image saver
            ImageSaver imageSaver = new ImageSaver(@"F:\temp\MotionSensor\2.1\movement", "movement");

            //create the motion sensor, and listen for images
            MotionSensor_2a motionSensor = new MotionSensor_2a();
            motionSensor.motionDetected += new MotionSensor_2.MotionDetected(imageSaver.WriteBytesToFileAsync);

            //create the validator 
            ImageValidator imageValidator = new ImageValidator();
            imageValidator.ListenForImages(imageExtractor);
            imageValidator.imageValidated += new ImageValidator.ImageValidatedEvent(motionSensor.ImageCreatedAsync);//subscribe to events from the validator

            imageExtractor.Run();
        }

        static void RunMotionSensor2_b(string sessKey)
        {
            //set up the extractor
            string uri = "http://localhost:9000/api/jpeg/0/" + sessKey;

            ImageExtractor imageExtractor = new ImageExtractor(uri, username, password);
            imageExtractor.framerateBroadcast += new ImageExtractor.FramerateBroadcastEvent(FramerateBroadcastEventHandler);

            //set the image saver
            ImageSaver imageSaver = new ImageSaver(@"F:\temp\MotionSensor\2.2\movement", "movement");

            //create the motion sensor, and listen for images
            MotionSensor_2b motionSensor = new MotionSensor_2b();
            motionSensor.motionDetected += new MotionSensor_2.MotionDetected(imageSaver.WriteBytesToFileAsync);

            //create the validator 
            ImageValidator imageValidator = new ImageValidator();
            imageValidator.ListenForImages(imageExtractor);
            imageValidator.imageValidated += new ImageValidator.ImageValidatedEvent(motionSensor.ImageCreatedAsync);//subscribe to events from the validator

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
        static void ValidImageEventHandler(ByteWrapper img, EventArgs e)
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

        private static void Write(String s)
        {
            Console.WriteLine(DateTime.Now + " - " + s);
        }

        private static void JpegAnalysis()
        {
            //create am


            JpegAnalysis jpegAnalysis = new JpegAnalysis();

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

            //PixelMatrix matrix = new JpegComparision(@"f:\temp\analysis\movement\test_1.jpg", @"f:\temp\analysis\movement\test_2.jpg").ReturnPixelMatrix();

            PixelMatrix matrix = new JpegComparision(@"F:\captures\0\2016213162849209\1\test_281.jpg", @"f:\temp\analysis\movement\test_306.jpg").ReturnPixelMatrix();

            //matrix.SetReducedColumns();

            //Bitmap changeImage = matrix.DrawPixelChanges();
           // matrix.DumpToText(@"f:\temp\analysis\movement\pixelchanges.txt");
            //changeImage.Save(@"f:\temp\analysis\movement\pixelchanges_grey.bmp");
            matrix.DumpReducedToText(@"f:\temp\MotionSensor\2.2\pixelinfo.txt");


            //// Bitmap bm = new JpegComparision(new JPEG(@"f:\temp\analysis\movement\test_474.jpg"), new JPEG(@"f:\temp\analysis\movement\test_506.jpg")).ColourPixelChanges(Color.IndianRed);
            //Bitmap bm = new JpegComparision(@"f:\temp\analysis\movement\test_1.jpg", @"f:\temp\analysis\movement\test_0.jpg").ColourPixelChanges(Color.IndianRed);
            //bm.Save(@"f:\temp\analysis\movement\pixelchanges2.bmp");

        }

        //runs method 2 on multiple images
        //takes the range difference across many images
        private static void RunPixelChanges_3()
        {
            //movement
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<String> ranges = new List<string>();
            var files = Directory.GetFiles(@"f:\temp\analysis\multiple_movement");
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"f:\temp\analysis\multiple_movement\ranges.txt", true))
            {
                for (int i = 1; i < files.Length; i++)
                {
                    PixelMatrix matrix = new PixelMatrix();
                    matrix.Populate(files[i - 1], files[i]);

                    file.WriteLine(i - 1 + "~" + i + "~" + (matrix.MaxChanged - matrix.MinChanged));
                }//each file
            }//write to file

            Console.WriteLine(sw.Elapsed.TotalSeconds);

            //no movement
            ranges = new List<string>();
            files = Directory.GetFiles(@"f:\temp\analysis\multiple_no_movement");
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"f:\temp\analysis\multiple_no_movement\ranges.txt", true))
            {
                for (int i = 1; i < files.Length; i++)
                {
                    PixelMatrix matrix = new PixelMatrix();
                    matrix.Populate(files[i - 1], files[i]);

                    file.WriteLine(i - 1 + "~" + i + "~" + (matrix.MaxChanged - matrix.MinChanged));
                }//each file
            }//write to file

            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalSeconds);

        }//RunPixelChanges_3

        private static void Motion2b_analysis()
        {
            PixelMatrix matrix = new PixelMatrix();
            matrix.GridSystemOn = true;
            matrix.Populate(@"F:\temp\MotionSensor\2.2\test_101.jpg", @"F:\temp\MotionSensor\2.2\test_128.jpg");
            matrix.DumpGridToText(@"F:\temp\MotionSensor\2.2\grid.txt");

        }

    }
}
