using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace website_downloader_tests
{
    /// <summary>
    /// This class wraps the html code making it available
    /// to download all of it's resources so we could view the
    /// html page offline.
    /// </summary>
    class UrlDownloader
    {
        // Public Properties
        public string HtmlCode { get; private set; }    // Html code

        public string Url { get; private set; }

        // Private fields
        private HtmlDocument htmlDoc;
        private WebClient webClient;

        /// <summary>
        /// Instantiate a UrlDownloader object
        /// </summary>
        /// <param name="url">Url to the page</param>
        public UrlDownloader(string url, string html="")
        {
            this.webClient = new WebClient();
            if (string.IsNullOrEmpty(html))
                html = this.webClient.DownloadString(url);
            this.Url = url;
            this.HtmlCode = html;
            this.htmlDoc = new HtmlDocument(html);
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
            int id = 0;
            foreach (HtmlElement imgTag in htmlDoc.GetElementsByTagName("img"))
            {
                string href = imgTag.Attributes["src"];
                string url = GetAbsoluteUrl(this.Url, href);

                string fileName = Path.Combine(path, id.ToString());
                id++;
                Console.WriteLine("Downloading {0} and saves it as {1}", url, fileName);     // Debug message
                this.webClient.DownloadFile(url, fileName);
            }
        }


        #region Private Methods
        /// <summary>
        /// Returns the absolute Url of the relative url.
        /// for example:
        ///     current = "https://www.google.com", relativeUrl = "/helloworld"     -> "https://www.google.com/helloworld"
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="relativeUrl"></param>
        private static string GetAbsoluteUrl(string currentUrl, string relativeUrl)
        {
            // Get base url, for example 'https://www.google.com/foo/foo1/foo2' -> base url = 'https://www.google.com'
            string baseUrl = Regex.Match(currentUrl, @"https?://[a-zA-Z\.0-9]*").Groups[0].Value;
            bool isSecured = baseUrl.StartsWith("https");

            // In case starts with double slash
            if (relativeUrl.StartsWith("//"))
            {
                if (isSecured)
                    return "https:" + relativeUrl;
                else
                    return "http:" + relativeUrl;
            }
            // In case starts with slash
            else if (relativeUrl.StartsWith("/"))
                return baseUrl + relativeUrl;
            // in case the relative url is actually an absolute url
            else if (relativeUrl.StartsWith("http://") || relativeUrl.StartsWith("https://"))
                return relativeUrl;
            // In case the relative url starts with '..'
            else if (relativeUrl.StartsWith(".."))
                return currentUrl.Slice(0, currentUrl.LastIndexOf('/', currentUrl.Length - 2) + 1) + relativeUrl;
            // In case the relative url is just a relative url
            else
                return currentUrl + relativeUrl;
        }
        #endregion
    }
}
