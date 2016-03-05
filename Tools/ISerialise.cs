using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    interface ISerialise
    {
        System.Xml.XmlDocument SerialiseMe();
    }
}
