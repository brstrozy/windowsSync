using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace windowsSync;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    static Dictionary<string, string> paths = new Dictionary<string, string>();

    static List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
    // --delete --backup --backup-dir=\"/cygdrive/z/trash\"
    static string rsync_command = "rsync -rlptoDxvh";

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

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

        while (!stoppingToken.IsCancellationRequested)
        {
            // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker Starting...");
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker Stopping...");
        return base.StopAsync(cancellationToken);
    }

    public void OnChange(object sender, FileSystemEventArgs e){
        _logger.LogInformation("OnChange before rsync call");
        RunRsync(sender);
        _logger.LogInformation("OnChange after rsync call");
    }

    public void OnRename(object sender, RenamedEventArgs e){
        _logger.LogInformation("OnRename before rsync call");
        RunRsync(sender);
        _logger.LogInformation("OnRename after rsync call");
    }

    public void RunRsync(object sender){
        var watch_path = (FileSystemWatcher)sender;

        string src = paths[watch_path.Path];
        string dest = "/cygdrive/z/storage/desktop";
        string full_rsync_command = "\"" + rsync_command + " " + src + " " + dest + "\"";
        _logger.LogInformation(full_rsync_command);

        // rsync_command + \"/cygdrive/d/tempdir\" \"/cygdrive/z/storage/desktop/temp\""
        var info = new ProcessStartInfo ( "cmd.exe" , "/K" + full_rsync_command)
        {
            RedirectStandardOutput = true ,
            UseShellExecute = false ,
            CreateNoWindow = true
        } ;

        System.Diagnostics.Process proc = new System.Diagnostics.Process () ;
                        proc.StartInfo = info ;
                        proc.Start () ;

        // System.Diagnostics.Process.Start("cmd.exe", "/K" + full_rsync_command);
    
    }
}
