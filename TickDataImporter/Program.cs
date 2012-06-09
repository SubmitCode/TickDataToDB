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
            //var tickFileManagerh
            Console.ReadKey();
        }

        //private static int counter = 1;
    }
}