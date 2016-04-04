using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageAnalysis.Camera
{
    public class CameraModel
    {
        /// <summary>
        /// At the moment always set to 0, but made available for future development to support
        /// multiple cameras
        /// </summary>
        public int cameraId { get { return 0; } }

        /// <summary>
        /// The IP address which the camera can be found at
        /// </summary>
        public string cameraIpAddress { get; set; }

        /// <summary>
        /// The HHTP location of the MPEG endpoint, if not provided will try
        /// to auto-populate
        /// </summary>
        public string mpegEndpoint
        {
            get
            {
                if(this._mpegEndpoint == null || this._mpegEndpoint == String.Empty)
                {
                    return @"/axis-cgi/mjpg/video.cgi";
                }
                else
                {
                    if(_mpegEndpoint.ToCharArray()[0].ToString() == @"/")
                    {
                        return this._mpegEndpoint;
                    }
                    else
                    {
                        return @"/" + this._mpegEndpoint;
                    }

                }
            }
            set
            {
                this._mpegEndpoint = value;
            }
        }
        private string _mpegEndpoint;

        public string mpegUrl
        {
            get
            {
                if (this._mpegUrl == null || this._mpegUrl == String.Empty)
                {
                    return "http://" + this.cameraIpAddress + this.mpegEndpoint;
                }
                else
                {
                    return this._mpegUrl;
                }
            }
            set
            {
                this._mpegUrl = value;
            }
        }

        private string _mpegUrl;

        private string _username;
        public string username
        {
            get
            {
                if (this._username == null || this._username == String.Empty)
                {
                    return "root";
                }
                else
                {
                    return this._username;
                }
            }
            set
            {
                this._username = value;
            }
        }

        private string _password;
        public string password
        {
            get
            {
                if (this._password == null || this._password == String.Empty)
                {
                    return "root";
                }
                else
                {
                    return this._password;
                }
            }
            set
            {
                this._password = value;
            }
        }

    }
}
