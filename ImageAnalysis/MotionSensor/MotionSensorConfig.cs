using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ImageAnalysis.Camera;
using System.Reflection;
using System.Web.Configuration;

namespace ImageAnalysis.MotionSensor
{
    /// <summary>
    /// contains configuration information, allows the web.config
    /// settings to be overridden
    /// </summary>
    public class MotionSensorConfigList
    {
        public List<MotionSensorConfig> list = new List<MotionSensorConfig>();

        public CameraModel cameraModel; //set by client

        /// <summary>
        /// Ggets the config data from the database and populates the list
        /// </summary>
        public void LoadTemplate()
        {
            DataTable dt = new ImageAnalysisDAL.CaptureDb(ConfigurationManager.ConnectionStrings["LOCALDB"].ToString()).ReturnDetectionConfiguration();
            foreach(DataRow dr in dt.Rows)
            {
                list.Add(new MotionSensorConfig(dr));
            }

        }

        /// <summary>
        /// Updates the config file based on the list populated
        /// returns MotionSensorSettings, with settings updated for whatever is in the config file
        /// </summary>
        /// <returns></returns>
        public MotionSensorSetup UpdateConfig()
        {
            MotionSensorSetup result = new MotionSensorSetup();
            UpdateWebConfig();

            //update the result where appropriate
            foreach(MotionSensorConfig config in list)
            {
                foreach (PropertyInfo property in typeof(MotionSensorSetup).GetProperties())
                {
                    if (property.Name == config.configName && config.userInput != null)
                    {
                        property.SetValue(result, config.userInput);
                    }

                    if (config.configName.ToLower() == "emailalarmto" && config.userInput.Length > 0)
                    {
                        if (result.emailAlarm == null) { result.emailAlarm = new Alarms.EmailAlarm(); }
                        result.emailAlarm.emailAddress = config.userInput;
                    }
                    else if (config.configName.ToLower() == "emailsubject" && config.userInput.Length > 0)
                    {
                        if (result.emailAlarm == null) { result.emailAlarm = new Alarms.EmailAlarm(); }
                        result.emailAlarm.emailSubject = config.userInput;
                    }
                }
            }

            result.camera = cameraModel;
            return result;
        }

        private void UpdateWebConfig()
        {
            //to implement
        }

    }

    public class MotionSensorConfig
    {
        public string configName { get; set; }
        public string niceName { get; set; }
        public string configType { get; set; }
        public string validationRegex { get; set; }

        public string userInput { get; set; }

        public MotionSensorConfig() { }

        public MotionSensorConfig(DataRow dr)
        {
            new Tools.Data().ConvertDataRow<MotionSensorConfig>(dr, this);
        }

    }
}
