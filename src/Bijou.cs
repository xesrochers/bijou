using System; 
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Collections;


/**************************************************
 * <summary>
 * Man-O-Man... all static methods... this is not
 * meant for public viewing... ha! well... 
 * </summary>
 *************************************************/
public class Bijou { 
	public static bool Debug = false;
	public static bool Index = false;
	public static bool Verbose = false;
	public static bool Watcher = false;
	public static bool Home = false;
	// public static bool HasSearch = false;
	public static string Folder = ".";
	public static string TopNav = "";
	public static string Breadcrumb = "";
	public static string SiteFolder = "/site";
	public static string WebRoot = "";
	public static string Children = "";
	public static string CurrentPageUrl = "/";
	public static string[] TemplateTypes = null;
	public static ArrayList Path = new ArrayList();
	public static int Level = -1;

	public static void CreateFolder(string folder){
		if(!Directory.Exists(folder)) {
			if (Verbose) Console.WriteLine("Creating "+folder);			
			Directory.CreateDirectory(folder);
		}
	}

	public static void CreateScafolding(){
		CreateFolder(Folder+"/content");
		CreateFolder(Folder+"/template");
		//CreateFolder(Folder+"/site");
		/* CreateTemplateHeader();
		CreateTemplateFooter();
		CreateTemplateHeader();*/
	}

	public static string[] ScanTemplateTypes() {
		ArrayList result = new ArrayList();

		DirectoryInfo folder = new DirectoryInfo(Folder + "/template"); 
		foreach(FileInfo fi in folder.GetFiles("*")) {
			result.Add(fi.Extension);
		}
		return (string[]) result.ToArray( typeof( string ) );
	}

	private static void WriteFile(string filename, string content) {
		using (StreamWriter sw = File.CreateText(filename)) {
	        sw.WriteLine(content);
	    }
	} 

	private static void WriteFile(string filename, string template, string content) {
		string title = BijouUtils.ParsePageTitle(filename);
		using (StreamWriter sw = File.CreateText(filename)) {
			SubstitutionEngine se = SubstitutionEngine.GetSubstitutionEngine(title, content);
			if (SearchProcessor.SearchData.Length > 0) {
				se.Add("search", SearchProcessor.SearchData.ToString());
			}
			string text = se.Substitute(template);
	        sw.WriteLine(text);
	    }
	} 

	private static void HtmlClone(string contentFolder, string siteFolder, string filename, string ext, string content) {
		string templateFile = BaseProcessor.GetTemplateFilename(filename.Replace(ext, ".html"), ".html");
		string siteFile = siteFolder + "/index.html";
		string template = BijouUtils.SharedRead(templateFile);
		WriteFile(siteFile, template, content);
	}


	private static void ApplyTemplate(string contentFolder, string siteFolder, FileInfo fi) {
		if (Verbose) Console.WriteLine("Processing "+ fi.Extension + " " + fi.Name);

		if (fi.Extension == ".html") {
			HtmlProcessor processor = new HtmlProcessor();
			processor.Consume(contentFolder, siteFolder, fi.Name, fi.Extension);
			WriteFile(processor.SiteFile, processor.Template, processor.Content);
			SearchProcessor.TagSearchFile(processor.Content, contentFolder, siteFolder, fi);
		} else if (fi.Extension == ".xml") {
			string title = BijouUtils.ParsePageTitle(siteFolder+"/bogus.xxx");
			XmlProcessor processor = new XmlProcessor();
			processor.XslArgs = XmlProcessor.BuildXsltArgumentList(title);
			processor.Consume(contentFolder, siteFolder, fi.Name, fi.Extension);
			WriteFile(processor.SiteFile, processor.Content);
		} else if (fi.Extension == ".csv") {
			CsvProcessor processor = new CsvProcessor();
			processor.Consume(contentFolder, siteFolder, fi.Name, fi.Extension);
			WriteFile(processor.SiteFile, processor.Template, processor.Content);
		} else if (fi.Extension == ".md") {
			MdProcessor processor = new MdProcessor();
			processor.Consume(contentFolder, siteFolder, fi.Name, fi.Extension);
			WriteFile(processor.SiteFile, processor.Template, processor.Content);
		} else if (fi.Extension == ".rss") {
			RssProcessor processor = new RssProcessor();
			processor.Consume(contentFolder, siteFolder, fi.Name, fi.Extension);
			WriteFile(processor.SiteFile, processor.Template, processor.Content);
			HtmlClone(contentFolder, siteFolder, fi.Name, fi.Extension, processor.Clone);
		} else if (fi.Extension == ".ics") {
			IcsProcessor processor = new IcsProcessor();
			processor.Consume(contentFolder, siteFolder, fi.Name, fi.Extension);
			WriteFile(processor.SiteFile, processor.Template, processor.Content);
			HtmlClone(contentFolder, siteFolder, fi.Name, fi.Extension, processor.Clone);
		}
	}

	private static void ProcessFolder(string contentFolder, string siteFolder) {
		DirectoryInfo folder = new DirectoryInfo(contentFolder); 
		Path.Add(siteFolder.Replace("."+SiteFolder, WebRoot));
		Level++;
		foreach(FileInfo fi in folder.GetFiles("*")) {
			string templateFile = BaseProcessor.GetTemplateFilename(fi.Name, fi.Extension);

			if (!string.IsNullOrEmpty(templateFile)) {

				Children = MenuBuilder.BuildChildLinks(folder);
				CurrentPageUrl = folder.Name + "/";

				ApplyTemplate(contentFolder, siteFolder, fi);

				if (!BijouUtils.IsInvisible(folder.Name)) SearchProcessor.AppendSearchData(contentFolder, siteFolder, fi.Name, fi.Extension);

			} else {
				string contentFile = contentFolder + "/" + fi.Name;
				string siteFile = siteFolder + "/" + fi.Name;

				File.Copy(contentFile, siteFile, true);
			}
	    }

		foreach(DirectoryInfo di in folder.GetDirectories()) {
			string stripped = BijouUtils.StripPrefix(di.Name, '.');
			string childContent = contentFolder + "/" + di.Name; 
			string childSite = siteFolder + "/" + stripped; 

			CreateFolder(childSite);
			ProcessFolder(childContent, childSite);
		}
		Level--;
		Path.RemoveAt(Path.Count-1);

	}

	public static void CreateSite() {
		string siteFolder = (SiteFolder == "/site") ? Folder+"/site" : SiteFolder;

		// CheckExtensions(Folder+"/content");

		TemplateTypes = ScanTemplateTypes();

		StringBuilder nav = new StringBuilder();

		CreateFolder(siteFolder);
		MenuBuilder.BuildTopNav(nav, Folder+"/content", "");

		TopNav = nav.ToString();

		ProcessFolder(Folder+"/content", siteFolder);

		if (SearchProcessor.HasSearchData()) {
			SearchProcessor processor = new SearchProcessor();
			processor.Consume();
			WriteFile(processor.SiteFile, processor.Template, processor.Content);
		}
	}


	public static void Usage(bool detailed) {
		Console.WriteLine("bijou [option] ");
		Console.WriteLine("  -c to clear out the existing 'site' folder");
		Console.WriteLine("  -i to activate index.html based urls (NOT IMPLEMENTED)");
		Console.WriteLine("  -m to inject the home icon");
		// Console.WriteLine("  -s to inject the search page");
		Console.WriteLine("  -o:path to change the output folder");
		Console.WriteLine("  -r:path to change the root folder");
		Console.WriteLine("  -w to start the file system watcher");
		Console.WriteLine("  -v verbose");
		Console.WriteLine("  -d debug");
		if (detailed) {
			Console.WriteLine("Bijou walks through the content folder and creates the html files based on the given template.");
		}
	}

	public static void Main(string[] args) {
	    try {
	    	bool scalfolding = false;
            foreach (string arg in args) {
                if (arg.StartsWith("-")) {
					if (arg == "-h") {
						Usage(true);
					} else if (arg == "-v") {
						Bijou.Verbose = true;
					} else if (arg == "-w") {
						Bijou.Watcher = true;
					} else if (arg == "-d") {
						Bijou.Debug = true;
					} else if (arg == "-i") {
						Bijou.Index = true;
					} else if (arg == "-m") {
						Bijou.Home = true;
					} else if (arg == "-s") {
						scalfolding = true;
					} else if (arg.StartsWith("-o")) {
						string[] tokens = arg.Split(':');
						if (tokens.Length == 2) {
							if (tokens[1] == "content" || tokens[1] == "template") {
								Bijou.SiteFolder = tokens[1];
							} else {
								Console.WriteLine("Bijou Reserved Folder: Please specify a folder name that is not 'content' or 'template'.");
							}
						} else {
							Console.WriteLine("Please specify folder name along with -o option (using -o:path format).");
						}
					} else if (arg.StartsWith("-r")) {
						string[] tokens = arg.Split(':');
						if (tokens.Length == 2) {
							Bijou.WebRoot = "/" + tokens[1];
						} else {
							Console.WriteLine("Please specify folder name along with -r option (using -o:path format).");
						}
					} else {
						Console.WriteLine(string.Format("'{0}' option is not supported. Please use -h for help.", args[0] ));
					}
                } else {
                	Bijou.Folder = arg;
                }
            }

			// System.Diagnostics.Debugger.Break();

            if (scalfolding) CreateScafolding();
            
			CreateSite();

			if (Bijou.Watcher) {
				//Bijou.StartWatcher();
				Watcher watcher = new Watcher();
				watcher.Start();
			}

		} catch (Exception e) {
		    Console.WriteLine(e.ToString());
        }
	}
}