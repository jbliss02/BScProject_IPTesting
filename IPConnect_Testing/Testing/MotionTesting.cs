using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPConnect_Testing.Data;
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
        public CaptureListTest captures { get; set; }

        /// <summary>
        /// Tests all items in the captures list, against the specified motion sensor type
        /// </summary>
        /// <param name="motionSensorType"></param>
        public void TestAllCaptures(MotionSensorTypes motionSensorType)
        {
            captures = new CaptureListTest();
            captures.PopulateAllCaptures(true);
            captures.list.ForEach(x => TestCapture(x, motionSensorType));
        }

        public void TestCapture(Capture capture, MotionSensorTypes motionSensorType)
        {
            if (motionSensorType == MotionSensorTypes.Motion2a)
            {
                MotionSensor2aTest test = new MotionSensor2aTest();
                capture.detectedMovmentFrames = new List<int>();
                capture.detectedMovmentFrames.AddRange(test.ReturnMovements(capture.captureId));
                
            }
        }

        public void TestCapture(string captureId, MotionSensorTypes motionSensorType)
        {


        }//TestCapture


    }
}
