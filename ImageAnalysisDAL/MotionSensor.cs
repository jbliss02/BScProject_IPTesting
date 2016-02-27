using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageAnalysisDAL
{
    /// <summary>
    /// Tools for writing and extracting information on motion detection to and from database
    /// </summary>
    public class MotionSensorDb : Db
    {
        public MotionSensorDb(string connectionString) :base(connectionString) { }


    }
}
