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
            setup = new MotionSensorSetup();
            setup.camera = new ImageAnalysis.Camera.CameraModel();
            setup.camera.cameraIpAddress = "192.168.0.8";
            setup.imageSaveLocation = @"d:\motion";
            setup.saveImagesToFileServer = true;

            setup.emailAlarm = new ImageAnalysis.Alarms.EmailAlarm();
            setup.emailAlarm.emailAddress = "james.bliss@outlook.com";

            MotionSensorStartup motionSensor = new MotionSensorStartup(setup);

        }

    }
}
 