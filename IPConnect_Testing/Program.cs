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

    /// <summary>
    /// This static calling program works as the BUS
    /// </summary>

    class Program
    {
        static string url = "http://192.168.0.3/axis-cgi/mjpg/video.cgi?date=1&clock=1&resolution=320x240";
        static string username = "root";
        static string password = "root";

        static void Main(string[] args)
        {
            ImageExtractor imageExtractor = new ImageExtractor(url, username, password);

            //create the validator and subs
            ImageValidator imageValidator = new ImageValidator();
            imageValidator.ListenForImages(imageExtractor);

            //subscribe to events from the validator
            imageValidator.imageValidated += new ImageValidator.ImageValidatedEvent(ValidImageEventHandler);
            



           // new ImageSaver().ListenForImages(imageExtractor);

            imageExtractor.Run();

        }

        /// <summary>
        /// Subscribes to a Image Validated event, then calls the methods to save the image, asyncrohously
        /// </summary>
        /// <param name="img"></param>
        /// <param name="e"></param>
        static void ValidImageEventHandler(byte[] img, EventArgs e)
        {
            Task getInt = new ImageSaver().FileCreatedAsync(img, e);

        }

        private static UInt64 Hash()
        {
            //this needs to be changed and validated
            ulong hash = (UInt64)(int)DateTime.Now.Kind;
            return (hash << 62) | (UInt64) DateTime.Now.Ticks;
        }

    }
}
