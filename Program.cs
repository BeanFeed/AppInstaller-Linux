using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
namespace AppInstaller;

class Program {
    private static List<string> defDesk = new List<string>() {
        "[Desktop Entry]",
        "Name=",
        "Comment=",
        "GenericName=App",
        "Keywords=",
        "Exec=",
        "Terminal=false",
        "Type=Application",
        "Icon="

    };
    static void Main(string[] args) {
        if(!Environment.OSVersion.ToString().Contains("Unix")) { Console.WriteLine("This OS isn't supported. This program was made for Unix and the Linux File System."); return; }
        string envPath = Environment.CurrentDirectory;
        string arg = args.Length != 0 ? args[0] : "";
        if(arg == "" || arg == "-h" || arg == "--help") {
            Console.WriteLine("AppInstaller <Path To Executable>");
            return;
        }
        Console.Write("Name of Application: ");
        string name = Console.ReadLine();
        Console.Write("Comment for Application(Optional): ");
        string comment = Console.ReadLine();
        Console.Write("Keywords(Optional) (Separate with ;)");
        string keywords = Console.ReadLine();
        Console.Write("This will install the program to /usr/share/" + name + ". Would you like to change the install location [y/N]: ");
        string installLoc = "/usr/share/" + name;
        if(Console.ReadLine() == "y") {
            do {
                
                Console.Write("Path To New Location That Exists[/usr/share/" + name + "]: ");
            } while(installLoc != "" || !Directory.Exists(installLoc));
        }
        string icon = "";
        Console.Write("Would You Like To Add an Icon [y/N]: ");
        if(Console.ReadLine().ToLower() == "y") {
            Console.Write("Path to Icon(Optional): ");
            icon = Console.ReadLine();
        }
        if(!File.Exists(icon)) icon = "";
        if(installLoc == "/usr/share/" + name && !Directory.Exists(installLoc)) {
            Directory.CreateDirectory(installLoc);
        }
        else if(!Directory.Exists(installLoc)) {
            Console.WriteLine("Failed To Install. The Directory: " + installLoc + " Does Not Exist.");
            return;
        }
        defDesk[1] += name;
        defDesk[2] += comment;
        defDesk[4] += keywords;
        defDesk[5] += Path.Join(installLoc, name);
        if(icon != "") defDesk[8] += Path.Join(installLoc, GetFileName(icon));
        var file = File.Create(Path.Join(installLoc, name + ".desktop"));
        file.Close();
        File.WriteAllLines(Path.Join(installLoc, name + ".desktop"), defDesk.ToArray<string>());
        var ln = new Process();
        try
        {
            ln.StartInfo.FileName = "ln";
            ln.StartInfo.ArgumentList.Add("-s");
            ln.StartInfo.ArgumentList.Add(Path.Join(installLoc, name + ".desktop"));
            ln.StartInfo.ArgumentList.Add("/usr/share/applications/" + name + ".desktop");
            ln.StartInfo.UseShellExecute = true;
            ln.Start();
        } catch(err) {Console.WriteLine(name + ".desktop Is Already In /usr/share/applications");}
        File.Move(arg, Path.Join(installLoc, arg));
        try 
        {
            ln = new Process();
            ln.StartInfo.FileName = "ln";
            ln.StartInfo.ArgumentList.Add("-s");
            ln.StartInfo.ArgumentList.Add(Path.Join(installLoc, arg));
            ln.StartInfo.ArgumentList.Add("/usr/bin/" + arg);
            ln.StartInfo.UseShellExecute = true;
            ln.Start();
        }
        catch(err){Console.WriteLine(arg + " Is Already In /usr/bin");}
        
        File.Copy(icon, Path.Join(installLoc, GetFileName(icon)));
        
    }

    private static string GetFileName(string path) {
        //Console.WriteLine("Path: " + path);
        if(path.Contains('/')) {
            int lastSIndex = path.Length - 1;
            for(int i = path.Length - 1; path[i] != '/'; i--) {
                lastSIndex = i;
            }
            string name = "";
            for(int i = lastSIndex; i < path.Length; i++) {
                name += path[i];
            }
            //Console.WriteLine(name);
            return name;
        }else return path;
    }
}