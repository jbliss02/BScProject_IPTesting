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

namespace IPConnect_Testing.Testing
{
    public class CaptureListTesting : CaptureList, ICaptureList
    {
        public new List<CaptureTesting> list { get; set; }

        /// <summary>
        /// Populates metadata about each capture session available
        /// allData will load additional metadata, like when movement occurs
        /// </summary>
        /// <param name="allData"></param>
        public new void PopulateAllCaptures(bool allData)
        {
            list = new List<CaptureTesting>();
            ConnectionStringSettingsCollection connections = ConfigurationManager.ConnectionStrings;
            captureInfo = new ImageAnalysisDAL.CaptureDb(connections["AZURE"].ConnectionString);
            DataTable dt = captureInfo.ReturnAllCaptures();

            foreach (DataRow dr in dt.Rows)
            {
                CaptureTesting capture = new CaptureTesting();
                capture.captureId = dr.Field<String>("captureId");
                capture.capturedOn = dr.Field<DateTime?>("capturedOn");
                list.Add(capture);
            }

            if (allData) { PopulateMovement(); }

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
        /// Returns information about the CaptureMotionTesting 
        /// </summary>
        /// <returns></returns>
        public XmlDocument CaptureTestingXml()
        {
           //serialise testing object XML and return
            XmlDocument doc = new XmlDocument();
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer x = new XmlSerializer(typeof(CaptureTesting));
                x.Serialize(stream, this);
                stream.Seek(0, System.IO.SeekOrigin.Begin); //without this there is a 'missing' root element error
                doc.Load(stream);
            }

            return doc;

        }//DetectedMovementXml 

    }
}
