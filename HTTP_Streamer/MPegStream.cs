using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using Tools;

namespace HTTP_Streamer
{
    /// <summary>
    /// Writes JEPG files to an output stream, asynchrously
    /// Used to stream an MPEG
    /// </summary>
    public class MPegStream
    {
        public string boundary { get; set; } = "myboundary";
        public string filePath { get; set; }

        /// <summary>
        /// Defines the file path in which the .jpg files will merged
        /// </summary>
        /// <param name="filePath"></param>
        public MPegStream(string filePath)
        {
            this.filePath = filePath;
        }

    public async Task WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
    {
        try
        {
            byte[] crlf = Encoding.UTF8.GetBytes("\r\n");

            //get the files in name order, important for frames to be played in sequence
            List<String> files = Directory.GetFiles(filePath, "*.jpg").ToList();

            files = (from f in files
                        orderby f.Split('_')[1].Split('.')[0].ToString().StringToInt()
                        ascending
                        select f).ToList();

            foreach (var file in files)
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
        /// Returns a delay bases ./......................
        /// </summary>
        /// <returns></returns>
        private int Delay()
        {
            return 1;
        }

    }
}
