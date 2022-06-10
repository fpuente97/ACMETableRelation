using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MainConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello. This Software was developed to check how employees had coincided during the week\n");
            Console.WriteLine("The format required is as the following example:\n" +
                             "FERNANDO=MO10:00-12:00,TU10:00-12:00,WE10:00-12:00,TH01:00-03:00,FR14:00-18:00,SA14:00-18:00,SU20:00-21:00\n");
            Console.WriteLine("Where: \nMO = Monday\nTU = Tuesday\nWE = Wednesday\nTH = Thursday\nFR = Friday\nSA = Saturday\nSU = Sunday\n");
            Console.WriteLine("Enter only the name of the .txt file (without extension). \nPlease ensure that the file is in the directory of this app");
            string txtName = Console.ReadLine();
            var getfile = new GetFile(txtName);
            getfile.CheckIfFileExistThenRead();
        }
    }

    class GetFile
    {

        private string CurrentPath { get; set; }
        private string TextName { get; set; }
        private List<string> TextFileLines { get; set; }

        public GetFile(string textName)
        {
            TextName = textName;
            CurrentPath = Directory.GetCurrentDirectory() + $"\\{TextName}.txt";
            TextFileLines = new List<string>();
        }

        public void CheckIfFileExistThenRead()
        {
            bool FileExists = File.Exists(CurrentPath);
            Console.WriteLine(FileExists ? "\nFile found!\n" : "\nFile does not exists in directory. Please put the text file in the directory of this app");
            if (!FileExists)
                return;
            
            using(StreamReader sr = new StreamReader(CurrentPath))
            {
                string line;
                while((line = sr.ReadLine()) != null)
                {
                    string CleanedString = Regex.Replace(line, "\\s+", "");
                    TextFileLines.Add(CleanedString);
                }
            }

        }
    }
}
