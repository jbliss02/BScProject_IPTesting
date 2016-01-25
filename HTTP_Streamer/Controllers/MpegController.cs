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

        [HttpGet]
        public HttpResponseMessage Stream()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.PartialContent);

            List<byte[]> bytes = new MJPEG().BytesFromFiles(@"f:\temp\stream");
            List<byte> allbytes = new List<byte>();

            //create the multipart section
            for (int i = 0; i < bytes.Count; i++)
            {
                allbytes.AddRange(Encoding.ASCII.GetBytes("--myboundary"));
                allbytes.AddRange(Encoding.ASCII.GetBytes("\r\n"));
                allbytes.AddRange(Encoding.ASCII.GetBytes("Content-Type: image/jpeg"));
                allbytes.AddRange(Encoding.ASCII.GetBytes("\r\n"));
                allbytes.AddRange(Encoding.ASCII.GetBytes("Content-Length: " + bytes[i].Length));
                allbytes.AddRange(Encoding.ASCII.GetBytes("\r\n"));
                allbytes.AddRange(Encoding.ASCII.GetBytes("\r\n"));
                allbytes.AddRange(bytes[i].ToList());
            }

            response.Content = new ByteArrayContent(allbytes.ToArray());
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            response.Content.Headers.Remove("Content-Type");
            response.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/x-mixed-replace;boundary=myboundary");

            return response;

        }//Stream

    }//MPeg controller ends

}
