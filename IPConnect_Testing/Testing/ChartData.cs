using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Tools;
using IPConnect_Testing.Testing.DataObjects;

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
            chartData.datasets = new List<ChartDataSet>();
            
            ChartDataSet tp = new ChartDataSet();
            tp.label = "TP";
            tp.AssignColours(0);

            ChartDataSet fn = new ChartDataSet();
            fn.label = "FN";
            fn.AssignColours(1);

            ChartDataSet fp = new ChartDataSet();
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

        public void AddLagTestData(DataTable dt, string chartTitle)
        {
            ChartData chartData = new ChartData();
            chartData.chartTitle = chartTitle;
            chartData.datasets = new List<ChartDataSet>();

            //get the data
            LagTestDataList lagData = new LagTestDataList();
            lagData.Populate();

            //split the labels out
            var labels = lagData.list.GroupBy(x => x.numberMinutes).Select(y => y.First()).ToList();
            labels.ForEach(x => chartData.labels.Add(x.numberMinutes.ToString()));

            //one set of data each for sync/async and memory combinations
            var syncList = lagData.list.Where(x => !x.asynchronous).ToList();
            var asyncList = lagData.list.Where(x => x.asynchronous).ToList();

            var memoryList = lagData.list.GroupBy(x => x.memoryGb).Select(y => y.First().memoryGb.ToString()).ToList();

            //get the different data types
            int count = 0;
            foreach(String memory in memoryList)
            {
                var sync = syncList.Where(x => x.memoryGb.ToString().Equals(memory));
                var async = asyncList.Where(x => x.memoryGb.ToString().Equals(memory));

                var syncDataSet = new ChartDataSet();
                syncDataSet.label = "Sync " + memory + "GB";
                syncDataSet.AssignColours(count);
                count++;

                var asyncDataSet = new ChartDataSet();
                asyncDataSet.label = "ASync " + memory + "GB";
                asyncDataSet.AssignColours(count);
                count++;

                foreach (LagTestData data in sync)
                {
                    syncDataSet.data.Add(data.detectionSeconds);
                }

                foreach (LagTestData data in async)
                {
                    asyncDataSet.data.Add(data.detectionSeconds);
                }

               // chartData.datasets.Add(syncDataSet);
                chartData.datasets.Add(asyncDataSet);
            }

            //add the base line (processing same as minutes to play) for comparision purposes on the graph
            var controlSet = new ChartDataSet();
            controlSet.label = "Realtime";
            controlSet.AssignColours(count);
            chartData.labels.ForEach(x => controlSet.data.Add(x.StringToDec()));
            chartData.datasets.Add(controlSet);

            list.Add(chartData);

        }//AddLagTestData

    }//ChartDataList


    public class ChartData
    {
        public string chartTitle { get; set; }
        public List<String> labels = new List<string>();
        public List<ChartDataSet> datasets;
    }

    public class ChartDataSet
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

        public ChartDataSet()
        {
            pointStrokeColor = "#fff";
            pointHighlightStroke = "#fff";
            fillColor = "rgba(220,220,220,0)";
            data = new List<decimal>();
        }

        public void AssignColours(int number)
        {
            if (number == 0)
            {
                strokeColor = "rgba(39,208,67,225)";
                pointColor = "rgba(39,208,67,225)";
                pointHighlightStroke = "rgba(39,208,67,225)";

            }
            else if (number == 1)
            {
                strokeColor = "rgba(151,187,205,1)";
                pointColor = "rgba(151,187,205,1)";
                pointHighlightStroke = "rgba(151,187,205,1)";

            }
            else if (number == 2)
            {
                strokeColor = "rgba(232,46,46,225)";
                pointColor = "rgba(232,46,46,225)";
                pointHighlightStroke = "rgba(232,46,46,225)";
            }
            else if (number == 3)
            {
                strokeColor = "rgba(255,100,100,225)";
                pointColor = "rgba(240,100,100,225)";
                pointHighlightStroke = "rgba(240,100,100,225)";
            }
            else if (number == 4)
            {
                strokeColor = "rgba(255,145,0,225)";
                pointColor = "rgba(255,145,0,225)";
                pointHighlightStroke = "rgba(255,145,0,225)";
            }


        }

    }


}
