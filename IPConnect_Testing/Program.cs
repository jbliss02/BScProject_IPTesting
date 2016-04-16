using System;
using System.Collections.Generic;
using System.Configuration;
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
using IPConnect_Testing.DAL;
using Microsoft.Owin.Hosting;

namespace IPConnect_Testing
{

    /// <summary>
    /// This static calling program runs test programs that cannot feasibily be run through unit tests
    /// </summary>

    class Program
    {
        static string url = "http://192.168.0.8/axis-cgi/mjpg/video.cgi";

        static string username = "root";
        static string password = "root";
        static List<Bitmap> bitmaps = new List<Bitmap>(); //the converted bitmap's which are looked at 

        static ImageSaver imageSaver;

        static void Main(string[] args)
        {
            Write("IPConnect_Testing started");
            Console.Beep(1000,250);
            TestStartup();
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
            lagTesting.TestCapture_BothSyncs(MotionSensorTypes.Motion2a);
        }

        /// <summary>
        /// Records a number of live captures across a range of time periods, and saves to the 
        /// capture database. Later used for lag testing.
        /// </summary>
        static void CreateTimedTests()
        {
            //1 to 20 minutes
            for(int i = 1; i < 21; i++)
            {
                Write("Setting up " + i + " minute test");
                //set up the extractor
                ImageExtractor imageExtractor = new ImageExtractor(url, username, password);

                //set up the save file object
                imageSaver = new ImageSaver(0, 1);
                imageExtractor.imageCreated += new ImageExtractor.ImageCreatedEvent(imageSaver.ImageCreatedAsync);
                imageExtractor.framerateBroadcast += new ImageExtractor.FramerateBroadcastEvent(FramerateBroadcastEventHandler);

                imageExtractor.Run(i);

                CaptureDbTest db = new CaptureDbTest(ConfigurationManager.ConnectionStrings["LOCALDB"].ConnectionString);
                db.AddTimedCapture(imageSaver.captureId, i);
            
            }
            
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

        /// <summary>
        /// Tests the MotionSensorStartup wrapper which is used to fire a motion
        /// detection session, mimics what would come in from a webservice
        /// </summary>
        static void TestStartup()
        {
            MotionSensorSetup setup = new MotionSensorSetup();
            setup.camera = new ImageAnalysis.Camera.CameraModel();
            setup.camera.cameraIpAddress = "192.168.0.8";

            setup.emailAlarm = new ImageAnalysis.Alarms.EmailAlarm();
            setup.emailAlarm.emailAddress = "james.bliss@outlook.com";

            MotionSensorStartup motionSensor = new MotionSensorStartup(setup);

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
