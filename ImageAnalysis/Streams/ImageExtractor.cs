using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tools;
using ImageAnalysis.Images.Jpeg;

namespace ImageAnalysis.Streams
{
    /// <summary>
    /// Attachs to an MJPEG HTTP stream and extracts individual JPEG's
    /// </summary>
    public class ImageExtractor
    {
        //ImageCreated event, called each time an image is extracted
        public event ImageCreatedEvent imageCreated;
        public delegate void ImageCreatedEvent(ByteWrapper img, EventArgs e);

        //Sends the current framerate based on the framesPerBroadcast
        public event FramerateBroadcastEvent framerateBroadcast;       
        public delegate void FramerateBroadcastEvent(double framerate, EventArgs e);
        int imagesAnalysed; //the number of images in the current broadcast (re-set on each broadcast)
        int framesPerBroadcast; //the number of frames to process until the rate is broadcast
        Stopwatch frameStopwatch; //used to calculate the frame rate

        public bool asyncrohous; //whether the events are raised async

        //ip camera logon info
        string url;
        string username;
        string password;

        //JPEG header extraction
        Regex contentLengthRegex = new Regex("Content-Length: (?<length>[0-9]+)\r\n\r\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        string boundaryString = @"--myboundary";
        byte[] boundaryBytes; //a byte version of the boundary

        //main stopwatch for timing of the whole capture session
        Stopwatch stopwatch;
        int minutesToRun;
        bool EndCaputre;
        bool singleImageExtraction; //whether we only want one image
        int imagesReceived;

        public ImageExtractor(string url, string username, string password)
        {
            this.username = username;
            this.password = password;
            this.url = url;

            this.framesPerBroadcast = ConfigurationManager.AppSettings["FramesPerBroadcast"].ToString().StringToInt();

            boundaryBytes = Encoding.ASCII.GetBytes(boundaryString); //set the boundary bytes from the boundaryString
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

        /// <summary>
        /// Only extracts a single image, and then stops
        /// </summary>
        /// <param name="singleImage"></param>
        public void Run(bool singleImage)
        {
            singleImageExtraction = true;
            Run();
        }

        //Runs the Image extractor for a set period
        public void Run(int minutes)
        {
            minutesToRun = minutes;
            stopwatch = new Stopwatch();
            stopwatch.Start();
            Run();
        }

        public async void Run()
        {
            Setup();

            if (boundaryBytes == null) { throw new Exception("boundaryBytes was null"); }

            HttpWebResponse resp = ReturnHttpResponse(url);

            if(resp.StatusCode != HttpStatusCode.OK) { throw new Exception("HTTP Request returned a " + resp.StatusCode + " status code"); }

          //  Console.WriteLine(resp.Headers.ToString());

            BinaryReader reader = new BinaryReader(resp.GetResponseStream());

            while (reader.BaseStream.CanRead && !EndCaputre)
            {
                String header = ReadHeader(reader); //moves the stream on, and extracts the header
                if (EndCaputre) { return; }
                int contentLength = GetContentLength(header);

                byte[] img = reader.ReadBytes(contentLength);

                if (asyncrohous)
                {
                    await OnFileCreateAsync(img);
                }
                else
                {
                    OnFileCreate(img);
                }

                if ((stopwatch != null && stopwatch.Elapsed.Minutes == minutesToRun) || singleImageExtraction)
                {
                    resp.Dispose();
                    reader.Dispose();
                    return;
                }
                
            }//while stream.CanRead

            resp.Dispose();
            reader.Dispose();

        }//Run

        private void Setup()
        {
            if(framerateBroadcast != null) { frameStopwatch = new Stopwatch(); } //records the framerate
        }

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
                if (EndCaputre) {  return String.Empty; }

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

            if (bytes.Count > 40000 && bytes[0] == 13 && bytes[1] == 10 && bytes[2] == 0 && bytes[3] == 0 &&
                bytes[4] == 13 && bytes[5] == 10 && bytes[6] == 0 && bytes[7] == 0
                ) {
                EndCaputre = true;} //no more images

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
                    return Int32.Parse(split2[1].Trim());
                }
            }

             throw new Exception("GetContentLength could not find Content-Length");

        }//GetContentLength

        protected void OnFileCreate(byte[] img)
        {
            if (imageCreated != null)
            {
                imagesReceived++;
                imageCreated(new ByteWrapper(img, imagesReceived), EventArgs.Empty);
            }

            if(framerateBroadcast != null)
            {
                if (!frameStopwatch.IsRunning) { frameStopwatch.Start();  } //only start once the images start to come through

                imagesAnalysed++;
                if(imagesAnalysed % framesPerBroadcast == 0)
                {
                    frameStopwatch.Stop();
                    BroadcastFramerate(frameStopwatch.Elapsed.TotalMilliseconds, imagesAnalysed);

                    imagesAnalysed = 0;
                    frameStopwatch.Restart();

                }
            }

        }//OnFileCreate

private async Task OnFileCreateAsync(byte[] img)
{
    await Task.Run(() =>
    {

        if (imageCreated != null)
        {
            imagesReceived++;
            imageCreated(new ByteWrapper(img, imagesReceived), EventArgs.Empty);
        }

        if (framerateBroadcast != null)
        {
            if (!frameStopwatch.IsRunning) { frameStopwatch.Start(); } //only start once the images start to come through

            imagesAnalysed++;

            if (imagesAnalysed % framesPerBroadcast == 0)
            {
                frameStopwatch.Stop();
                BroadcastFramerate(frameStopwatch.Elapsed.TotalMilliseconds, imagesAnalysed);

                imagesAnalysed = 0;
                frameStopwatch.Restart();

            }
        }

    });

}//OnFileCreateAsync

        private async void BroadcastFramerate(double ms, int imagesAnalysed)
        {
            var framerate = ms / imagesAnalysed;

            await Task.Run(() => {
                framerateBroadcast(framerate, EventArgs.Empty);
            });            
        }

    }//ImageExtractor.cs

}
