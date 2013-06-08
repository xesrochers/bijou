using System;
using System.IO;
using System.Text;


/**************************************************
 * <summary>
 * HtmlProcessor
 * </summary>
 *************************************************/
public class CsvProcessor : BaseProcessor { 

	public void Consume(string contentFolder, string siteFolder, string filename, string ext) {
		string contentFile = contentFolder + "/" + filename;
		string templateFile = GetTemplateFilename(filename, ext);
		SiteFile = siteFolder + "/index.html";
		try {
			Template = File.ReadAllText(templateFile);
		    StringBuilder sb = new StringBuilder();
			using (StreamReader sr = new StreamReader(contentFile)) {
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
			Content = sb.ToString();
		} catch (Exception ex) {
			Console.WriteLine("Unable to apply template to content file", ex);
			Console.WriteLine(string.Format("Template: {0}",  templateFile));
			Console.WriteLine(string.Format("Content:  {0}",  contentFile));
		}

	}
}
