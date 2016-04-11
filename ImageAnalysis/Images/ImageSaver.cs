using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;
using Tools;
using ImageAnalysis.Images.Jpeg;

namespace ImageAnalysis.Images
{
    /// <summary>
    /// Saves bytes as JPEG images
    /// Defines the folder structure, file allocation and file naming convention
    /// </summary>
    public class ImageSaver
    {
        public event ImageSavedEvent imageCreated;
        public delegate void ImageSavedEvent(String imagePath, EventArgs e);

        public int framesPerSection { get; set; } = 1000; //the number of frames per section
        public int initialFrameDetection { get; set; } = 200; //number of frames after which to write initial framerate

        public string ParentDirectory { get; set; }
        public string captureId { get; set; }
        public string CaptureDirectory { get; set; }  //the parent directory, in which all section directories will be stored
        public string SaveDirectory { get; set; } //some images (like motion) saved outside of captures

        int sessionCount = 0;
        Int64 fileNumber = 0;
        int cameraId;

        public List<double> framerates { get; set; } = new List<double>();

        /// <summary>
        /// The start string for each file that is saved
        /// </summary>
        public string fileStartName { get; set; }
            
        public ImageSaver()
        {
            SetUp();
        }

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

        /// <summary>
        /// Defines the parent directory to which files will be saved (a session directory will be automatically
        /// created and added to this directory). Also defines the start of the file name
        /// </summary>
        public ImageSaver(string saveDirectory, string fileStartName, int cameraId)
        {
            if (!System.IO.Directory.Exists(saveDirectory))
            {
                throw new Exception("saveDirectory " + saveDirectory + " could not be accessed");
            }

            this.SaveDirectory = saveDirectory + @"\" + cameraId + @"\" + Tools.ExtensionMethods.DateStamp();
            CreateDirectory(this.SaveDirectory);
            this.fileStartName = fileStartName;
        }

        private void SetUp()
        {
            if (fileStartName == String.Empty) { fileStartName = "image"; }

            ParentDirectory = ConfigurationManager.AppSettings["SaveLocation"].ToString();

            //define the camera directory, and create if not exists
            string cameraDirectory = ParentDirectory + @"\" + cameraId;
            if (!Directory.Exists(cameraDirectory)) { CreateDirectory(cameraDirectory); }

            //define, and create, this session's directory
            captureId = Tools.ExtensionMethods.DateStamp();
            CaptureDirectory = cameraDirectory + @"\" + captureId;
            CreateDirectory(CaptureDirectory);

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
            SaveDirectory = CaptureDirectory + @"\" + sessionCount;
            CreateDirectory(SaveDirectory);
        }

        /// <summary>
        /// Event listener for when an image is created
        /// Fires the appropriate methods to classify and save the
        /// Image
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public async Task ImageCreatedAsync(ByteWrapper img)
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
        public async void ImageCreatedAsync(ByteWrapper img, EventArgs e)
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
        public async Task SaveFiles(List<ByteWrapper> imgs)
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

                if(framerates.Count > 0)
                {
                    double framerate = framerates.Average();
                    framerates.Clear(); //start again

                    //replace the FR in the log file, or write as new
                    string logfile = CaptureDirectory + @"\" + sessionNumber + @"\logfile.txt";

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
                }


            });

        }//WriteDatafileSummary

        private void WriteBytesToFile(ByteWrapper img)
        {
            try
            {
                string fileName = GenerateFileName();
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    fs.Write(img.bytes, 0, img.bytes.Length);
                }
                if (imageCreated != null) { imageCreated(fileName, EventArgs.Empty); }

            }
            catch(Exception ex)
            {
                Console.WriteLine(DateTime.Now + " - " + ex.Message);
            }
        }

        private String GenerateFileName()
        {          
            String ret = SaveDirectory + @"\" + fileStartName + "_" + fileNumber.ToString() + ".jpg";
            fileNumber++;
            return ret;
        }

        private String GenerateFileName(int sequenceNumber)
        {
            String ret = SaveDirectory + @"\" + fileStartName + "_" + sequenceNumber.ToString() + ".jpg";
            fileNumber++;
            return ret;
        }

        private void WriteScreen(string st)
        {
            Console.WriteLine(DateTime.Now + " - " + st);
        }

        /// <summary>
        /// Writes a single file to the location specified, called outide of this
        /// </summary>
        public async void WriteBytesToFileAsync(ByteWrapper image, EventArgs e)
        {
            await Task.Run(() => {
                using (FileStream fs = new FileStream(GenerateFileName(image.sequenceNumber), FileMode.Create))
                {
                    fs.Write(image.bytes, 0, image.bytes.Length);
                }
            });

        }

        public static async void WriteBytesToFileAsync(ByteWrapper image, string filepath)
        {
            await Task.Run(() => {
                using (FileStream fs = new FileStream(filepath, FileMode.Create))
                {
                    fs.Write(image.bytes, 0, image.bytes.Length);
                }
            });
        }
            

    }
}
