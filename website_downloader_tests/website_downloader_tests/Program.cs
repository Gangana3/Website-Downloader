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
            var urlDownloader = new UrlDownloader("https://he.wikipedia.org/wiki/%D7%91%D7%A0%D7%99%D7%9E%D7%99%D7%9F_%D7%A0%D7%AA%D7%A0%D7%99%D7%94%D7%95");


            urlDownloader.DownloadImages(@"C:\Users\ganga\Documents\try");
        }
    }
}
