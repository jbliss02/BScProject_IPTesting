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
    class Program
    {
        static string url = "http://192.168.0.3/axis-cgi/mjpg/video.cgi?date=1&clock=1&resolution=320x240";
        static string username = "root";
        static string password = "root";

        static Regex contentLengthRegex = new Regex("Content-Length: (?<length>[0-9]+)\r\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        static string boundaryString = @"--myboundary";
        static byte[] boundaryBytes;

        static HttpClient client;
        static BinaryReader binReader;

        static List<byte[]> buffer; //keep the array of bytes to find the seperator
        static List<byte[]> images; //the final image files

        static List<byte> headerBuffer = new List<byte>(); //used to hold the stream whilst extracting the header

        static void Main(string[] args)
        {

            new ImageExtractor();
            return;
            boundaryBytes = Encoding.ASCII.GetBytes(boundaryString); //set the boundary bytes from the boundaryString
            RunStream();

            //ExtractSingleJpeg();


        }

        static void RunStream()
        {
            if (boundaryBytes == null) { throw new Exception("boundaryBytes was null"); }

            byte[] img; //the image that is going to be built on the fly

            String lsResponse = string.Empty;
            using (HttpWebResponse lxResponse = (HttpWebResponse)ReturnHttpResponse(url))
            {
                using (BinaryReader reader = new BinaryReader(lxResponse.GetResponseStream()))
                {
                    while (reader.BaseStream.CanRead)
                    {
                        byte[] headBuff = new byte[4];
                        reader.Read(headBuff, 0, 4); //read the stream, 12 bytes at a time to find the boundary
                        headerBuffer.AddRange(headBuff); //build up a full 

                        int boundaryStart = FindBoundary(headerBuffer);

                        if(boundaryStart > -1)
                        {
                            //re-set th buffers, want a much smaller space
                            headBuff = new byte[1];
                            headerBuffer = new List<byte>();

                            bool foundSplit = false;
                            while (!foundSplit)
                            {
                                //found the boundary, content-length starts 36 bytes later
                                reader.Read(headBuff, 0, 1); //read the stream byte by byte
                                headerBuffer.AddRange(headBuff);
                                string st = Encoding.UTF8.GetString(headerBuffer.ToArray());

                                if(contentLengthRegex.Match(st).Success)
                                {

                                }

                                var x = "kd";


                            }//looking for split

                        }//boundary found



                    }//while stream.CanRead
                }
            }
        }//RunStream

        static string ReturnHeader()
        {
            return "x";
        }

        static bool SeperatorBytesExistsInArray(int position, byte[] array)
        {
            bool result = false;
            for (int i = position, j = 0; j < boundaryBytes.Length; i++, j++)
            {
                result = array[i] == boundaryBytes[j];
                if (!result)
                {
                    break;
                }
            }
            return result;
        }

        static int FindBoundary(byte[] bytes)
        {
            //looks for the boundary, if found returns the position in the byte array
            //that the boundary starts, it not returns -1
            for (int i = 0; i < bytes.Length; i++)
            {
                byte[] splitBytes = SplitByteArray(bytes, i, i + boundaryBytes.Length);
                if (BytesEqual(splitBytes, boundaryBytes))
                {
                    return i;
                }
            }

            return -1;
        }

        static int FindBoundary(List<byte> bytes)
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
        }

        static byte[] SplitByteArray(byte[] bytes, int start, int end)
        {
            byte[] returnBytes = new byte[end - start];

            if(bytes.Length < (end - start) + start) { return returnBytes; }

            int count = 0;

            for(int i = start; i < end; i++)
            {
                returnBytes[count] = bytes[i];
                count++;
            }

            return returnBytes;
        }

        static byte[] SplitByteArray(List<byte> bytes, int start, int end)
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
        }

        static int ContentLength(string contentLengthString)
        {
            //array starts at the line for contentLengthBytes
            // string contentLength = System.Text.Encoding.Default.GetString(contentLengthBytes).Split(@"/r/n") ;
            string[] split = Regex.Split(contentLengthString, "\r\n");

            return Int32.Parse(split[1].Replace("Content-Length: ", "").Trim());
        }

        static bool BytesEqual(byte[] a, byte[] b)
        {
            if(a.Length != b.Length) { return false; }

            for(int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) { return false; }
            }

            return true;
        }

        #region ramblings

        static void InitiateHandler()
        {
            WebRequestHandler handler = new WebRequestHandler();
            handler.Credentials = new NetworkCredential(username, password);
            client = new HttpClient(handler);
            client.BaseAddress = new Uri(url);
            client.Timeout = TimeSpan.FromMilliseconds(-1);

            var stream = client.GetStreamAsync(url).Result;

            binReader = new BinaryReader(new BufferedStream(stream));

            using (var reader = new BinaryReader(new BufferedStream(stream)))
            {
                while (!reader.BaseStream.CanRead)
                {
                    Console.WriteLine(reader.ReadBytes(1000));
                }
            }



            //using (var reader = new StreamReader(stream))
            //{
            //    while (!reader.EndOfStream)
            //    {
            //        Console.WriteLine(reader.ReadLine());
            //    }
            //}


        }

        static HttpWebResponse ReturnHttpResponse(string URI)
        {
            HttpWebRequest webrequest = null;
            HttpWebResponse webresponse = null;
            webrequest = (HttpWebRequest)WebRequest.Create(URI);
            webrequest.Credentials = new NetworkCredential(username, password);
            webresponse = (HttpWebResponse)webrequest.GetResponse();

            return webresponse;
        }

        static void ExtractBoundary()
        {
            //used to find lines (like Content-Type and so on)
            HttpWebRequest webrequest = null;
            HttpWebResponse webresponse = null;
            webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Credentials = new NetworkCredential(username, password);
            webresponse = (HttpWebResponse)webrequest.GetResponse();

            using (Stream resStream = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(resStream, Encoding.ASCII);

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (line.Contains("boundary"))
                    {
                        Console.WriteLine(line);
                        boundaryBytes = new System.Text.ASCIIEncoding().GetBytes(line);
                        return;
                    }
                }
            }
        }

        static void ExtractPostInfo()
        {
            List<String> lines = new List<string>();
            List<byte[]> bytes = new List<byte[]>();

            int count = 0; //sends on the second instance of hitting boundary

            using (Stream resStream = ReturnHttpResponse(url).GetResponseStream())
            {
                StreamReader reader = new StreamReader(resStream, Encoding.ASCII);

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    lines.Add(line);
                    bytes.Add(Encoding.ASCII.GetBytes(line));

                    if (line.Contains("boundary"))
                    {
                        count++;
                        if(count == 2) {
                            Try4(bytes, lines);
                        }
                    }
                }
            }
        }
        
        static void ExtractSingleJPeg_test()
        {
            HttpWebRequest lxRequest = (HttpWebRequest)WebRequest.Create("http://www.productimageswebsite.com/images/stock_jpgs/34891.jpg");

            // returned values are returned as a stream, then read into a string
            String lsResponse = string.Empty;
            using (HttpWebResponse lxResponse = (HttpWebResponse)lxRequest.GetResponse())
            {
                using (BinaryReader reader = new BinaryReader(lxResponse.GetResponseStream()))
                {
                    Byte[] lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                    using (FileStream lxFS = new FileStream(@"c:\temp\34891.jpg", FileMode.Create))
                    {
                        lxFS.Write(lnByte, 0, lnByte.Length);
                    }
                }
            }
        }

        static void ExtractSingleJpeg()
        {

            //THIS, SORT OF, WORKS
            String lsResponse = string.Empty;
            using (HttpWebResponse lxResponse = (HttpWebResponse)ReturnHttpResponse(url))
            {
                using (BinaryReader reader = new BinaryReader(lxResponse.GetResponseStream()))
                {
                    Byte[] lnByte = reader.ReadBytes(30000);
                    using (FileStream lxFS = new FileStream(@"c:\temp\test222.jpg", FileMode.Create))
                    {
                        lxFS.Write(lnByte, 0, lnByte.Length);
                    }
                }
            }
        }//ExtractSingleJpeg

        static List<byte[]> ReturnImage(List<byte[]> input)
        {
            //strips the HTTP gumpf
            input.RemoveAt(3);
            input.RemoveAt(2);
            input.RemoveAt(1);
            input.RemoveAt(0);
            input.RemoveAt(input.Count - 1);

            return input;
        }

        static void Try2()
        {
            buffer = new List<byte[]>();

            HttpWebRequest webrequest = null;
            HttpWebResponse webresponse = null;
            webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Credentials = new NetworkCredential(username, password);
            webresponse = (HttpWebResponse)webrequest.GetResponse();

            string fileName = @"c:\temp\jpgtest.jpg";
            using (System.IO.FileStream fs = System.IO.File.Create(fileName, 1024))
            {
                using (Stream resStream = webresponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(resStream, Encoding.ASCII);

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        //extract the content length byte number, then try to read only that many bytes

                        if (line.Contains("Content-Type"))
                        {
                            //Console.WriteLine(line);

                            for(int i = 0; i < 5; i++)
                            {
                                //Console.WriteLine(reader.ReadLine());
                            }

                        }

                        var nBytes = reader.CurrentEncoding.GetByteCount(line);

                        buffer.Add(new System.Text.ASCIIEncoding().GetBytes(line));

                        if (line.Contains("Content-Length"))
                        {
                            Console.WriteLine("Found it");

                            //while (!_contRegex.Match(line).Success)
                            //{
                            //    line = reader.ReadLine();
                            //    byte[] info = new System.Text.UTF8Encoding(true).GetBytes(line);
                            //    fs.Write(info, 0, info.Length);
                            //}

                            //while (!SeperatorBytesExistsInArray(line))
                            //{
                            //    line = reader.ReadLine();
                            //    byte[] info = new System.Text.ASCIIEncoding().GetBytes(line);
                            //    fs.Write(info, 0, info.Length);
                            //}


                        }//line.Contains("Content-Length"))

                    }//while there is a stream

                }
            }//using streanm

        }

        static void Try3()
        {
            //run stream putting thinfgs into the buffer, starting just after the Content-Length line 
            //extract the number of bytes the Content-Length declares 
            //continue running stream, when the second Content-Length is found then extract from the buffer the number of bytes specified
                 
            HttpWebRequest webrequest = null;
            HttpWebResponse webresponse = null;
            webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Credentials = new NetworkCredential(username, password);
            webresponse = (HttpWebResponse)webrequest.GetResponse();

            using (Stream resStream = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(resStream, Encoding.ASCII);

                int nBytes = 0; //number of bytes for the current stream
                int nBoundary = 0; //number of time Content-Length is found

                while (!reader.EndOfStream)
                {

                    string line = reader.ReadLine();

                    if (line.Contains("Content-Length"))
                    {
                        nBoundary++;
                        if (nBoundary == 1) { buffer = new List<byte[]>(); reader.ReadLine(); }//get rid of blank line 
                        if (nBoundary > 1) { ExtractFile(nBytes); }
                        nBytes = Int32.Parse(line.Replace("Content-Length: ", "").Trim()); //extract the number of bytes this file has                     
                    }

                    if (buffer != null) {

                        line = reader.ReadLine();

                        if(ContainsBoundaryBytes(line)){ return; }

                        buffer.Add(new System.Text.ASCIIEncoding().GetBytes(line));
                    }
                    else
                    {
                        reader.ReadLine();
                    }

                }//while there is a stream

            }//using streanm

        }

        static void Try4(List<byte[]> bytes, List<String> lines)
        {
            //bytes is a whole http post
            //top 4 lines are header
            //bottom line is boundary end
            //remove these then we have a full image in bytes

            int fileNumber = 1;

            bytes.RemoveAt(3);
            bytes.RemoveAt(2);
            bytes.RemoveAt(1);
            bytes.RemoveAt(0);
            bytes.RemoveAt(bytes.Count - 1);

            string fileName = @"c:\temp\jpgtest_" + fileNumber + ".jpg";

            using (System.IO.FileStream fs = System.IO.File.Create(fileName, 1024))
            {
                for (int i = 0; i < bytes.Count; i++)
                {
                    fs.Write(bytes[i], 0, bytes[i].Length);
                    //byte[] newline = Encoding.ASCII.GetBytes(Environment.NewLine);
                    //fs.Write(newline, 0, newline.Length);
                }

            }//using 

            lines.RemoveAt(3);
            lines.RemoveAt(2);
            lines.RemoveAt(1);
            lines.RemoveAt(0);
            lines.RemoveAt(lines.Count - 1);

        }

        static void ExtractFile(int nBytes)
        {
            //get the current buffer size
            var currentBuffer = (from b in buffer
                                 select b.Length).Sum();


            if (currentBuffer > nBytes)
            {            
                int countBytes = 0; 

                for (int i = 0; i < buffer.Count; i++)
                {
                    countBytes = countBytes + buffer[i].Length;

                    if(countBytes > nBytes)
                    {
                        Console.WriteLine("yes");
                    }
                }

           //     var newFileBytes = (from b in thisFile select b.Length).Sum();

                var x = "njld";

            }

        }//ExtractFile

        static void WriteBufferToFile(int fileNumber)
        {
            string fileName = @"c:\temp\jpgtest_" + fileNumber + ".jpg";
            using (System.IO.FileStream fs = System.IO.File.Create(fileName, 1024))
            {
                for(int i = 0; i < buffer.Count; i++)
                {
                    fs.Write(buffer[i], 0, buffer[i].Length);
                    byte[] newline = Encoding.ASCII.GetBytes(Environment.NewLine);
                    fs.Write(newline, 0, newline.Length);
                }

            }//using 
        }

        private static bool ContainsBoundaryBytes(String st)
        {
            byte[] array = Encoding.ASCII.GetBytes(st);

            if (array.Length < boundaryBytes.Length) return false;

            bool result = false;
            for (int i = 0, j = 0; j < boundaryBytes.Length; i++, j++)
            {
                result = array[i] == boundaryBytes[j];
                if (!result)
                {
                    break;
                }
            }

            if (result) {
                Console.WriteLine("line break found"); }

            return result;
        }

        //private static bool SeperatorBytesExistsInArray(String st)
        //{
        //    byte[] array = Encoding.ASCII.GetBytes(st);

        //    if (array.Length < seperatorBytes.Length) return false;

        //    bool result = false;
        //    for (int i = 0, j = 0; j < seperatorBytes.Length; i++, j++)
        //    {
        //        result = array[i] == seperatorBytes[j];
        //        if (!result)
        //        {
        //            break;
        //        }
        //    }

        //    if (result) { Console.WriteLine("line break found"); }

        //    return result;
        //}

        #endregion


    }
}
