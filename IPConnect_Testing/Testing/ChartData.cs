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
        public List<ChartData> list = new List<ChartData>();

        public void AddMotionTestData(DataTable dt, string chartTitle)
        {
            ChartData chartData = new ChartData();
            chartData.chartTitle = chartTitle;
            chartData.datasets = new List<ChartDataSets>();
            
            ChartDataSets tp = new ChartDataSets();
            tp.label = "TP";
            tp.AssignColours(0);

            ChartDataSets fn = new ChartDataSets();
            fn.label = "FN";
            fn.AssignColours(1);

            ChartDataSets fp = new ChartDataSets();
            fp.label = "FP";
            fp.AssignColours(2);

            foreach (DataRow dr in dt.Rows)
            {
                chartData.labels.Add(Math.Round(dr["value"].ToString().StringToDec(), 1).ToString());
                tp.data.Add(dr["TP"].ToString().StringToDec());
                fn.data.Add(dr["FN"].ToString().StringToDec());
                fp.data.Add(dr["FP"].ToString().StringToDec());
            }

            chartData.datasets.Add(tp);
            chartData.datasets.Add(fn);
            chartData.datasets.Add(fp);

            list.Add(chartData);
        }


    }//ChartDataList


    public class ChartData
    {
        public string chartTitle { get; set; }
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
        public string pointHighlightFill { get; set; }
        public string color { get; set; }

        public List<decimal> data; 

        public ChartDataSets()
        {
            pointStrokeColor = "#fff";
            pointHighlightStroke = "#fff";
            fillColor = "rgba(220,220,220,0)";
            data = new List<decimal>();
        }

        public void AssignColours(int number)
        {
            if(number == 0)
            {
                strokeColor = "rgba(39,208,67,225)";
                pointColor = "rgba(39,208,67,225)";
                pointHighlightStroke = "rgba(39,208,67,225)";

            }
            else if(number == 1)
            {
                strokeColor= "rgba(151,187,205,1)";
                pointColor= "rgba(151,187,205,1)";
                pointHighlightStroke= "rgba(151,187,205,1)";

            }
            else if(number == 2)
            {
                strokeColor = "rgba(232,46,46,225)";
                pointColor = "rgba(232,46,46,225)";
                pointHighlightStroke = "rgba(232,46,46,225)";

            }
                

        }

    }


}
