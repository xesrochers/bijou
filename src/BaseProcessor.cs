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

}
