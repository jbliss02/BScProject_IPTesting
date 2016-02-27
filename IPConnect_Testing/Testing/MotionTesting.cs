using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageAnalysis.MotionSensor;
using ImageAnalysis.Data;

namespace IPConnect_Testing.Testing
{
    /// <summary>
    /// Used for testing existing captures against various motion detectors,
    /// with various settings and recording that information
    /// </summary>
    public class MotionTesting
    {
        public CaptureList captures { get; set; }

        /// <summary>
        /// Tests all items in the captures list, against the specified motion sensor type
        /// </summary>
        /// <param name="motionSensorType"></param>
        public void TestAllCaptures(MotionSensorTypes motionSensorType)
        {
            captures = new CaptureList();
            captures.PopulateAllCaptures(true);
            captures.list.ForEach(x => TestMotion(x, motionSensorType));
        }

        public void TestMotion(Capture capture, MotionSensorTypes motionSensorType)
        {
            if (motionSensorType == MotionSensorTypes.Motion2a)
            {
                MotionSensor2aTest test = new MotionSensor2aTest();
                capture.testing = new ImageAnalysis.CaptureTesting();
                capture.testing.startTime = DateTime.Now;
                capture.testing.detectedMovmentFrames = new List<int>();
                test.Run(capture.captureId);

                capture.testing.detectedMovmentFrames = test.movementFrames;
                capture.testing.endTime = DateTime.Now;
            }
        }

    }
}
