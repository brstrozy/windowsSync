using System;
using System.IO;
using System.Threading.Tasks;


namespace windowsSyncBackup{
    class SyncBackup{

        //Array of paths, to add more paths simply add them to the array
        static string[] paths = {@"D:\tempdir",@"D:\dirtest",@"D:\testdir"};
        //Array of FileSystemWatchers, make sure size matches paths
        static FileSystemWatcher[] watchers = new FileSystemWatcher[paths.Length];

        static void Main(string[] args){
            for (int i = 0; i < paths.Length; ++i){
                watchers[i] = new FileSystemWatcher(paths[i]){
                    EnableRaisingEvents = true
                };
            }
            for (int i = 0; i < paths.Length; ++i){
                var watcher = watchers[i];
                watcher.Changed += OnChange;
                watcher.Created += OnChange;
                watcher.Deleted += OnChange;
                watcher.Renamed += OnRename;
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        private static void OnChange(object sender, FileSystemEventArgs e){
            Console.WriteLine("=== Some file change occurred ===");
            Console.WriteLine(e.ChangeType);
            Console.WriteLine(e.Name);
            var watch_path = (FileSystemWatcher)sender;
            Console.WriteLine(watch_path.Path);

        }

        private static void OnRename(object sender, RenamedEventArgs e){
            Console.WriteLine("=== file name changed ===");
            Console.WriteLine(e.OldName);
            Console.WriteLine(e.Name);
            var watch_path = (FileSystemWatcher)sender;
            Console.WriteLine(watch_path.Path);
        }
        
    }
}