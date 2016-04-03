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
    public class CameraController : ApiController
    {

        public HttpResponseMessage Get()
        {
            CameraFinder finder = new CameraFinder("192.168.0.8");
            finder.GetImage();

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(finder.ImageBytes);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            return result;


        }

        public string Get(string ipAddress)
        {
            return String.Empty;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }



        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}