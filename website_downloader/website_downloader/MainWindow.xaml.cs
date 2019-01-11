using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace website_downloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string errorMessageColor = "#e80000";       // Color for error messages
        private static readonly string defaultMessageColor = "#1dc439";     // Color for regular messages

        public MainWindow()
        {
            InitializeComponent();

            
        }

        /// <summary>
        /// Executes when the client clicks the browse button.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">Event Args</param>
        private void BrowseLabelClick(object sender, MouseButtonEventArgs e)
        {
            // Open a folder dialog so the user can choose a folders
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.pathTextBox.Text = dialog.SelectedPath;
            }
        }


        /// <summary>
        /// Validate that all the fields are valid
        /// </summary>
        /// <returns>whther the fields are valid or not</returns>
        private bool ValidateFields()
        {
            // In case the user is not connected to the internet
            if (CheckInternetConnection() == false)
            {
                this.PromptMessage("You must connect to the internet in order to download a webpage!", errorMessageColor);
                return false;
            }
            // In case the path textbox is empty
            if (this.pathTextBox.Text == string.Empty)
            {
                this.PromptMessage("Path field cannot be empty!", errorMessageColor);
                return false;
            }
            // In case the given path does not exist
            if (!Directory.Exists(this.pathTextBox.Text))
            {
                this.PromptMessage("The given path does not exist!", errorMessageColor);
                return false;
            }
            // In case the url textbox is empty
            if (this.urlTextBox.Text == string.Empty)
            {
                this.PromptMessage("The URL field cannot be empty!", errorMessageColor);
                return false;
            }
            // In case the url textbox is not in valid format
            if (!IsUrl(this.urlTextBox.Text))
            {
                this.PromptMessage("The given URL is in an invalid format!", errorMessageColor);
                return false;
            }
            // In case host of the given url does not exist
            if (!HostExists(new Uri(this.urlTextBox.Text).Host))
            {
                
                this.PromptMessage("The given URL cannot be reached!", errorMessageColor);
                return false;
            }

            return true;    // All fields are valid
        }

        /// <summary>
        /// Prompts an error message to the screen
        /// </summary>
        private void PromptMessage(string message, string color)
        {
            this.promptLabel.Visibility = Visibility.Visible;
            var converter = new BrushConverter();
            Brush brush = (Brush)converter.ConvertFrom(color);

            // Write the message with the wanted color
            this.promptLabel.Content = message;
            this.promptLabel.Foreground = brush;
        }

        /// <summary>
        /// Executes when the user press the download button
        /// </summary>
        /// <param name="sender"> Event sender </param>
        /// <param name="e">Event args</param>
        private void DownloadImageClick(object sender, MouseButtonEventArgs e)
        {
            bool isValid = this.ValidateFields();

            if (isValid)
            {
                string host = new Uri(this.urlTextBox.Text).Host;
                string logFile = @"C:\Users\ganga\Desktop\output.log";
                var downloader = new WebsiteDownloader.WebpageDownloader(this.urlTextBox.Text, this.pathTextBox.Text, host, logFile);

                downloader.StartedDownloading += (object _sender, EventArgs _e) =>
                {
                // Show the loading animation, prompt a message, disable the download and browse buttons
                this.Dispatcher.Invoke(() =>
                    {
                        this.PromptMessage("Downloading...", defaultMessageColor);
                        this.loadingGif.Visibility = Visibility.Visible;
                        this.downloadImage.IsEnabled = false;
                        this.browseLabel.IsEnabled = false;
                    });
                };

                downloader.FinishedDownloading += (object _sender, EventArgs _e) =>
                {
                // Show the loading animation, prompt a message, disable the download and browse buttons
                this.Dispatcher.Invoke(() =>
                    {
                        this.PromptMessage("Downloaded Successfully!", defaultMessageColor);
                        this.downloadImage.IsEnabled = true;
                        this.browseLabel.IsEnabled = true;
                        this.loadingGif.Visibility = Visibility.Hidden;
                    });
                };

                Task task = new Task(downloader.Download);
                task.Start();   // Start downloading the webpage.
            }
        }

        /// <summary>
        /// Checks the internet connection
        /// </summary>
        /// <returns>Whether the user is connected or not</returns>
        public static bool CheckInternetConnection()
        {
            try
            {
                // Request this address for checking the internet connection
                string address = "http://clients3.google.com/generate_204"; 
                using (var client = new WebClient())
                using (client.OpenRead(address))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the host name exist
        /// </summary>
        /// <returns>whether the host exists or not</returns>
        private static bool HostExists(string host)
        {
            try
            {
                Dns.GetHostAddresses(host);
                return true;
            }
            catch (System.Net.Sockets.SocketException)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the given string is in url format
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static bool IsUrl(string url)
        {
            var regex = new Regex(@"https?://[-a-zA-Z0-9@:%_\+.~#?&//=]*");
            return regex.IsMatch(url);
        }
    }
}
