using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using ImageAnalysisDAL;

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
            return DataTableFromProc("test.ReturnTestConfusionData_byType",p);
        }

      
    }
}
