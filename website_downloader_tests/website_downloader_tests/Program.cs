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
            string code = "<img src=\"auto:blank\" style='width: 100%'> </body>";
            var htmlElement = new HtmlElement(code);

            Console.WriteLine(htmlElement.TagName);
            foreach (var pair in htmlElement.Attributes)
                Console.WriteLine(pair.Key + "   $   " + pair.Value);
        }
    }
}
