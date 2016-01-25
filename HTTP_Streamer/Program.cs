using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using System.Web.Http;

namespace HTTP_Streamer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new HttpSelfHostConfiguration("http://localhost:8080");

            config.Routes.MapHttpRoute(
                name: "api",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            using (HttpSelfHostServer server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                Write("Web API started");
                Console.ReadLine();
            }

        }//Main

        private static void Write(String s)
        {
            Console.WriteLine(DateTime.Now + " - " + s);
        }
    }
}
