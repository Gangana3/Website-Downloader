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
            string code = @"<body><div> <div> Hello World!</div></div><img src='auto:blank'/> <div> <div> hi </div> </div> </body>";
            var element = new HtmlElement(code);

            foreach (HtmlElement e in element.InnerElements)
            {
                Console.WriteLine("{0}", e.RawCode);
            }
        }
    }
}
