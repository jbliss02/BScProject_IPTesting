using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Xml;
using Tools;

namespace ImageAnalysisDAL
{
    /// <summary>
    /// Writes, and returns, information on IP camera capture sessions
    /// </summary>
    public class CaptureDb : Db
    {
        public CaptureDb(string connectionString) :base(connectionString) {  }

        public DataTable ReturnAllCaptures()
        {
            return DataTableFromView("dbo.allCaptures");
        }

        public DataTable ReturnCaptureMovement(XmlDocument captureXml) 
        {
            SqlParameter p = new SqlParameter();
            p.ParameterName = "@xml";
            p.DbType = DbType.Xml;
            p.Value = captureXml.OuterXml;
            return DataTableFromProc("dbo.returnCaptureMovement", p);
        }

        /// <summary>
        /// Creates a database record for a new detection session, adds the header data
        /// the detected movement frames, and the motion sensor detectors
        /// </summary>
        /// <param name="detecionSessionXml"></param>
        /// <returns></returns>
        public void CreateDetectionSession(XmlDocument motionTestingXml, XmlDocument motionSettingsXml)
        {
            //add the header data
            SqlParameter p = new SqlParameter();
            p.ParameterName = "@xml";
            p.DbType = DbType.Xml;
            p.Value = motionTestingXml.OuterXml;

            string id = RunProcWithReturn("dbo.addDetectionData", p);

            //add the settings
            List<SqlParameter> paras = new List<SqlParameter>();
            p = new SqlParameter();
            p.ParameterName = "@xml";
            p.DbType = DbType.Xml;
            p.Value = motionSettingsXml.OuterXml;
            paras.Add(p);

            p = new SqlParameter();
            p.ParameterName = "@detectionId";
            p.Value = id.StringToInt();
            p.DbType = DbType.Int16;
            paras.Add(p);

            RunProc("test.addDetectionSessionSettings", paras);

        }

    }
}
