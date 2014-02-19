using System;
using System.IO;
using System.Text;

/**************************************************
 * <summary>
 * MenuBuilder
 * </summary>
 *************************************************/
public class MenuBuilder  { 

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

}


