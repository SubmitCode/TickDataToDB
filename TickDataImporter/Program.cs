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
        //private static int counter = 1;

        private static void Main(string[] args)
        {
            if (Directory.Exists(@"C:\Temp\Zip"))
                Directory.Delete(@"C:\Temp\Zip", true);
            Directory.CreateDirectory(@"C:\Temp\Zip\RawFiles");
            string[] paths = Directory.GetFiles(@"M:\TickData\TickData\FUT\A\AD", "*.zip", SearchOption.AllDirectories);
            //paths = paths.Skip(272).ToArray();
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
                        //log.Info(Path.GetFileName(path));
                        ExtractGz(path, @"C:\Temp\Zip\RawFiles");
                        string[] ascFilePaths = Directory.GetFiles(@"C:\Temp\Zip\RawFiles", "*.asc", SearchOption.AllDirectories);
                        ascFilePaths = ascFilePaths.Where(x => Path.GetFileName(x).Length == 20).ToArray();
                        foreach (var ascFile in ascFilePaths)
                        {
                            try
                            {
                                log.Info(Path.GetFileName(ascFile));
                                //WriteFileNameToDB(Path.GetFileName(ascFile));
                                var lst = ReadAsciFile(ascFile);
                                WriteTickEntryToFile(@"C:\Temp\Zip\RawFiles\" + Path.GetFileNameWithoutExtension(ascFile) + ".txt", lst);
                                //WriteTickDataToDB(ReadAsciFile(ascFile));
                                WriteBulkInsertToDB(Path.Combine(Path.GetDirectoryName(ascFile), Path.GetFileNameWithoutExtension(ascFile) + ".txt"));
                                File.Delete(ascFile);
                            }
                            catch (Exception e)
                            {
                                log.Error(e);
                            }
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

        private static void WriteTickEntryToFile(string toFile, List<TickDate> ticks)
        {
            using (var writer = new StreamWriter(toFile))
            {
                foreach (var item in ticks)
                {
                    writer.WriteLine(item);
                }
            }
        }

        private static void WriteBulkInsertToDB(string filenmae)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=mwa\sqlexpress;Initial Catalog=TickData;Integrated Security=True");
            try
            {
                conn.Open();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandText = string.Format("BULK INSERT [TickData].[dbo].[tblTickData] FROM '{0}' WITH (FORMATFILE = '{1}')", filenmae,
                    @"C:\Users\Willi\Documents\visual studio 2010\Projects\TickDataToDB\TickDataImporter\Otherstuff\bulkFormat.fmt");
                sqlCmd.Connection = conn;
                sqlCmd.ExecuteNonQuery();
                File.Delete(filenmae);
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            finally
            {
                conn.Close();
            }
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
            var builder = new StringBuilder();
            sqlCmd.CommandTimeout = 1000;
            foreach (var item in lstTickdata)
            {
                builder.AppendLine(string.Format(
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
                           ('{0}','{1}',{2},{3},'{4}',{5},'{6}',{7})", item.GetTickDataEntries()));
            }
            sqlCmd.CommandText = builder.ToString();
            sqlCmd.Connection = conn;
            sqlCmd.ExecuteNonQuery();
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
                    }
                }
            }
        }
    }
}