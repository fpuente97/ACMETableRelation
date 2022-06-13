using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace MainConsole
{
    public class GetFile
    {

        private string CurrentPath { get; set; }
        private List<string> TextFileLines { get; set; } = new List<string>();
        private List<EmployeeWorkedDayAndTime> comparingTables { get; set; }

        private string[] days = new string[7]
            {
                "MO",
                "TU",
                "WE",
                "TH",
                "FR",
                "SA",
                "SU"
            };

        public GetFile(string textName)
        {
            CurrentPath = $"{Directory.GetCurrentDirectory()}\\{textName}.txt";
        }

        public void CheckIfFileExistThenRead()
        {
            bool FileExists = File.Exists(CurrentPath);
            Console.WriteLine(FileExists ? "File found!\n" : "File does not exists in directory. Please put the text file in the directory of this app");
            if (!FileExists)
                return;

            using (StreamReader sr = new StreamReader(CurrentPath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string WhiteSpaceCleanedString = Regex.Replace(line, "\\s+", "");
                    if (WhiteSpaceCleanedString != "")
                        TextFileLines.Add(WhiteSpaceCleanedString);
                }
            }
        }

        public void SplitTextFileToList()
        {
            comparingTables = new List<EmployeeWorkedDayAndTime>();
            Regex CheckEqualChar = new Regex(@"[a-zA-Z0-9][=][a-zA-Z]");
            int lineIndex = 1;
            foreach (string line in TextFileLines)
            {
                string lineToCheck = Regex.Replace(line, @"[=]+", "=");

                if (!CheckEqualChar.IsMatch(lineToCheck))
                {
                    Console.WriteLine($"There is misspelling near the '=' character in line {lineIndex}\n");
                    continue;
                }
                string[] GetNameAndDays = lineToCheck.Split('=');

                if (GetNameAndDays.Count() > 2)
                {
                    Console.WriteLine($"There is more than one '=' char in line {lineIndex}\n");
                    continue;
                }

                string[] GetWorkedDaysTime = GetNameAndDays[1].Split(',');
                for (int i = 0; i < GetWorkedDaysTime.Length; i++)
                {
                    if (GetWorkedDaysTime[i] == "")
                        continue;

                    var GetEmployee = new EmployeeWorkedDayAndTime();
                    GetEmployee.Name = GetNameAndDays[0];

                    if (!days.Contains(GetWorkedDaysTime[i].Substring(0, 2)))
                    {
                        Console.WriteLine($"'{GetWorkedDaysTime[i].Substring(0, 2)}' in '{GetEmployee.Name}' worked schedule is not a valid day spelling. Line {lineIndex}");
                        continue;
                    }

                    GetEmployee.WorkedDay = GetWorkedDaysTime[i].Substring(0, 2);

                    var GetDayTime = GetWorkedDaysTime[i].Substring(2).Split("-");
                    try
                    {
                        GetEmployee.WorkedStartTime = Convert.ToDateTime(GetDayTime[0]);
                        GetEmployee.WorkedEndTime = Convert.ToDateTime(GetDayTime[1]);
                    }
                    catch
                    {
                        Console.WriteLine($"There is a misspelling error with the worked time at:\n'{GetEmployee.ToString()}'\n");
                        continue;
                    }
                    comparingTables.Add(GetEmployee);
                }
                lineIndex++;
            }
        }

        public void ComparingEmployees()
        {
            if (TextFileLines.Count == 0)
            {
                Console.WriteLine("There was no file read. Canceling..");
                return;
            }

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
            Console.WriteLine($"Employees coincidences:");
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
        public string WorkedDay { get; set; }
        public DateTime WorkedStartTime { get; set; }
        public DateTime WorkedEndTime { get; set; }

        public override string ToString()
        {
            return $"Employee {Name} worked on {WorkedDay} From {WorkedStartTime.ToShortTimeString()} to {WorkedEndTime.ToShortTimeString()}";
        }
    }
}
