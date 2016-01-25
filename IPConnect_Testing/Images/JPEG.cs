using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IPConnect_Testing.Images
{
    public class JPEG
    {
        string filePath;
        public JPEG(string filePath)
        {
            this.filePath = filePath;
        }

        public byte[] Bytes()
        {
            return File.ReadAllBytes(filePath);
        }

    }
}
