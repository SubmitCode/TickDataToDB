using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestProject1
{
    internal class CurrentFolder
    {
        public static String GetCurrentTestFileFolder()
        {
            string pathExe = (System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(8);
            pathExe = Directory.GetParent(pathExe).FullName;
            pathExe = Directory.GetParent(pathExe).FullName;
            pathExe = Directory.GetParent(pathExe).FullName;
            pathExe = Path.Combine(pathExe, "TestFiles");
            return pathExe;
        }
    }
}