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

using AdwareScanner.Classes;

namespace AdwareScanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private RegistryHandler reghandler;
        private FileWatchWrapper watchhandler;

        public MainWindow()
        {
            InitializeComponent();

            watchhandler = new FileWatchWrapper();

            btn_scanTwo.IsEnabled = false;
            btn_InstallDone.IsEnabled = false;
        }

        private void btn_scanOne_Click(object sender, RoutedEventArgs e)
        {
            txtLog.Text = "Collecting Registry Entries\n";

            reghandler = new RegistryHandler();
            int num_keys = reghandler.enumerateRegistryKeys();
            
            txtLog.Text += "Save " + num_keys.ToString() + " Registry Entries to File\n";
            // reghandler.saveToFile(@"C:\Users\kurtn\source\repos\AdwareScanner\registrylist.txt");

            txtLog.Text += "Registry Clone done\n\n";


            txtLog.Text += "Start FileWatcher\n";
            // start file watcher
            List<string> watchlist = new List<string>();
            watchlist.Add(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            watchlist.Add(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            watchlist.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            watchlist.Add("C:\\Program Files (x86)");
            watchlist.Add("C:\\Program Files");

            /*
            watchlist.Add(Environment.GetFolderPath(Environment.SpecialFolder.Windows));
            watchlist.Add(Environment.GetFolderPath(Environment.SpecialFolder.System));
            watchlist.Add(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86));
            */

            foreach (string watchpoint in watchlist)
            {
                watchhandler.AddFileWatch(watchpoint);
                txtLog.Text += "  " + watchpoint + "\n";
            }

            btn_InstallDone.IsEnabled = true;
        }


        private void btn_scanTwo_Click(object sender, RoutedEventArgs e)
        {
            // after registry colletion
            txtLog.Text += "\nRegistry After Clone\n";
            reghandler.before = false;  // enable after check
            int num_keys2 = reghandler.enumerateRegistryKeys();
            txtLog.Text += "Save " + num_keys2.ToString() + " Registry Entries to File\n";

            txtLog.Text += "Compare Results\n";
            int newkeys = reghandler.compareResults();
            txtLog.Text += "Discovered " + newkeys.ToString() + " new Keys";
        }

        private void btn_startFileWatch_Click(object sender, RoutedEventArgs e)
        {
            List<string> watchlist = new List<string>();

            string userhome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            
            watchlist.Add(userhome);
            watchlist.Add(appdata);
            watchlist.Add(documents);
            watchlist.Add("C:\\Program Files (x86)");
            watchlist.Add("C:\\Program Files");

            /*
            watchlist.Add(Environment.GetFolderPath(Environment.SpecialFolder.Windows));
            watchlist.Add(Environment.GetFolderPath(Environment.SpecialFolder.System));
            watchlist.Add(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86));
            */

            foreach (string watchpoint in watchlist)
            {
                watchhandler.AddFileWatch(watchpoint);
            }

            btn_InstallDone.IsEnabled = true;
        }

        private void btn_InstallDone_Click(object sender, RoutedEventArgs e)
        { 
            string log = watchhandler.killAndGetLogs();
            txtLog.Text += "FileWatcher stopped\n";

            txtLog.Text += "Changed Files:\n" + log;
            btn_scanTwo.IsEnabled = true;
        }
    }
}
