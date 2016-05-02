using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageAnalysis.MotionSensor;

namespace Testing.MotionSensor
{
    [TestClass]
    public class MotionSensorConfigTest
    {
        [TestMethod]
        public void TestConfig()
        {

            MotionSensorConfigList config = new MotionSensorConfigList();
            config.LoadTemplate();

            Assert.IsTrue(config.list.Count > 1);

        }
    }
}
