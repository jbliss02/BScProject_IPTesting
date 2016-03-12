using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;
using System.Media;
using System.Diagnostics;
using System.Configuration;
using ImageAnalysis.Images;
using ImageAnalysis.Images.Bitmaps;
using ImageAnalysis.Images.Jpeg;
using ImageAnalysis.Analysis;
using ImageAnalysis.Streams;
using ImageAnalysis;
using ImageAnalysis.MotionSensor;
using ImageAnalysis.Data;
using Tools;
using IPConnect_Testing.Testing.DataObjects;

namespace IPConnect_Testing.Testing
{
    public class MotionSensor2aTest : IDisposable
    {
        string saveFilePath;
        string captureId;
        string saveDirectory { get { return saveFilePath + @"\" + captureId; } }
        ImageExtractor imageExtractor;
        public List<Int32> movementFrames { get; private set; }
        public MotionSensorSettingsTest settings { get; set; }
        public int expectedFrames { get; set; }//the number of frames expected in capture passed in
        public DateTime imageExtractionEnd { get; set; }//recordsthe time that the ImgaeExtractor says broadcast is finisged

        //test and lag timinig
        public Boolean timedTest { get; set; }// whether the runtime needs to be recorded in this test
        public Stopwatch testTimer { get; set; }


        /// <summary>
        /// Run, with a captureId will make the system stream an old capture
        /// </summary>
        /// <param name="captureId"></param>
        public void Run(string captureId)
        {
            this.captureId = captureId;
            string uri = "http://localhost:9000/api/jpeg/0/" + captureId;
            imageExtractor = new ImageExtractor(uri, "root", "root");
            movementFrames = new List<int>();
            saveFilePath = ConfigurationManager.AppSettings["MotionSaveLocation"] + @"\" + settings.HashCode;
            Go();
        }

        /// <summary>
        /// Run will use a live stream
        /// </summary>
        public void Run(string url, string username, string password)
        {
            imageExtractor = new ImageExtractor(url, "root", "root");
            movementFrames = new List<int>();
            captureId = Tools.ExtensionMethods.DateStamp();
            saveFilePath = ConfigurationManager.AppSettings["MotionSaveLocation"];
            Go();
        }

        /// <summary>
        /// called once the Setups are complete
        /// </summary>
        private void Go()
        {
            //create the motion sensor, and listen for images
            MotionSensor_2a motionSensor = new MotionSensor_2a();
            motionSensor.settings = settings == null ? motionSensor.settings = new MotionSensorSettings() : motionSensor.settings = settings;

            motionSensor.motionDetected += new MotionSensor_2.MotionDetected(MotionDetected); //set up the motion detector hook
            motionSensor.logging.LoggingOn = true;

            //create the validator 
            ImageValidator imageValidator = new ImageValidator();
            imageValidator.ListenForImages(imageExtractor);

            imageExtractor.asyncrohous = settings.asynchronous;

            if (settings.asynchronous)
            {
                imageValidator.imageValidated += new ImageValidator.ImageValidatedEvent(motionSensor.ImageCreatedAsync); //subscribe to events from the validator
            }
            else
            {
                imageValidator.imageValidated += new ImageValidator.ImageValidatedEvent(motionSensor.ImageCreated); //subscribe to events from the validator
            }

            if (timedTest) { testTimer = new Stopwatch(); testTimer.Start(); }

            imageExtractor.Run();

            //if here and async then the motion detector is likely still going        
            if (settings.asynchronous && timedTest)
            {
                imageExtractionEnd = DateTime.Now;
                while (motionSensor.logging.imagesReceived < expectedFrames)
                {
                    System.Threading.Thread.Sleep(250);
                }

                testTimer.Stop();
                //DateTime motionFinished = DateTime.Now;
                //TimeSpan span = motionFinished - imageExtractionEnd;
                //Console.WriteLine(span.Seconds);
            }
            else
            {
                testTimer.Stop();
            }


        }

        private async void MotionDetected(ByteWrapper image, EventArgs e)
        {
            await Task.Run(() =>
            {
                Console.Beep(1000, 250);

                SaveMotionFile(image);
                if (movementFrames != null)
                {
                    movementFrames.Add(image.sequenceNumber);
                }
            });              
        }

        private async void SaveMotionFile(ByteWrapper image)
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists(saveDirectory)) { CreateSaveDirectory(); }
                ImageSaver.WriteBytesToFileAsync(image, saveDirectory + @"\movement_" + image.sequenceNumber + ".jpg");
            });
        }

        private void CreateSaveDirectory()
        {
            Directory.CreateDirectory(saveDirectory);
        }

        public void Dispose()
        {
            imageExtractor = null;
            movementFrames = null;
            settings = null;

        }

    }
}
