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
            var htmlDownloader = new UrlDownloader("https://www.djangoproject.com/");
            htmlDownloader.DownloadImages(@"https://twitter.com/wikipedia");
        }
    }
}
