using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IPConnect_Testing.Images
{
    /// <summary>
    /// Tools for MJPEG file analysis
    /// </summary>
    class MJPEG
    {
        private string filePath; 
        private byte[] bytes; //loads to memory, the first method that loads adds here for subsequent use

        public byte[] startBoundary = new byte[2] { 255, 216 };
        public byte[] endBoundary = new byte[2] { 255, 217 };

        public MJPEG(string filePath) { this.filePath = filePath; bytes = File.ReadAllBytes(filePath); }

        public byte[] ByteArray()
        {
            return bytes;
        }

        public String HeaderString()
        {
            return Encoding.UTF8.GetString(Header());
        }

        /// <summary>
        /// Returns the byte representation of the MJPEG header.
        /// This is all characters up to the first ff d8 boundary
        /// </summary>
        /// <returns></returns>
        public byte[] Header()
        {
            MemoryStream stream = new MemoryStream(bytes);
            BinaryReader reader = new BinaryReader(stream);
            List<byte> header = new List<byte>(); //what will be returned
            bool boundaryFound = false;

            while(!boundaryFound)
            {
                header.AddRange(reader.ReadBytes(1)); //read byte by byte
                
                if(header.Count > 2 && header[header.Count - 2] == startBoundary[0] && header[header.Count - 1] == startBoundary[1])
                {
                    //remove the boundary flag
                    header.RemoveAt(header.Count - 1);
                    header.RemoveAt(header.Count - 1);
                    boundaryFound = true;
                }

            }//whole boundary not found

            return header.ToArray();

        }//Header

    }
}
