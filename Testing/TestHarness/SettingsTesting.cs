using System;
using IPConnect_Testing.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing.TestHarness
{
    [TestClass]
    public class SettingsTesting
    {
        /// <summary>
        /// Tests that the test setting ranges are extracted from the database and the appropriate
        /// number of setting lists are created. The settings are factorial in that every combination
        /// requires testing
        /// </summary>
        [TestMethod]
        public void CombinationTesting()
        {
            MotionSensorSettingsList testSettings = new MotionSensorSettingsList();

            Assert.IsTrue(testSettings.seperateSettingLists.Count > 3);
            Assert.IsTrue(testSettings.seperateSettingLists[0].list.Count > 0);
            var x = "m;d";
        }
    }
}
