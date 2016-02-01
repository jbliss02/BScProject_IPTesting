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
using HTTP_Streamer.Models;
using IPConnect_Testing.Images;
using System.Diagnostics;
using System.Configuration;

namespace HTTP_Streamer.Controllers
{
    public class MpegController : ApiController
    {
        public const int ReadStreamBufferSize = 1024 * 1024;
        public const string boundary = "myboundary";

        /// <summary>
        /// Syncrohous steram, loads all files as bytes before trasmitting
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //public HttpResponseMessage SyncStream()
        //{
        //    HttpResponseMessage response = null;
        //    try
        //    {
        //        response = new HttpResponseMessage(HttpStatusCode.OK);
        //        response.Content = new MJPEG().HTTPMultiPartPost(@"f:\temp\3", boundary);
        //        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        //        response.Content.Headers.Remove("Content-Type");
        //        response.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/x-mixed-replace;boundary=myboundary");
        //        //response.Content.Headers.TryAddWithoutValidation("X-FrameRate", "20000");
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        //    }

        //    return response;

        //}//Stream

        /// <summary>
        /// Asyncrohous stream
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpGet]
        //public HttpResponseMessage Stream(int id)
        //{
        //    var jpegStream = new MPegStream(@"f:\temp\4");
        //    Func<Stream, HttpContent, TransportContext, Task> func = jpegStream.WriteToStream;

        //    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        //    response.Content = new PushStreamContent(func);
        //    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        //    response.Content.Headers.Remove("Content-Type");
        //    response.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/x-mixed-replace;boundary=" + jpegStream.boundary);
        //    return response;


        //}//StreamAsync

        //Id is the session
        [Route("api/mpeg/{cameraId}/{sessionKey}")]
        public HttpResponseMessage Get(int cameraId, string sessionKey)
        {

            MPegStream jpegStream = new MPegStream(cameraId, sessionKey);
            Func<Stream, HttpContent, TransportContext, Task> func = jpegStream.WriteToStream;

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new PushStreamContent(func);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            response.Content.Headers.Remove("Content-Type");
            response.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/x-mixed-replace;boundary=" + jpegStream.boundary);
            return response;
        }

        //public HttpResponseMessage Get(int id, int jid)
        //{
        //    var jpegStream = new MPegStream(@"f:\temp\4");
        //    Func<Stream, HttpContent, TransportContext, Task> func = jpegStream.WriteToStream;

        //    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        //    response.Content = new PushStreamContent(func);
        //    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        //    response.Content.Headers.Remove("Content-Type");
        //    response.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/x-mixed-replace;boundary=" + jpegStream.boundary);
        //    return response;
        //}

    }//MPeg controller ends

}
