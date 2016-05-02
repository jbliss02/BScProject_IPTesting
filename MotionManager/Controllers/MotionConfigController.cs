using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ImageAnalysis.MotionSensor;
using System.Drawing;
using System.Net.Http.Headers;
using System.Web.Http.Cors;

namespace MotionManager.Controllers
{
    public class MotionConfigController : ApiController
    {
        [Route("api/motionconfig")]
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult Get()
        {
            try
            {
                MotionSensorConfigList list = new MotionSensorConfigList();
                list.LoadTemplate();
                return Ok(list);
            }
            catch
            {
                return InternalServerError();
            }

        }
    }
}
