# WindowsSync
(Deprecated)
- Changed to model using cron/bash on WSL running recurring backups/syncs instead of on a file system event with C#/.Net


Script to sync contents of set directories with a destination (local or remote). 

- Uses System.IO.FileSystemWatcher to watch for changes in a specified directory or directories.
- On File System Event runs cwrsync to sync changes with local or remote destination.

- Plans to make this a background windows service with possible installer to automate/make solution portable.
