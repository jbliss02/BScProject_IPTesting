﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageAnalysis.Camera;
using ImageAnalysis.Alarms;
using System.Configuration;

namespace ImageAnalysis.MotionSensor
{
    /// <summary>
    /// Exposes the attributes required to setup a motion sensor session, can be created from a number of sources
    /// including a web service
    /// </summary>
    public class MotionSensorSetup
    {
        public CameraModel camera { get; set; }

        public IAlarm emailAlarm { get; set; }

        public IAlarm smtpAlarm { get; set; }

        public string imageSaveLocation
        {
            get {
                if(this._imageSaveLocation == null || this._imageSaveLocation == String.Empty)
                {
                    return ConfigurationManager.AppSettings["SaveLocation"];
                }
                else
                {
                    return this._imageSaveLocation;
                }
            }
            set { this._imageSaveLocation = value; }
        }

        private string _imageSaveLocation;
    }
}
