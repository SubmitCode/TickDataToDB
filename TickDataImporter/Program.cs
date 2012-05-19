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
            foreach (var item in paths)
            {
                Console.WriteLine(item);
                ExtractZip(item, @"C:\Temp\Zip");
                string[] zipPahts = Directory.GetFiles(@"C:\Temp\Zip", "*.gz", SearchOption.AllDirectories);
                foreach (var path in zipPahts)
                {
                    Console.WriteLine("     " + path);
                    ExtractGz(path, @"C:\Temp\Zip\RawFiles");
                }
            }
            Console.ReadLine();
        }

        private static void WriteFileToDb(string path)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=mwa\sqlexpress;Initial Catalog=TickData;Integrated Security=True");
            SqlCommand sqlCmd = new SqlCommand();
            using(var reader = new StreamReader(path))
	        {
                conn.Open();
                string line = "";
                string[] values = null;
                while ((line = reader.ReadLine()) != null)
	            {
                    values = line.Split(',');
                    sqlCmd.CommandText = "INSERT INTO
	            }
	        }
        }

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