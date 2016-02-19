using System;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IPConnect_Testing.Images;
using IPConnect_Testing.Analysis;
using IPConnect_Testing.Images.Bitmaps;
using IPConnect_Testing.Images.Jpeg;
using IPConnect_Testing.MotionSensor;
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
    }
}
