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
            var downloader = new UrlDownloader("https://en.wikipedia.org/wiki/Benjamin_Netanyahu");
            downloader.DownloadImages(@"C:\Users\ganga\Documents\try");
        }
    }
}
