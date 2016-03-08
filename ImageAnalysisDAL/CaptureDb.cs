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

        public DataTable ReturnCapture(string captureId)
        {
            SqlParameter p = new SqlParameter();
            p.ParameterName = "@captureId";
            p.Value = captureId;
            p.DbType = DbType.String;
            return DataTableFromProc("dbo.returnCapture",p);
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
        /// Returns the default setting values 
        /// </summary>
        /// <returns></returns>
        public DataTable ReturnMotionSettingDefaults()
        {
            return DataTableFromProc("dbo.returnDetectionSettingDefaults");
        }
            





    }
}
