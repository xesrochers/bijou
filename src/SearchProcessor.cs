using System;
using System.IO;

/**************************************************
 * <summary>
 * SearchProcessor
 * </summary>
 *************************************************/
public class SearchProcessor : BaseProcessor { 

	public void Consume(string contentFolder, string siteFolder, string filename, string ext) {
		string contentFile = contentFolder + "/" + filename;
		string templateFile = GetTemplateFilename(filename, ext);
		SiteFile = siteFolder + "/index.html";
		Template = File.ReadAllText(templateFile);
		Content = File.ReadAllText(contentFile);
	}
}
