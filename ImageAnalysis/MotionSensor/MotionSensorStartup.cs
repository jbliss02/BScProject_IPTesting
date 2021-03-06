﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageAnalysis.Images.Jpeg;
using ImageAnalysis.Images;
using ImageAnalysis.Streams;
using ImageAnalysis.Alarms;
using ImageAnalysisDAL;
using System.Configuration;

namespace ImageAnalysis.MotionSensor
{
    /// <summary>
    /// Starts up a MotionSensorSession. Hooks together the image extractor, image validator, image saver and
    /// links image creation events with any specified alarms
    /// </summary>
    public class MotionSensorStartup
    {
        private IImageSaver imageSaver;
        private IImageValidator imageValidator;
        private IImageExtractor imageExtractor;
        private IMotionSensor motionSensor;
        private List<IAlarm> alarms; //all the alarms to sound if movement is detected
        private ICaptureDb database;
        private string captureId;
        private int detectionId;

        public MotionSensorStartup(MotionSensorSetup setup)
        {
            //setup the extractor
            imageExtractor = new ImageExtractor(setup.camera);
            imageExtractor.framerateBroadcast += new ImageExtractor.FramerateBroadcastEvent(FramerateBroadcastEventHandler);
            imageExtractor.asyncrohous = true;

            //set up the save file object
            imageSaver = new ImageSaver(setup.imageSaveLocation, "movement", setup.camera.cameraId);
            imageSaver.saveToFileServer = true; //setup.saveImagesToFileServer;
            imageSaver.saveToDatabase = setup.saveImagesToDatabase;
            imageSaver.ParentDirectory = setup.imageSaveLocation;
            captureId = imageSaver.captureId;

            //set up the database object
            if(setup.saveImagesToDatabase)
            {
                database = new CaptureDb(ConfigurationManager.ConnectionStrings["LOCALDB"].ConnectionString);
                database.CreateCaptureSession(captureId, imageSaver.SaveDirectory);
                detectionId = database.CreateDetectionSession(captureId);
                imageSaver.detectionId = detectionId;
            }

            //setup the motion sensor
            motionSensor = new MotionSensor_2a();
            motionSensor.settings = new MotionSensorSettings();
            motionSensor.settings.LoadDefaults();

            motionSensor.motionDetected += new MotionSensor_2.MotionDetected(imageSaver.ImageCreatedAsync);

            //create the validator 
            imageValidator = new ImageValidator();
            imageValidator.ListenForImages(imageExtractor);
            imageValidator.imageValidated += new ImageValidator.ImageValidatedEvent(motionSensor.ImageCreatedAsync);//subscribe to events from the validator

            //setup the alarms
            alarms = new List<IAlarm>();
            if (setup.emailAlarm != null)
            {
                alarms.Add(setup.emailAlarm as EmailAlarm);
                imageSaver.imageCreated += new ImageSaver.ImageSavedEvent(setup.emailAlarm.ImageExtracted);
            }

            imageExtractor.Run();

        }

        private void FramerateBroadcastEventHandler(double framerate, EventArgs args)
        {
            if (imageSaver != null)
            {
                imageSaver.framerates.Add(framerate);
            }
        }

    }
}
