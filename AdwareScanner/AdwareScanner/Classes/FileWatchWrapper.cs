using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;

namespace AdwareScanner.Classes
{
    class PARAM
    {
        public string watchpath = "";
        public string log = "";
        public FileWatchHandler filewatcher;
        public PARAM(string watchp)
        {
            watchpath = watchp;
        }
    }

    class FileWatchWrapper
    {
        private List<PARAM> paramList = new List<PARAM>();

        public void AddFileWatch(string watchpath)
        {
            PARAM param = new PARAM(watchpath);
            paramList.Add(param);

            Thread t = new Thread(Run);
            t.Start(param);
        }

        public string killAndGetLogs()
        {
            string log = "";
            foreach(PARAM param in paramList)
            {
                param.filewatcher.Stop();

                log += "\n";
                log += param.filewatcher.GetWatchpath() + "\n";
                log += param.filewatcher.GetLog();
            }

            return log;
        }

        private static void Run(object param)
        {
            PARAM arg = (PARAM)param;
            arg.filewatcher = new FileWatchHandler();
            arg.filewatcher.Run(arg.watchpath);
        }
      
    }
}
