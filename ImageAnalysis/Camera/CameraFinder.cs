using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ImageAnalysis.Streams;
using ImageAnalysis.Images.Jpeg;

namespace ImageAnalysis.Camera
{
    /// <summary>
    /// Tries to connect to a camera at the specified IP location
    /// </summary>
    public class CameraFinder
    {
        IPAddress ipAddress;

        public bool ConnectSuccess { get; set; }
        public byte[] CameraImage { get; set; }
        private string cameraUrl { get; set; }

        public CameraFinder(string ipAddress)
        {
           // ipAddress = ip;
            cameraUrl = ipAddress + @"/axis-cgi/mjpg/video.cgi";
        }

        public void GetImage()
        {

                ImageExtractor imageExtractor = new ImageExtractor(cameraUrl, "root", "root");
                imageExtractor.asyncrohous = false; //need to wait for a success or fail
                imageExtractor.imageCreated += new ImageExtractor.ImageCreatedEvent(ImageExtracted);
                imageExtractor.Run(true);

        }

        private void ImageExtracted(ByteWrapper img, EventArgs e)
        {
            CameraImage = img.bytes;
            ConnectSuccess = true;
        }

    }
}
