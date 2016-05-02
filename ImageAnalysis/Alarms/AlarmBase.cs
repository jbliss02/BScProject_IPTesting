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
        private List<DateTime> lastMovementReceived; //when the movements were received (so old images don't fire the alarm)
        object lastMovementReceivedLock = new object(); //locks the lastMovementReceived list

        public AlarmBase()
        {
            //may throw a casting exception, can be caught upstream
            secondsBetweenAlarms = ConfigurationManager.AppSettings["secondsBetweenAlarms"].ToString().StringToInt();
            minimumMovementsForAlarm = ConfigurationManager.AppSettings["minimumMovementsForAlarm"].ToString().StringToInt();
            lastAlarm = null;
            lastMovementReceived = new List<DateTime>();
        }

        public void ImageExtracted(String imagePath, EventArgs e)
        {
            lock(objLock){images.Add(imagePath);}

            lock (lastMovementReceivedLock)
            {
                if(lastMovementReceived.Count > minimumMovementsForAlarm) {
                    lastMovementReceived.RemoveAt(0);
                }
                lastMovementReceived.Add(DateTime.Now);
            }

            if (lastAlarm == null || (DateTime.Now - lastAlarm.Value).Seconds > secondsBetweenAlarms)
            {
                
                if(AboveImageCount())
                {
                    lastAlarm = DateTime.Now;
                    RaiseAlarm();
                    lock (objLock) { images.Clear(); } //start again                  
                }
            }

        }//ImageExtracted

        /// <summary>
        /// whether enough images have been received to raise an alarm
        /// based on the minimumMovementsForAlarm variable
        /// </summary>
        /// <returns></returns>
        private bool AboveImageCount()
        {
            if(images.Count >= minimumMovementsForAlarm && lastMovementReceived.Count >= minimumMovementsForAlarm)
            {
                if(lastMovementReceived.Count == 2)
                {
                    if((lastMovementReceived[1] - lastMovementReceived[0]).Seconds <= 1)
                    {
                        return true;
                    }
                }
                else if(lastMovementReceived.Count > 2)
                {
                    if ((lastMovementReceived[1] - lastMovementReceived[0]).Seconds <= 1 && (lastMovementReceived[2] - lastMovementReceived[1]).Seconds <= 1)
                    {
                        return true;
                    }
                }

            }

            return false;
        }

        public abstract void RaiseAlarm();
    }
}
