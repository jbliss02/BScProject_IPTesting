﻿using System;
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

namespace IPConnect_Testing.API
{
    public class TestDataController : ApiController
    {

        public ChartData Get()
        {

            string conn = ConfigurationManager.ConnectionStrings["LOCALDB"].ConnectionString;
            DataTable dt = new DAL.CaptureDbTest(conn).ReturnTestConfusionData(2);

            ChartDataList chartDataList = new ChartDataList();
            ChartData chartData = chartDataList.ConvertMotionTestData(dt);
            return chartData;
        }
    }
}
