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
using IPConnect_Testing.Images;


namespace IPConnect_Testing
{

    /// <summary>
    /// This static calling program works as the BUS
    /// </summary>

    class Program
    {
        // static string url = "http://192.168.0.3/axis-cgi/mjpg/video.cgi?date=1&clock=1&resolution=320x240";
        static string url = "http://192.168.0.3/axis-cgi/mjpg/video.cgi";
        //static string url = "http://localhost:8080/api/Mpeg/stream";
        // static string url = "http://192.168.0.3/axis-cgi/mjpg/video.cgi?date=1&clock=1";
        //static string url = "http://localhost:9000/api/Mpeg/Stream?id=1";



        static string username = "root";
        static string password = "root";
        static List<Bitmap> bitmaps = new List<Bitmap>(); //the converted bitmap's which are looked at 
        static MotionSensor motionSensor;
        static ImageSaver imageSaver;
        static StreamAnalyser streamAnalyser;

        static void Main(string[] args)
        {
            Write("IPConnect started");
           // ExtractImages();
            Console.WriteLine("Finished");
            Console.ReadLine();
            //RunMotionSensor();
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

            ImageExtractor imageExtractor = new ImageExtractor(url, username, password);

            //create the validator and subs
            ImageValidator imageValidator = new ImageValidator();
            imageValidator.ListenForImages(imageExtractor);

            //subscribe to events from the validator (to and analyse)
            imageValidator.imageValidated += new ImageValidator.ImageValidatedEvent(ValidImageEventHandler);

            //set up the save file object
            imageSaver = new ImageSaver(0, 1);

            //set up image logger
            streamAnalyser = new StreamAnalyser(@"f:\temp\imageLogger.txt", true);

            imageExtractor.Run();
        }


        /// <summary>
        /// Subscribes to a Image Validated event, then calls various other tied in methods
        /// </summary>
        /// <param name="img"></param>
        /// <param name="e"></param>
        static void ValidImageEventHandler(byte[] img, EventArgs e)
        {
            // Task saveImage = new ImageSaver(0).FileCreatedAsync(img, e);

            Task saveImage = imageSaver.ImageCreatedAsync(img, e);

            Task streamAnalysis = streamAnalyser.ImageCreatedAsync(img, e);
            

            ////Extract the bitmap and do some testing - this needs to be moved to a thread so it is asyncronhous
            //bitmaps.Add(new ImageConverter().ReturnBitmap(img));
  
            //if(bitmaps.Count > 20 && motionSensor != null) {
            //    motionSensor.CheckForMotion(bitmaps);
            //    bitmaps = new List<Bitmap>();
            //}
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

    }
}
