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
using IPConnect_Testing.DAL;
using Tools;

namespace IPConnect_Testing.Testing.DataObjects
{
    /// <summary>
    /// The test version of the Capture List, reimplements the list and the population of that list
    /// </summary>
    public class CaptureListTesting : CaptureList, ICaptureList
    {
        public new List<CaptureTesting> list { get; set; }
        public new void PopulateCapture(bool movementData, string captureId)
        {
            list = new List<CaptureTesting>();
            ConnectionStringSettingsCollection connections = ConfigurationManager.ConnectionStrings;
            captureInfo = new CaptureDbTest(connections["LOCALDB"].ConnectionString);

            DataTable dt = captureInfo.ReturnCapture(captureId);

            foreach (DataRow dr in dt.Rows)
            {
                AddCapture(dr);
            }

            if (movementData) { PopulateMovement(); }
        }

        /// <summary>
        /// Populates metadata about each capture session available
        /// allData will load additional metadata, like when movement occurs
        /// </summary>
        /// <param name="movementData"></param>
        public new void PopulateAllCaptures(bool movementData)
        {
            list = new List<CaptureTesting>();
            ConnectionStringSettingsCollection connections = ConfigurationManager.ConnectionStrings;
            captureInfo = new CaptureDbTest(connections["LOCALDB"].ConnectionString);
            DataTable dt = captureInfo.ReturnAllCaptures();

            foreach (DataRow dr in dt.Rows)
            {
                AddCapture(dr);
            }

            if (movementData) { PopulateMovement(); }

        }//PopulateAllCaptures

        /// <summary>
        /// Populates all captures with a defined length (numberMinutes)
        /// </summary>
        public void PopulateTimedCaptures()
        {
            list = new List<CaptureTesting>();
            ConnectionStringSettingsCollection connections = ConfigurationManager.ConnectionStrings;
            CaptureDbTest captureInfo = new CaptureDbTest(connections["LOCALDB"].ConnectionString);
            DataTable dt = captureInfo.ReturnTimedCaptures();

            foreach (DataRow dr in dt.Rows)
            {
                AddCapture(dr);
            }

        }

        private void AddCapture(DataRow dr)
       {
            CaptureTesting capture = new CaptureTesting();
            capture.captureId = dr.Field<String>("captureId");
            capture.capturedOn = dr.Field<DateTime?>("capturedOn");
            capture.numberFrames = dr.Field<Int32>("numberFrames");

            list.Add(capture);
        }

        /// <summary>
        /// Extracts movement info from the items in the capture list, adds to those objects in the list
        /// </summary>
        public new void PopulateMovement()
        {
            DataTable dt = captureInfo.ReturnCaptureMovement(SerialiseMe());

            foreach (DataRow dr in dt.Rows)
            {
                List<CaptureTesting> capture = (from cap in list
                                         where cap.captureId == dr.Field<String>("captureId")
                                         select cap).ToList();

                capture.ForEach(x => x.movement.Add(new Movement(dr)));
            }

        }

        /// <summary>
        /// Scans through the captures directory of the passed in cameraId and counts the number
        /// of frames within that capture. Adds to the database
        /// </summary>
        public void UpdateNumberFrames(int cameraId)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("IPUPDATE");
            doc.AppendChild(root);

            string rootDir = ConfigurationManager.AppSettings["saveLocation"] + @"\" + cameraId;

            foreach (String captureDir in Directory.GetDirectories(rootDir))
            {
                string _captureId = System.Text.RegularExpressions.Regex.Split(captureDir, "\\\\")[3];
                int nFrames = 0;

                foreach(String sectionDir in Directory.GetDirectories(captureDir))
                {
                    nFrames += Directory.GetFiles(sectionDir).Count() - 1; //remove one for the logfile which is in every directory
                }

                XmlElement node = doc.CreateElement("capture");
                root.AppendChild(node);

                XmlElement capture = doc.CreateElement("captureId");
                capture.InnerXml = _captureId;
                node.AppendChild(capture);

                XmlElement numberFrames = doc.CreateElement("numberFrames");
                numberFrames.InnerXml = nFrames.ToString();
                node.AppendChild(numberFrames);
            }

            //add to the database
            new DAL.CaptureDbTest(ConfigurationManager.ConnectionStrings["LOCALDB"].ConnectionString).AddFrameNumbersToCaptures(doc);

        }//UpdateNumberFrames

        /// <summary>
        /// XML representation of the captures in the list
        /// </summary>
        public new XmlDocument SerialiseMe()
        {
            return new Tools.Data().SerialiseObject(list);
        }

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

        public int numberFrames { get; set; }

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
            return new Tools.Data().SerialiseObject(this);
        }

    }
}
