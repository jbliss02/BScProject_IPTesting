using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageAnalysisDAL;
using IPConnect_Testing.Data;
using System.Configuration;

namespace IPConnect_Testing.Data
{
    public class CaptureList
    {
        public void PopulateAllCaptures()
        {
            ConnectionStringSettingsCollection connections = ConfigurationManager.ConnectionStrings;


            CaptureInfo captureInfo = new CaptureInfo(ConfigurationManager.ConnectionStrings["AZURE"].ConnectionString);
            captureInfo.ReturnAllCaptures();
        }
    }
    public class Capture
    {
    }
}
