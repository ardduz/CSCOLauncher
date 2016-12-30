using System;
using System.IO;
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
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using System.Xml;

namespace CSCOlauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string LOCAL_VERSION = "undefined";
        string CSCO_FOLDER = "C:\\Program Files (x86)\\Steam\\steamapps\\sourcemods\\csco\\";
        public MainWindow()
        {
            InitializeComponent();
            refreshFeed();
            readLocalVersion();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            bool IsCSGORunning;
            IsCSGORunning = ProcessRunningCheck("csgo");
            if (IsCSGORunning == false)
            {
                statusText.Text = "";
                Process CSCO = new Process();
                CSCO.StartInfo.FileName = "steam://rungameid/14825029003206722266";
                CSCO.StartInfo.Arguments = "-novid";
                CSCO.Start();
                return;
            }
            else
            {
                statusText.Text = "csgo.exe already running!";
            }
            //System.Diagnostics.Process.Start("D:\\SteamLibrary\\steamapps\\common\\Counter-Strike Global Offensive\\csgo.exe");
        }
        private void homepageButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.moddb.com/mods/counter-strike-classic-offensive");
        }

        private void serverlistButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://onepointsix.org/");
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        public bool ProcessRunningCheck(string name)
        {
            //here we're going to get a list of all running processes on
            //the computer
            foreach (Process clsProcess in Process.GetProcesses())
            {
                //now we're going to see if any of the running processes
                //match the currently running processes. Be sure to not
                //add the .exe to the name you provide, i.e: NOTEPAD,
                //not NOTEPAD.EXE or false is always returned even if
                //notepad is running.
                //Remember, if you have the process running more than once, 
                //say IE open 4 times the loop thr way it is now will close all 4,
                //if you want it to just close the first one it finds
                //then add a return; after the Kill
                if (clsProcess.ProcessName.Contains(name))
                {
                    //if the process is found to be running then we
                    //return a true
                    return true;
                }
            }
            //otherwise we return a false
            return false;
        }

        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Counter-Strike: Classic Offensive by ZooL" + Environment.NewLine + "CS:CO Launcher by ardduz");
        }

        private void lstFeedItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstFeedItems.SelectedItem == null) return;
            SyndicationItem sItem = (SyndicationItem)lstFeedItems.SelectedItem;
            System.Diagnostics.Process.Start(sItem.Links[0].Uri.ToString());

        }

        private void refreshFeed()
        {
            try
            {
                using (XmlReader reader = XmlReader.Create("http://rss.moddb.com/mods/counter-strike-classic-offensive/articles/feed/rss.xml"))
                {
                    SyndicationFeed feed = SyndicationFeed.Load(reader);
                    lstFeedItems.ItemsSource = feed.Items;
                }

                using (XmlReader reader = XmlReader.Create("http://rss.moddb.com/mods/counter-strike-classic-offensive/downloads/feed/rss.xml"))
                {
                    SyndicationFeed rFeed = SyndicationFeed.Load(reader);
                    releaseFeedItems.ItemsSource = rFeed.Items;
                }
            }
            catch (System.Net.WebException)
            {
                MessageBoxResult result = MessageBox.Show("The news feed couldn't be fetched (System.Net.WebException)");
            }
        }

        private void readLocalVersion()
        {
            try
            {
                foreach (string line in File.ReadLines(@CSCO_FOLDER + "version.txt"))
                {
                    if (line.Contains("%"))
                    {
                        LOCAL_VERSION = line.Substring(1, line.Length - 1);
                    }
                }
                if (LOCAL_VERSION == "undefined")
                {
                    versionText.Foreground = Brushes.Red;
                    versionText.Text = "Couldn't check installed version. Please repair your installation";
                    return;
                }
                versionText.Foreground = Brushes.White;
                versionText.Text = string.Format("You are running version {0}", LOCAL_VERSION);
            }
            catch (System.Exception)
            {
                versionText.Foreground = Brushes.Red;
                versionText.Text = "Couldn't check installed version. Please repair your installation";
                return;
            }

        }

        private void releaseFeedItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (releaseFeedItems.SelectedItem == null) return;
            SyndicationItem rItem = (SyndicationItem)releaseFeedItems.SelectedItem;
            System.Diagnostics.Process.Start(rItem.Links[0].Uri.ToString());
        }

        private void downloadsButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.moddb.com/mods/counter-strike-classic-offensive/downloads");
        }
    }
}
