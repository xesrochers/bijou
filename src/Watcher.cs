using System;
using System.IO;

public class Watcher {

    public void Start() {
        Console.WriteLine("Watcher is started. Monitoring changes to content and template folders");
        FileSystemWatcher content = new FileSystemWatcher();
        content.Path = "content";

        FileSystemWatcher template = new FileSystemWatcher();
        template.Path = "template";

        WireUp(content);
        WireUp(template);
    
        do {
            // just watch!
        } while (true);  
 
    }

    private void WireUp(FileSystemWatcher watcher) {
        //watch.NotifyFilter = /*NotifyFilters.LastAccess | */ NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
        // Only watch text files. 
        //watch.Filter = "*.txt"; 

        watcher.Changed += new FileSystemEventHandler(OnChanged);
        watcher.Created += new FileSystemEventHandler(OnChanged);
        watcher.Deleted += new FileSystemEventHandler(OnChanged);
        watcher.Renamed += new RenamedEventHandler(OnChanged); 
        watcher.EnableRaisingEvents = true;        
    }

    private void OnChanged(object source, FileSystemEventArgs e) {
        Bijou.CreateSite();
    }

}