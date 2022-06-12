using System;
using System.IO;
using System.Linq;
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
                             "USER=MO10:00-12:00,TU10:00-12:00,WE10:00-12:00,TH01:00-03:00,FR14:00-18:00,SA14:00-18:00,SU20:00-21:00\n");
            Console.WriteLine("Where: \nMO = Monday\nTU = Tuesday\nWE = Wednesday\nTH = Thursday\nFR = Friday\nSA = Saturday\nSU = Sunday\n");
            Console.WriteLine("Enter only the name of the .txt file (without extension). \nPlease ensure that the file is in the directory of this app");

            string txtName = Console.ReadLine();
            var getfile = new GetFile(txtName);

            getfile.CheckIfFileExistThenRead();
            getfile.SplitTextFileToList();
            getfile.ComparingEmployees();
        }
    }

    class GetFile
    {

        private string CurrentPath { get; set; }
        private List<string> TextFileLines { get; set; } = new List<string>();
        private List<EmployeeWorkedDayAndTime> comparingTables { get; set; }

        public GetFile(string textName)
        {
            CurrentPath =  $"{Directory.GetCurrentDirectory()}\\{textName}.txt";
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
                    if (CleanedString != "")
                        TextFileLines.Add(CleanedString);
                }
            }
        }

        public void SplitTextFileToList()
        {
            comparingTables = new List<EmployeeWorkedDayAndTime>();

            foreach(string line in TextFileLines)
            {
                string[] GetNameAndDays = line.Split('=');
                string[] GetWorkedDaysTime = GetNameAndDays[1].Split(',');
                for (int i = 0; i < GetWorkedDaysTime.Length; i++)
                {
                    var GetEmployee = new EmployeeWorkedDayAndTime();
                    GetEmployee.Name = GetNameAndDays[0];
                    GetEmployee.WorkedDay = GetWorkedDaysTime[i].Substring(0, 2);

                    var GetDayTime = GetWorkedDaysTime[i].Substring(2).Split("-");
                    GetEmployee.WorkedStartTime = Convert.ToDateTime(GetDayTime[0]);
                    GetEmployee.WorkedEndTime = Convert.ToDateTime(GetDayTime[1]);

                    comparingTables.Add(GetEmployee);
                }
            }
        }

        public void ComparingEmployees()
        {
            if (TextFileLines.Count == 0)
            {
                Console.WriteLine("There was no file read. Canceling..");
                return;
            }

            string[] days = new string[7]
            {
                "MO",
                "TU",
                "WE",
                "TH",
                "FR",
                "SA",
                "SU"
            };

            List<EmployeeCompared> OutputTable = new List<EmployeeCompared>();

            foreach (string day in days)
            {
                var FilteringEmployeeInfo = from c in comparingTables
                                           where c.WorkedDay == day
                                           select c;

                List<EmployeeWorkedDayAndTime> CheckingList = FilteringEmployeeInfo.ToList();

                foreach (var employeeOne in FilteringEmployeeInfo)
                {
                    foreach (var employeeTwo in CheckingList)
                    {
                        if (employeeOne == employeeTwo)
                            continue;

                        bool GotCoincidence = false;

                        if (employeeOne.WorkedStartTime >= employeeTwo.WorkedStartTime && employeeOne.WorkedStartTime < employeeTwo.WorkedEndTime)
                            GotCoincidence = true;

                        if (employeeOne.WorkedEndTime > employeeTwo.WorkedStartTime && employeeOne.WorkedStartTime <= employeeTwo.WorkedEndTime)
                            GotCoincidence = true;

                        if (employeeTwo.WorkedStartTime >= employeeOne.WorkedStartTime && employeeTwo.WorkedStartTime < employeeOne.WorkedEndTime)
                            GotCoincidence = true;

                        if (employeeTwo.WorkedEndTime > employeeOne.WorkedStartTime && employeeTwo.WorkedEndTime <= employeeOne.WorkedEndTime)
                            GotCoincidence = true;

                        if (!GotCoincidence)
                            continue;

                        OutputTable.Add(new EmployeeCompared
                        {
                            EmployeePair = $"{employeeOne.Name} - {employeeTwo.Name}"
                        });
                    }
                    CheckingList.Remove(employeeOne);
                }
            }
            var CountingCoincidences = from employee in OutputTable
                                       group employee by employee.EmployeePair into employeePairCoincidences
                                       select new
                                       {
                                           EmployeePair = employeePairCoincidences.Key,
                                           Count = employeePairCoincidences.Count()
                                       };
            foreach (var printPairs in CountingCoincidences)
                Console.WriteLine($"{printPairs.EmployeePair}: {printPairs.Count}");
        }
    }

    public class EmployeeCompared
    {
        public string EmployeePair { get; set; }

        public override string ToString()
        {
            return EmployeePair;
        }
    }

    public class EmployeeWorkedDayAndTime
    {
        public string Name { get; set; }
        public string WorkedDay{ get; set; }
        public DateTime WorkedStartTime { get; set; }
        public DateTime WorkedEndTime { get; set; }

        public override string ToString()
        {
            return $"Employee {Name} worked on {WorkedDay} From {WorkedStartTime} to {WorkedEndTime}";
        }
    }
}
