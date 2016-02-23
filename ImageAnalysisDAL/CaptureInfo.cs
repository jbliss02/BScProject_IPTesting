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
    public class CaptureInfo : Db
    {
        public CaptureInfo(string connectionString) :base(connectionString) {  }

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

        public void WriteMovementFrames(XmlDocument capturedMovements)
        {

        }

    }
}
