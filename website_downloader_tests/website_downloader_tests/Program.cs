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
            string code = "<img sRc=\"auto:blank\" style='width: 100%'>";
            var htmlElement = new HtmlElement(code);
            htmlElement.Attributes["src"] = "Hello world";
            htmlElement.Attributes["style"] = "width: 50%";
            Console.WriteLine(htmlElement);
        }
    }
}
