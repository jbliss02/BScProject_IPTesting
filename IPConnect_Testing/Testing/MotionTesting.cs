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
            captures.list.ForEach(x => TestMotion(x, motionSensorType));

        }

        public void TestMotion(Capture capture, MotionSensorTypes motionSensorType)
        {
            if (motionSensorType == MotionSensorTypes.Motion2a)
            {
                MotionSensor2aTest test = new MotionSensor2aTest();

                CaptureMotionTesting captureTesting = new CaptureMotionTesting();
                captureTesting.startTime = DateTime.Now;
                captureTesting.detectedMovmentFrames = new List<int>();
                captureTesting.detectionMethod = "a";

                test.Run(capture.captureId);

                captureTesting.detectedMovmentFrames = test.movementFrames;
                captureTesting.endTime = DateTime.Now;

                WriteToDatabase(captureTesting);
            }
        }

        private void WriteToDatabase(CaptureMotionTesting captureTest)
        {
            var db = new CaptureDb(ConfigurationManager.ConnectionStrings["AZURE"].ConnectionString);
            db.CreateDetectionSession(captureTest.MotionTestingXml());
        }

    }
}
