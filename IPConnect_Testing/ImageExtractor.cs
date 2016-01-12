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
        static string url = "http://192.168.0.3/axis-cgi/mjpg/video.cgi?date=1&clock=1&resolution=320x240";
        static string username = "root";
        static string password = "root";

        static int fileNumber = 0;

        static Regex contentLengthRegex = new Regex("Content-Length: (?<length>[0-9]+)\r\n\r\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        static string boundaryString = @"--myboundary";
        static byte[] boundaryBytes;
        static List<byte[]> images; //the final image files

        public ImageExtractor()
        {
            boundaryBytes = Encoding.ASCII.GetBytes(boundaryString); //set the boundary bytes from the boundaryString
            images = new List<byte[]>();
            Run();
        }

        private HttpWebResponse ReturnHttpResponse(string URI)
        {
            HttpWebRequest webrequest = null;
            HttpWebResponse webresponse = null;
            webrequest = (HttpWebRequest)WebRequest.Create(URI);
            webrequest.Credentials = new NetworkCredential(username, password);
            webresponse = (HttpWebResponse)webrequest.GetResponse();

            return webresponse;
        }

        private void Run()
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
                WriteBytesToFile(img);

            }//while stream.CanRead

            resp.Dispose();
            reader.Dispose();

        }//Run

        private string ReadHeader(BinaryReader reader)
        {
            string ret = String.Empty; //what gets returned
            bool gotHeader = false;

            List<byte> headerBuffer = new List<byte>(); //used to hold the stream whilst extracting the header 
            byte[] headBuff = new byte[4];

            while(!gotHeader)
            {
                reader.Read(headBuff, 0, 4); //read the stream, 4 bytes at a time to find the boundary
                headerBuffer.AddRange(headBuff); //build up a cumulative view of the header

                int boundaryStart = FindBoundary(headerBuffer);

                if (boundaryStart > -1)
                {
                    //re-set the buffers, want a much smaller space
                    headBuff = new byte[1];
                    headerBuffer = new List<byte>();

                    bool foundSplit = false;
                    while (!foundSplit)
                    {
                        //found the boundary, Content-Length starts 36 bytes later
                        reader.Read(headBuff, 0, 1); //read the stream byte by byte
                        headerBuffer.AddRange(headBuff);
                        ret = Encoding.UTF8.GetString(headerBuffer.ToArray());

                        if (contentLengthRegex.Match(ret).Success){
                            foundSplit = true;
                            gotHeader = true;
                        }

                    }//looking for split

                }//boundary found
            }//while looking for header

            return ret;
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

        private void WriteBytesToFile(byte[] img)
        {
            using (FileStream fs = new FileStream(GenerateFileName(), FileMode.Create))
            {
                fs.Write(img, 0, img.Length);
            }

            WriteScreen("Image creeted");
        }

        private String GenerateFileName()
        {
            return @"c:\temp\test_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_"
            + DateTime.Now.Millisecond + "_" + fileNumber + ".jpg";
        }

        private void WriteScreen(string st)
        {
            Console.WriteLine(DateTime.Now + " - " + st);
        }

    }//ImageExtractor.cs

}
