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
    }
}
