﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ishimotto.Core
{
    /// <summary>
    /// Writes large amounts of data to files
    /// </summary>
    public class FileWriter : IDisposable
    {
        #region Data Members
        /// <summary>
        /// The directory to save the files to
        /// </summary>
        private readonly string mOutputDirectory;

        /// <summary>
        /// Streams to write the files
        /// </summary>
        private StreamWriter[] mWriters;

        /// <summary>
        /// Indicates if the streams are initialized
        /// </summary>
        private bool mAreStreamInitialized = false;

        /// <summary>
        /// Collection of all the paths creates by the <see cref="FileWriter"/>
        /// </summary>
        private List<string> mFilesesPaths;

        /// <summary>
        /// The name of the file to create
        /// </summary>
        private string mFileName; 
        #endregion

        #region Properties
        /// <summary>
        /// <see cref="mFilesesPaths"/>
        /// </summary>
        public IEnumerable<string> FilesPaths { get { return mFilesesPaths; } }

        /// <summary>
        /// The extension of the document to create
        /// </summary>
        public string Extension { get; private set; }

        /// <summary>
        /// Optional a perfix to append to the text
        /// </summary>
        public string Perfix { get; set; }

        /// <summary>
        /// Optional a suffix to append to the text
        /// </summary>
        public string Suffix { get; set; }

        #endregion        
        
        #region Constructors
        /// <summary>
        /// Creates new instance of <see cref="FileWriter"/>
        /// </summary>
        /// <param name="outputDirectory"><see cref="mOutputDirectory"/></param>
        /// <param name="fileName"><see cref="mFileName"/></param>
        /// <param name="numOfFiles">Number of files to create</param>
        /// <param name="extension"><see cref="Extension"/></param>
        public FileWriter(string outputDirectory, string fileName, int numOfFiles, string extension)
        {
            mOutputDirectory = Path.Combine(outputDirectory, fileName);

            CreateDirectory();

            mFileName = fileName;
            Extension = extension;

            mFilesesPaths = new List<string>(GetFilePaths(numOfFiles));

            mWriters = new StreamWriter[numOfFiles];

        }

        /// <summary>
        /// Creates new instance of <see cref="FileWriter"/>
        /// </summary>
        /// <param name="outputDirectory"><see cref="mOutputDirectory"/></param>
        /// <param name="fileName"><see cref="mFileName"/></param>
        /// <param name="numOfFiles">Number of files to create</param>
        /// <param name="perfix">perfix to append to the text</param>
        /// <param name="suffix">suffix to append to the text</param>
        /// <param name="extension"><see cref="Extension"/></param>
        public FileWriter(string outputDirectory, string fileName, int numOfFiles, string extension, string perfix, string suffix)
            : this(outputDirectory, fileName, numOfFiles, extension)
        {
            Suffix = suffix;
            Perfix = perfix;
        } 
        #endregion

        #region Public Methods
        /// <summary>
        /// Write texts to <see cref="FilesPaths"/>, adding perfix and suffix if exists
        /// </summary>
        /// <param name="lines">Lines to write</param>
        public async Task WriteToFiles(IEnumerable<string> lines)
        {

            if (!mAreStreamInitialized)
            {
                InitializeWriters();
            }

            int elementIndex = 0;

            await Task.Factory.StartNew(() =>
                {
                    foreach (var text in lines)
                    {
                        var line = String.Concat(Perfix, text, Suffix);
                        mWriters[elementIndex%mWriters.Length].WriteLine(line);
                        Interlocked.Increment(ref elementIndex);
                    }
                });

        }

        /// <summary>
        /// Disposes the instance of <see cref="FileWriter"/>
        /// </summary>
        public void Dispose()
        {
            foreach (var streamWriter in mWriters)
            {
                streamWriter.Dispose();
            }
        } 
        #endregion

        #region Private Methods
        /// <summary>
        /// Creates the <see cref="mOutputDirectory"/> if doesn't exist
        /// </summary>
        private void CreateDirectory()
        {
            if (!Directory.Exists(mOutputDirectory))
            {
                Directory.CreateDirectory(mOutputDirectory);
            }
        }

        /// <summary>
        /// Gets the paths of the files to write
        /// </summary>
        /// <param name="numOfFiles">Number of files to create</param>
        /// <returns>Enumrable of all the files paths</returns>
        private IEnumerable<string> GetFilePaths(int numOfFiles)
        {
            var files = new List<string>(numOfFiles);

            int index = 1;

            for (int fileNo = 0; fileNo < numOfFiles; fileNo++)
            {

                string filePath = Path.Combine(mOutputDirectory, String.Concat(mFileName, index, ".", Extension));

                while (File.Exists(filePath))
                {
                    index++;

                    filePath = Path.Combine(mOutputDirectory, String.Concat(mFileName, index, ".", Extension));

                }

                files.Add(filePath);

                index++;
            }

            return files;
        }

        /// <summary>
        /// Initialize the streams to write to <see cref="FilesPaths"/>
        /// </summary>
        private void InitializeWriters()
        {


            for (int writerPosition = 0; writerPosition < mWriters.Length; writerPosition++)
            {

                mWriters[writerPosition] = new StreamWriter(mFilesesPaths[writerPosition], false);

            }

            mAreStreamInitialized = true;

        } 
        #endregion
    }
}