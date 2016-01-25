﻿using System;
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
    public class MJPEG
    {
        private string filePath; 
        private byte[] bytes; //loads to memory, the first method that loads adds here for subsequent use

        public byte[] startBoundary = new byte[2] { 255, 216 };
        public byte[] endBoundary = new byte[2] { 255, 217 };

        public MJPEG() { }

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

        /// <summary>
        /// returns all bytes found between the start and end bytes of a JPEG
        /// </summary>
        /// <returns></returns>
        public List<byte[]> JpegBoundaryBytes()
        {
            List<byte[]> returnList = new List<byte[]>(); 
            List<byte> currentSection = null;

            for(int i = 1; i < bytes.Length; i++)
            {
                if (bytes[i - 1] == endBoundary[0] && bytes[i] == endBoundary[1])
                {
                    //end of jpeg found, start a new byte list
                    currentSection = new List<byte>();
                }
                else if(bytes[i - 1] == startBoundary[0] && bytes[i] == startBoundary[1])
                {
                    //start of jpeg found, add the bytes to the return list
                    if(currentSection != null) {
                        currentSection.RemoveAt(currentSection.Count - 1);
                        returnList.Add(currentSection.ToArray());
                    }
                }
                else if(currentSection != null)
                {
                    //in a jpeg so add the bytes
                    currentSection.Add(bytes[i]);
                }

            }//for each byte

            return returnList;

        }//JPegBoundaryBytes

        /// <summary>
        /// Takes a file directory and returns the bytes from each JPEG file within that directory
        /// </summary>
        /// <param name="fileDirectory"></param>
        /// <returns></returns>
        public List<byte[]> BytesFromFiles(string fileDirectory)
        {
            List<byte[]> ret = new List<byte[]>();

            foreach(var file in Directory.GetFiles(fileDirectory, "*.jpg"))
            {
                byte[] bytes = File.ReadAllBytes(file);
                ret.Add(bytes);
            }//for each file

            return ret;

        }//BytesFromFiles

    }
}