using System;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageAnalysis.Images;
using ImageAnalysis.Analysis;
using ImageAnalysis.Images.Bitmaps;
using ImageAnalysis.Images.Jpeg;
using ImageAnalysis.MotionSensor;

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

    }
}
