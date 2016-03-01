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

        public List<MotionSetting> seperateSettingLists { get; set; } //CHANGE TO PRIOVATE

        public MotionSensorSettingsList() { }

        /// <summary>
        /// Populates the settings list will all possible combination of ranges, as defined in the 
        /// database. This can result in many elements.
        /// </summary>
        public void PopulateAllPossible()
        {
            list = new List<MotionSensorSettingsTest>();
            seperateSettingLists = new List<MotionSetting>();

            var db = new DAL.CaptureDbTest(ConfigurationManager.ConnectionStrings["AZURE"].ConnectionString);
            Convert(db.ReturnSettingTypeRanges());

        }//Populate

        /// <summary>
        /// Populates a list of where the settingType passed in is the only value that changes, 
        /// </summary>
        /// <param name="settingType"></param>
        public void PopulateRange(MotionSensorSettingTypes settingType)
        {
             
        }

        private void Convert(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                MotionSetting singleSetting = new MotionSetting();
                singleSetting.propertyName = dr["settingTypeName"].ToString();
                singleSetting.list = new List<MotionSensorSettingsTest>();

                if (dr["dataType"].ToString() == "decimal")
                {
                    decimal min = dr["minimum"].ToString().StringToDec();
                    decimal max = dr["maximum"].ToString().StringToDec();
                    decimal inc = dr["increment"].ToString().StringToDec();

                    for (decimal i = min; i <= max; i += inc)
                    {
                        MotionSensorSettingsTest test = new MotionSensorSettingsTest();
                        test.UpdateProperty(singleSetting.propertyName, i);
                        singleSetting.list.Add(test);
                    }

                }
                else if(dr["dataType"].ToString() == "integer")
                {
                    int min = dr["minimum"].ToString().StringToInt();
                    int max = dr["maximum"].ToString().StringToInt();
                    int inc = dr["increment"].ToString().StringToInt();

                    for (int i = min; i <= max; i += inc)
                    {
                        MotionSensorSettingsTest test = new MotionSensorSettingsTest();
                        test.UpdateProperty(singleSetting.propertyName, i);
                        singleSetting.list.Add(test);
                    }

                }

                seperateSettingLists.Add(singleSetting);

            }//each datarow

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

        public void UpdateProperty<T>(string propertyName, T value)
        {
            foreach (PropertyInfo property in typeof(MotionSensorSettingsTest).GetProperties())
            {
                if (property.Name.Equals(propertyName)) { property.SetValue(this, value); }
            }
        }
    
    }

    /// <summary>
    /// CHANGE TO INTERNAL
    /// </summary>
    public class MotionSetting
    {
        public List<MotionSensorSettingsTest> list { get; set; }
        public string propertyName { get; set; }//the property that has changed within this object

    }


}
