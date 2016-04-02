using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace HTTP_Streamer.Models
{
    /// <summary>
    /// Represents a set of JEPG files used to form part of a MPEG stream
    /// Extracts the tags from the settings file
    /// </summary>
    public class JpegSection
    {
        public Dictionary<string, string> settings { get; set; }
        public List<String> imageFiles { get; set; }

        private string directory { get; set; }
        private int startFrame { get; set; }
        private int endFrame { get; set; }
        private bool limitedFrames { get; set; }

        /// <summary>
        /// Extracts the JPEG files, and settings from the specified directory
        /// </summary>
        /// <param name="directory"></param>
        public JpegSection(string directory)
        {          
            if (!Directory.Exists(directory)) { throw new DirectoryNotFoundException(directory); }
            this.directory = directory;
            Setup();
        }

        /// <summary>
        /// Extracts the JPEG files, and settings from the specified directory
        /// If the frame number of the file falls into the specified range
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="startframe"></param>
        /// <param name="endframe"></param>
        public JpegSection(string directory, int startframe, int endframe)
        {
            if (!Directory.Exists(directory)) { throw new DirectoryNotFoundException(directory); }
            this.directory = directory;
            this.startFrame = startframe;
            this.endFrame = endframe;
            Setup();
        }

        private void Setup()
        {
            if (startFrame > 0 && endFrame > 1) { limitedFrames = true; }
            ExtractSettings();
            ExtractFileNames();
        }

        /// <summary>
        /// Extracts the log file from the directory, and populates the settings
        /// </summary>
        /// <param name="directory"></param>
        private void ExtractSettings()
        {
            settings = new Dictionary<string, string>();
            string settingsFile = directory + @"\logfile.txt";

            if (!File.Exists(settingsFile)) { throw new FileNotFoundException(settingsFile);  }

            System.IO.StreamReader file = new System.IO.StreamReader(settingsFile);

            string line;
            while ((line = file.ReadLine()) != null)
            {
                string[] data = line.Split(':');

                //only load if valid, don't error on misformed lines
                if(data.Length == 2)
                {
                    settings.Add(data[0], data[1]);
                }
            }

            file.Close();

        }//ExtractSettings

        /// <summary>
        /// Extracts the file names from the directory
        /// </summary>
        private void ExtractFileNames()
        {
            imageFiles = new List<string>();

            if(limitedFrames)
            {
                var files = (from file in Directory.EnumerateFiles(directory).ToList()
                             where new FileInfo(file).Extension == ".jpg"
                             && file.Split('_')[1].Split('.')[0].ToString().StringToInt() >= startFrame
                             && file.Split('_')[1].Split('.')[0].ToString().StringToInt() <= endFrame
                             orderby file.Split('_')[1].Split('.')[0].ToString().StringToInt() ascending
                             select file).ToList();

                imageFiles.AddRange(files);
            }
            else
            {
var files = (from file in Directory.EnumerateFiles(directory).ToList()
                where new FileInfo(file).Extension == ".jpg"
                orderby file.Split('_')[1].Split('.')[0].ToString().StringToInt() ascending
                select file).ToList();

                imageFiles.AddRange(files);
            }

        }//ExtractFilesNames

        /// <summary>
        /// Extracts the framerate from the settings 
        /// </summary>
        /// <returns></returns>
        public double Framerate()
        {
            return settings["FR"].ToString().StringToDouble();
        }

    }
}
