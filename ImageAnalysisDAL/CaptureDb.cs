using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Xml;
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
            return DataTableFromView("dbo.allCaptures", connectionString);
        }

        public DataTable ReturnCaptureMovement(XmlDocument captureXml)
        {
            SqlParameter p = new SqlParameter();
            p.ParameterName = "@xml";
            p.DbType = DbType.Xml;
            p.Value = captureXml.OuterXml;
            return DataTableFromProc("dbo.returnCaptureMovement", connectionString, p);
        }

        /// <summary>
        /// Creates a database record for a new detection session
        /// returns the primary key detectionId for the session
        /// </summary>
        /// <param name="detecionSessionXml"></param>
        /// <returns></returns>
        public int CreateDetectionSession(XmlDocument motionTestingXml)
        {
            SqlParameter p = new SqlParameter();
            p.ParameterName = "@xml";
            p.DbType = DbType.Xml;
            p.Value = motionTestingXml.OuterXml;



        }

    }
}
