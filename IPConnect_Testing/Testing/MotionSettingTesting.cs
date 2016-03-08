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
using IPConnect_Testing.DAL;
using IPConnect_Testing.Testing.DataObjects;

namespace IPConnect_Testing.Testing
{
    /// <summary>
    /// Used for testing captures with various settings, and recording that information
    /// </summary>
    public class MotionSettingTesting : MotionSensorTest
    {
        /// <summary>
        /// Tests all items in the captures list, against the specified motion sensor type
        /// </summary>
        /// <param name="motionSensorType"></param>
        public void TestAllCaptures(MotionSensorTypes motionSensorType)
        {
            PopulateAllCaptures();

            MotionSensorSettingsList motionSensorSettingsList = new MotionSensorSettingsList();
            motionSensorSettingsList.PopulateAllPossible();
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

        internal override void WriteToDatabase(CaptureTesting captureTest, MotionSensorSettingsTest motionSettings)
        {
            var db = new CaptureDbTest(ConfigurationManager.ConnectionStrings["LOCALDB"].ConnectionString);
            db.CreateDetectionSession(captureTest.SerialiseMe(), motionSettings.SerialiseMe(), captureTest.captureId);

        }
    }
}
