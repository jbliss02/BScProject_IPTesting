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
using IPConnect_Testing.Testing.DataObjects;

namespace IPConnect_Testing.Testing
{
    /// <summary>
    /// Base class for MotionSensorTesting, implemented by
    /// classes that test specifici elements of the solution
    /// </summary>
    public abstract class MotionSensorTest
    {
        public CaptureListTesting captures { get; set; }

        /// <summary>
        /// Populates the captures List with all captures in the database
        /// </summary>
        protected void PopulateAllCaptures()
        {
            captures = new CaptureListTesting();
            captures.PopulateAllCaptures(true);
        }

        protected void PopulateCapture(string captureId)
        {
            captures = new CaptureListTesting();
            captures.PopulateCapture(true, captureId);
        }

        protected void TestMotion(CaptureTesting captureTesting, MotionSensorTypes motionSensorType, MotionSensorSettingsTest settings)
        {
            if (motionSensorType == MotionSensorTypes.Motion2a)
            {
                using (MotionSensor2aTest test = new MotionSensor2aTest())
                {
                    test.settings = settings;

                    captureTesting.detectionStartTime = DateTime.Now;
                    captureTesting.detectedMovmentFrames = new List<int>();
                    captureTesting.detectionMethod = "a";

                    test.Run(captureTesting.captureId);

                    captureTesting.detectedMovmentFrames = test.movementFrames;
                    captureTesting.detectionEndTime = DateTime.Now;

                }

                WriteToDatabase(captureTesting, settings);

            }
        }

        internal abstract void WriteToDatabase(CaptureTesting captureTest, MotionSensorSettingsTest motionSettings);

    }
}
