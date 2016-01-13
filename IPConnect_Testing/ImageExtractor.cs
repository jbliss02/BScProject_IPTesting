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
    /// Attachs to an MJPEG HTTP stream and extracts the individual JPEG's
    /// </summary>
    public class ImageExtractor
    {
        public event ImageCreateEvent imageCreated;
        public delegate void ImageCreateEvent(byte[] img, EventArgs e);

        public List<byte[]> images; //the final image files

        string url;
        string username;
        string password;

        Regex contentLengthRegex = new Regex("Content-Length: (?<length>[0-9]+)\r\n\r\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        string boundaryString = @"--myboundary";
        byte[] boundaryBytes; //a byte version of the boundary

        public ImageExtractor(string url, string username, string password)
        {
            this.username = username;
            this.password = password;
            this.url = url;

            boundaryBytes = Encoding.ASCII.GetBytes(boundaryString); //set the boundary bytes from the boundaryString
            images = new List<byte[]>();
        }

        private HttpWebResponse ReturnHttpResponse(string URI)
        {
            HttpWebRequest req = null;
            HttpWebResponse resp = null;
            req = (HttpWebRequest)WebRequest.Create(URI);
            req.Credentials = new NetworkCredential(username, password);
            resp = (HttpWebResponse)req.GetResponse();

            return resp;
        }

        public void Run()
        {
            if (boundaryBytes == null) { throw new Exception("boundaryBytes was null"); }

            HttpWebResponse resp = ReturnHttpResponse(url);
            BinaryReader reader = new BinaryReader(resp.GetResponseStream());

            while (reader.BaseStream.CanRead)
            {
                String header = ReadHeader(reader); //moves the stream on, and extracts the header
                int contentLength = GetContentLength(header);

                byte[] img = reader.ReadBytes(contentLength);
                images.Add(img);
                OnFileCreate(img);

                //WriteBytesToFile(img);

            }//while stream.CanRead

            resp.Dispose();
            reader.Dispose();

        }//Run

        /// <summary>
        /// Steps through the stream until a header is identified
        /// Moves the stream to the position immediatley after the header (the jpeg)
        /// Returns the multipart HTTP header
        /// </summary>
        /// <param name="reader">the MJPEG HTTP binary stream</param>
        /// <returns></returns>
        private string ReadHeader(BinaryReader reader)
        {
            string header = String.Empty; //gets returned
            List<byte> headerBuffer = new List<byte>(); //a cumulative view of the stream
            byte[] headBuff = new byte[4]; //used to step throughthe stream

            while(header == String.Empty)
            {
                reader.Read(headBuff, 0, 4); //read the stream, 4 bytes at a time to find the boundary
                headerBuffer.AddRange(headBuff); 

                int boundaryStart = FindBoundary(headerBuffer);

                if (boundaryStart > -1)
                {
                    //boundary found, re-set the buffers and step through the stream byte by byte
                    headBuff = new byte[1];
                    headerBuffer = new List<byte>();

                    //step through the rest of the stream, looking for the Content-Length line
                    bool found = false;
                    while (!found)
                    {
                        reader.Read(headBuff, 0, 1); //read the stream byte by byte
                        headerBuffer.AddRange(headBuff);

                        header = Encoding.UTF8.GetString(headerBuffer.ToArray());

                        if (contentLengthRegex.Match(header).Success){
                            found = true;
                        }

                    }//looking for split

                }//boundary found
            }//while looking for header

            return header;

        }//ReadHeader

        private int FindBoundary(List<byte> bytes)
        {
            //looks for the boundary, if found returns the position in the byte array
            //that the boundary starts, it not returns -1
            for (int i = 0; i < bytes.Count; i++)
            {
                byte[] splitBytes = SplitByteArray(bytes, i, i + boundaryBytes.Length);
                if (BytesEqual(splitBytes, boundaryBytes))
                {
                    return i;
                }
            }

            return -1;
        }//FindBoundary

        private byte[] SplitByteArray(List<byte> bytes, int start, int end)
        {
            byte[] returnBytes = new byte[end - start];

            if (bytes.Count < (end - start) + start) { return returnBytes; }

            int count = 0;

            for (int i = start; i < end; i++)
            {
                returnBytes[count] = bytes[i];
                count++;
            }

            return returnBytes;

        }//SplitByteArray

        private bool BytesEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) { return false; }

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) { return false; }
            }

            return true;
        }//BytesEqual

        private int GetContentLength(string st)
        {
            //string passed in should contain Content-Length: xxx
            //extract the xxx and set as the 
            string[] split = Regex.Split(st, "\r\n");

            for(int i = 0; i < split.Length; i++)
            {
                if(split[i].Contains("Content-Length:"))
                {
                    string[] split2 = Regex.Split(split[i], "Content-Length: ");
                    return Int16.Parse(split2[1].Trim());
                }
            }

             throw new Exception("GetContentLength could not find Content-Length");

        }//GetContentLength

        protected virtual void OnFileCreate(byte[] img)
        {
            if (imageCreated != null)
            {
                imageCreated(img, EventArgs.Empty);
            }

        }



    }//ImageExtractor.cs

}
