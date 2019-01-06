using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace website_downloader_tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var downloader = new UrlDownloader("https://www.ynet.co.il/home/0,7340,L-8,00.html");
            downloader.DownloadCss(@"C:\Users\ganga\Documents\try");
        }
    }
}
