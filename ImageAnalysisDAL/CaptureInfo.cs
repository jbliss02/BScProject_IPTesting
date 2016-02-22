using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Sql;

namespace ImageAnalysisDAL
{
    /// <summary>
    /// Writes, and returns, information on IP camera capture sessions
    /// </summary>
    public class CaptureInfo : Db
    {
        string connectionString;
        public CaptureInfo(string connectionString) { this.connectionString = connectionString; }

        public void ReturnAllCaptures()
        {
            DataTable dt = DataTableFromView("dbo.allCaptures", connectionString);

            var x = dt.Rows.Count;
        }

    }
}
