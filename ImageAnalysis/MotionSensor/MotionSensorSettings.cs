using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Reflection;
using Tools;
using ImageAnalysisDAL;

namespace ImageAnalysis.MotionSensor
{
    public enum MotionSensorSettingTypes
    {
        ASYNCROHOUS,
        FRAMES_TO_SKIP,
        HORIZONTAL_FRAMES_TO_SKIP,
        VERTICAL_FRAMES_TO_SKIP,
        SENSITIVITY
    }

    /// <summary>
    /// Various settings that are applied to the motion detector during testing
    /// </summary>
    public class MotionSensorSettings
    {
        /// <summary>
        /// Whether the motion detector will analyse frames asyncrohously
        /// </summary>
        public bool asynchronous { get; set; }

        /// <summary>
        /// The number of frames to skip during image analysis of motion detection
        /// Varaible is always used, defaults to 0
        /// </summary>
        public int framesToSkip { get { return _framesToSkip; } set { if (value >= 0) { _framesToSkip = value; }; } }
        private int _framesToSkip;

        /// <summary>
        /// The number of pixels skipped horizontally when scanning an image
        /// Variable is always used, defaults to 0
        /// </summary>
        public int horizontalPixelsToSkip { get { return _horizontalPixelsToSkip; } set { if (value >= 0) { this._horizontalPixelsToSkip = value; }; } }
        private int _horizontalPixelsToSkip;

        /// <summary>
        /// The number of pixels skipped vertically when scanning an image
        /// Variable is always used, defaults to 0
        /// </summary>
        public int verticalPixelsToSkip { get { return _verticalPixelsToSkip; } set { if (value >= 0) { this._verticalPixelsToSkip = value; }; } }
        private int _verticalPixelsToSkip;

        /// <summary>
        /// Multiplier used when determining whether there is motion, or not. Defaults to 1. Less than 1 increases
        /// sensitivity, great than 1 decreases sensitivity
        /// </summary>
        public decimal sensitivity { get; set; }

        /// <summary>
        /// The number of pixels to search horizontally. Defaults to the image width if not set
        /// </summary>
        public int searchWidth { get; set; }

        /// <summary>
        /// The number of pixels to search vertically. Defaults to the image height if not set
        /// </summary>
        public int searchHeight { get; set; }

        /// <summary>
        /// Whether a compare action should try and share data with the next compare action
        /// only applicable when running syncrohously
        /// </summary>
        public bool linkCompare { get; set; }

        public MotionSensorSettings() { framesToSkip = 0; horizontalPixelsToSkip = 0; verticalPixelsToSkip = 0; sensitivity = 1; asynchronous = true; }

        /// <summary>
        /// Loads the default settings, from values in the database
        /// </summary>
        public void LoadDefaults()
        {

            var db = new CaptureDb(ConfigurationManager.ConnectionStrings["LOCALDB"].ConnectionString);
            DataTable dt = db.ReturnMotionSettingDefaults();

            //iterate over the properties in this object, and the properties in the Datatable until a match is found
            foreach(DataRow dr in dt.Rows)
            {
                foreach (PropertyInfo property in typeof(MotionSensorSettings).GetProperties())
                {
                    if (property.Name.Equals(dr["settingTypeName"].ToString()))
                    {
                        if(property.PropertyType.ToString() == "System.Decimal")
                        {
                            property.SetValue(this, dr["value"].ToString().StringToDec());
                        }
                        else if(property.PropertyType.ToString() == "System.Int32")
                        {
                            property.SetValue(this, dr["value"].ToString().StringToInt());
                        }
                        else if (property.PropertyType.ToString() == "System.Boolean")
                        {
                            property.SetValue(this, dr["value"].ToString().StringToBool());
                        }
                    }
                }

            }//foreach datarow
        }

    }


    public class TestSetting : System.Attribute
    {
        private string description;

        public string Description { get { return description; } }

        public TestSetting(string description)
        {
            this.description = description;
        }
    }

}
