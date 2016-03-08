using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageAnalysisDAL;
using System.Configuration;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using ImageAnalysis.Data;
using Tools;

namespace IPConnect_Testing.Testing.DataObjects
{
    public class CaptureListTesting : CaptureList, ICaptureList
    {
        public new List<CaptureTesting> list { get; set; }
    }

    /// <summary>
    /// Used to hold testing data about a single Capture object
    /// </summary>
    public class CaptureTesting : Capture, ICapture
    {
        public CaptureTesting() {  }

        public DateTime detectionStartTime { get; set; }

        public DateTime detectionEndTime { get; set; }

        public String detectionMethod { get; set; }

        /// <summary>
        /// What frame numbers have been identified as having movement
        /// </summary>
        public List<Int32> detectedMovmentFrames { get; set; }

        /// <summary>
        ///Serialises the CaptureTesting class into XML
        /// </summary>
        /// <returns></returns>
        public XmlDocument SerialiseMe()
        {
            return new Tools.Xml().SerialiseObject(this);
        }

    }
}
