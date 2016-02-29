using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using ImageAnalysis.MotionSensor;
using ImageAnalysis.Data;
using ImageAnalysisDAL;
using Tools;

namespace IPConnect_Testing.Testing
{
    /// <summary>
    /// Collection of various motion sensor settings. Contains methods to extract and populate those settings
    /// </summary>
    public class MotionSensorSettingsList
    {
        public List<MotionSensorSettingsTest> list { get; set; }

        public MotionSensorSettingsList() { Populate(); }

        public void Populate()
        {
            //get a list of different sensitivities to start
            list = new List<MotionSensorSettingsTest>();

            var db = new DAL.CaptureDbTest(ConfigurationManager.ConnectionStrings["AZURE"].ConnectionString);
            Convert(db.ReturnSettingTypeRanges());

        }//Populate

        private void Convert(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {

                if (dr["dataType"].ToString() == "decimal" && dr["settingTypeName"].ToString() == "sensitivity")
                {
                    decimal min = dr["minimum"].ToString().StringToDec();
                    decimal max = dr["maximum"].ToString().StringToDec();
                    decimal inc = dr["increment"].ToString().StringToDec();

                    for (decimal i = min; i <= max; i += inc)
                    {
                        MotionSensorSettingsTest test = new MotionSensorSettingsTest();
                        test.sensitivity = i;
                        list.Add(test);
                    }

                }
                else if(dr["settingTypeName"].ToString() == "int")
                {

                }

            }
        }//Convert

    }

    /// <summary>
    /// A testing wrapper for MotionSensorSettings
    /// includes methods to set pre-defined settings and to write to database
    /// </summary>
    public class MotionSensorSettingsTest : MotionSensorSettings
    {
        public string captureId { get; set; }

        public XmlDocument SerialiseMe()
        {
            //uses reflection to create an XML document which contains each property 
            //as a seperate element

            XmlDocument xmldoc = new XmlDocument();
            XmlElement root = xmldoc.CreateElement("MotionSensorSettingsTest");
            xmldoc.AppendChild(root);

            foreach (PropertyInfo property in typeof(MotionSensorSettingsTest).GetProperties())
            {
                XmlElement node = xmldoc.CreateElement("DataType");
                root.AppendChild(node);

                XmlElement propertyName = xmldoc.CreateElement("propertyName");
                propertyName.InnerXml = property.Name;
                node.AppendChild(propertyName);

                XmlElement propertyValue = xmldoc.CreateElement("propertyValue");

                var value = property.GetValue(this);

                if (value != null)
                {
                    propertyValue.InnerXml = value.ToString();
                }
                else
                {
                    propertyValue.InnerXml = "";
                }

                
                node.AppendChild(propertyValue);
            }

            return xmldoc;

        }

    }

}
