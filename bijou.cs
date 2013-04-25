using System; 
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

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
	public static bool Home = false;
	public static string Folder = ".";
	public static string TopNav = "";
	public static string Breadcrumb = "";
	public static string SiteFolder = "/site";
	public static string WebRoot = "";
	public static string Children = "";
	// private static int Level = 0;

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

	public static void CreateScafolding(){
		CreateFolder(Folder+"/content");
		CreateFolder(Folder+"/template");
		//CreateFolder(Folder+"/site");
		/* CreateTemplateHeader();
		CreateTemplateFooter();
		CreateTemplateHeader();*/
	}

	private static void WriteFile(string filename, string template, string content) {

		using (StreamWriter sw = File.CreateText(filename)) {
			string text = "";
			text = template.Replace("{$content}", content);
			text = text.Replace("{$topnav}", TopNav);
			text = text.Replace("{$breadcrumb}", Breadcrumb);
			text = text.Replace("{$children}", Children);
			text = text.Replace("{$root}", WebRoot);
	        sw.WriteLine(text);
	    }
	} 

	private static bool IsNavigation(string folder) {
		return folder.Contains(".");
	} 

	private static bool IsInvisible(string folder) {
		return folder.StartsWith("0.");
	} 

	private static bool IsTemplateDriven(string filename) {
		return File.Exists("template/"+filename);
	} 

	private static void BuildLink(StringBuilder stream, string currentPath, DirectoryInfo di) {
		string stripped = StripPrefix(di.Name, '.');
		string displayName = ParseDisplayName(stripped); 
		string strippedPath = StripPrefix(currentPath);
		if (Index) {
			stream.AppendFormat("<a href='{0}/index.html'>{1}</a>", strippedPath, displayName);
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
		DirectoryInfo folder = new DirectoryInfo(contentFolder + "/" + path); 

		nav.Append("<ul>");
		if (path=="" && Home) {
			nav.AppendFormat("<li><a href='{0}' class='icon-home'></a></li>", WebRoot);
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
	}


	private static void ProcessHtmlFile(string contentFolder, string siteFolder, string filename) {
		string contentFile = contentFolder + "/" + filename;
		string templateFile = "template/" + filename;
		string siteFile = siteFolder + "/index.html";
		string template = File.ReadAllText(templateFile);
		string content = File.ReadAllText(contentFile);
		WriteFile(siteFile, template, content);
	}

	private static void ProcessXmlFile(string contentFolder, string siteFolder, string filename) {
		string contentFile = contentFolder + "/" + filename;
		string templateFile = "template/" + filename;
		string siteFile = siteFolder + "/index.html";

		XsltArgumentList xslArg = new XsltArgumentList();
        xslArg.AddParam("root", "", WebRoot);
        xslArg.AddParam("topnav", "", TopNav);
        xslArg.AddParam("breadcrumb", "", Breadcrumb);
        xslArg.AddParam("children", "", Children);

        XslCompiledTransform xslt = new XslCompiledTransform();
        xslt.Load(templateFile); 
        using (XmlWriter writer = XmlWriter.Create(siteFile)) {
            xslt.Transform(contentFile, xslArg, writer);
        }

	}

	private static void ProcessCsvFile(string contentFolder, string siteFolder, string filename) {
		string contentFile = contentFolder + "/" + filename;
		string templateFile = "template/" + filename;
		string siteFile = siteFolder + "/index.html";
		string template = File.ReadAllText(templateFile);
	    StringBuilder sb = new StringBuilder();
		using (StreamReader sr = new StreamReader(contentFile)) {
		    string line;
		    string tag = "th";
	    	sb.Append("<table>");
		    while ((line = sr.ReadLine()) != null) {
		    	if (line.StartsWith("#")) {
			    	sb.AppendFormat("<caption>{0}</caption>", line.Replace("#", "").Trim());
		    	} else {
			    	string[] tokens = line.Split(',');
			    	sb.Append("<tr>");
			    	for (int i =0 ; i< tokens.Length; i++){
				    	sb.AppendFormat("<{0}>{1}</{0}>", tag, tokens[i]);
			    	}

			    	sb.Append("</tr>");
			    	tag = "td";
			    }
		    }
	    	sb.Append("</table>");
		}
		string content = sb.ToString();
		WriteFile(siteFile, template, content);
	}


  	/*
  	  	<item>
	      	<title>My First Article</title>
	      	<pubDate>Tue, 03 Jun 2003 09:39:21 GMT</pubDate>
	      	<description>It's my first article. Hello World!</description>
    	</item>
	*/
	private const int RSS_START = 0;
	private const int RSS_TITLE = 1;
	private const int RSS_SKIP  = 2;
	private const int RSS_DATE  = 3;
	private const int RSS_DESC  = 4;

	private static void ProcessRssFile(string contentFolder, string siteFolder, string filename) {
		string contentFile = contentFolder + "/" + filename;
		string templateFile = "template/" + filename;
		string siteFile = siteFolder + "/index.rss";
		string template = File.ReadAllText(templateFile);
	    StringBuilder sb = new StringBuilder();
		using (StreamReader sr = new StreamReader(contentFile)) {
		    string line;
		    int state = RSS_START;
		    while ((line = sr.ReadLine()) != null) {
	    		switch (state) {
	    			case RSS_START:
				    	if (line.StartsWith("---")) {
					    	sb.Append("<item>");
					    	state = RSS_TITLE;
					    }
				    	break;
	    			case RSS_TITLE:
					    	sb.AppendFormat("<title>{0}</title>", line);
					    	state = RSS_SKIP;
				    	break;
	    			case RSS_SKIP:
					    	state = RSS_DATE;
				    	break;
	    			case RSS_DATE:
					    	sb.AppendFormat("<pubDate>{0}</pubDate>\n<description>", line);
					    	state = RSS_DESC;
				    	break;
	    			case RSS_DESC:
				    	if (line.StartsWith("---")) {
				    		sb.Append("</description>\n</item>\n<item>");
				    		state = RSS_TITLE;
				    	} else {
					    	sb.Append(line);
					    }
				    	break;
		    		
		    	}
		    }
		}
		if (sb.Length > 0) sb.Append("</description>\n</item>");

		string content = sb.ToString();
		WriteFile(siteFile, template, content);
	}

	/*
	BEGIN:VEVENT
DTSTART:20110331T230000Z
DTEND:20110401T010000Z
DTSTAMP:20130413T040110Z
UID:CoachClinicsYouth01
CREATED:19000101T120000Z
DESCRIPTION:
LAST-MODIFIED:20130413T034716Z
LOCATION:Chelsea Community Center
SEQUENCE:0
STATUS:CONFIRMED
SUMMARY:Formation jeunes entraîneurs - Session 1
TRANSP:OPAQUE
END:VEVENT
*/

	private const int ICS_START    = 0;
	private const int ICS_TITLE    = 1;
	private const int ICS_LOCATION = 2;
	private const int ICS_DATE     = 3;
	private const int ICS_ID       = 4;
	private const int ICS_SKIP     = 5;
	private const int ICS_DESC     = 6;

	private static void StartIcsEvent(StringBuilder sb) {
		DateTime now = DateTime.Now;
		string date = string.Format("{0:yyyyMMdd}", now); // "20130413T034716"; // FIX THIS!
		string time = string.Format("{0:hhmmss}", now); // "20130413T034716"; // FIX THIS!
		sb.Append("BEGIN:VEVENT\n");
		sb.Append("CREATED:19000101T120000Z\n");
		sb.Append("SEQUENCE:0\n");
		sb.Append("STATUS:CONFIRMED\n");
		sb.AppendFormat("DTSTAMP:{0}T{1}Z\n", date,time);
		sb.AppendFormat("LAST-MODIFIED:{0}T{1}Z\n", date,time);
	}

	private static void ProcessIcsFile(string contentFolder, string siteFolder, string filename) {
		string contentFile = contentFolder + "/" + filename;
		string templateFile = "template/" + filename;
		string siteFile = siteFolder + "/index.ics";
		string template = File.ReadAllText(templateFile);
	    StringBuilder sb = new StringBuilder();
		using (StreamReader sr = new StreamReader(contentFile)) {
		    string line;
		    int state = ICS_START;
		    while ((line = sr.ReadLine()) != null) {
	    		switch (state) {
	    			case ICS_START:
				    	if (line.StartsWith("---")) {
				    		StartIcsEvent(sb);
					    	state = RSS_TITLE;
					    }
				    	break;
	    			case ICS_TITLE:
					    	sb.AppendFormat("SUMMARY:{0}\n", line);
					    	state = ICS_LOCATION;
				    	break;
	    			case ICS_LOCATION:
					    	sb.AppendFormat("LOCATION:{0}\n", line);
					    	state = ICS_DATE;
				    	break;
	    			case ICS_DATE:
	    					if (line.Contains(":")) {
	    						string[] date = line.Replace("(","").Replace(")","").Trim().Split(' ');
	    						if (date.Length == 2) {
		    						string[] time = date[1].Replace(":","").Split('-');
		    						if (time.Length == 2) {
								    	sb.AppendFormat("DTSTART:{0}T{1}00Z\n", date[0].Replace("/","").Trim(), time[0].Trim());
								    	sb.AppendFormat("DTSTART:{0}T{1}00Z\n", date[0].Replace("/","").Trim(), time[1].Trim());
								    }
							    }
								//DTSTART:20110331T230000Z
								//DTEND:20110401T010000Z
								//DTSTAMP:20130413T040110Z
								
	    					} else {
	    						string[] date = line.Split('-');
	    						if (date.Length == 2) {
							    	sb.AppendFormat("DTSTART;VALUE=DATE:{0}\n", date[0].Replace("/","").Trim());
							    	sb.AppendFormat("DTEND;VALUE=DATE:{0}\n", date[1].Replace("/","").Trim());
							    }
						    	//DTSTART;VALUE=DATE:20100913
								//DTEND;VALUE=DATE:20101010
	    					}
					    	state = ICS_ID;
				    	break;
	    			case ICS_ID:
					    	if (!line.StartsWith("---")) {
					    		sb.AppendFormat("UID:{0}\n", line);
					    		state = ICS_SKIP;
				    		} else {
						    	sb.Append("DESCRIPTION:");
						    	state = ICS_DESC;
				    		}
				    	break;
	    			case ICS_SKIP:
					    	sb.Append("DESCRIPTION:");
					    	state = ICS_DESC;
				    	break;
	    			case ICS_DESC:
				    	if (line.StartsWith("---")) {
					    	sb.Append("\nTRANSP:OPAQUE\n");
					    	sb.Append("END:VEVENT\n");
					    	StartIcsEvent(sb);
				    		state = ICS_TITLE;
				    	} else {
					    	sb.Append(line);
					    }
				    	break;
		    		
		    	}
		    }
		}
		if (sb.Length > 0) sb.Append("\nTRANSP:OPAQUE\nEND:VEVENT");

		string content = sb.ToString();
		WriteFile(siteFile, template, content);
	}


	private static void ProcessBlock(StreamReader sr, StringBuilder sb, string trigger, string wrapTag, string itemTag, string line) {
    	if (wrapTag == "code") sb.Append("<pre>");
    	sb.AppendFormat("<{0}>", wrapTag);

   		bool loop = false;
	    do {
	    	// if (Debug) Console.WriteLine(line);
	    	if (wrapTag == "ol") {
		    	string[] tokens = line.Split('.');
		    	if (tokens.Length == 2) sb.AppendFormat("<li>{0}</li>", tokens[1].Trim());
    		} else if (wrapTag == "ul") { 
    			line = line.Replace(trigger, "").Trim();
    			sb.AppendFormat("<li>{0}</li>", line);
    		} else if (wrapTag == "code") { 
    			line = line.Replace(trigger, "");
    			sb.AppendFormat("{0}\n", line);
    		} else if (wrapTag == "p") { 
    			sb.AppendFormat("{0}\n", line);
    		} else {
    			line = line.Replace(trigger, "").Trim();
    			sb.Append(line);
    		}

    		loop = false;
    		line = sr.ReadLine();
    		if (!string.IsNullOrEmpty(line)) {
		    	if (wrapTag == "ol") {
		    		loop = line.Contains(".");
	    		} else if ((wrapTag == "ul") || (wrapTag == "code") || (wrapTag == "blockquote")) {
	    			loop = line.StartsWith(trigger);
	    		} else if (wrapTag == "p")  {
	    			loop = !line.StartsWith(">");
	    		} else {
	    			loop = true;
	    		}
    		}
    	} while (loop);

    	sb.AppendFormat("</{0}>", wrapTag);
    	if (wrapTag == "code") sb.Append("</pre>");
	}

	private static void ProcessTable(StreamReader sr, StringBuilder sb, string trigger, string line) {
    	sb.Append("<table>");
    	string itemTag = "th";
    	bool thDone = false;
	    do  {
			sb.Append("<tr>");
			string[] tokens = line.Split('|');
			for (int i=1 ; i< tokens.Length-1; i++){
				if (!tokens[i].Contains("---")) {
		    		sb.AppendFormat("<{0}>{1}</{0}>", itemTag, tokens[i]);
		    		thDone = true;
		    	}
			}
			sb.Append("</tr>");
			if (thDone) itemTag = "td";
		} while (((line = sr.ReadLine()) != null) && (line.StartsWith(trigger)));
    	sb.Append("</table>");
	}

	private static void ProcessAnchor(StreamReader sr, StringBuilder sb, string line) {
		// [ny times]: http://www.nytimes.com/
		line = line.Replace("://", "*//"); // Tmp Hack
		string[] tokens = line.Split(':');
		if (tokens.Length == 2) {
			string title = tokens[0].Replace("[","").Replace("]","").Trim();
			string url = tokens[1].Replace("*//", "://").Trim(); // Restore Hack
	    	sb.AppendFormat("<a href='{0}'>{1}</a>", url, title);
	    }
	}

	private static void ProcessImage(StreamReader sr, StringBuilder sb, string line) {
		// ![alt text]:/path/to/img.jpg *** NOT APPROVED SYNTAX 
		string[] tokens = line.Split(':');
		if (tokens.Length == 2) {
			string alt = tokens[0].Replace("!","").Replace("[","").Replace("]","").Trim();
			string url = tokens[1].Trim();
	    	sb.AppendFormat("<img alt='{0}' src='{1}'/>", alt, url);
	    }
	}

	private static void ProcessMarkdownFile(string contentFolder, string siteFolder, string filename) {
		string contentFile = contentFolder + "/" + filename;
		string templateFile = "template/" + filename;
		string siteFile = siteFolder + "/index.html";
		string template = File.ReadAllText(templateFile);
		string content = ""; //File.ReadAllText(contentFile);

	    StringBuilder sb = new StringBuilder();
		using (StreamReader sr = new StreamReader(contentFile)) {
		    string line;
		    while ((line = sr.ReadLine()) != null) {
				//if (Debug) Console.WriteLine(line);

		    	if (line.Trim() == "") {
		    		continue;
		    	} else if (line.StartsWith("<")) {
			    	sb.Append(line); // It's HTML
		    	} else if (line.StartsWith("######")) {
		    		line = line.Replace("######", "").Trim();
			    	sb.AppendFormat("<h6>{0}</h6>", line);
		    	} else if (line.StartsWith("#####")) {
		    		line = line.Replace("#####", "").Trim();
			    	sb.AppendFormat("<h5>{0}</h5>", line);
		    	} else if (line.StartsWith("####")) {
		    		line = line.Replace("####", "").Trim();
			    	sb.AppendFormat("<h4>{0}</h4>", line);
		    	} else if (line.StartsWith("###")) {
		    		line = line.Replace("###", "").Trim();
			    	sb.AppendFormat("<h3>{0}</h3>", line);
		    	} else if (line.StartsWith("##")) {
		    		line = line.Replace("##", "").Trim();
			    	sb.AppendFormat("<h2>{0}</h2>", line);
		    	} else if (line.StartsWith("#")) {
		    		line = line.Replace("#", "").Trim();
			    	sb.AppendFormat("<h1>{0}</h1>", line);
		    	} else if (line.StartsWith(">")) {
		    		ProcessBlock(sr, sb, ">", "blockquote", "", line);
		    	} else if (line.StartsWith("`")) {
		    		ProcessBlock(sr, sb, "`", "code", "", line);
		    	} else if (line.StartsWith("+++") || line.StartsWith("***") || line.StartsWith("---") ) {
		    		sb.Append("<hr/>");
		    	} else if (line.StartsWith("+")) {
		    		ProcessBlock(sr, sb, "+", "ul", "li", line);
		    	} else if (line.StartsWith("-")) {
		    		ProcessBlock(sr, sb, "-", "ul", "li", line);
		    	} else if (line.StartsWith("*")) {
		    		ProcessBlock(sr, sb, "*", "ul", "li", line);
		    	} else if (line.StartsWith("1.")) {
		    		ProcessBlock(sr, sb, "1.", "ol", "li", line);
		    	} else if (line.StartsWith("|")) {
		    		ProcessTable(sr, sb, "|", line);
		    	} else if (line.StartsWith("[")) {
		    		ProcessAnchor(sr, sb, line);
		    	} else if (line.StartsWith("!")) {
		    		ProcessImage(sr, sb, line);
			    } else {
		    		ProcessBlock(sr, sb, " ", "p", "", line);
			    }
		    }
		    content = sb.ToString();
		}
		WriteFile(siteFile, template, content);
	}

	private static void ProcessFolder(string contentFolder, string siteFolder) {
		DirectoryInfo folder = new DirectoryInfo(contentFolder); 

		foreach(FileInfo fi in folder.GetFiles("*")) {
			if (IsTemplateDriven(fi.Name)) {

				Children = BuildChildLinks(folder);

				if (Verbose) Console.WriteLine("Processing "+ fi.Extension + " " + fi.Name);			
				if (fi.Extension == ".html") {
					ProcessHtmlFile(contentFolder, siteFolder, fi.Name);
				} else if (fi.Extension == ".xml") {
					ProcessXmlFile(contentFolder, siteFolder, fi.Name);					
				} else if (fi.Extension == ".csv") {
					ProcessCsvFile(contentFolder, siteFolder, fi.Name);					
				} else if (fi.Extension == ".md") {
					ProcessMarkdownFile(contentFolder, siteFolder, fi.Name);					
				} else if (fi.Extension == ".rss") {
					ProcessRssFile(contentFolder, siteFolder, fi.Name);					
				} else if (fi.Extension == ".ics") {
					ProcessIcsFile(contentFolder, siteFolder, fi.Name);					
				}
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
	}

	public static void CreateSite() {
		string siteFolder = (SiteFolder == "/site") ? Folder+"/site" : SiteFolder;
		StringBuilder nav = new StringBuilder();

		CreateFolder(siteFolder);
		BuildTopNav(nav, Folder+"/content", "");

		TopNav = nav.ToString();

		ProcessFolder(Folder+"/content", siteFolder);
	}


	public static void Usage(bool detailed) {
		Console.WriteLine("bijou [option]");
		Console.WriteLine("  -c to clear out the existing 'site' folder");
		Console.WriteLine("  -i to activate index.html based urls");
		Console.WriteLine("  -m to inject the home icon");
		Console.WriteLine("  -o:path to change the output folder");
		Console.WriteLine("  -r:path to change the root folder");
		Console.WriteLine("  -v verbose");
		Console.WriteLine("  -d debug");
		if (detailed) {
			Console.WriteLine("bijou walks through the content folder and creates the html files based on the given template.");

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

		} catch (Exception e) {
		    Console.WriteLine(e.ToString());
        }
	}
}