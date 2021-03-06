﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ImageAnalysis.Camera;
using System.Drawing;
using System.Net.Http.Headers;
using System.Web.Http.Cors;

namespace MotionManager.Controllers
{
    public class CameraController : ApiController
    {

        [Route("api/camera/{ipAddress}")]
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult Get(string ipAddress)
        {
            try
            {
                CameraModel camera = new CameraModel();
                camera.cameraIpAddress = ipAddress;
                return Ok(camera);
            }
            catch
            {
                return InternalServerError();
            }

        }

    }
}