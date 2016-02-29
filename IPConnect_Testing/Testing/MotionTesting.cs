using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Threading.Tasks;
using ImageAnalysis.MotionSensor;
using ImageAnalysis.Data;
using ImageAnalysisDAL;

namespace IPConnect_Testing.Testing
{
    /// <summary>
    /// Used for testing existing captures against various motion detectors,
    /// with various settings and recording that information
    /// </summary>
    public class MotionTesting
    {
        public CaptureListTesting captures { get; set; }

        /// <summary>
        /// Tests all items in the captures list, against the specified motion sensor type
        /// </summary>
        /// <param name="motionSensorType"></param>
        public void TestAllCaptures(MotionSensorTypes motionSensorType)
        {
            captures = new CaptureListTesting();
            captures.PopulateAllCaptures(true);

            var settingsList = new MotionSensorSettingsList().list;

            foreach (MotionSensorSettingsTest setting in settingsList)
            {
                captures.list.ForEach(x => TestMotion(x, motionSensorType, setting));
            }
        }

        public void TestMotion(CaptureTesting captureTesting, MotionSensorTypes motionSensorType, MotionSensorSettingsTest settings)
        {
            if (motionSensorType == MotionSensorTypes.Motion2a)
            {
                MotionSensor2aTest test = new MotionSensor2aTest();
                test.settings = settings;

                captureTesting.detectionStartTime = DateTime.Now;
                captureTesting.detectedMovmentFrames = new List<int>();
                captureTesting.detectionMethod = "a";

                test.Run(captureTesting.captureId);

                captureTesting.detectedMovmentFrames = test.movementFrames;
                captureTesting.detectionEndTime = DateTime.Now;

                WriteToDatabase(captureTesting, settings);
            }
        }

        private void WriteToDatabase(CaptureTesting captureTest, MotionSensorSettingsTest motionSettings)
        {
            var db = new CaptureDb(ConfigurationManager.ConnectionStrings["AZURE"].ConnectionString);
            db.CreateDetectionSession(captureTest.SerialiseMe(), motionSettings.SerialiseMe());

        }

    }
}
