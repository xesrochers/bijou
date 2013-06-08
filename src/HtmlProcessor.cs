using System;
using System.IO;

/**************************************************
 * <summary>
 * HtmlProcessor
 * </summary>
 *************************************************/
public class HtmlProcessor : BaseProcessor { 

	public void Consume(string contentFolder, string siteFolder, string filename, string ext) {
		string contentFile = contentFolder + "/" + filename;
		string templateFile = GetTemplateFilename(filename, ext);
		SiteFile = siteFolder + "/index.html";
		try {
			Template = File.ReadAllText(templateFile);
			Content = File.ReadAllText(contentFile);
		} catch (Exception ex) {
			Console.WriteLine("Unable to apply template to content file", ex);
			Console.WriteLine(string.Format("Template: {0}",  templateFile));
			Console.WriteLine(string.Format("Content:  {0}",  contentFile));
		}
	}
}
