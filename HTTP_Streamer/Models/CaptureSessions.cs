using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
using Tools;

namespace HTTP_Streamer.Models
{
    /// <summary>
    /// Contains information on the different capture sections, from a specific camera Is
    /// </summary>
    public class CaptureSessions
    {
        public List<String> sessions;

        public CaptureSessions(int cameraId)
        {
            GetAllSessions(cameraId);
        }

        private void GetAllSessions(int cameraId)
        {
            List<String> directories = new List<string>();
            directories = Directory.GetDirectories(ConfigurationManager.AppSettings["SaveLocation"].ToString() + @"\" + cameraId).ToList();

            sessions = (from d in directories
                        where Regex.Split(d, "\\\\").Count() >= 4
                        select Regex.Split(d, "\\\\")[3]).ToList();

        }
    }
}
