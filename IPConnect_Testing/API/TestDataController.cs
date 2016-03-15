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
    public class TestDataController : ApiController
    {

        //Enable CORS allows this web service to send to a localhost dev box
        //stops the cross-origin errors
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public ChartDataList Get()
        {
            string conn = ConfigurationManager.ConnectionStrings["LOCALDB"].ConnectionString;
            var db = new DAL.CaptureDbTest(conn);
            DataTable settingTypes = db.ReturnSettingTypes();

            ChartDataList chartDataList = new ChartDataList();

            foreach(DataRow dr in settingTypes.Rows)
            {
                DataTable dt = db.ReturnTestConfusionData(dr.Field<int>("settingTypeId"));
                if(dt.Rows.Count > 0)
                {
                chartDataList.AddMotionTestData(dt, dr["settingTypeName"].ToString().ToTitleString());
                }
            }

            return chartDataList;
        }



    }
}
