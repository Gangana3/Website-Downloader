using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace website_downloader_tests
{
    /// <summary>
    /// This class wraps the html code making it available
    /// to download all of it's resources so we could view the
    /// html page offline.
    /// </summary>
    class HtmlDownloader
    {
        public string HtmlCode { get; private set; }    // Html code

        /// <summary>
        /// Instantiate HtmlCode class
        /// </summary>
        /// <param name="code">Html code to parse and handle</param>
        public HtmlDownloader(string htmlCode)
        {
            this.HtmlCode = htmlCode;
        }

        /// <summary>
        /// Downloads the html file with all of the resources.
        /// Images, Javascripts etc...
        /// </summary>
        /// <param name="path">The location in which the webpage will be saved</param>
        public static void Download(string path)
        {

        }
    }
}
