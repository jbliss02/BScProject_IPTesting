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
    public class JpegController : ApiController
    {
        public const int ReadStreamBufferSize = 1024 * 1024;
        public const string boundary = "myboundary";

        /// <summary>
        /// Gets a MPeg stream from the camera and session specified
        /// </summary>
        /// <param name="cameraId"></param>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        [Route("api/mpeg/{cameraId}/{sessionKey}")]
        public HttpResponseMessage Get(int cameraId, string sessionKey)
        {
            try
            {
                JpegStream jpegStream = new JpegStream(cameraId, sessionKey);
                Func<Stream, HttpContent, TransportContext, Task> func = jpegStream.WriteToStream;

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new PushStreamContent(func);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                response.Content.Headers.Remove("Content-Type");
                response.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/x-mixed-replace;boundary=" + jpegStream.boundary);
                return response;
            }
            catch(Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }


        /// <summary>
        /// Gets a MPeg stream from the camera and session specified
        /// Only shows frames between startframe and endframe
        /// </summary>
        /// <param name="cameraId"></param>
        /// <param name="sessionKey"></param>
        /// <param name="startframe"></param>
        /// <param name="endframe"></param>
        /// <returns></returns>
        [Route("api/mpeg/{cameraId}/{sessionKey}/{startframe}/{endframe}")]
        public HttpResponseMessage Get(int cameraId, string sessionKey, int startframe, int endframe)
        {
            if(endframe <= startframe) { return new HttpResponseMessage(HttpStatusCode.BadRequest); }

            try
            {
                JpegStream jpegStream = new JpegStream(cameraId, sessionKey, startframe, endframe);
                Func<Stream, HttpContent, TransportContext, Task> func = jpegStream.WriteToStream;

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new PushStreamContent(func);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                response.Content.Headers.Remove("Content-Type");
                response.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/x-mixed-replace;boundary=" + jpegStream.boundary);
                return response;
            }
            catch(Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

        }


    }//MPeg controller ends

}
