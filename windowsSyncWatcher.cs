using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace windowsSyncWatcher{
    class SyncWatcher{

        static Dictionary<string, string> paths = new Dictionary<string, string>();

        static List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

        static void Main(string[] args){

            // CREATE DICTIONARY --- WINDOWS_PATH --> WSL PATH
            string[] lines = System.IO.File.ReadAllLines("paths.txt");
            foreach (string path in lines){
                string wsl_drive_format = "/mnt/" + char.ToLower(path[0]);
                string replacedSlashes = path.Replace('\\','/');
                string replaced_drive_letter = wsl_drive_format + replacedSlashes.Substring(1);
                string formatted_path = replaced_drive_letter.Replace(":", string.Empty);

                paths.Add(path, formatted_path);
            }

            Dictionary<string, string>.KeyCollection keys = paths.Keys;

            // CREATE FILE SYSTEM WATCHERS FOR EACH PATH IN paths.txt file
            int i = 0;
            foreach (string path in keys){
                FileSystemWatcher new_watcher = new FileSystemWatcher(path){
                    EnableRaisingEvents = true
                };
                watchers.Add(new_watcher);
                i += 1;
            }

            foreach (FileSystemWatcher watcher in watchers){
                watcher.Changed += OnChange;
                watcher.Created += OnChange;
                watcher.Deleted += OnChange;
                watcher.Renamed += OnRename;
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        private static void OnChange(object sender, FileSystemEventArgs e){
            callRsync();
        }

        private static void OnRename(object sender, RenamedEventArgs e){
            callRsync();
        }

        private static void callRsync(){
            string command = "wsl bash /home/brstrozy/windowsSync/windowsSync.bash";
            string output;

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.Arguments = command;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                output = process.StandardOutput.ReadToEnd();

                process.WaitForExit();
            }

            Console.WriteLine(output);
        }
        
    }
}