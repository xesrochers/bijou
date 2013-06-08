using System;
using System.IO;
using System.Text;


/**************************************************
 * <summary>
 * RssProcessor
 * </summary>
 *************************************************/
public class RssProcessor : BaseProcessor { 

	private const int RSS_START = 0;
	private const int RSS_TITLE = 1;
	private const int RSS_SKIP  = 2;
	private const int RSS_DATE  = 3;
	private const int RSS_DESC  = 4;

	public void Consume(string contentFolder, string siteFolder, string filename, string ext) {
		string contentFile = contentFolder + "/" + filename;
		string templateFile = GetTemplateFilename(filename, ext);
		SiteFile = siteFolder + "/index.rss";

		try {
			Template = File.ReadAllText(templateFile);
		    StringBuilder rss = new StringBuilder();
		    StringBuilder htm = new StringBuilder();
		    //StringBuilder js = new StringBuilder();
			using (StreamReader sr = new StreamReader(contentFile)) {
			    string line;
			    int state = RSS_START;
			    while ((line = sr.ReadLine()) != null) {
		    		switch (state) {
		    			case RSS_START:
					    	if (line.StartsWith("---")) {
						    	rss.Append("<item>");
						    	htm.Append("<article class='news-item'>");
						    	state = RSS_TITLE;
						    }
					    	break;
		    			case RSS_TITLE:
						    	rss.AppendFormat("<title>{0}</title>", line);
						    	htm.AppendFormat("<h2>{0}</h2>", line);
						    	state = RSS_SKIP;
					    	break;
		    			case RSS_SKIP:
						    	state = RSS_DATE;
					    	break;
		    			case RSS_DATE:
						    	rss.AppendFormat("<pubDate>{0}</pubDate>\n<description>", line);
						    	htm.AppendFormat("<q>{0}</q>\n<p>", line);
						    	state = RSS_DESC;
					    	break;
		    			case RSS_DESC:
					    	if (line.StartsWith("---")) {
					    		rss.Append("</description>\n</item>\n<item>");
					    		htm.Append("</p>\n</article>\n<article>");
					    		state = RSS_TITLE;
					    	} else {
						    	rss.Append(line);
						    	htm.Append(line);
						    }
					    	break;
			    		
			    	}
			    }
			}
			if (rss.Length > 0) rss.Append("</description>\n</item>");
			if (htm.Length > 0) htm.Append("</p>\n</article>");

			Content = rss.ToString();
			Clone   = htm.ToString();
		} catch (Exception ex) {
			Console.WriteLine("Unable to apply template to content file", ex);
			Console.WriteLine(string.Format("Template: {0}",  templateFile));
			Console.WriteLine(string.Format("Content:  {0}",  contentFile));
		}
	}
}
