using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using System.Web.Http;
using Microsoft.Owin.Hosting;

namespace HTTP_Streamer
{
    class Program
    {
        static void Main(string[] args)
        {
            WebApp.Start<Startup>(url: "http://localhost:9009/");
            Write("HTTP WEBAPI Stremer started");
            Console.ReadLine();

        }

        private static void Write(String s)
        {
            Console.WriteLine(DateTime.Now + " - " + s);
        }
    }
}
