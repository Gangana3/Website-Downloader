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
            string code = @"";
            var element = new HtmlElement(code);

            foreach (HtmlElement e in element.InnerElements)
            {
                Console.WriteLine("\n{0}\n{1}\n", e.TagName, e.Content);
            }
        }
    }
}
