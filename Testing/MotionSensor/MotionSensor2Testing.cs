using System;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageAnalysis.Images;
using ImageAnalysis.Analysis;
using ImageAnalysis.Images.Bitmaps;
using ImageAnalysis.Images.Jpeg;
using ImageAnalysis.MotionSensor;
using IPConnect_Testing.Testing;

namespace Testing.MotionSensor
{
    [TestClass]
    public class MotionSensor2Testing
    {

        /// <summary>
        /// Checks that 2b is setting the threshold grid correctly
        /// </summary>
        [TestMethod]
        [TestCategory("Motion Sensor")]
        public void MotionSensor_2b_threshold()
        {
            MotionSensor_2b motion = new MotionSensor_2b();
            motion.ControlImageNumber = 10;

            foreach (var file in Directory.EnumerateFiles(@"f:\bsc\project\TestImages"))
            {
                ByteWrapper wrapper = ImageConvert.ReturnByteWrapper(file);
                motion.ImageCreatedAsync(wrapper, EventArgs.Empty);
                Thread.Sleep(250);
            }
            Assert.IsNotNull(motion.ThresholdImage);

            for(int i = 0; i < motion.ThresholdImage.Columns.Count; i++)
            {
                for(int n = 0; n < motion.ThresholdImage.Columns[i].grids.Count;n++)
                {
                    Assert.IsTrue(motion.ThresholdImage.Columns[i].grids[n].threshold != 0);
                }                     
            }

        }//MotionSensor_2b_threshold

        /// <summary>
        /// checks that defaults are added to the motion sensor settings
        /// </summary>
        [TestMethod]
        [TestCategory("Motion Sensor")]
        public void MotionSensorSettingDefaults()
        {
            MotionSensorSettings settings = new MotionSensorSettings();
            settings.LoadDefaults();
            Assert.IsTrue(settings.sensitivity != 1);

        }//MotionSensor_2b_threshold

        /// <summary>
        /// tests that a list of settings can be created and that contains the list
        /// of values for the specified setting, across the range specified in the database
        /// </summary>
        [TestMethod]
        [TestCategory("Motion Sensor")]
        public void MotionSensorSettingRange()
        {
            MotionSensorSettings settings = new MotionSensorSettings();
            settings.LoadDefaults();
            Assert.IsTrue(settings.sensitivity != 1);

        }


        /// <summary>
        /// Checks that we can correctly clone a MotionSensorSettingsList class
        /// </summary>
        [TestMethod]
        [TestCategory("Motion Sensor")]
        public void MotionSensorSettingCloning()
        {
            MotionSensorSettingsTest a = new MotionSensorSettingsTest();
            a.sensitivity = 199;
            a.horizontalPixelsToSkip = 199;

            MotionSensorSettingsTest b = new MotionSensorSettingsTest(a);
            b.sensitivity = 22;

            Assert.IsTrue(a.sensitivity != b.sensitivity);
            Assert.IsTrue(a.horizontalPixelsToSkip == b.horizontalPixelsToSkip);

        }


        /// <summary>
        /// Checks that the MotionSensorSettingsList.PopulateSequentialChange method creates a single list
        /// </summary>
        [TestMethod]
        [TestCategory("Motion Sensor")]
        public void SequentialSettingLists()
        {
            MotionSensorSettingsList test = new MotionSensorSettingsList();
            test.PopulateSequentialChange();
            Assert.IsTrue(test.list.Count > 50);
        }


        /// <summary>
        /// Tests that hash codes are unique across similar objects
        /// </summary>
        [TestMethod]
        [TestCategory("Motion Sensor")]
        public void HashTest()
        {
            MotionSensorSettingsList test = new MotionSensorSettingsList();
            test.PopulateSequentialChange();
            Assert.IsTrue(test.list[0].GetHashCode() != test.list[1].GetHashCode());
        }









    }
}
