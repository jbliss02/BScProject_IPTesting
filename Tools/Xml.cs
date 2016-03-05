using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Tools
{
    /// <summary>
    /// Generic tools to manipulate, parse and serialise XML data
    /// </summary>
    public  class Xml
    {
        /// <summary>
        /// Serialises, and returns, a generic object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public XmlDocument SerialiseObject<T>(T obj)
        {
            XmlDocument result = new XmlDocument();
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer x = new XmlSerializer(typeof(T));
                x.Serialize(stream, obj);
                stream.Seek(0, System.IO.SeekOrigin.Begin); //without this there is a 'missing' root element error
                result.Load(stream);
            }

            return result;
        }

    }
}
