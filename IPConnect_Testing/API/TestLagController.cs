using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using IPConnect_Testing.DAL;
using IPConnect_Testing.Testing;
using System.Web.Http.Cors;
using Tools;

namespace IPConnect_Testing.API
{
    public class TestLagController : ApiController
    {
        //[EnableCors(origins: "http://localhost:3328", headers: "*", methods: "*")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/testlag")]
        public ChartDataList Get()
        {
            string conn = ConfigurationManager.ConnectionStrings["LOCALDB"].ConnectionString;
            var db = new DAL.CaptureDbTest(conn);
            DataTable dt = db.ReturnLagTestData();

            ChartDataList chartDataList = new ChartDataList();
            chartDataList.AddLagTestData(dt, "Lag Tests");

            return chartDataList;
        }

    }
}
