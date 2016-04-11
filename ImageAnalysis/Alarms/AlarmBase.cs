using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageAnalysis.Images.Jpeg;
using System.Configuration;
using Tools;

namespace ImageAnalysis.Alarms
{
    /// <summary>
    /// Base Alarm service that holds the logic common to any alarm 
    /// Receives the incoming messages, decides how many alarms to send etc.
    /// </summary>
    public abstract class AlarmBase : IAlarm
    {
        public List<String> images = new List<String>(); //saved moment images, needs to be cleared regularly
        object objLock = new object(); //locks images collection as may be called async

        private DateTime? lastAlarm; //when the last alarm was sounded
        private int secondsBetweenAlarms; //what delay should be added so continuous alarms are not raised
        private int minimumMovementsForAlarm; //only sounds the alarm after this many movements

        public AlarmBase()
        {
            //may throw a casting exception, can be caught upstream
            secondsBetweenAlarms = ConfigurationManager.AppSettings["secondsBetweenAlarms"].ToString().StringToInt();
            minimumMovementsForAlarm = ConfigurationManager.AppSettings["minimumMovementsForAlarm"].ToString().StringToInt();
            lastAlarm = null;
        }

        public void ImageExtracted(String imagePath, EventArgs e)
        {
            lock(objLock){images.Add(imagePath);}

            if (lastAlarm == null || (DateTime.Now - lastAlarm.Value).Seconds > secondsBetweenAlarms)
            {
                if(images.Count >= minimumMovementsForAlarm)
                {
                    lastAlarm = DateTime.Now;
                    RaiseAlarm(imagePath);
                    lock (objLock) { images.Clear(); } //start again                  
                }
            }

        }//ImageExtracted

        public abstract void RaiseAlarm();
    }
}
