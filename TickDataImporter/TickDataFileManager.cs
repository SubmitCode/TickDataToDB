using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Ionic.Zip;
using log4net;

namespace TickDataImporter
{
    internal class TickDataFileManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private TickDataFileManager() { }

        string PathDirectoryTickData;
        string PathTemporaryOutputfolder;

        public TickDataFileManager(string pathDirectoryTickData, string pathTemporaryOutputfolder)
        {
            PathDirectoryTickData = pathDirectoryTickData;
            PathTemporaryOutputfolder = pathTemporaryOutputfolder;
        }

        public TickDataFileManager(string[] pathsTickFiles, string pathTemporaryOutputfolder)
        {
            DeleteAndCreateFolderStructure(pathTemporaryOutputfolder);
            foreach (var item in pathsTickFiles)
            {
                ExtractZipIntoRawfiles(item, pathTemporaryOutputfolder);
            }
        }

        private void ExtractZip(string file, string extractToFolder)
        {
            string zipToUnpack = file;
            string unpackDirectory = extractToFolder;
            using (ZipFile zip1 = ZipFile.Read(zipToUnpack))
            {
                // here, we extract every entry, but we could extract conditionally
                // based on entry name, size, date, checkbox status, etc.
                foreach (ZipEntry e in zip1)
                {
                    e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        private void ExtractGz(string filePath, string toFolder)
        {
            var fi = new FileInfo(filePath);
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                // Get original file extension, for example
                // "doc" from report.doc.gz.
                string curFile = fi.FullName;

                //Create the decompressed file.
                using (FileStream outFile = File.Create(Path.Combine(toFolder, Path.GetFileNameWithoutExtension(filePath))))
                {
                    using (GZipStream Decompress = new GZipStream(inFile,
                            CompressionMode.Decompress))
                    {
                        // Copy the decompression stream
                        // into the output file.
                        Decompress.CopyTo(outFile);
                    }
                }
            }
        }

        private void DeleteAndCreateFolderStructure(string pathTempFolder)
        {
            if (Directory.Exists(pathTempFolder))
                Directory.Delete(pathTempFolder, true);
            Directory.CreateDirectory(pathTempFolder);
        }

        private void ExtractZipIntoRawfiles(string pathFile, string pathTempFolder)
        {
            ExtractZip(pathFile, pathTempFolder);
            var pathGzFiles = Directory.GetFiles(pathTempFolder, "*.gz", SearchOption.AllDirectories);
            pathGzFiles = pathGzFiles.Where(x => !Path.GetFileName(x).Contains("SUMMARY")).ToArray();
            foreach (var path in pathGzFiles)
            {
                //log.Info(Path.GetFileName(path));
                ExtractGz(path, pathTempFolder);
                File.Delete(path);
            }
        }

        private void ConvertTickAsciFileIntoTickdateFile(string rawTickFile, string pathAsciFile)
        {
            var entries = ReadAsciFile(rawTickFile);
            WriteTickEntryToFile(pathAsciFile, entries);
        }

        private List<ITickDate> ReadAsciFile(string path)
        {
            string InstrumendID = Path.GetFileName(path).Substring(0, 5);
            var tickentryList = new List<ITickDate>();
            using (var reader = new StreamReader(path))
            {
                string line = "";

                while ((line = reader.ReadLine()) != null)
                {
                    tickentryList.Add(new TickDate(line, InstrumendID));
                }
            }
            return tickentryList;
        }

        private void WriteTickEntryToFile(string toFile, List<ITickDate> ticks)
        {
            using (var writer = new StreamWriter(toFile))
            {
                foreach (var item in ticks)
                {
                    writer.WriteLine(item);
                }
            }
        }
    }
}