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
    public class CaptureDb : Db, ICaptureDb
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
            
        /// <summary>
        /// Creates a new capture session in the database
        /// </summary>
        /// <param name="captureId"></param>
        /// <param name="saveLocation"></param>
        public void CreateCaptureSession(string captureId, string saveLocation)
        {
            if(captureId == null || captureId == String.Empty)
            {
                throw new Exception("CaptureId cannot be blank");
            }

            //check the captureId doesn't already exist
            if (CaptureIdExists(captureId))
            {
                throw new Exception("CaptureId already exists in the database");
            }

            //add the capture session
            List<SqlParameter> paras = new List<SqlParameter>();
            SqlParameter p = new SqlParameter("@captureId", SqlDbType.Text);
            p.Value = captureId;
            paras.Add(p);

            p = new SqlParameter("@saveLocation", SqlDbType.Text);
            p.Value = saveLocation == String.Empty || saveLocation == null ? p.Value = DBNull.Value : saveLocation;
            paras.Add(p);

            RunProc("dbo.createCaptureSession", paras);

        }

        public bool CaptureIdExists(string captureId)
        {
            SqlParameter p = new SqlParameter("@captureId", SqlDbType.Text);
            p.Value = captureId;

            DataTable dt = DataTableFromProc("dbo.returnCapture", p);

            return dt.Rows.Count > 0;

        }

        /// <summary>
        /// Creates a motion detection session in the database
        /// Returns the motion detection session Id
        /// </summary>
        /// <param name="captureId"></param>
        /// <returns></returns>
        public int CreateDetectionSession(string captureId)
        {
            SqlParameter p = new SqlParameter("@captureId", SqlDbType.Text);
            p.Value = captureId;

            DataTable dt = DataTableFromProc("dbo.createDetectionSession", p);

            if(dt.Rows.Count > 0)
            {
                return dt.Rows[0]["detectionId"].ToString().StringToInt();
            }
            else
            {
                throw new Exception("detectionId was not returned from database");
            }


        }

        public DataTable ReturnDetectionConfiguration()
        {
            return DataTableFromView("dbo.v_motionSensorConfig");
        }

        public void SaveDetectionImage(string captureId, int detectionId, byte[] image, int imageNumber)
        {

            List<SqlParameter> paras = new List<SqlParameter>();
            SqlParameter p = new SqlParameter("@captureId", SqlDbType.Text);
            p.Value = captureId;
            paras.Add(p);

            p = new SqlParameter("@detectionId", SqlDbType.Int);
            p.Value = detectionId;
            paras.Add(p);

            p = new SqlParameter("@binaryImage", SqlDbType.VarBinary);
            p.Value = image;
            paras.Add(p);

            p = new SqlParameter("@imageNumber", SqlDbType.Int);
            p.Value = imageNumber;
            paras.Add(p);

            RunProc("dbo.addDetetctionImage", paras);
        }

    }
}
