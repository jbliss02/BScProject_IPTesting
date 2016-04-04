using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageAnalysis.Images.Jpeg;

namespace ImageAnalysis.Alarms
{
    /// <summary>
    /// Classes that want to be informed when movement has been detected implement this interface
    /// </summary>
    public interface IAlarm
    {
        void ImageExtracted(ByteWrapper img, EventArgs e);
    }
}
