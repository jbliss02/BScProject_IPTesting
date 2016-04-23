using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ImageAnalysis.MotionSensor;

namespace MotionManager.Controllers
{
    public class MotionController : ApiController
    {
        // GET: api/Motion
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Motion/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Motion
        public void Post(MotionSensorSetup setup)
        {
            var x = "jkd";
        }

    }
}
 