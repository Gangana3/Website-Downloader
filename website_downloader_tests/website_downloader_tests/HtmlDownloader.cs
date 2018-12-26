using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace website_downloader_tests
{
    /// <summary>
    /// This class wraps the html code making it available
    /// to download all of it's resources so we could view the
    /// html page offline.
    /// </summary>
    class HtmlDownloader
    {
        // Public Properties
        public string HtmlCode { get; private set; }    // Html code

        // Private fields
        private HtmlDocument htmlDoc;

        /// <summary>
        /// Instantiate HtmlCode class
        /// </summary>
        /// <param name="code">Html code to parse and handle</param>
        public HtmlDownloader(string htmlCode)
        {
            this.HtmlCode = htmlCode;
            this.htmlDoc = new HtmlDocument(htmlCode);
        }

        /// <summary>
        /// Downloads the html file with all of the resources.
        /// Images, Javascripts etc...
        /// </summary>
        /// <param name="path">The location in which the webpage will be saved</param>
        public void Download(string path)
        {

        }

        /// <summary>
        /// Downloads the images from the html page to the given path
        /// </summary>
        /// <param name="path">path to download the images to</param>
        public void DownloadImages(string path)
        {
            foreach (HtmlElement imgTag in htmlDoc.GetElementsByTagName("img"))
            {

            }
        }


        #region Private Methods
        /// <summary>
        /// Returns the absolute Url of the relative url.
        /// for example:
        ///     baseUrl = "https://www.google.com", relativeUrl = "/helloworld"     -> "https://www.google.com/helloworld"
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="relativeUrl"></param>
        private static void GetAbsoluteUrl(string baseUrl, string relativeUrl)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
