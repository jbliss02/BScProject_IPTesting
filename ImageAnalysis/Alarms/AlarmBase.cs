using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageAnalysis.Images.Jpeg;

namespace ImageAnalysis.Alarms
{
    /// <summary>
    /// Base Alarm service that holds the logic common to any alarm 
    /// Receives the incoming messages, decides how many alarms to send etc.
    /// </summary>
    public abstract class AlarmBase : IAlarm
    {
        public List<ByteWrapper> images = new List<ByteWrapper>(); //saved moment images

        public void ImageExtracted(ByteWrapper img, EventArgs e)
        {
            images.Add(img);
            OnImageExtracted();
        }

        public abstract void OnImageExtracted();
    }
}
