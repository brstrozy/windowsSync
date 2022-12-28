using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace windowsSyncBackup{
    class SyncBackup{

        static Dictionary<string, string> paths = new Dictionary<string, string>();

        static List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        static string rsync_command = "rsync -rlptoDxvh --delete --backup --backup-dir=/cygdrive/z/trash";

        static void Main(string[] args){

            string[] lines = System.IO.File.ReadAllLines("paths.txt");
            foreach (string path in lines){
                string rsync_drive_format = "/cygdrive/" + char.ToLower(path[0]);
                string replacedSlashes = path.Replace('\\','/');
                string replaced_drive_letter = rsync_drive_format + replacedSlashes.Substring(1);
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
            RunRsync(sender);
        }

        private static void OnRename(object sender, RenamedEventArgs e){
            RunRsync(sender);
        }

        private static void RunRsync(object sender){
            var watch_path = (FileSystemWatcher)sender;

            string src = paths[watch_path.Path];
            string dest = "/cygdrive/z/storage/desktop";
            string full_rsync_command = rsync_command + " " + src + " " + dest;
            // Console.WriteLine(full_rsync_command);

            // rsync_command + \"/cygdrive/d/tempdir\" \"/cygdrive/z/storage/desktop/temp\""
            var info = new ProcessStartInfo ( "cmd.exe" , "/K" +  full_rsync_command)
           {
                RedirectStandardOutput = true ,
                UseShellExecute = false ,
                CreateNoWindow = true
           } ;

            System.Diagnostics.Process proc = new System.Diagnostics.Process () ;
                           proc.StartInfo = info ;
                           proc.Start () ;
        }
        
    }
}