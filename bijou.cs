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

	public static bool Debug = true;
	public static string Folder = ".";
	public static string TopNav = "";
	public static string Breadcrumb = "";

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

	private static void WriteFile(string filename, string template, string content) {

		using (StreamWriter sw = File.CreateText(filename)) {
			string text = "";
			text = template.Replace("{content}", content);
			text = text.Replace("{topnav}", TopNav);
			text = text.Replace("{breadcrumb}", Breadcrumb);
	        sw.WriteLine(text);
	    }
	} 

	private static bool IsNavigation(string folder) {
		return folder.Contains(".");
	} 

	private static bool IsTemplateDriven(string filename) {
		filename = filename.Replace(".xml", ".xslt"); // The only one that changes name
		return File.Exists("template/"+filename);
	} 

	private static string BuildTopNav(string contentFolder, string siteFolder) {
		DirectoryInfo folder = new DirectoryInfo(contentFolder); 

		StringBuilder nav = new StringBuilder();
		nav.Append("<ul>");
		foreach(DirectoryInfo di in folder.GetDirectories()) {
			if (IsNavigation(di.Name)){
				string stripped = StripPrefix(di.Name, '.');
				string displayName = ParseDisplayName(stripped); 
				if (Debug) Console.WriteLine("Processing "+ stripped);
				nav.Append("<li>");
				nav.AppendFormat("<a href='/{0}'>{1}</a>", stripped, displayName);
				nav.Append("</li>");
			}
		}
		nav.Append("</ul>");
		return (nav.ToString());
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
		string templateFile = "template/" + filename.Replace(".xml", ".xslt");
		string siteFile = siteFolder + "/index.html";

		XsltArgumentList xslArg = new XsltArgumentList();
        xslArg.AddParam("topnav", "", TopNav);
        xslArg.AddParam("breadcrumb", "", Breadcrumb);

        // XslTransform xslt = new XslTransform();
        // xslt.Load(templateFile); 
        // xslt.Transform(contentFile, xslArg, siteFile); 
        XslCompiledTransform xslt = new XslCompiledTransform();
        xslt.Load(templateFile); 
        using (XmlWriter writer = XmlWriter.Create(siteFile)) {
            xslt.Transform(contentFile, xslArg, writer);
        }

	}

	private static void ProcessCsvFile(string contentFolder, string siteFolder, string filename) {
	    StringBuilder sb = new StringBuilder();
		using (StreamReader sr = new StreamReader(contentFolder + "/" + filename)) {
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
		string templateFile = "template/" + filename;
		string siteFile = siteFolder + "/index.html";
		string template = File.ReadAllText(templateFile);
		WriteFile(siteFile, template, content);
	}


	private static void ProcessFolder(string contentFolder, string siteFolder) {
		DirectoryInfo folder = new DirectoryInfo(contentFolder); 

		foreach(FileInfo fi in folder.GetFiles("*")) {
			if (IsTemplateDriven(fi.Name)) {
				if (Debug) Console.WriteLine("Processing "+ fi.Extension + " " + fi.Name);			
				if (fi.Extension == ".html") {
					ProcessHtmlFile(contentFolder, siteFolder, fi.Name);
				} else if (fi.Extension == ".xml") {
					ProcessXmlFile(contentFolder, siteFolder, fi.Name);					
				} else if (fi.Extension == ".csv") {
					ProcessCsvFile(contentFolder, siteFolder, fi.Name);					
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
		TopNav = BuildTopNav(Folder+"/content", Folder+"/site");
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