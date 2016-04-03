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
using MotionManager.Models;
using ImageAnalysis.Images;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Configuration;

namespace MotionManager.Controllers
{
    public class StreamController : ApiController
    {
        public const string boundary = "myboundary";

        /// <summary>
        /// Gets a MPeg stream from the camera and session specified
        /// </summary>
        /// <param name="cameraId"></param>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        [Route("api/jpeg/{cameraId}/{sessionKey}")]
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
            catch
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
        [Route("api/jpeg/{cameraId}/{sessionKey}/{startframe}/{endframe}")]
        public HttpResponseMessage Get(int cameraId, string sessionKey, int startframe, int endframe)
        {
            if (endframe <= startframe) { return new HttpResponseMessage(HttpStatusCode.BadRequest); }

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
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

        }

        /// <summary>
        /// Returns a JSON list of all the camera Id's that have a directory in the file system
        /// </summary>
        /// <returns></returns>
        [Route("api/jpeg")]
        public IHttpActionResult Get()
        {
            try
            {
                //get and return the list of cameraIds
                Cameras cameras = new Cameras();
                return Ok(JsonConvert.SerializeObject(cameras));
            }
            catch
            {
                return InternalServerError();
            }

        }//MPeg controller ends

        /// <summary>
        /// Returns a JSON list of all the capture session Ids 
        /// within the cameraId specified
        /// </summary>
        /// <returns></returns>
        [Route("api/jpeg/{cameraId}")]
        public IHttpActionResult Get(int cameraId)
        {
            try
            {
                //get and return the list of sessions in this cameraId
                CaptureSessions captures = new CaptureSessions(cameraId);
                return Ok(JsonConvert.SerializeObject(captures));
            }
            catch
            {
                return InternalServerError();
            }

        }//MPeg controller ends
    }
}
