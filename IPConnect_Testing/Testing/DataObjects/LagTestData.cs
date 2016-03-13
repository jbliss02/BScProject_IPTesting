using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using System.Data;
using System.Configuration;

namespace IPConnect_Testing.Testing.DataObjects
{
    public class LagTestDataList
    {
        public List<LagTestData> list;
        public void Populate()
        {
            list = new List<LagTestData>();

            string conn = ConfigurationManager.ConnectionStrings["LOCALDB"].ConnectionString;
            var db = new DAL.CaptureDbTest(conn);
            DataTable dt = db.ReturnLagTestData();

            foreach (DataRow dr in dt.Rows)
            {
                list.Add(new LagTestData(dr));
            }
        }

    }

    public class LagTestData
    {
        public string captureId { get; set; }
        public int numberMinutes { get; set; }
        public decimal detectionMs { get; set; }
        public Boolean asynchronous { get; set; }
        public int memoryGb { get; set; }
        public decimal detectionSeconds { get
            {
                return (detectionMs / 1000) / 60;
            }
        }
        public LagTestData() { }
        public LagTestData(DataRow dr)
        {     
            new Tools.Data().ConvertDataRow<LagTestData>(dr, this);
        }

    }
}
