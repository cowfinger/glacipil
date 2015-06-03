using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FinDataCrawler
{
    class SinaFinDataProvider
    {
        private static string UrlPattern = "http://market.finance.sina.com.cn/downxls.php?date={0}&symbol={1}";

        private static string SymbolPattern = @"<a target=""_blank"" href=""http://quote\.eastmoney\.com/{0}(\d+)\.html"">";

        public static string SymbolListPageUlr = "http://quote.eastmoney.com/stock_list.html";

        public static IEnumerable<int> GetStockCode(string prefix)
        {
            var page = OpenStockListPage();
            if (string.IsNullOrEmpty(page))
            {
                return new List<int>();
            }

            var pattern = string.Format(SymbolPattern, prefix);
            var matches = Regex.Matches(page, pattern);
            return from Match match in matches
                   let stockId = int.Parse(match.Groups[1].Value)
                   select stockId;
        }

        public static string BuildUrl(DateTime day, string symbol)
        {
            var dayString = day.ToString("yyyy-MM-dd");
            return string.Format(UrlPattern, dayString, symbol);
        }

        public static void DownloadFile(DateTime day, string symbol, string rootDir)
        {
            var symbolDir = Path.Combine(rootDir, symbol);
            if (!Directory.Exists(symbolDir))
            {
                Directory.CreateDirectory(symbolDir);
            }

            var dayString = day.ToString("yyyy-MM-dd");
            var fileName = Path.Combine(symbolDir, dayString);
            if (File.Exists(fileName))
            {
                return;
            }

            var url = BuildUrl(day, symbol);
            DumpUrlToFile(url, fileName);
        }

        public static void DumpUrlToFile(string url, string fileName)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var fileStream = new FileStream(fileName, FileMode.Create))
                {
                    response.GetResponseStream().CopyTo(fileStream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error :{0} when dump ulr {1} to {2}", e.Message, url, fileName);
            }
        }

        public static string OpenStockListPage()
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(SymbolListPageUlr);
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error :{0} when Open Stock List", e.Message);
                return string.Empty;
            }
        }
    }
}
