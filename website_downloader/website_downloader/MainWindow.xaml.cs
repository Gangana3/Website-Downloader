using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        // Loads the 2 download images, one for when the download button is active, and the other for inactive
        private static BitmapImage downloadActive = new BitmapImage(new Uri("Images/download1.png", UriKind.Relative));
        private static BitmapImage downloadInactive = new BitmapImage(new Uri("Images/download2.png", UriKind.Relative));


        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Executes when the mouse enters the download image
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void DownloadImageMouseEnter(object sender, MouseEventArgs e)
        {
            // Show the active image of the download button
            this.downloadImage.Source = downloadActive;
        }

        /// <summary>
        /// Executes when the mouse leaves the download image
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void DownloadImageMouseLeave(object sender, MouseEventArgs e)
        {
            // Show the inactive image of the download button
            this.downloadImage.Source = downloadInactive;
        }
    }
}
