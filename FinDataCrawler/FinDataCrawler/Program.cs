using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinDataCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            var argResult = new ArgumentsParser(args);
            if (!argResult.Success)
            {
                ArgumentsParser.PrintUsage();
                return;
            }
        }
    }
}
