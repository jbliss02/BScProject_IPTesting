using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Threading.Tasks;
using System.Reflection;
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

            MotionSensorSettingsList motionSensorSettingsList = new MotionSensorSettingsList();
            motionSensorSettingsList.PopulateAllPossible();
            var settingsList = motionSensorSettingsList.list;

            foreach (MotionSensorSettingsTest setting in settingsList)
            {
                captures.list.ForEach(x => TestMotion(x, motionSensorType, setting));
            }
        }

        /// <summary>
        /// Tests all captures available, changes motionSensorSetting passed in based on the range of 
        /// values in the database. 
        /// </summary>
        /// <param name="motionSensorType"></param>
        /// <param name="motionSensorSettings"></param>
        public void TestAllCaptures(MotionSensorTypes motionSensorType, MotionSensorSettingTypes motionSensorSetting)
        {
            captures = new CaptureListTesting();
            captures.PopulateAllCaptures(true);

            MotionSensorSettingsList motionSensorSettingsList = new MotionSensorSettingsList();
            motionSensorSettingsList.PopulateRange(motionSensorSetting);
            var settingsList = motionSensorSettingsList.list;

            foreach (MotionSensorSettingsTest setting in settingsList)
            {
                captures.list.ForEach(x => TestMotion(x, motionSensorType, setting));
            }
        }

        /// <summary>
        /// Tests all captures in the database. Each setting is taken through its predefined range, with all captures being tested 
        /// throughout those ranges. All other settings remain at their default value
        /// </summary>
        public void TestAllCaptures_SequentialSettingChanges(MotionSensorTypes motionSensorType)
        {
            captures = new CaptureListTesting();
            captures.PopulateAllCaptures(true);

            MotionSensorSettingsList motionSensorSettingsList = new MotionSensorSettingsList();
            motionSensorSettingsList.PopulateSequentialChange();

            foreach (MotionSensorSettingsTest setting in motionSensorSettingsList.list)
            {
                captures.list.ForEach(x => TestMotion(x, motionSensorType, setting));
            }

        }

        public void TestMotion(CaptureTesting captureTesting, MotionSensorTypes motionSensorType, MotionSensorSettingsTest settings)
        {
            if (motionSensorType == MotionSensorTypes.Motion2a)
            {
                using(MotionSensor2aTest test = new MotionSensor2aTest())
                {
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
        }

        private void WriteToDatabase(CaptureTesting captureTest, MotionSensorSettingsTest motionSettings)
        {
            var db = new CaptureDb(ConfigurationManager.ConnectionStrings["AZURE"].ConnectionString);
            db.CreateDetectionSession(captureTest.SerialiseMe(), motionSettings.SerialiseMe());

        }

    }
}
