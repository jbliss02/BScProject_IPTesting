using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using ImageAnalysisDAL;
using System.Xml;
using Tools;

namespace IPConnect_Testing.DAL
{
    public class CaptureDbTest : CaptureDb
    {
        public CaptureDbTest(string connectionString) : base(connectionString) { }

        /// <summary>
        /// Gets the detection setting ranges for testing purposes
        /// Comes with multi variable types, these are described in 
        /// </summary>
        /// <returns></returns>
        public DataTable ReturnSettingTypeRanges()
        {
            return DataTableFromView("test.settingTypeRanges");
        }

        /// <summary>
        /// Returns any confusion data (TP, FN, FP) from test data
        /// inclduing setting data. This can take up to a minite to run
        /// </summary>
        /// <returns></returns>
        public DataTable ReturnTestConfusionData(int settingId)
        {
            SqlParameter p = new SqlParameter();
            p.ParameterName = "@settingTypeId";
            p.Value = settingId;
            p.DbType = DbType.Int16;
            return DataTableFromProc("test.ReturnTestConfusionData_byType", p);
        }

        public DataTable ReturnSettingTypes()
        {
            return DataTableFromView("dbo.detectionSettingTypes");
        }

        /// <summary>
        /// Creates a database record for a new detection session, adds the header data
        /// the detected movement frames, and the motion sensor settings
        /// </summary>
        /// <param name="detecionSessionXml"></param>
        /// <returns></returns>
        public void CreateDetectionSession(XmlDocument motionTestingXml, XmlDocument motionSettingsXml, string captureId)
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

            p = new SqlParameter();
            p.ParameterName = "@captureId";
            p.Value = captureId;
            p.DbType = DbType.String;
            paras.Add(p);

            RunProc("test.addDetectionSessionSettings", paras);
        }

        public void CreateLagTestSession(XmlDocument motionTestingXml, XmlDocument motionSettingsXml, string captureId)
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

            p = new SqlParameter();
            p.ParameterName = "@captureId";
            p.Value = captureId;
            p.DbType = DbType.String;
            paras.Add(p);

            RunProc("test.addDetectionSessionSettings", paras);

        }

        public void AddTimedCapture(string captureId, int minutes)
        {
            List<SqlParameter> paras = new List<SqlParameter>();

            SqlParameter p = new SqlParameter("@captureId", DbType.String);
            p.Value = captureId;
            paras.Add(p);

            p = new SqlParameter("@minutes", DbType.Int16);
            p.Value = minutes;
            paras.Add(p);

            RunProc("dbo.AddTimedCapture", paras);
        }

    }
}