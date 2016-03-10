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
        //static string url = "http://192.168.0.8/axis-cgi/mjpg/video.cgi?resolution=480x360";
        static string url = "http://192.168.0.8/axis-cgi/mjpg/video.cgi";
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
            TestAllCapturesForLag();
            //TestAllCaptures();
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
            lagTesting.TestCapture_BothSyncs(MotionSensorTypes.Motion2a, "201613113444883");
        }

        /// <summary>
        /// Records a number of live captures across a range of time periods, and saves to the 
        /// capture database. Later used for lag testing.
        /// </summary>
        static void CreateTimedTests()
        {

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

    }
}
