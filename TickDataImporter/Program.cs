using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Ionic.Zip;

namespace TickDataImporter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //string connection = @"Data Source=MWA\sqlexpress;Initial Catalog=test54;Integrated Security=True";
            //var conn = new SqlConnection(connection);
            //conn.Open();
            //conn.Close();

            string[] paths = Directory.GetFiles(@"M:\TickData\TickData", "*.zip", SearchOption.AllDirectories);
            string[] zipPahts = null;
            foreach (var item in paths)
            {
                Console.WriteLine(item);
                ExtractZip(item, @"C:\Temp\Zip");
                zipPahts = Directory.GetFiles(@"C:\Temp\Zip", "*.gz", SearchOption.AllDirectories);
                foreach (var path in zipPahts)
                {
                    Console.WriteLine("     " + path);
                    ExtractGz(path, @"C:\Temp\Zip\RawFiles");
                    string[] ascFilePaths = Directory.GetFiles(@"C:\Temp\Zip\RawFiles", "*.asc", SearchOption.AllDirectories);
                    ascFilePaths = ascFilePaths.Where(x => Path.GetFileName(x).Length == 20).ToArray();
                    foreach (var ascFile in ascFilePaths)
                    {
                        WriteTickDataToDB(ReadAsciFile(ascFile));
                        File.Delete(ascFile);
                    }
                    File.Delete(path);
                }
            }
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

        private static void WriteTickDataToDB(List<TickDate> lstTickdata)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=mwa\sqlexpress;Initial Catalog=TickData;Integrated Security=True");
            SqlCommand sqlCmd = new SqlCommand();
            foreach (var item in lstTickdata)
            {
                sqlCmd.CommandText = string.Format(
                        @"INSERT INTO [TickData].[dbo].[tblTickData]
                           ([InstrumentID]
                           ,[DateTime]
                           ,[Price]
                           ,[Volume]
                           ,[MarketFlag]
                           ,[SalesCondition]
                           ,[ExcluedFlag]
                           ,[UnfilteredPrice])
                     VALUES
                           ('{0}','{1}',{2},{3},'{4}',)", item.GetTickDataEntries());
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