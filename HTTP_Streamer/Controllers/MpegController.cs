using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

using IPConnect_Testing.Images;
using System.Diagnostics;

namespace HTTP_Streamer.Controllers
{
    public class MpegController : ApiController
    {
        public const int ReadStreamBufferSize = 1024 * 1024;
        public const string boundary = "myboundary";

        [HttpGet]
        public HttpResponseMessage Stream()
        {
            HttpResponseMessage response = null;
            try
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new MJPEG().HTTPMultiPartPost(@"f:\temp\3", boundary);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                response.Content.Headers.Remove("Content-Type");
                response.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/x-mixed-replace;boundary=myboundary");
                //response.Content.Headers.TryAddWithoutValidation("X-FrameRate", "20000");
            }
            catch (Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            return response;

        }//Stream

        [HttpGet]
        public HttpResponseMessage Stream(int id)
        {
            var jpegStream = new MPegStream(@"f:\temp\3");
            Func<Stream, HttpContent, TransportContext, Task> func = jpegStream.WriteToStream;

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new PushStreamContent(func);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            response.Content.Headers.Remove("Content-Type");
            response.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/x-mixed-replace;boundary=" + jpegStream.boundary);
            return response;


        }//StreamAsync

    }//MPeg controller ends

}
