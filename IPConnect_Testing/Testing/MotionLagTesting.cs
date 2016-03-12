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
using IPConnect_Testing.DAL;
using IPConnect_Testing.Testing.DataObjects;

namespace IPConnect_Testing.Testing
{
    /// <summary>
    /// Used for testing existing captures, and the lag between analysis of images and incoming images
    /// </summary>
    public class MotionLagTesting : MotionSensorTest
    {
        MotionSensorTypes motionSensorType;
        
        /// <summary>
        /// Tests all the captures for lag time, with sync and async settings
        /// </summary>
        /// <param name="motionSensorType"></param>
        public void TestAllCaptures_BothSyncs(MotionSensorTypes motionSensorType)
        {
            this.motionSensorType = motionSensorType;
            PopulateAllCaptures();
            RunTests();
        }

        /// <summary>
        /// Tests a specific capture for lag time, with sync and asyc settings
        /// </summary>
        /// <param name="motionSensorType"></param>
        public void TestCapture_BothSyncs(MotionSensorTypes motionSensorType, string captureId)
        {
            this.motionSensorType = motionSensorType;
            PopulateCapture(captureId);
            RunTests();
        }

        /// <summary>
        /// Tests all defined length captures and tests for lag time, with sync and asyc settings
        /// </summary>
        /// <param name="motionSensorType"></param>
        public void TestCapture_BothSyncs(MotionSensorTypes motionSensorType)
        {
            this.motionSensorType = motionSensorType;
            captures = new CaptureListTesting();
            captures.PopulateTimedCaptures();
            RunTests();
        }


        private void RunTests()
        {
            MotionSensorSettingsTest settings = new MotionSensorSettingsTest();

            timedTest = true;

            //sync tests
            //settings.asynchronous = false;
            //captures.list.ForEach(x => TestMotion(x, motionSensorType, settings));

            //async tests
            settings.asynchronous = true;
            captures.list.ForEach(x => { TestMotion(x, motionSensorType, settings); });
        }

        internal override void WriteToDatabase(CaptureTesting captureTest, MotionSensorSettingsTest motionSettings)
        {

            var el = elapsedMilliseconds;

            var x = "WSS";

            var db = new CaptureDbTest(ConfigurationManager.ConnectionStrings["LOCALDB"].ConnectionString);
            db.CreateLagTestSession(captureTest.SerialiseMe(), motionSettings.SerialiseMe(), captureTest.captureId);

        }

    }
}
