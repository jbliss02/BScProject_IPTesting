using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;

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

            foreach (var file in Directory.GetFiles(filePath, "*.jpg"))
            {
                var fileInfo = new FileInfo(file);

                byte[] header = HTTPHeader(fileInfo.Length);
                await outputStream.WriteAsync(header, 0, header.Length); //write the header                  
                await fileInfo.OpenRead().CopyToAsync(outputStream); //write the JPEG bytes 
                await outputStream.WriteAsync(crlf, 0, crlf.Length); //write the new line 
                await Task.Delay(100); //a delay

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

    }
}
