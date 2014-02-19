using System;
using System.IO;

/**************************************************
 * <summary>
 * BaseProcessor
 * </summary>
 *************************************************/
public class BaseProcessor { 

	public string SiteFile {get;set;}
	public string Template {get;set;}
	public string Content {get;set;}
	public string Clone {get;set;}

	public static string GetTemplateFilename(string filename, string ext) {
		string result = string.Empty;
		if (File.Exists(string.Format("template/{0}", filename))) {
			result = string.Format("template/{0}", filename);
		} else if (File.Exists(string.Format("template/default{0}", ext))) {
			result = string.Format("template/default{0}", ext);
		}
		return result;
	}

	public void ReportError(string templateFile, string contentFile, Exception ex) {
		Console.WriteLine("Unable to apply template to content file", ex);
		Console.WriteLine(string.Format("Template: {0}",  templateFile));
		Console.WriteLine(string.Format("Content:  {0}",  contentFile));
		Console.WriteLine(string.Format("SiteFile: {0}",  SiteFile));
	}

}
