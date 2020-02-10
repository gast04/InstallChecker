using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace AdwareScanner.Classes
{

    class RegEntry
    {
        public RegEntry()
        {
            keys = new List<string>();
        }

        public RegEntry(List<string> keys1)
        {
            keys = keys1;
        }

        public List<string> keys;

        public void addKey(string key)
        {
            keys.Add(key);
        }

        // TODO: values
    }


    class RegistryHandler
    {

        public bool before = true;

        private Dictionary<string, RegEntry> RegListBefore = new Dictionary<string, RegEntry>();
        private Dictionary<string, RegEntry> RegListAfter = new Dictionary<string, RegEntry>();

        private int enumerateSubKeys(string path)
        {
            int found_keys = 0;
            RegistryKey key = Registry.CurrentUser.OpenSubKey(path);
            foreach (var v in key.GetSubKeyNames())
            {
                // only log keys which have a value
                RegistryKey productKey = key.OpenSubKey(v);
                if (productKey != null)
                {
                    RegEntry regentry = new RegEntry();
                    foreach (var value in productKey.GetValueNames())
                    {
                        if (value == "")
                            regentry.addKey("(Default)");
                        else
                            regentry.addKey(value);
                    }

                    if (productKey.ValueCount > 0)
                    {
                        if (before)
                            RegListBefore.Add(path + "\\" + v, regentry);
                        else
                            RegListAfter.Add(path + "\\" + v, regentry);

                        found_keys += productKey.ValueCount;
                    }
                }

                found_keys += enumerateSubKeys(path + "\\" + v);
            }
            return found_keys;
        }

        public int enumerateRegistryKeys()
        {
            int found_keys = 0;
            RegistryKey key = Registry.CurrentUser.OpenSubKey("");
            foreach (var v in key.GetSubKeyNames())
            {
               found_keys += enumerateSubKeys(v);
            }
            return found_keys;
        }

        public void saveToFile(string filename, bool before=true)
        {
            // write log
            Dictionary<string, RegEntry> saveme = RegListBefore;
            if (!before)
                saveme = RegListAfter;

            using (StreamWriter file = new StreamWriter(filename))
            {
                foreach(KeyValuePair<string, RegEntry> kp in saveme)
                {
                    file.WriteLine(kp.Key);
                    foreach(string key in kp.Value.keys)
                    {
                        file.WriteLine("  " + key);
                    }
                }
            }
        }

        public void saveToFile(string filename, Dictionary<string, RegEntry> diff)
        {
            using (StreamWriter file = new StreamWriter(filename))
            {
                foreach (KeyValuePair<string, RegEntry> kp in diff)
                {
                    file.WriteLine(kp.Key);
                    foreach (string key in kp.Value.keys)
                    {
                        file.WriteLine("  " + key);
                    }
                }
            }
        }

        public int compareResults()
        {
            Dictionary<string, RegEntry> newEntries = new Dictionary<string, RegEntry>();

            int newkeys_cnt = 0;
            foreach( KeyValuePair<string, RegEntry> kp in RegListAfter)
            {
                // check if a new folder exists
                if(!RegListBefore.ContainsKey(kp.Key))
                {
                    Console.WriteLine("Found new Key: " + kp.Key);
                    newkeys_cnt += 1;
                    newEntries[kp.Key] = kp.Value;
                    continue;
                }

                // check if a new key is added
                List<string> before = RegListBefore[kp.Key].keys;
                List<string> newkeys = new List<string>();
                foreach(var key in kp.Value.keys)
                {
                    if (!before.Contains(key))
                    {
                        Console.WriteLine("Found new Key-Entry: " + key + " " + kp.Key);
                        newkeys.Add(key);
                        newkeys_cnt += 1;
                    }
                }

                if (newkeys.Count > 0)
                {
                    RegEntry newentry = new RegEntry(newkeys);
                    newEntries[kp.Key] = newentry;
                }

            }
            saveToFile(@"C:\Users\kurtn\source\repos\AdwareScanner\registrylist.txt", newEntries);
            return newkeys_cnt;
        }

    }
}
