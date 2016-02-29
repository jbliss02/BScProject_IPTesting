using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Threading.Tasks;
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

        public struct sentitivity
        {
            public double min { get; set; }
            public double max { get; set; }
            public double increment { get; set; }
        }

        /// <summary>
        /// TODO - MAKE THIS DYNAMIC INTO A DATABASE 
        /// </summary>
        public void Populate()
        {
            //get a list of different sensitivities to start
            list = new List<MotionSensorSettingsTest>();

            sentitivity sens = new sentitivity();
            sens.min = 0.1;
            sens.max = 5;
            sens.increment = 0.2;

            for(double i = sens.min; i < sens.max; i += sens.increment)
            {

            }

        }
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
