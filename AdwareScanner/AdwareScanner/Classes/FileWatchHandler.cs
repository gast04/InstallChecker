using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Security.Permissions;
using System.Threading;

namespace AdwareScanner.Classes
{
 

    class FileWatchHandler
    {
        private static bool RUNNING = true;
        private static string log = "";
        private string watchpath = "";

        public void Stop()
        {
            RUNNING = false;
        }

        public string GetLog()
        {
            string tmp = log;
            log = "";
            return tmp;
        }
        public string GetWatchpath()
        {
            return watchpath;
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Run(string watchpath)
        {
            this.watchpath = watchpath;

            // Create a new FileSystemWatcher and set its properties.
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                Console.WriteLine("Start: " + watchpath);
                watcher.Path = watchpath;

                // Watch for changes in LastAccess and LastWrite times, and
                // the renaming of files or directories.
                watcher.NotifyFilter = NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.FileName
                                     | NotifyFilters.DirectoryName;

                // Only watch text files
                watcher.Filter = "*.*";

                // Add event handlers.
                watcher.Changed += OnChanged;
                watcher.Created += OnChanged;
                watcher.Deleted += OnChanged;
                watcher.Renamed += OnRenamed;

                // Begin watching.
                watcher.EnableRaisingEvents = true;

                // Wait for the program to quit
                while (RUNNING) ;
            }
        }

        // Define the event handlers.
        private static void OnChanged(object source, FileSystemEventArgs e) =>
            // Specify what is done when a file is changed, created, or deleted.
            // Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            log += $"File: {e.Name} {e.FullPath} {e.ChangeType}\n";
            //ChangeHandle();

        private static void OnRenamed(object source, RenamedEventArgs e) =>
            // Specify what is done when a file is renamed.
            // Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
            log += $"File: {e.OldFullPath} renamed to {e.FullPath}\n";
    }
}
