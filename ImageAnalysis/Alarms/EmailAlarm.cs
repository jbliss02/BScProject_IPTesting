using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageAnalysis.Images;
using ImageAnalysis.Images.Jpeg;

namespace ImageAnalysis.Alarms
{
    /// <summary>
    /// Subscribes to an image created event, sends an email to alert about movement
    /// </summary>
    public class EmailAlarm : AlarmBase, IAlarm 
    {
        public string userName { get; set; }
        public string password { get; set; }

    }
}
