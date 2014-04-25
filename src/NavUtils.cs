using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;


/**************************************************
 * <summary>
 * NavUtils
 * </summary>
 *************************************************/
public class NavUtils  { 

	public static string BuildTopNavXml(string contentFolder) {
		StringBuilder result = new StringBuilder();
		BuildTopNavXml(result, contentFolder, "");

		/* for debugging 
		FileUtils.WriteFile("./site/topnav.xml", result.ToString());
		*/

		return result.ToString();
	}

	private static void BuildTopNavXml(StringBuilder xml, string contentFolder, string path) {
		DirectoryInfo folder = new DirectoryInfo(contentFolder + "/" + path); 
		string stripped = BijouUtils.StripPrefix(folder.Name, '.');
		string displayName = BijouUtils.ParseDisplayName(stripped); 
		string strippedPath = BijouUtils.StripPrefix(contentFolder + "/" + path);

		xml.AppendFormat("<node title='{0}' name='{1}' path='{2}'>", displayName, stripped, strippedPath);

		foreach(DirectoryInfo di in folder.GetDirectories()) {
			if (BijouUtils.IsNavigation(di.Name) && !BijouUtils.IsInvisible(di.Name)) {
				DirectoryInfo children = new DirectoryInfo(contentFolder+"/"+ path + "/" + di.Name);
				if ((children !=null) && (children.GetDirectories().Length > 0)) {
					string childPath = path + "/" + di.Name;
					if (Bijou.Debug) Console.WriteLine("BuildTopNav "+ childPath + " has children");	
					BuildTopNavXml(xml, contentFolder, childPath);
				} else {
					string childPath = path + "/" + di.Name;
					string diStripped = BijouUtils.StripPrefix(di.Name, '.');
					string diDisplayName = BijouUtils.ParseDisplayName(diStripped); 
					string diStrippedPath = BijouUtils.StripPrefix(childPath);
					xml.AppendFormat("<node title='{0}' name='{1}' path='{2}' />", diDisplayName, diStripped, diStrippedPath);
				}
			}
		}
		xml.Append("</node>");
	}


	public static string BuildTopNav() {

		Bijou.Level++;

		string root = FileUtils.BuildRelativePath(Bijou.Level);
    XsltArgumentList xslArg = new XsltArgumentList();
    if (Bijou.Index) {
    	xslArg.AddParam("root", "", root);
    	xslArg.AddParam("index", "", "index.html");
    } else if (!string.IsNullOrEmpty(Bijou.WebRoot)) {
    	xslArg.AddParam("root", "", Bijou.WebRoot);
    	xslArg.AddParam("index", "", "");
    } else { 
    	xslArg.AddParam("root", "", "/");
    	xslArg.AddParam("index", "", "");
    }

    Bijou.Level--;

		return XmlUtils.Transform("etc/topnav.xslt", Bijou.TopNavXml, xslArg);
    
	}

	/*
	public static void BuildTopNav(StringBuilder nav, string contentFolder, string path) {

		Bijou.Level++;

		DirectoryInfo folder = new DirectoryInfo(contentFolder + "/" + path); 
		nav.Append("<ul>");
		if (Bijou.Level==0 && Bijou.Home) {
			string root = BijouUtils.BuildRootPath(Bijou.Level);
			if (Bijou.Index) {
				nav.AppendFormat("<li><a href='{0}/index.html' class='icon-home'></a></li>", root);
			} else if (string.IsNullOrEmpty(Bijou.WebRoot)) {
				nav.Append("<li><a href='/' class='icon-home'></a></li>");
			} else {
				nav.AppendFormat("<li><a href='{0}' class='icon-home'></a></li>", root);
			}
		}

		foreach(DirectoryInfo di in folder.GetDirectories()) {

			if (BijouUtils.IsNavigation(di.Name) && !BijouUtils.IsInvisible(di.Name)){
				string currentPath =  path + "/" + di.Name;
				if (Bijou.Debug) Console.WriteLine("BuildTopNav "+ currentPath);
				nav.Append("<li>");

				BijouUtils.BuildLink(nav, currentPath, di);

				// Check if we have children
				DirectoryInfo children = new DirectoryInfo(contentFolder+"/"+ path + "/" + di.Name);
				if ((children !=null) && (children.GetDirectories().Length > 0)) {
					string childPath = path + "/" + di.Name;
					if (Bijou.Debug) Console.WriteLine("BuildTopNav "+ childPath + " has children");	
					BuildTopNav(nav, contentFolder, childPath);
				}
				nav.Append("</li>");
			}
		}
		nav.Append("</ul>");
		// return (nav.ToString());
		Bijou.Level--;
	}
	*/ 

	public static string BuildBreadcrumb() {
		StringBuilder result = new StringBuilder();
		if (Bijou.Path.Count > 1) {
			result.Append("<ul>");
			foreach(string item in Bijou.Path) {
				string[] tokens = item.Split('/');
				string last = (tokens.Length>0) ? tokens[tokens.Length-1] : "";
				string displayName = BijouUtils.ParseDisplayName(last); 
				if (!string.IsNullOrEmpty(displayName)) {
					result.AppendFormat("<li><a href='{0}'>{1}</a></li>", item, displayName);
				}
			}
			result.Append("</ul>");
		}

		return result.ToString();
	}


	public static string BuildChildLinks(DirectoryInfo folder) {
		StringBuilder result = new StringBuilder();
		DirectoryInfo[] children = folder.GetDirectories();
		if ((children !=null) && (children.Length > 0)) {

			result.Append("<ul>");
			foreach(DirectoryInfo di in children) {
				if (BijouUtils.IsNavigation(di.Name)  && !BijouUtils.IsInvisible(di.Name)){
					string currentPath =  folder.Name + "/" + di.Name;

					//string stripped = StripPrefix(di.Name, '.');
					//string displayName = ParseDisplayName(stripped); 
					//string strippedPath = StripPrefix(currentPath);
					if (Bijou.Debug) Console.WriteLine("BuildChildLinks "+ currentPath);
					result.Append("<li>");
					BijouUtils.BuildLink(result, currentPath, di);
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

}


