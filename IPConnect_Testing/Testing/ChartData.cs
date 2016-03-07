using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Tools;

namespace IPConnect_Testing.Testing
{
    /// <summary>
    /// Transforms data into a form that ChartJs uses
    /// </summary>
    public class ChartDataList
    {
        public List<ChartData> data;

        public ChartData ConvertMotionTestData(DataTable dt)
        {
            data = new List<ChartData>();

            ChartData chartData = new ChartData();
            chartData.datasets = new List<ChartDataSets>();
            
            ChartDataSets tp = new ChartDataSets();
            tp.label = "TP";
            ChartDataSets fn = new ChartDataSets();
            fn.label = "FN";
            ChartDataSets fp = new ChartDataSets();
            fp.label = "FP";

            foreach (DataRow dr in dt.Rows)
            {
                chartData.labels.Add(dr["value"].ToString());
                tp.data.Add(dr["TP"].ToString().StringToDec());
                fn.data.Add(dr["FN"].ToString().StringToDec());
                fp.data.Add(dr["FP"].ToString().StringToDec());
            }

            chartData.datasets.Add(tp);
            chartData.datasets.Add(fn);
            chartData.datasets.Add(fp);

            return chartData;
        }


    }//ChartDataList


    public class ChartData
    {
        public List<String> labels = new List<string>();
        public List<ChartDataSets> datasets;
    }

    public class ChartDataSets
    {
        public string label { get; set; }
        public string fillColor { get; set; }
        public string strokeColor { get; set; }
        public string pointColor { get; set; }
        public string pointStrokeColor { get; set; }
        public string pointHighlightStroke { get; set; }

        public List<decimal> data; 

        public ChartDataSets()
        {
            fillColor = "rgba(220,220,220,0.2)";
            strokeColor = "rgba(220,220,220,1)";
            pointColor = "rgba(220,220,220,1)";
            pointStrokeColor = "#fff";
            pointHighlightStroke = "rgba(220,220,220,1)";
            data = new List<decimal>();
        }
    }


}
