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
using ImageAnalysis.Images;
using ImageAnalysis.Images.Bitmaps;
using ImageAnalysis.Images.Jpeg;
using ImageAnalysis.Analysis;
using ImageAnalysis.Streams;
using ImageAnalysis;
using ImageAnalysis.MotionSensor;

namespace IPConnect_Testing.Testing
{
    public class MotionSensor2aTest
    {
        string saveFilePath;
        string captureKey;

        string saveDirectory { get { return saveFilePath + @"\" + captureKey; } }

        ImageSaver imageSaver;

        public void Run(string captureKey)
        {
            //set up the extractor
            this.captureKey = captureKey;
            string uri = "http://localhost:9000/api/jpeg/0/" + captureKey;

            ImageExtractor imageExtractor = new ImageExtractor(uri, "root", "root");

            //create the motion sensor, and listen for images
            MotionSensor_2a motionSensor = new MotionSensor_2a();
            motionSensor.motionDetected += new MotionSensor_2.MotionDetected(MotionDetected);
            motionSensor.logging.LoggingOn = true;

            //create the validator 
            ImageValidator imageValidator = new ImageValidator();
            imageValidator.ListenForImages(imageExtractor);
            imageValidator.imageValidated += new ImageValidator.ImageValidatedEvent(motionSensor.ImageCreated); //subscribe to events from the validator (testing so sync only)

            //saver
            saveFilePath = @"f:\temp\analysis\motion";

            imageExtractor.Run();

            motionSensor.logging.WriteToLog(@"f:\temp\analysis\227\matrixinfo.txt");
            Console.WriteLine("Finished");



        }
        
        private void MotionDetected(ByteWrapper image, EventArgs e)
        {
            SaveMotionFile(image);

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

    }
}
