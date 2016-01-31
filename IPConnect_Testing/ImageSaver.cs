﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Tools;

namespace IPConnect_Testing
{
    /// <summary>
    /// Saves bytes as JPEG images
    /// Defines the folder structure, file allocation and file naming convention
    /// </summary>
    public class ImageSaver
    {
        public int framesPerSection { get; set; } = 1000;
        public string parentDirectory { get; set; } = @"f:\captures"; //MOVE TO A CONFIG FILE
        public string captureDirectory { get; set; }  //the parent directory, in which all section directories will be stored

        string saveDirectory;
        int sessionCount = 0;
        Int64 fileNumber = 0;
        int cameraId;

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
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task ImageCreatedAsync(byte[] img, EventArgs e)
        {
            await Task.Run(() => {
                WriteBytesToFile(img);
                SetSection();
            } );
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
            if(fileNumber % framesPerSection == 0)
            {
                CreateNewSaveDirectory();
            }
        }

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
            String ret = captureDirectory + @"\" + fileStartName + "_" + fileNumber.ToString() + ".jpg";
            fileNumber++;
            return ret;
        }

        private void WriteScreen(string st)
        {
            Console.WriteLine(DateTime.Now + " - " + st);
        }
    }
}
