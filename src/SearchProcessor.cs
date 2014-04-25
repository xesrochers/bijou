using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


/**************************************************
 * <summary>
 * SearchProcessor
 * </summary>
 *************************************************/
public class SearchProcessor : BaseProcessor { 

	public static StringBuilder SearchData = new StringBuilder();
	public static string SearchContentFolder = null;
	public static string SearchSiteFolder = null;
	public static string SearchTemplate = null;
	public static string SearchExtension = null;


	public void Consume() {
		Consume(SearchContentFolder, SearchSiteFolder, SearchTemplate, SearchExtension);
	}

	private void Consume(string contentFolder, string siteFolder, string filename, string ext) {
		string contentFile = contentFolder + "/" + filename;
		string templateFile = GetTemplateFilename(filename, ext);
		SiteFile = siteFolder + "/index.html";
		try {
			Template = FileUtils.SharedRead(templateFile);
			Content = FileUtils.SharedRead(contentFile);
			
		} catch (Exception ex) {
			ReportError(templateFile, contentFile, ex);
		}
	}

	public static bool HasSearchData() {
		return !string.IsNullOrEmpty(SearchContentFolder) && (SearchData.Length >0);
	}

	/*********************************************
	 * Check to see if it's the search file. 
	 *********************************************/
	public static void TagSearchFile(string content,string contentFolder, string siteFolder, FileInfo fi) {
		if (!string.IsNullOrEmpty(content) && content.Contains("{$search}")) {
			SearchContentFolder = contentFolder;
			SearchSiteFolder = siteFolder;
			SearchTemplate = fi.Name;
			SearchExtension = fi.Extension;
		}
	}

	public static void AppendSearchData(string contentFolder, string siteFolder, string filename, string ext) {
		string contentFile = contentFolder + "/" + filename;
		//string templateFile = BuildTemplateFilename(filename, ext);
		//string siteFile = siteFolder + "/index.html";
		//string template = FileUtils.SharedRead(templateFile);
		string content = FileUtils.SharedRead(contentFile);

		siteFolder = siteFolder.Replace("."+Bijou.SiteFolder, Bijou.WebRoot); 
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

}
