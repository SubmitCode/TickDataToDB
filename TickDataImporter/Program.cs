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
    }
}