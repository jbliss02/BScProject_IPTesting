using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ImageAnalysis.Camera;
using System.Drawing;
using System.Net.Http.Headers;

namespace MotionManager.Controllers
{
    public class ImageController : ApiController
    {
        [Route("api/image/{ipAddress}")]
        [HttpGet]
        public HttpResponseMessage Get(string ipAddress)
        {
            try
            {
                CameraModel camera = new CameraModel();
                camera.cameraIpAddress = ipAddress;

                CameraFinder finder = new CameraFinder(camera);
                finder.GetImage();

                var result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(finder.ImageBytes);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                return result;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

        }
    }
}
