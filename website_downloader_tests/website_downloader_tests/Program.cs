using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace website_downloader_tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = htmlWeb.Load("https://www.ynet.co.il");
            HtmlNodeCollection nodes = htmlDocument.DocumentNode.SelectNodes("//img");
            foreach (HtmlNode imgTag in nodes)
                Console.WriteLine(imgTag.Attributes["src"].Value);
        }
    }
}
