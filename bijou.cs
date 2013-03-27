using System; 
using System.IO;
using System.Text;

public class Bijou { 

	public static bool Debug = true;
	public static string Folder = ".";

	public static void CreateFolder(string folder){
		if(!Directory.Exists(folder)) {
			if (Debug) Console.WriteLine("Creating "+folder);			
			Directory.CreateDirectory(folder);
		}
	}

	public static string StripPrefix(string text, char separator) {
		string result = text;
		if (!string.IsNullOrEmpty(text)) {
			string[] tokens = text.Split(separator);
			if (tokens.Length >= 2) {
				result = tokens[1];
			}
		}
		return result;
	}

	public static string ParseDisplayName(string text) {
		string result = text;

		if (!string.IsNullOrEmpty(text)) {
			StringBuilder sb = new StringBuilder();
			foreach(char c in text) {
				if (char.IsUpper(c) && (sb.Length > 0)) sb.Append(" ");
				sb.Append(c);
			}
			result = sb.ToString();
		}
		return result;
	}

	public static void CreateScafolding(){
		CreateFolder(Folder+"/content");
		CreateFolder(Folder+"/template");
		CreateFolder(Folder+"/site");
		/* CreateTemplateHeader();
		CreateTemplateFooter();
		CreateTemplateHeader();*/
	}

	private static void WriteFile(string filename, string template, string content, string topnav) {
		using (StreamWriter sw = File.CreateText(filename)) {
	        sw.WriteLine(template.Replace("{content}", content).Replace("{topnav}", topnav));
	    }
	} 

	public static void ProcessFolder(string contentFolder, string siteFolder) {
		DirectoryInfo folder = new DirectoryInfo(contentFolder); 

		StringBuilder nav = new StringBuilder();
		nav.Append("<ul>");
		foreach(DirectoryInfo di in folder.GetDirectories()) {
			string stripped = StripPrefix(di.Name, '.');
			string displayName = ParseDisplayName(stripped); 
			if (Debug) Console.WriteLine("Processing "+ stripped);
			nav.Append("<li>");
			nav.AppendFormat("<a href='{0}'>{1}</a>", stripped, displayName);
			nav.Append("</li>");
		}
		nav.Append("</ul>");

		foreach(FileInfo fi in folder.GetFiles("*.html")) {
			string stripped = fi.Name; // filename.Replace(contentFolder+"/","");
			//if (Debug) Console.WriteLine("Processing "+stripped);

			string templateFile = "template/"+stripped;
			string siteFile = siteFolder + "/index.html";
			if (File.Exists(templateFile)) {
				string template = File.ReadAllText(templateFile);
				string content = File.ReadAllText(contentFolder + "/" + stripped);
				WriteFile(siteFile, template, content, nav.ToString());
			}
	    }
		foreach(DirectoryInfo di in folder.GetDirectories()) {
			string stripped = StripPrefix(di.Name, '.');
			string childContent = contentFolder + "/" + di.Name; 
			string childSite = siteFolder + "/" + stripped; 

			CreateFolder(childSite);
			ProcessFolder(childContent, childSite);
		}
	}

	public static void CreateSite() {
		ProcessFolder(Folder+"/content", Folder+"/site");
	}


	public static void Usage(bool detailed) {
		Console.WriteLine("bijou [option]");
		Console.WriteLine("  -i to initialize the folder structure");
		Console.WriteLine("  -i to initialize the folder structure");
		if (detailed) {
			Console.WriteLine("bijou walks through the content folder and creates the html files based on the given template.");

		}
	}

	public static void Main(string[] args) {
	    try {
	    	bool createScafolding = false;
            foreach (string arg in args) {
                if (arg.StartsWith("-")) {
					if (args[0] == "-h") {
						Usage(true);
					} else if (args[0] == "-i") {
						createScafolding = true;
					} else {
						Console.WriteLine(string.Format("'{0}' option is not supported. Please use -h for help.", args[0] ));
					}
                } else {
                	Bijou.Folder = arg;
                }
            }

			if (createScafolding) CreateScafolding();
			CreateSite();

		} catch (Exception e) {
		    Console.WriteLine(e.ToString());
        }
	}
}