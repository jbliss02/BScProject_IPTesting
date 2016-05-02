using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageAnalysis.Alarms;

namespace Testing.Alarms
{
    [TestClass]
    public class EmailAlarmTest
    {
        ///[TestMethod]
        /////This doesn't work now as settings are taken from web.config in web startup
        public void SendEmail()
        {
            bool success;

            try
            {
                EmailAlarm alarm = new EmailAlarm();
                alarm.emailAddress = "james.bliss@outlook.com";
                alarm.SendAlarmEmail();
                success = true;
            }
            catch(Exception ex)
            {
                success = false;
            }

            Assert.IsTrue(success);

        }
    }
}
