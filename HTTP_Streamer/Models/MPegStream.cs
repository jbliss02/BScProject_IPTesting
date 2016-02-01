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

namespace HTTP_Streamer.Models
{
    /// <summary>
    /// Writes JEPG files to an output stream, asynchrously
    /// Used to stream an MPEG
    /// </summary>
    public class MPegStream
    {
        public string boundary { get; set; } = "myboundary";

        private string sessionKey { get; set; }
        private int cameraId { get; set; }
        private string filePath { get; set; }
        /// <summary>
        /// Defines the file path in which the .jpg files will merged
        /// </summary>
        /// <param name="filePath"></param>


        public MPegStream(int cameraId, string sessionKey)
        {
            this.sessionKey = sessionKey;
            this.cameraId = cameraId;
        }

        public MPegStream(string filePath)
        {
            this.filePath = filePath;
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
                    await Task.Delay(new TimeSpan(0,0,0,0,49)); //a ms delay to regulate the speed

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

    }
}
