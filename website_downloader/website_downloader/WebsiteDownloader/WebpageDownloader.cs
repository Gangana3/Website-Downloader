﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace website_downloader.WebsiteDownloader
{
    /// <summary>
    /// This class wraps the html code making it available
    /// to download all of it's resources so we could view the
    /// html page offline.
    /// </summary>
    class WebpageDownloader
    {
        // Names
        private static readonly string CssDirectoryName = "css";        // The name of the directory that contains the css
        private static readonly string JsDirectoryName = "js";          // The name of the directory that contains the javascript files
        private static readonly string ImgsDirectoryName = "images";    // The name of the directory that contains the images
        private static readonly string CssResourcesDirectoryName = "css_resources";   // A directory that contains the files that the css file use
        private static readonly string MainPageName = "index.html";     // The name of the local page

        // Public Properties
        public string HtmlCode { get; private set; }    // Html code
        public string Url { get; private set; }         // Url to download

        // Private fields
        private string basePath;
        private HtmlDocument htmlDoc;                           // Parsed html page
        private WebClient webClient;                            // Http client for downloading the pages
        private int imgId = 0;                                  // Used for naming the images files
        private int jsId = 0;                                   // Used for naming the javascript files
        private int cssId = 0;                                  // Used for naming the css files
        private int cssResourceId = 0;                          // Used for naming the resources inside the css files
        private enum Resource { Img, Css, Js, CssResource };    // Kinds of resources that can exist
        private Dictionary<string, string> resourcesNames;      // mapping between the downloaded url and local resource path

        // Public events
        public event EventHandler StartedDownloading;
        public event EventHandler FinishedDownloading;

        // Private properties
        private string CssResourcesPath { get { return Path.Combine(this.basePath, CssResourcesDirectoryName); } }
        private string CssPath { get { return Path.Combine(this.basePath, CssDirectoryName); } }
        private string JsPath { get { return Path.Combine(this.basePath, JsDirectoryName); } }
        private string ImgsPath { get { return Path.Combine(this.basePath, ImgsDirectoryName); } }
        private string MainPagePath { get { return Path.Combine(this.basePath, MainPageName); } }

        /// <summary>
        /// Url Downloader constructor
        /// </summary>
        /// <param name="url">a url that the client want to download</param>
        /// <param name="path">Destination path</param>
        public WebpageDownloader(string url, string path, string websiteName = "Downloaded Website")
        {
            this.webClient = new WebClient();
            string html = this.webClient.DownloadString(url);
            this.Url = url;
            this.HtmlCode = html;
            this.htmlDoc = new HtmlDocument(html);                      // Parse the html code
            this.resourcesNames = new Dictionary<string, string>();

            // Create a new main directory
            this.basePath = Path.Combine(path, websiteName);
            
            if (Directory.Exists(this.basePath))
            {
                System.Windows.Forms.MessageBox.Show("DEBUG: directory exists");
                this.basePath += Directory.EnumerateDirectories(path).Count(dir => Path.GetFileName(dir).StartsWith(websiteName)).ToString();  // Add an id to the directory
                
                //DEBUG
                foreach (string dir in Directory.EnumerateDirectories(path))
                    System.Windows.Forms.MessageBox.Show(string.Format("DEBUG: {0}", dir));
            }
            Directory.CreateDirectory(basePath);

            // Create sub directories
            Directory.CreateDirectory(this.CssPath);            // Create a directory for css files
            Directory.CreateDirectory(this.JsPath);             // Create a directroy for javascript files
            Directory.CreateDirectory(this.ImgsPath);           // Create a directory for images
            Directory.CreateDirectory(this.CssResourcesPath);   // Create a directory for resources retrieved from the css file

        }



        /// <summary>
        /// Downloads the html file with all of the resources.
        /// Images, Javascripts etc...
        /// </summary>
        public void Download()
        {
            this.OnStartedDownloading();            // downloading is starting

            // Download all the resources
            this.DownloadImages(this.ImgsPath);
            this.DownloadCss(this.CssPath);
            this.DownloadJs(this.JsPath);
            this.InsertLinks();                     // Insert the link into the html code
            this.RemoveSrcsetAttribute();           // Remove srcset attribute that make images disappear
            this.SaveMainPage(this.MainPagePath);   // Save the html code into a file

            this.OnFinishedDownloading();           // downloading is finished
        }


        /// <summary>
        /// Downloads the images from the html page to the given path
        /// </summary>
        /// <param name="path">path to download the images to</param>
        public void DownloadImages(string path)
        {
            foreach (HtmlElement imgTag in htmlDoc.GetElementsByTagName("img"))
            {
                if (imgTag.Attributes.Keys.Contains("src"))
                {
                    string url = imgTag.Attributes["src"];         // Get the attribute's value from the element
                    string absoluteUrl = GetAbsoluteUrl(this.Url, url);    // Get the absolute furl from the given url

                    if (!this.IsDownloaded(absoluteUrl) && url.Length > 0)
                    {
                        WriteLineToLogFile("DEBUG: {0} Downloading {1}", this.imgId, absoluteUrl);

                        string filePath = Path.Combine(path, imgId.ToString());     // The local file path             
                        try
                        {
                            this.webClient.DownloadFile(absoluteUrl, filePath);
                            this.RegisterDownload(url, filePath, Resource.Img);
                        }
                        catch (WebException ex)
                        {
                            this.WriteLineToLogFile("DEBUG: ERROR: Failed to download {0}  MESSAGE==>{1}", absoluteUrl, ex.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Downloads the Stylesheets from the given url, Including the resources that
        /// The stylesheet uses like another stylesheets, images etc...
        /// </summary>
        public void DownloadCss(string path)
        {
            bool FilterLinks(HtmlElement e) => e.Attributes.Keys.Contains("rel") && e.Attributes["rel"].ToLower() == "stylesheet" && e.Attributes.Keys.Contains("href");
            foreach (HtmlElement element in htmlDoc.GetElementsBy(FilterLinks))
            {
                string absoluteUrl = GetAbsoluteUrl(this.Url, element.Attributes["href"]);

                if (!this.IsDownloaded(absoluteUrl))
                {
                    WriteLineToLogFile("DEBUG: Downloading Css {0}", absoluteUrl);

                    this.DownloadCssRecursively(this.webClient.DownloadString(absoluteUrl), absoluteUrl);
                }
            }
        }


        /// <summary>
        /// Downloads the javascript from the html page
        /// </summary>
        public void DownloadJs(string path)
        {
            foreach (HtmlElement element in htmlDoc.GetElementsByTagName("script"))
            {
                // In case the script element has an 'src' attribute
                if (element.Attributes.Keys.Contains("src"))
                {
                    string src = element.Attributes["src"];
                    string absoluteUrl = GetAbsoluteUrl(this.Url, src);

                    if (!this.IsDownloaded(absoluteUrl))
                    {
                        WriteLineToLogFile("DEBUG: Downloading {0}", absoluteUrl);

                        string filePath = Path.Combine(path, this.jsId.ToString());     // The local file path
                        try
                        {
                            this.webClient.DownloadFile(absoluteUrl, filePath);
                            this.RegisterDownload(absoluteUrl, filePath, Resource.Img);
                        }
                        catch (WebException ex)
                        {
                            this.WriteLineToLogFile("DEBUG: ERROR: Failed to download {0}  MESSAGE==>{1}", absoluteUrl, ex.Message);
                        }
                    }
                }
            }

        }


        #region Private Methods
        /// <summary>
        /// Removes srcset from the html code because it's making the images
        /// disappear
        /// </summary>
        private void RemoveSrcsetAttribute()
        {
            var srcsetRegex = new Regex(@"srcset=[""'].+[""']", RegexOptions.IgnoreCase);
            foreach (Match match in srcsetRegex.Matches(this.HtmlCode))
                this.HtmlCode = HtmlCode.Replace(match.Value, "");
        }

        /// <summary>
        /// Inserts the downloaded local files into the html code
        /// so that the links in srcs will refer to the local files.
        /// </summary>
        private void InsertLinks()
        {
            foreach (KeyValuePair<string, string> pair in resourcesNames)
            {
                WriteLineToLogFile("DEBUG: Replacing {0} -> {1}", pair.Key, pair.Value);

                this.HtmlCode = HtmlCode.Replace(pair.Key, GetRelativeUrl(pair.Value));
            }
        }

        /// <summary>
        /// Saves the main page in the given path
        /// </summary>
        /// <param name="path">Path the save the html file in</param>
        private void SaveMainPage(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
                sw.Write(this.HtmlCode);
        }

        /// <summary>
        /// Returns a relative url of a downloaded file, the result url
        /// is relative to the main page file.
        /// </summary>
        /// <returns>Relative url of a downloaded file</returns>
        private static string GetRelativeUrl(string filePath)
        {
            string relativeDirectory = string.Empty;
            // In case filePath contains the path to a css resource
            if (filePath.Contains(CssResourcesDirectoryName))
                relativeDirectory = CssResourcesDirectoryName;
            // In case filePath contains the path to a css file
            else if (filePath.Contains(CssDirectoryName))
                relativeDirectory = CssDirectoryName;
            // In case filePath contains the path to a javascript file
            else if (filePath.Contains(JsDirectoryName))
                relativeDirectory = JsDirectoryName;
            // In case filePath contains the path to an image
            else if (filePath.Contains(ImgsDirectoryName))
                relativeDirectory = ImgsDirectoryName;

            return Path.Combine(relativeDirectory, Path.GetFileName(filePath)).Replace(Path.DirectorySeparatorChar, '/');
        }

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
                resultUrl = resultUrl.Remove(resultUrl.LastIndexOf('/'));
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
                    this.cssId++;
                    break;

                case Resource.Img:
                    this.imgId++;
                    break;

                case Resource.Js:
                    this.jsId++;
                    break;

                case Resource.CssResource:
                    this.cssResourceId++;
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
            return this.resourcesNames.Keys.Any(relativeUrl => GetAbsoluteUrl(this.Url, relativeUrl) == absoluteUrl);
        }


        /// <summary>
        /// Downloads the css resources. for example a css file may contain 'url("url/to/resource)"', in that case,
        /// we have to download this resource
        /// </summary>
        /// <param name="cssCode"></param>
        /// <returns></returns>
        private void DownloadCssRecursively(string cssCode, string currentUrl)
        {
            // Regexes for finding the resources used inside the css code
            var urlRegex = new Regex(@"url\([""']?([-a-zA-Z0-9@:%_\+.~#?&//=]*)[""']?\)");  // The url is in group[1]
            var importRegex = new Regex(@"@import (?:url\()?[""']?([-a-zA-Z0-9@:%_\+.~#?&//=]*)[""']?\)?"); // The url is in group[1]

            var resourcesUrlList = new List<string>();      // A list of all urls that are used
            var importsUrlList = new List<string>();        // A list of the url of the imported css sheets

            // Build the resources list
            foreach (Match match in urlRegex.Matches(cssCode))
            {
                // In case the resources url list does not contain this url
                string url = match.Groups[1].Value;
                if (!resourcesUrlList.Contains(url))
                    resourcesUrlList.Add(url);
            }

            // Build the imported urls list
            foreach (Match match in importRegex.Matches(cssCode))
            {
                string url = match.Groups[1].Value;

                // In case the import urls list does not contain this url
                if (!importsUrlList.Contains(url))
                    importsUrlList.Add(url);    // Add it to the list

                // In case the resources url list contain this url
                if (resourcesUrlList.Contains(url))
                    resourcesUrlList.Remove(url);   // Remove it from the list
            }

            // Download all resources used in this stylesheet
            foreach (string url in resourcesUrlList)
            {
                string absoluteUrl = GetAbsoluteUrl(currentUrl, url);
                // In case the resource was not downloaded yet
                if (!this.IsDownloaded(absoluteUrl))
                {
                    WriteLineToLogFile("DEBUG: Downloading Css Resource {0}", absoluteUrl);
                    string filePath = Path.Combine(this.CssResourcesPath, cssResourceId.ToString());
                    this.RegisterDownload(absoluteUrl, filePath, Resource.CssResource);
                    try
                    {
                        this.webClient.DownloadFile(absoluteUrl, filePath);

                        cssCode.Replace(url, GetRelativeUrl(filePath));    // Replace url by the local file
                    }
                    catch (System.Net.WebException ex)
                    {
                        WriteLineToLogFile("DEBUG: ERROR: Could not download {0}    MESSAGE ==> {1}", absoluteUrl, ex.Message);
                    }
                }
                // In case this resource was already downloaded
                else
                {
                    cssCode.Replace(url, GetRelativeUrl(this.resourcesNames[url]));   // Replace url by the local file
                }
            }

            // Download all imported stylesheets
            foreach (string url in importsUrlList)
            {
                string absoluteUrl = GetAbsoluteUrl(currentUrl, url);

                // In case the stylesheet was not downloaded yet
                if (!this.IsDownloaded(absoluteUrl))
                {
                    try
                    {
                        string innerCssCode = this.webClient.DownloadString(url);

                        WriteLineToLogFile("DEBUG: Downloading inner stylesheet {0}", absoluteUrl);
                        this.DownloadCssRecursively(innerCssCode, absoluteUrl);     // Recursively download the inner css file

                        int previousId = cssId - 1;     // The id of the inner css resource
                        cssCode.Replace(url, CssDirectoryName + "/" + previousId.ToString());   // Replace the url by the local location of the stylesheet
                    }
                    catch (WebException ex)
                    {
                        this.WriteLineToLogFile("DEBUG: ERROR: Failed to download {0}  MESSAGE==>{1}", absoluteUrl, ex.Message);                        
                    }
                }
                // In case the stylesheet was already downloaded
                else
                {
                    cssCode.Replace(url, GetRelativeUrl(this.resourcesNames[absoluteUrl]));  // Replace the url by the local location of the stylesheet
                }

            }

            // Save the css file
            string finalCssFilePath = Path.Combine(this.CssPath, this.cssId.ToString());
            this.RegisterDownload(currentUrl, finalCssFilePath, Resource.Css);
            using (StreamWriter stream = new StreamWriter(finalCssFilePath))
                stream.Write(cssCode);
        }
        #endregion

        #region Protected Event Methods
        /// <summary>
        /// Invoking the StartedDownloading event subscribers.
        /// </summary>
        protected virtual void OnStartedDownloading() => this.StartedDownloading?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Invoking the FinishedDownloading event subscribers.
        /// </summary>
        protected virtual void OnFinishedDownloading() => this.FinishedDownloading?.Invoke(this, EventArgs.Empty);
        #endregion

        #region Debug utilities
        private string logFilePath;
        private StreamWriter logStream;
        /// <summary>
        /// Constructor for debugging using log file
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        /// <param name="logFilePath"></param>
        /// <param name="websiteName"></param>
        public WebpageDownloader(string url, string path, string websiteName = "Downloaded Website", string logFilePath="") : this(url, path, websiteName)
        {
            this.logFilePath = logFilePath;
            if (!File.Exists(logFilePath))
                this.logStream = new StreamWriter(File.Create(logFilePath));
            else
            {
                this.logStream = new StreamWriter(logFilePath);
            }
            this.FinishedDownloading += (object sender, EventArgs e) => logStream.Close();  // Release the file
        }

        /// <summary>
        /// Writes the input to log file
        /// </summary>
        /// <param name="input"></param>
        private void WriteLineToLogFile(string input, params object[] args)
        {
            if (!string.IsNullOrEmpty(this.logFilePath))
                logStream.WriteLine(string.Format(input, args));
        }
        #endregion
    }
}