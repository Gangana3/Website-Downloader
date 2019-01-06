using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.IO;

/// <summary>
/// TODO: Make the class work for multithreaded download. for example the css, the js and the imgs may
/// be downloaded each from a different thread. Maybe split the three of these into groups.
/// 
/// TODO: Check the DownloadImages and DownloadJS functions.
/// </summary>
namespace website_downloader_tests
{
    /// <summary>
    /// This class wraps the html code making it available
    /// to download all of it's resources so we could view the
    /// html page offline.
    /// </summary>
    class UrlDownloader
    {
        private static readonly object lockObj = new object();

        // Public Properties
        public string HtmlCode { get; private set; }    // Html code
        public string Url { get; private set; }         // Url to download

        // Private fields
        private HtmlDocument htmlDoc;                           // Parsed html page
        private WebClient webClient;                            // Http client for downloading the pages
        private int imgId = 0;                                  // Used for naming the images files
        private int jsId = 0;                                   // Used for naming the javascript files
        private int cssId = 0;                                  // Used for naming the css files
        private enum Resource { Img, Css, Js };                 // Kinds of resources that can exist
        private Dictionary<string, string> resourcesNames;      // mapping between the downloaded url and local resource path

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
            this.resourcesNames = new Dictionary<string, string>();
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
                string href = imgTag.Attributes["src"];         // Get the href attribute from the element
                string url = GetAbsoluteUrl(this.Url, href);    // Get the absolute url from the given url

                if (!this.IsDownloaded(url))
                {
                    Console.WriteLine("DEBUG: Downloading {0}", url);
                    string fileName = Path.Combine(path, imgId.ToString());
                    this.RegisterDownload(url, fileName, Resource.Img);
                    this.webClient.DownloadFile(url, fileName);
                }
            }
        }

        /// <summary>
        /// Downloads the Stylesheets from the given url, Including the resources that
        /// The stylesheet uses like another stylesheets, images etc...
        /// </summary>
        public void DownloadStylesheets(string path)
        {

        }


        /// <summary>
        /// Downloads the javascript from the html page
        /// </summary>
        public void DownloadJS(string path)
        {
            foreach (HtmlElement element in htmlDoc.GetElementsByTagName("script"))
            {
                // In case the script element has an 'src' attribute
                if (element.Attributes.Keys.Contains("src"))
                {
                    string src = element.Attributes["src"];
                    string url = GetAbsoluteUrl(this.Url, src);

                    if (!this.IsDownloaded(url))
                    {
                        string fileName = Path.Combine(path, this.jsId.ToString());
                        this.webClient.DownloadFile(url, fileName);
                        this.RegisterDownload(url, fileName, Resource.Js);
                    }
                }
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
        public static string GetAbsoluteUrl(string currentUrl, string relativeUrl)
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
            {
                string resultUrl = currentUrl;
                // In case url ends with '/', remove it
                if (relativeUrl.EndsWith("/"))
                    relativeUrl.Remove(relativeUrl.Length - 1);
                // Move backwards in the url
                while (relativeUrl.StartsWith(".."))
                {
                    resultUrl = resultUrl.Remove(resultUrl.LastIndexOf('/'));
                    relativeUrl = relativeUrl.Substring(relativeUrl.IndexOf('/') + 1);
                }
                resultUrl += "/" + relativeUrl;     // Add the relative url to the result
                return resultUrl;
            }
            // In case the relative url is just a relative url
            else
                return currentUrl + relativeUrl;
        }

        /// <summary>
        /// Used to mark a url as downloaded, In order to be able to retreive it's file name
        /// and insert it to the page.
        /// </summary>
        /// <param name="absoluteUrl">Absolute url to the downloaded file</param>
        /// <param name="resourceId">Id of the given file</param>
        private void RegisterDownload(string absoluteUrl, string filePath, Resource resourceType)
        {
            switch (resourceType)
            {
                case Resource.Css:
                    cssId++;     
                    break;

                case Resource.Img:
                    imgId++;     
                    break;

                case Resource.Js:
                    jsId++;      
                    break;

                default:
                    throw new NotImplementedException();
            }

            this.resourcesNames.Add(absoluteUrl, filePath);
        }


        /// <summary>
        /// Checks whether the url was downloaded.
        /// </summary>
        /// <param name="absoluteUrl"> Absolute url to a resource. </param>
        /// <returns> whether the url was downloaded. </returns>
        private bool IsDownloaded(string absoluteUrl)
        {
            return this.resourcesNames.Keys.Contains(absoluteUrl);
        }
        #endregion
    }
}
