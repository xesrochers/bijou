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
			Template = FileUtils.SharedRead(templateFile);
			Content = FileUtils.SharedRead(contentFile);
		} catch (Exception ex) {
			ReportError(templateFile, contentFile, ex);
		}
	}
}
