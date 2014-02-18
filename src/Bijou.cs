using System; 
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Collections;
using System.Text.RegularExpressions;


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
	private static int Level = -1;

	private static StringBuilder SearchData = new StringBuilder();
	private static string SearchContentFolder = null;
	private static string SearchSiteFolder = null;
	private static string SearchTemplate = null;
	private static string SearchExtension = null;

	public static void CreateFolder(string folder){
		if(!Directory.Exists(folder)) {
			if (Verbose) Console.WriteLine("Creating "+folder);			
			Directory.CreateDirectory(folder);
		}
	}

	public static string StripPrefix(string contentPath) {
		StringBuilder result = new StringBuilder("/");
		contentPath = contentPath.Replace("./content", "");
		string[] tokens = contentPath.Split('/');
		foreach (string item in tokens) {
			if (!string.IsNullOrEmpty(item)) {
				result.AppendFormat("{0}/", StripPrefix(item, '.'));
			}
		}
		// result.Append("/");

 		if (Debug) Console.WriteLine("StripPath " + result);

		return result.ToString();
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

	public static string ParsePageTitle(string filename) {
		string result = filename;
		if (!string.IsNullOrEmpty(filename)) {
			string[] tokens = filename.Split('/');
			if (tokens.Length >= 2) {
				result = tokens[tokens.Length-2];
			}
		}
		if (result == "site") result = "";
		return ParseDisplayName(result);
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


	private static string BuildRelativePath(int level) {
		string result = "";
		if (level > 0){
			for(int i=0; i<level; i++){
				if (!string.IsNullOrEmpty(result)) result += "/";
				result += "..";
			}
		} else {
			result = ".";
		}
		return result;
	}


	private static string BuildRootPath(int level) {
		string result = string.Empty;
		if (Index) {
			result = BuildRelativePath(level);
		} else {
			result = WebRoot;
		}
		return result;
	}


	private static string BuildBreadcrumb() {
		StringBuilder result = new StringBuilder();
		if (Path.Count > 1) {
			result.Append("<ul>");
			foreach(string item in Path) {
				string[] tokens = item.Split('/');
				string last = (tokens.Length>0) ? tokens[tokens.Length-1] : "";
				string displayName = ParseDisplayName(last); 
				if (!string.IsNullOrEmpty(displayName)) {
					result.AppendFormat("<li><a href='{0}'>{1}</a></li>", item, displayName);
				}
			}
			result.Append("</ul>");
		}

		return result.ToString();
	}

	private static void WriteFile(string filename, string content) {
		using (StreamWriter sw = File.CreateText(filename)) {
	        sw.WriteLine(content);
	    }
	} 


	private static SubstitutionEngine GetSubstitutionEngine(string title, string content) {
		SubstitutionEngine result = new SubstitutionEngine();
		Breadcrumb = BuildBreadcrumb();
		DateTime now = DateTime.Now;
		result.Add("content", content);
		result.Add("title", title);
		result.Add("root", BuildRootPath(Level));
		result.Add("topnav", TopNav);
		result.Add("breadcrumb", Breadcrumb);
		result.Add("children", Children);
		result.Add("url", CurrentPageUrl);
		result.Add("date", string.Format("{0:yyyy/MM/dd}", now ));
		result.Add("time", string.Format("{0:hh}:{1:mm}", now, now ));
		result.Add("gmt", string.Format("{0:yyyyMMdd}T{1:hhmmss}Z", now, now));
		return result;
	}

	private static void WriteFile(string filename, string template, string content) {
		string title = ParsePageTitle(filename);
		using (StreamWriter sw = File.CreateText(filename)) {
			SubstitutionEngine se = GetSubstitutionEngine(title, content);
			if (SearchData.Length > 0) {
				se.Add("search", SearchData.ToString());
			}
			string text = se.Substitute(template);
	        sw.WriteLine(text);
	    }
	} 

	private static XsltArgumentList BuildXsltArgumentList(string title) {
		SubstitutionEngine se = GetSubstitutionEngine(title,"");
		return se.ToXsltArgumentList();
	}

	private static bool IsNavigation(string folder) {
		return folder.Contains(".");
	} 

	private static bool IsInvisible(string folder) {
		return folder.StartsWith("0.");
	} 

	private static void BuildLink(StringBuilder stream, string currentPath, DirectoryInfo di) {
		string stripped = StripPrefix(di.Name, '.');
		string displayName = ParseDisplayName(stripped); 
		string strippedPath = StripPrefix(currentPath);

		if (Index) {
			string root = BuildRelativePath(Level);
			stream.AppendFormat("<a href='{0}{1}index.html'>{2}</a>", root, strippedPath, displayName);
		} else if (WebRoot == "") {
			stream.AppendFormat("<a href='{0}'>{1}</a>", strippedPath, displayName);
		} else {
			stream.AppendFormat("<a href='{0}/{1}'>{2}</a>", WebRoot, strippedPath, displayName);					
		}
	}

	private static string BuildChildLinks(DirectoryInfo folder) {
		StringBuilder result = new StringBuilder();
		DirectoryInfo[] children = folder.GetDirectories();
		if ((children !=null) && (children.Length > 0)) {

			result.Append("<ul>");
			foreach(DirectoryInfo di in children) {
				if (IsNavigation(di.Name)  && !IsInvisible(di.Name)){
					string currentPath =  folder.Name + "/" + di.Name;

					//string stripped = StripPrefix(di.Name, '.');
					//string displayName = ParseDisplayName(stripped); 
					//string strippedPath = StripPrefix(currentPath);
					if (Debug) Console.WriteLine("BuildChildLinks "+ currentPath);
					result.Append("<li>");
					BuildLink(result, currentPath, di);
					/*if (Index) {
						result.AppendFormat("<a href='{0}/index.html'>{1}</a>", strippedPath, displayName);
					} else {
						result.AppendFormat("<a href='{0}'>{1}</a>", strippedPath, displayName);
					}*/
					result.Append("</li>");
				}
			}
			result.Append("</ul>");
		}
		return result.ToString();
	}


	private static void BuildTopNav(StringBuilder nav, string contentFolder, string path) {
		Level++;
		DirectoryInfo folder = new DirectoryInfo(contentFolder + "/" + path); 
		nav.Append("<ul>");
		if (Level==0 && Home) {
			string root = BuildRootPath(Level);
			if (Index) {
				nav.AppendFormat("<li><a href='{0}/index.html' class='icon-home'></a></li>", root);
			} else if (string.IsNullOrEmpty(WebRoot)) {
				nav.Append("<li><a href='/' class='icon-home'></a></li>");
			} else {
				nav.AppendFormat("<li><a href='{0}' class='icon-home'></a></li>", root);
			}
		}


		foreach(DirectoryInfo di in folder.GetDirectories()) {

			if (IsNavigation(di.Name) && !IsInvisible(di.Name)){
				string currentPath =  path + "/" + di.Name;
				if (Debug) Console.WriteLine("BuildTopNav "+ currentPath);
				nav.Append("<li>");

				BuildLink(nav, currentPath, di);

				// Check if we have children
				DirectoryInfo children = new DirectoryInfo(contentFolder+"/"+ path + "/" + di.Name);
				if ((children !=null) && (children.GetDirectories().Length > 0)) {
					string childPath = path + "/" + di.Name;
					if (Debug) Console.WriteLine("BuildTopNav "+ childPath + " has children");	
					BuildTopNav(nav, contentFolder, childPath);
				}
				nav.Append("</li>");
			}
		}
		nav.Append("</ul>");
		// return (nav.ToString());
		Level--;
	}

	private static void HtmlClone(string contentFolder, string siteFolder, string filename, string ext, string content) {
		string templateFile = BaseProcessor.GetTemplateFilename(filename.Replace(ext, ".html"), ".html");
		string siteFile = siteFolder + "/index.html";
		string template = File.ReadAllText(templateFile);

		WriteFile(siteFile, template, content);
	}

	/*********************************************
	 * Check to see if itls the seartch file. 
	 *********************************************/
	private static void TagSearchFile(string content,string contentFolder, string siteFolder, FileInfo fi) {
		if (content.Contains("{$search}")) {
			SearchContentFolder = contentFolder;
			SearchSiteFolder = siteFolder;
			SearchTemplate = fi.Name;
			SearchExtension = fi.Extension;
		}
	}


	private static void ApplyTemplate(string contentFolder, string siteFolder, FileInfo fi) {
		if (Verbose) Console.WriteLine("Processing "+ fi.Extension + " " + fi.Name);

		if (fi.Extension == ".html") {
			HtmlProcessor processor = new HtmlProcessor();
			processor.Consume(contentFolder, siteFolder, fi.Name, fi.Extension);
			WriteFile(processor.SiteFile, processor.Template, processor.Content);
			TagSearchFile(processor.Content, contentFolder, siteFolder, fi);
		} else if (fi.Extension == ".xml") {
			string title = ParsePageTitle(siteFolder+"/bogus.xxx");
			XmlProcessor processor = new XmlProcessor();
			processor.XslArgs = BuildXsltArgumentList(title);
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

				Children = BuildChildLinks(folder);
				CurrentPageUrl = folder.Name + "/";

				ApplyTemplate(contentFolder, siteFolder, fi);

				if (!IsInvisible(folder.Name)) AppendSearchData(contentFolder, siteFolder, fi.Name, fi.Extension);

			} else {
				string contentFile = contentFolder + "/" + fi.Name;
				string siteFile = siteFolder + "/" + fi.Name;

				File.Copy(contentFile, siteFile, true);
			}
	    }

		foreach(DirectoryInfo di in folder.GetDirectories()) {
			string stripped = StripPrefix(di.Name, '.');
			string childContent = contentFolder + "/" + di.Name; 
			string childSite = siteFolder + "/" + stripped; 

			CreateFolder(childSite);
			ProcessFolder(childContent, childSite);
		}
		Level--;
		Path.RemoveAt(Path.Count-1);

	}

	private static void AppendSearchData(string contentFolder, string siteFolder, string filename, string ext) {
		string contentFile = contentFolder + "/" + filename;
		//string templateFile = BuildTemplateFilename(filename, ext);
		//string siteFile = siteFolder + "/index.html";
		//string template = File.ReadAllText(templateFile);
		string content = File.ReadAllText(contentFile);

		siteFolder = siteFolder.Replace("."+SiteFolder, WebRoot); 
		string displayName = siteFolder;
		string cssClass = "";		
		if (string.IsNullOrEmpty(siteFolder)) {
 			siteFolder = "/";
 			displayName = "";
 			cssClass = " class='icon-home'";
		}

		content = Regex.Replace(content, @"<[^>]*>", String.Empty); // Remove HTML tags

		SearchData.AppendFormat("<li><a href='{0}' {1}>{2}</a><span class='hidden'>{3}</span></li>", siteFolder, cssClass, displayName, content);
	}

	public static void CreateSite() {
		string siteFolder = (SiteFolder == "/site") ? Folder+"/site" : SiteFolder;

		// CheckExtensions(Folder+"/content");

		TemplateTypes = ScanTemplateTypes();

		StringBuilder nav = new StringBuilder();

		CreateFolder(siteFolder);
		BuildTopNav(nav, Folder+"/content", "");

		TopNav = nav.ToString();

		ProcessFolder(Folder+"/content", siteFolder);

		if (!string.IsNullOrEmpty(SearchContentFolder) && (SearchData.Length >0)) {
			SearchProcessor processor = new SearchProcessor();
			processor.Consume(SearchContentFolder, SearchSiteFolder, SearchTemplate, SearchExtension);
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