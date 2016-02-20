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
using IPConnect_Testing.Images;
using IPConnect_Testing.Images.Bitmaps;
using IPConnect_Testing.Images.Jpeg;
using IPConnect_Testing.Analysis;
using IPConnect_Testing.Streams;
using IPConnect_Testing;
using IPConnect_Testing.MotionSensor;

namespace MotionSensorTesting
{
    public class MotionSensor2aTest
    {
        public void Run(string captureKey)
        {
            //set up the extractor
            string uri = "http://localhost:9000/api/jpeg/0/" + captureKey;

            ImageExtractor imageExtractor = new ImageExtractor(uri, "root", "root");

            //create the motion sensor, and listen for images
            MotionSensor_2a motionSensor = new MotionSensor_2a();
            motionSensor.logfile = @"F:\temp\MotionSensor\2.1\movement\info.txt";
            motionSensor.motionDetected += new MotionSensor_2.MotionDetected(MotionDetected);

            //create the validator 
            ImageValidator imageValidator = new ImageValidator();
            imageValidator.ListenForImages(imageExtractor);
            imageValidator.imageValidated += new ImageValidator.ImageValidatedEvent(motionSensor.ImageCreatedAsync);//subscribe to events from the validator

            imageExtractor.Run();

        }

        private void MotionDetected(ByteWrapper image, EventArgs e)
        {
            Console.WriteLine(image.sequenceNumber);
        }

    }
}
