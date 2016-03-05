﻿using System;
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
        public void Run(string captureId)
        {
            Setup(captureId);
            movementFrames = new List<int>();

            //create the motion sensor, and listen for images
            using (MotionSensor_2a motionSensor = new MotionSensor_2a())
            {
                if (settings == null)
                {
                    motionSensor.settings = new MotionSensorSettings();
                }
                else
                {
                    motionSensor.settings = settings;
                }

                motionSensor.motionDetected += new MotionSensor_2.MotionDetected(MotionDetected);
                motionSensor.logging.LoggingOn = true;

                //create the validator 
                ImageValidator imageValidator = new ImageValidator();
                imageValidator.ListenForImages(imageExtractor);
                imageValidator.imageValidated += new ImageValidator.ImageValidatedEvent(motionSensor.ImageCreated); //subscribe to events from the validator (testing so sync only)

                //save 
                saveFilePath = ConfigurationManager.AppSettings["MotionSaveLocation"] + @"\" + settings.HashCode;

                if (Directory.Exists(saveFilePath)) { throw new Exception("Motion writing directory already exists"); }

                imageExtractor.Run();

            }//using MotionSensor_2a

        }

        private void Setup(string captureId)
        {
            this.captureId = captureId;
            string uri = "http://localhost:9000/api/jpeg/0/" + captureId;
            imageExtractor = new ImageExtractor(uri, "root", "root");
        }
     
        private void MotionDetected(ByteWrapper image, EventArgs e)
        {
            SaveMotionFile(image);
            if(movementFrames != null)
            {
                movementFrames.Add(image.sequenceNumber);
            }                  
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
