using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public static class Helpers
    {

        /// <summary>
        /// returns a string made up of the current hour, minute, second and mllisecond
        /// </summary>
        /// <returns></returns>
        public static string ShortDateStamp()
        {
            DateTime now = DateTime.Now;

            return now.Hour.ToString() + now.Minute.ToString() + now.Second.ToString() + now.Millisecond.ToString();

        }

    }
}
