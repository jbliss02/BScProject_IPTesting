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
        //Event fired when extraction is complete (i.e. no more JPEG's are in stream)
        public event FinishedBroadcastEvent finishedBroadcastEvent;
        public delegate void FinishedBroadcastEvent(string captureId, EventArgs e);

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

        private void RunTests()
        {
            MotionSensorSettingsTest settings = new MotionSensorSettingsTest();

            //sync tests
            //settings.asynchronous = false;
            //captures.list.ForEach(x => TestMotion(x, motionSensorType, settings));

            //async tests
            settings.asynchronous = true;
            captures.list.ForEach(x => { TestMotion(x, motionSensorType, settings); OnFinishedBroadcast(captureId, EventArgs.Empty); });
        }

        internal override void WriteToDatabase(CaptureTesting captureTest, MotionSensorSettingsTest motionSettings)
        {
            var db = new CaptureDbTest(ConfigurationManager.ConnectionStrings["LOCALDB"].ConnectionString);
            db.CreateLagTestSession(captureTest.SerialiseMe(), motionSettings.SerialiseMe(), captureTest.captureId);

        }

        private void OnFinishedBroadcast(string captureId, EventArgs e)
        {
            if (finishedBroadcastEvent != null) { finishedBroadcastEvent(captureId, e); }
        }

    }
}
