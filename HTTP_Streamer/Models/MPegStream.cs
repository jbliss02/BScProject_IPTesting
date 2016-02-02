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
    public class MPegStream
    {
        public string boundary { get; set; } = "myboundary";

        //Frame regulation - REFACTOR!
        private int currentDelayMs; //current delay in milliseconds
        private int naturalFramesPerMinute; //frames per minute the web service would run without a delay
        private Stopwatch regulatorClock; //times the speed between frames
        private int framesPerRegulationCheck; //how many frames in a speed check
        private int frameCount; //the number of frames in this regulation section
        private int frameRate = 33; //to be re-set by the meta data file

        private string sessionKey { get; set; }
        private int cameraId { get; set; }
        private string filePath { get; set; }

        public MPegStream(int cameraId, string sessionKey)
        {
            this.sessionKey = sessionKey;
            this.cameraId = cameraId;
            Setup();
        }

        public MPegStream(string filePath)
        {
            this.filePath = filePath;
            Setup();
        }

        private void Setup()
        {
            naturalFramesPerMinute = 1500; //start somewhere, move to config
            currentDelayMs = 49;
            frameCount = 0;
            regulatorClock = new Stopwatch();
            framesPerRegulationCheck = 100;
        }

        public async Task WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
        {
            try
            {
                byte[] crlf = Encoding.UTF8.GetBytes("\r\n");
              
                foreach (var file in ImageFiles())
                {
                    var fileInfo = new FileInfo(file);

                    byte[] header = HTTPHeader(fileInfo.Length);
                    await outputStream.WriteAsync(header, 0, header.Length); //write the header                  
                    await fileInfo.OpenRead().CopyToAsync(outputStream); //write the JPEG bytes 
                    await outputStream.WriteAsync(crlf, 0, crlf.Length); //write the new line 
                    await Task.Delay(Delay()); //a ms delay to regulate the speed
                    await RegulateFramerate(); //set the delay

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
        /// Returns an array of the file paths belonging to this session key
        /// Sorts into the correct order, based on file name
        /// </summary>
        /// <returns></returns>
        public List<String> ImageFiles()
        {
            // string mainLocation = ConfigurationManager.AppSettings["SaveLocation"].ToString() + @"\" + cameraId + @"\" + sessionKey;
            string mainLocation = @"f:\captures\" + cameraId + @"\" + sessionKey;
            if (!Directory.Exists(mainLocation)) { throw new Exception("Directory does not exist"); }

            List<String> ret = new List<string>(); //the return collection

            //get all the section directories
            List<String> dirs = (from dir in Directory.EnumerateDirectories(mainLocation)
                        orderby dir.ToString().StringToInt() ascending
                        select dir).ToList();

            //iterate over each directory and retrieve the files
            foreach (var dir in dirs)
            {
                var files = (from file in Directory.EnumerateFiles(dir).ToList()
                             where new FileInfo(file).Extension == ".jpg"
                             orderby file.Split('_')[1].Split('.')[0].ToString().StringToInt() ascending
                             select file).ToList();

                ret.AddRange(files);
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
                //start the stopwatch on the initial image creation
                if(!regulatorClock.IsRunning)
                {
                    regulatorClock.Start();
                }
                else
                {
                    frameCount++;

                    if(frameCount % framesPerRegulationCheck == 0)
                    {
                        //section is full, calculate the natural speed
                        regulatorClock.Stop();
                        double totalDelayMs = currentDelayMs * frameCount; 
                        double totalTransmissionMs = regulatorClock.Elapsed.TotalMilliseconds;

                        double naturalTransmission = totalTransmissionMs - totalDelayMs;
                        double naturalFramerate = naturalTransmission / framesPerRegulationCheck;




                        //work out what the delay should be
                        double newDelay = (1 / naturalFramerate) * 1000;
                       // currentDelayMs = newDelay 

                        //reset everything
                        frameCount = 0;
                        regulatorClock.Restart();
                    }
                }

            });
        }//RegulateFramerate

    }
}
