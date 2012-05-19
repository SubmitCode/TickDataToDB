using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Ionic.Zip;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace TickDataImporter
{
    internal class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private static void Main(string[] args)
        {
            if (Directory.Exists(@"C:\Temp\Zip"))
            {
                Directory.Delete(@"C:\Temp\Zip", true);
                Directory.CreateDirectory(@"C:\Temp\Zip\RawFiles");
            }
            string[] paths = Directory.GetFiles(@"M:\TickData\TickData", "*.zip", SearchOption.AllDirectories);
            string[] zipPahts = null;
            foreach (var item in paths)
            {
                try
                {
                    //log.Info(Path.GetFileName(item));
                    ExtractZip(item, @"C:\Temp\Zip");
                    zipPahts = Directory.GetFiles(@"C:\Temp\Zip", "*.gz", SearchOption.AllDirectories);
                    zipPahts = zipPahts.Where(x => !Path.GetFileName(x).Contains("SUMMARY")).ToArray();
                    foreach (var path in zipPahts)
                    {
                        try
                        {
                            //log.Info(Path.GetFileName(path));
                            ExtractGz(path, @"C:\Temp\Zip\RawFiles");
                            string[] ascFilePaths = Directory.GetFiles(@"C:\Temp\Zip\RawFiles", "*.asc", SearchOption.AllDirectories);
                            ascFilePaths = ascFilePaths.Where(x => Path.GetFileName(x).Length == 20).ToArray();
                            foreach (var ascFile in ascFilePaths)
                            {
                                log.Info("Import File" + Path.GetFileName(ascFile));
                                WriteFileNameToDB(Path.GetFileName(ascFile));
                                WriteTickDataToDB(ReadAsciFile(ascFile));
                                File.Delete(ascFile);
                            }
                        }
                        catch (Exception e)
                        {
                            log.Error(e);
                        }
                        File.Delete(path);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            }
            log.Info("finished");
            Console.ReadLine();
        }

        private static List<TickDate> ReadAsciFile(string path)
        {
            string InstrumendID = Path.GetFileName(path).Substring(0, 5);
            var tickentryList = new List<TickDate>();
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

        private static void WriteFileNameToDB(string fileName)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=mwa\sqlexpress;Initial Catalog=TickData;Integrated Security=True");
            conn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = conn;
            sqlCmd.CommandText = string.Format(
                    @"INSERT INTO [TickData].[dbo].[tblImported]
                        ([FileName])
                    VALUES
                        ('{0}')", fileName);
            sqlCmd.ExecuteNonQuery();
        }

        private static void WriteTickDataToDB(List<TickDate> lstTickdata)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=mwa\sqlexpress;Initial Catalog=TickData;Integrated Security=True");
            conn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            foreach (var item in lstTickdata)
            {
                sqlCmd.Connection = conn;
                sqlCmd.CommandText = string.Format(
                        @"INSERT INTO [TickData].[dbo].[tblTickData]
                           ([InstrumentID]
                           ,[DateTime]
                           ,[Price]
                           ,[Volume]
                           ,[MarketFlag]
                           ,[SalesCondition]
                           ,[ExcludeFlag]
                           ,[UnfilteredPrice])
                     VALUES
                           ('{0}','{1}',{2},{3},'{4}',{5},'{6}',{7})", item.GetTickDataEntries());
                sqlCmd.ExecuteNonQuery();
            }
            conn.Close();
        }

        //        private static

        private static void ExtractZip(string file, string extractToFolder)
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

        private static void ExtractGz(string filePath, string toFolder)
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
                        Console.WriteLine("         Decompressed: {0}", fi.Name);
                    }
                }
            }
        }
    }
}