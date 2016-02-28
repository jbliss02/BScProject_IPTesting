using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Threading.Tasks;
using ImageAnalysis.MotionSensor;
using ImageAnalysis.Data;
using ImageAnalysisDAL;
using Tools;

namespace IPConnect_Testing.Testing
{
    /// <summary>
    /// A testing wrapper for MotionSensorSettings
    /// includes methods to set pre-defined settings
    /// and to write to database
    /// </summary>
    public class MotionSensorSettingsTest : MotionSensorSettings
    {
        public string captureId { get; set; }

        public XmlDocument SerialiseMe()
        {
            return new Tools.Xml().SerialiseObject(this);
        }

    }
}
