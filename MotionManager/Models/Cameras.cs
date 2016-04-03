using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
using Tools;

namespace MotionManager.Models
{
    /// <summary>
    /// Contains information on what cameras there are 
    /// in the file server
    /// </summary>
    public class Cameras
    {
        public List<Int32> cameras;

        public Cameras()
        {
            GetAllCameras();
        }

        private void GetAllCameras()
        {
            List<String> directories = new List<string>();
            directories = Directory.GetDirectories(ConfigurationManager.AppSettings["SaveLocation"].ToString()).ToList();

            cameras = (from d in directories
                       where Regex.Split(d, "\\\\").Count() >= 3
                       select Regex.Split(d, "\\\\")[2].StringToInt()).ToList();

        }
    }
}
