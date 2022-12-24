# WindowsSync
Script to sync contents of set directories with a destination (local or remote). 

- Uses System.IO.FileSystemWatcher to watch for changes in a specified directory or directories.
- On File System Event runs cwrsync to sync changes with local or remote destination.

- Plans to make this a background windows service with possible installer to automate/make solution portable.
