using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace windowsSyncBackup{
    class SyncBackup{

        //Array of paths, to add more paths simply add them to the array
        // static List<string> raw_paths = new List<string> {@"D:\tempdir",@"D:\dirtest",@"D:\testdir"};
        static Dictionary<string, string> paths = new Dictionary<string, string>();
        // static string[] paths = {@"D:\tempdir",@"D:\dirtest",@"D:\testdir"};
        //Array of FileSystemWatchers, make sure size matches paths
        // static FileSystemWatcher[] watchers = new FileSystemWatcher[paths.Count];
        static List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        static string rsync_command = "rsync -rlptoDxvh --delete --backup --backup-dir=/cygdrive/z/trash";

        static void Main(string[] args){
            // string current_directory = Directory.GetCurrentDirectory();
            // string file_text = System.IO.File.ReadAllText("paths.txt");
            string[] lines = System.IO.File.ReadAllLines("paths.txt");
            foreach (string path in lines){
                string rsync_drive_format = "/cygdrive/" + char.ToLower(path[0]);
                string slashes = path.Replace('\\','/');
                string replace_drive_letter = rsync_drive_format + slashes.Substring(1);
                string formatted_path = replace_drive_letter.Replace(":", string.Empty);
                // Console.WriteLine(path);
//                 Console.WriteLine(formatted_path);
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
            // for (int i = 0; i < paths.Count; ++i){
            //     watchers[i] = new FileSystemWatcher(paths[i]){
            //         EnableRaisingEvents = true
            //     };
            // }
            foreach (FileSystemWatcher watcher in watchers){
                watcher.Changed += OnChange;
                watcher.Created += OnChange;
                watcher.Deleted += OnChange;
                watcher.Renamed += OnRename;
            }
            // for (int i = 0; i < paths.Count; ++i){
            //     var watcher = watchers[i];
            //     watcher.Changed += OnChange;
            //     watcher.Created += OnChange;
            //     watcher.Deleted += OnChange;
            //     watcher.Renamed += OnRename;
            // }

            // Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        private static void OnChange(object sender, FileSystemEventArgs e){
            // Console.WriteLine("=== Some file change occurred ===");
            // Console.WriteLine(e.ChangeType);
            // Console.WriteLine(e.Name);
            var watch_path = (FileSystemWatcher)sender;
            // Console.WriteLine(watch_path.Path);

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
            // System.Diagnostics.Process.Start("cmd.exe", "/K rsync -rlptoDxvh --progress \"/cygdrive/d/tempdir\" \"/cygdrive/z/storage/desktop/temp\"");
            // Console.WriteLine("should have ran rsync");

        }

        private static void OnRename(object sender, RenamedEventArgs e){
            // Console.WriteLine("=== file name changed ===");
            // Console.WriteLine(e.OldName);
            // Console.WriteLine(e.Name);
            var watch_path = (FileSystemWatcher)sender;
            // Console.WriteLine(watch_path.Path);
        }
        
    }
}