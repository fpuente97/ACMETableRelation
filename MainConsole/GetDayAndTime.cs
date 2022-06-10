using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainConsole
{
    public class GetDayAndTime
    {
        public DateTime? GetTime(string WorkedTime)
        {
            DateTime dt;
            try
            {
                dt = Convert.ToDateTime(WorkedTime);
                return dt;
            }
            catch
            {
                Console.WriteLine("Error in the format! Please check the .txt file for mispellings");
                return null;
            }
        }
    }
}
