using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using Tools;
using System.Configuration;
using System.Diagnostics;

namespace HTTP_Streamer.Models
{
    /// <summary>
    /// Writes JEPG files to an output stream, asynchrously
    /// Used to stream an MPEG
    /// </summary>
    public class JpegStream
    {
        public string boundary { get; set; } = "myboundary";

        //Frame regulation - REFACTOR!
        private int currentDelayMs; //current delay in milliseconds
        private Stopwatch regulatorClock; //times the speed between frames
        private int framesPerRegulationCheck; //how many frames in a speed check
        private int frameCount; //the number of frames in this regulation section
        private double requiredFramerate = 33; //to be re-set by the meta data file

        private string sessionKey { get; set; }
        private int cameraId { get; set; }
        private string filePath { get; set; }

        private int startFrame { get; set; }
        private int endFrame { get; set; }
        private bool limitedFrames { get; set; }

        public JpegStream(string filePath)
        {
            this.filePath = filePath;
            Setup();
        }

        public JpegStream(int cameraId, string sessionKey)
        {
            this.sessionKey = sessionKey;
            this.cameraId = cameraId;
            Setup();
        }

        public JpegStream(int cameraId, string sessionKey, int startFrame, int endFrame)
        {
            this.sessionKey = sessionKey;
            this.cameraId = cameraId;
            this.startFrame = startFrame;
            this.endFrame = endFrame;
            Setup();
        }

        private void Setup()
        {
            currentDelayMs = 32; 
            frameCount = 0;
            regulatorClock = new Stopwatch();
            framesPerRegulationCheck = 100;
            if(startFrame > 0 && endFrame > 1) { limitedFrames = true; }
        }

        /// <summary>
        /// Streams JPEG files across a HTTP connection, asyncrohously
        /// </summary>
        /// <param name="outputStream"></param>
        /// <param name="content"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
        {
            try
            {
                byte[] crlf = Encoding.UTF8.GetBytes("\r\n");

                foreach (var section in MpegSections())
                {
                    double sectionFramerate = section.Framerate();
                    if (sectionFramerate > 0) { requiredFramerate = section.Framerate(); }//each section has its own framerate, if not use the last

                    foreach (var file in section.imageFiles)
                    {
                        var fileInfo = new FileInfo(file);

                        byte[] header = HTTPHeader(fileInfo.Length);
                        await outputStream.WriteAsync(header, 0, header.Length); //write the header                  
                        await fileInfo.OpenRead().CopyToAsync(outputStream); //write the JPEG bytes 
                        await outputStream.WriteAsync(crlf, 0, crlf.Length); //write the new line 

                        System.Threading.Thread.Sleep(currentDelayMs); //block this thread for framerate regulation
                        await RegulateFramerate(); //set the delay
                    }

                }//each file

            }
            catch
            {
                return;
            }
            finally
            {
                outputStream.Close(); //browser will not show the stream without this
            }

        }//WriteToStream

        /// <summary>
        /// Returns the byte representation of the header, for a specific image length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private byte[] HTTPHeader(long length)
        {
            StringBuilder s = new StringBuilder();
            s.Append("--" + boundary);
            s.Append("\r\n");
            s.Append("Content-Type: image/jpeg");
            s.Append("\r\n");
            s.Append("Content-Length: " + length);
            s.Append("\r\n\r\n");

            return  Encoding.UTF8.GetBytes(s.ToString());
        }

        /// <summary>
        /// Returns a list of MpegSections from the paths belonging to this session key
        /// Each Mpeg section contains a sorted list of files and some settings related to the files
        /// </summary>
        /// <returns></returns>
        public List<JpegSection> MpegSections()
        {
            // string mainLocation = ConfigurationManager.AppSettings["SaveLocation"].ToString() + @"\" + cameraId + @"\" + sessionKey;
            string mainLocation = @"f:\captures\" + cameraId + @"\" + sessionKey;
            List<JpegSection> ret = new List<JpegSection>(); //the return collection

            //get all the section directories
            List<String> dirs = (from dir in Directory.EnumerateDirectories(mainLocation)
                        orderby dir.ToString().StringToInt() ascending
                        select dir).ToList();

            foreach (var dir in dirs)
            {
                JpegSection section = null;

                if (limitedFrames)
                {
                    section = new JpegSection(dir, startFrame, endFrame);
                }
                else
                {
                    section = new JpegSection(dir);
                }
                
                if (section.imageFiles.Count > 0) { ret.Add(section); } //if a range of frames is asked for may return no files         
            }

            return ret;

        }//ImageFiles

        public TimeSpan Delay()
        {
            return new TimeSpan(0, 0, 0, 0, currentDelayMs);
        }

public async Task RegulateFramerate()
{
    await Task.Run(() =>
    {                             
        if(!regulatorClock.IsRunning)
        {
            regulatorClock.Start(); //start the stopwatch on the initial image creation
        }
        else
        {
            frameCount++;

            if(frameCount % framesPerRegulationCheck == 0)
            {
                //calculate the natural speed
                regulatorClock.Stop();

                double transmissionMs = regulatorClock.Elapsed.TotalMilliseconds;
                double delayMs = (currentDelayMs * framesPerRegulationCheck);
                double naturalTransmissionMs = transmissionMs - delayMs;

                //calculate the actual, required and delta milliseconds per frame 
                double actualMsPerFrame = naturalTransmissionMs / framesPerRegulationCheck;
                double desiredMsPerFrame = (1 / requiredFramerate) * 1000;
                double requiredMsDelayPerFrame = desiredMsPerFrame - actualMsPerFrame;

                //the receiver may not be able to receive many packets, in which case the desired delay would be
                //zero, in those cases ignore
                if(requiredMsDelayPerFrame > 0)
                {
                    currentDelayMs = Convert.ToInt16(requiredMsDelayPerFrame); 
                }

                //reset counters
                frameCount = 0;
                regulatorClock.Restart();
            }
        }

    });

}//RegulateFramerate

          
    }
}
