using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace IPConnect_Testing
{

    class Program
    {
        static string url = "http://192.168.0.3/axis-cgi/mjpg/video.cgi?date=1&clock=1&resolution=320x240";
        static string username = "root";
        static string password = "root";

        static void Main(string[] args)
        {

            ImageExtractor imageExtractor = new ImageExtractor(url, username, password);
            new ImageSaver().ListenForImages(imageExtractor);
            new ImageValidator().ListenForImages(imageExtractor);
            imageExtractor.Run();

        }

    }
}
