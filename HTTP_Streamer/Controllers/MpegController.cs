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

namespace HTTP_Streamer.Controllers
{
    public class MpegController : ApiController
    {
        public const int ReadStreamBufferSize = 1024 * 1024;
        public const string boundary = "myboundary";

        [HttpGet]
        public HttpResponseMessage stream()
        {
            HttpResponseMessage response = null;
            try
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);

                // response.Content = new ByteArrayContent(allbytes.ToArray());
                response.Content = new MJPEG().HTTPMultiPartPost(@"f:\temp\1", boundary);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                response.Content.Headers.Remove("Content-Type");
                response.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/x-mixed-replace;boundary=myboundary");
                
            }
            catch(Exception ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            return response;

        }//Stream

    }//MPeg controller ends

}
