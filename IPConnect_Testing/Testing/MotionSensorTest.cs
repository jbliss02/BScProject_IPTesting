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
using System.Diagnostics;

namespace IPConnect_Testing.Testing
{
    /// <summary>
    /// Base class for MotionSensorTesting, implemented by
    /// classes that test specifici elements of the solution
    /// </summary>
    public abstract class MotionSensorTest
    {
        protected string captureId;

        protected CaptureListTesting captures { get; set; }

        //test and lag timinig
        protected Boolean timedTest { get; set; }// whether the runtime needs to be recorded in this test
        protected int elapsedMilliseconds { get; set; }


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
            captureId = captureTesting.captureId;

            if (motionSensorType == MotionSensorTypes.Motion2a)
            {
                using (MotionSensor2aTest test = new MotionSensor2aTest())
                {
                    test.settings = settings;

                    captureTesting.detectionStartTime = DateTime.Now;
                    captureTesting.detectedMovmentFrames = new List<int>();
                    captureTesting.detectionMethod = "a";

                    test.expectedFrames = captureTesting.numberFrames;
                    test.timedTest = timedTest;
                    test.Run(captureTesting.captureId);

                    captureTesting.detectedMovmentFrames = test.movementFrames;
                    captureTesting.detectionEndTime = DateTime.Now;

                    if(timedTest){ elapsedMilliseconds = test.testTimer.Elapsed.Milliseconds; }

                }

                WriteToDatabase(captureTesting, settings); //calls the subclass method

            }
        }

        internal abstract void WriteToDatabase(CaptureTesting captureTest, MotionSensorSettingsTest motionSettings);

    }
}
