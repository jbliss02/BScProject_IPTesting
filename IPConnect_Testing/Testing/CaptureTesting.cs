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

namespace IPConnect_Testing.Testing
{
    public class CaptureListTesting : CaptureList, ICaptureList
    {
        public new List<CaptureTesting> list { get; set; }

        /// <summary>
        /// Populates metadata about each capture session available
        /// allData will load additional metadata, like when movement occurs
        /// </summary>
        /// <param name="movementData"></param>
        public new void PopulateAllCaptures(bool movementData)
        {
            list = new List<CaptureTesting>();
            ConnectionStringSettingsCollection connections = ConfigurationManager.ConnectionStrings;
            captureInfo = new ImageAnalysisDAL.CaptureDb(connections["LOCALDB"].ConnectionString);
            DataTable dt = captureInfo.ReturnAllCaptures();

            foreach (DataRow dr in dt.Rows)
            {
                CaptureTesting capture = new CaptureTesting();
                capture.captureId = dr.Field<String>("captureId");
                capture.capturedOn = dr.Field<DateTime?>("capturedOn");
                list.Add(capture);
            }

            if (movementData) { PopulateMovement(); }

        }//PopulateAllCaptures
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
