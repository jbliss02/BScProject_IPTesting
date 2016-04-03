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
        public bool ConnectSuccess { get; set; }
        public byte[] ImageBytes { get; set; }
        CameraModel cameraInfo;


        public CameraFinder(CameraModel cameraInfo)
        {
            this.cameraInfo = cameraInfo;
        }

        public void GetImage()
        {
            try
            {
                ImageExtractor imageExtractor = new ImageExtractor(cameraInfo.mpegUrl, "root", "root");
                imageExtractor.asyncrohous = false; //need to wait for a success or fail
                imageExtractor.imageCreated += new ImageExtractor.ImageCreatedEvent(ImageExtracted);
                imageExtractor.Run(true);
                ConnectSuccess = true;
            }
            catch
            {
                ConnectSuccess = false;
            }

        }

        public void AttemptConnection()
        {
            try
            {
                ImageExtractor imageExtractor = new ImageExtractor(cameraInfo.mpegUrl, "root", "root");
                imageExtractor.asyncrohous = false; //need to wait for a success or fail
                imageExtractor.Run(true);
                ConnectSuccess = true;
            }
            catch
            {
                ConnectSuccess = false;
            }
        }

        private void ImageExtracted(ByteWrapper img, EventArgs e)
        {
            ImageBytes = img.bytes;
            ConnectSuccess = true;
        }

    }
}
