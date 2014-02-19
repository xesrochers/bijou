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
		try {
			Template = BijouUtils.SharedRead(templateFile);
			Content = BijouUtils.SharedRead(contentFile);
		} catch (Exception ex) {
			ReportError(templateFile, contentFile, ex);
		}
	}
}
