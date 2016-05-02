using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ImageAnalysis.MotionSensor;
using System.Net.Http.Headers;
using System.Web.Http.Cors;

namespace MotionManager.Controllers
{
    public class MotionController : ApiController
    {

        [Route("api/motion")]
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public void Post(MotionSensorConfigList config)
        {
            MotionSensorSetup setup = config.UpdateConfig();


            MotionSensorStartup startup = new MotionSensorStartup(setup);

        }

    }
}
 