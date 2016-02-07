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
using System.Configuration;
using Tools;

namespace IPConnect_Testing
{
    /// <summary>
    /// Saves bytes as JPEG images
    /// Defines the folder structure, file allocation and file naming convention
    /// </summary>
    public class ImageSaver
    {
        public int framesPerSection { get; set; } = 1000; //the number of frames per section
        public int initialFrameDetection { get; set; } = 200; //number of frames after which to write initial framerate

        public string parentDirectory { get; set; } 
        public string captureDirectory { get; set; }  //the parent directory, in which all section directories will be stored

        string saveDirectory;
        int sessionCount = 0;
        Int64 fileNumber = 0;
        int cameraId;

        public List<double> framerates { get; set; } = new List<double>();

        /// <summary>
        /// The start string for each file that is saved
        /// </summary>
        public string fileStartName { get; set; }
            
        public ImageSaver(int cameraId)
        {
            this.cameraId = cameraId;
            SetUp();
        }

        /// <summary>
        /// When overloaded with a sequence integer the first file is saved with this value as the prefix
        /// subsequent files are increased by 1
        /// </summary>
        /// <param name="cameraId"></param>
        public ImageSaver(int sequenceStart, int cameraId)
        {
            fileNumber = sequenceStart;
            SetUp();
        }

        private void SetUp()
        {
            fileStartName = "test";

            parentDirectory = ConfigurationManager.AppSettings["SaveLocation"].ToString();

            //define the camera directory, and create if not exists
            string cameraDirectory = parentDirectory + @"\" + cameraId;
            if (!Directory.Exists(cameraDirectory)) { CreateDirectory(cameraDirectory); }

            //define, and create, this session's directory
            captureDirectory = cameraDirectory + @"\" + Tools.ExtensionMethods.DateStamp();
            CreateDirectory(captureDirectory);

            //set up the initial save directory
            CreateNewSaveDirectory();
        }
      
        private void CreateDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath)) { throw new Exception("Directory already exists"); }
            Directory.CreateDirectory(directoryPath);
        }

        /// <summary>
        /// Called when a new saving directory is required, either when
        /// object is instaniated or when a new section is generated
        /// </summary>
        private void CreateNewSaveDirectory()
        {
            sessionCount++;
            saveDirectory = captureDirectory + @"\" + sessionCount;
            CreateDirectory(saveDirectory);
        }

        /// <summary>
        /// Event listener for when an image is created
        /// Fires the appropriate methods to classify and save the
        /// Image
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public async Task ImageCreatedAsync(byte[] img)
        {
            await Task.Run(() => {
                WriteBytesToFile(img);
                SetSection();
            } );
        }

        /// <summary>
        /// Event listener for when an image is created
        /// Fires the appropriate methods to classify and save the
        /// Image
        /// </summary>
        /// <param name="img"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public async void ImageCreatedAsync(byte[] img, EventArgs e)
        {
            await Task.Run(() => {
                WriteBytesToFile(img);
                SetSection();
            });
        }

        /// <summary>
        /// Takes a List of byte files and creates
        /// a seperate file for each element
        /// </summary>
        /// <param name="imgs"></param>
        /// <returns></returns>
        public async Task SaveFiles(List<byte[]> imgs)
        {
            await Task.Run(() => {
                for (int i = 0; i < imgs.Count; i++)
                {
                    WriteBytesToFile(imgs[i]);
                }

            });
        }

        /// <summary>
        /// Defines the current section directory
        /// Creates and assigns new directories when the current session directory reaches it limit
        /// </summary>
        private void SetSection()
        {
            if(fileNumber == initialFrameDetection) {
                WriteDatafileSummary(sessionCount); } //write after 10 so we have a framerate
            if(fileNumber % framesPerSection == 0)
            {
                WriteDatafileSummary(sessionCount);
                CreateNewSaveDirectory();
            }
        }

        /// <summary>
        /// Writes the data file summary once a section has been completed
        /// This includes framerate information
        /// </summary>
        private async void WriteDatafileSummary(int sessionNumber)
        {
            await Task.Run(() => {

                double framerate = framerates.Average();
                framerates.Clear(); //start again

                //replace the FR in the log file, or write as new
                string logfile = captureDirectory + @"\" + sessionNumber + @"\logfile.txt";

                if(File.Exists(logfile))
                {
                    string txt = File.ReadAllText(logfile);

                    Regex regex = new Regex("FR:(?<length>[0-9]+).(?<length>[0-9]+)\r\n");
                    if(regex.IsMatch(txt))
                    {
                        txt = regex.Replace(txt, "FR:" + framerate);
                        File.WriteAllText(logfile, txt);
                    }
                }
                else
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(logfile, true))
                    {
                        file.WriteLine("FR:" + framerate);
                    }
                }

            });

        }//WriteDatafileSummary

        private void WriteBytesToFile(byte[] img)
        {
            try
            {
                using (FileStream fs = new FileStream(GenerateFileName(), FileMode.Create))
                {
                    fs.Write(img, 0, img.Length);
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(DateTime.Now + " - " + ex.Message);
            }
        }

        private String GenerateFileName()
        {          
            String ret = saveDirectory + @"\" + fileStartName + "_" + fileNumber.ToString() + ".jpg";
            fileNumber++;
            return ret;
        }

        private void WriteScreen(string st)
        {
            Console.WriteLine(DateTime.Now + " - " + st);
        }
    }
}
