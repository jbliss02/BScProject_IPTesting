using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageAnalysisDAL;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace ImageAnalysis.Data
{
    public class CaptureList : ICaptureList
    {
        public List<Capture> list { get; set; }
        public CaptureDb captureInfo { get; set; }

        /// <summary>
        /// Populates metadata about each capture session available
        /// allData will load additional metadata, like when movement occurs
        /// </summary>
        /// <param name="movementData"></param>
        public void PopulateAllCaptures(bool movementData)
        {
            list = new List<Capture>();
            ConnectionStringSettingsCollection connections = ConfigurationManager.ConnectionStrings;
            captureInfo = new ImageAnalysisDAL.CaptureDb(connections["LOCALDB"].ConnectionString);
            DataTable dt = captureInfo.ReturnAllCaptures();
            
            foreach(DataRow dr in dt.Rows)
            {
                Capture capture = new Capture();
                capture.captureId = dr.Field<String>("captureId");
                capture.capturedOn = dr.Field<DateTime?>("capturedOn");
                list.Add(capture);
            }

            if (movementData) { PopulateMovement(); }

        }//PopulateAllCaptures

        public void PopulateCapture(bool movementData, string captureId)
        {
            list = new List<Capture>();
            ConnectionStringSettingsCollection connections = ConfigurationManager.ConnectionStrings;
            captureInfo = new ImageAnalysisDAL.CaptureDb(connections["LOCALDB"].ConnectionString);

            DataTable dt = captureInfo.ReturnCapture(captureId);

            foreach (DataRow dr in dt.Rows)
            {
                Capture capture = new Capture();
                capture.captureId = dr.Field<String>("captureId");
                capture.capturedOn = dr.Field<DateTime?>("capturedOn");
                list.Add(capture);
            }

            if (movementData) { PopulateMovement(); }
        }


        /// <summary>
        /// Extracts movement info from the items in the capture list, adds to those objects in the list
        /// </summary>
        public void PopulateMovement()
        {
            DataTable dt = captureInfo.ReturnCaptureMovement(SerialiseMe());

            foreach(DataRow dr in dt.Rows)
            {
                List<Capture> capture = (from cap in list
                                         where cap.captureId == dr.Field<String>("captureId")
                                         select cap).ToList();

                capture.ForEach(x => x.movement.Add(new Movement(dr)));
            }

        }

        /// <summary>
        /// XML representation of the captures in the list
        /// </summary>
        public XmlDocument SerialiseMe()
        {
            return new Tools.Xml().SerialiseObject(list);
        }

    }

    /// <summary>
    /// Represents a single capture session
    /// </summary>
    public class Capture : ICapture
    {
        public string captureId { get; set; }
        [XmlIgnoreAttribute]
        public DateTime? capturedOn { get; set; }

        public DateTime? captureStart { get; set; }

        public DateTime? captureEnd { get; set; }

        /// <summary>
        /// What frames we know have movement
        /// </summary>
        public List<Movement> movement { get; set; } = new List<Movement>();

    }
}
