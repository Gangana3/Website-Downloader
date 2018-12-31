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
            Console.WriteLine(UrlDownloader.GetAbsoluteUrl("https://www.google.com/foo1/foo2/foo3", "../../stylesheet.css"));
        }
    }
}
