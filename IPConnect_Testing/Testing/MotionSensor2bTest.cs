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
    public class MotionSensor2bTest
    {
        public void Run(string captureKey)
        {
            //set up the extractor
            string uri = "http://localhost:9000/api/jpeg/0/" + captureKey;

            ImageExtractor imageExtractor = new ImageExtractor(uri, "root", "root");

            //create the motion sensor, and listen for images
            MotionSensor_2b motionSensor = new MotionSensor_2b();
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
