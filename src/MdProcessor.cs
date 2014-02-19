using System;
using System.IO;
using System.Text;


/**************************************************
 * <summary>
 * MdProcessor
 * </summary>
 *************************************************/
public class MdProcessor : BaseProcessor { 


	private void ProcessBlock(StreamReader sr, StringBuilder sb, string trigger, string wrapTag, string itemTag, string line) {
    	if (wrapTag == "code") sb.Append("<pre>");
    	sb.AppendFormat("<{0}>", wrapTag);

   		bool loop = false;
	    do {
	    	// if (Debug) Console.WriteLine(line);
	    	if (wrapTag == "ol") {
		    	string[] tokens = line.Split('.');
		    	if (tokens.Length == 2) sb.AppendFormat("<li>{0}</li>", tokens[1].Trim());
    		} else if (wrapTag == "ul") { 
    			line = line.Replace(trigger, "").Trim();
    			sb.AppendFormat("<li>{0}</li>", line);
    		} else if (wrapTag == "code") { 
    			line = line.Replace(trigger, "");
    			sb.AppendFormat("{0}\n", line);
    		} else if (wrapTag == "p") { 
    			sb.AppendFormat("{0}\n", line);
    		} else {
    			line = line.Replace(trigger, "").Trim();
    			sb.Append(line);
    		}

    		loop = false;
    		line = sr.ReadLine();
    		if (!string.IsNullOrEmpty(line)) {
		    	if (wrapTag == "ol") {
		    		loop = line.Contains(".");
	    		} else if ((wrapTag == "ul") || (wrapTag == "code") || (wrapTag == "blockquote")) {
	    			loop = line.StartsWith(trigger);
	    		} else if (wrapTag == "p")  {
	    			loop = !line.StartsWith(">");
	    		} else {
	    			loop = true;
	    		}
    		}
    	} while (loop);

    	sb.AppendFormat("</{0}>", wrapTag);
    	if (wrapTag == "code") sb.Append("</pre>");
	}

	private void ProcessTable(StreamReader sr, StringBuilder sb, string trigger, string line) {
    	sb.Append("<table>");
    	string itemTag = "th";
    	bool thDone = false;
	    do  {
			sb.Append("<tr>");
			string[] tokens = line.Split('|');
			for (int i=1 ; i< tokens.Length-1; i++){
				if (!tokens[i].Contains("---")) {
		    		sb.AppendFormat("<{0}>{1}</{0}>", itemTag, tokens[i]);
		    		thDone = true;
		    	}
			}
			sb.Append("</tr>");
			if (thDone) itemTag = "td";
		} while (((line = sr.ReadLine()) != null) && (line.StartsWith(trigger)));
    	sb.Append("</table>");
	}

	private void ProcessAnchor(StreamReader sr, StringBuilder sb, string line) {
		// [ny times]: http://www.nytimes.com/
		line = line.Replace("://", "*//"); // Tmp Hack
		string[] tokens = line.Split(':');
		if (tokens.Length == 2) {
			string title = tokens[0].Replace("[","").Replace("]","").Trim();
			string url = tokens[1].Replace("*//", "://").Trim(); // Restore Hack
	    	sb.AppendFormat("<a href='{0}'>{1}</a>", url, title);
	    }
	}

	private void ProcessImage(StreamReader sr, StringBuilder sb, string line) {
		// ![alt text]:/path/to/img.jpg *** NOT APPROVED SYNTAX 
		string[] tokens = line.Split(':');
		if (tokens.Length == 2) {
			string alt = tokens[0].Replace("!","").Replace("[","").Replace("]","").Trim();
			string url = tokens[1].Trim();
	    	sb.AppendFormat("<img alt='{0}' src='{1}'/>", alt, url);
	    }
	}

	public void Consume(string contentFolder, string siteFolder, string filename, string ext) {
		string contentFile = contentFolder + "/" + filename;
		string templateFile = GetTemplateFilename(filename, ext);
		SiteFile = siteFolder + "/index.html";

		try {
			Template = BijouUtils.SharedRead(templateFile);
			Content = ""; //BijouUtils.SharedRead(contentFile);

		    StringBuilder sb = new StringBuilder();
			using (StreamReader sr = new StreamReader(contentFile)) {
			    string line;
			    while ((line = sr.ReadLine()) != null) {
					//if (Debug) Console.WriteLine(line);

			    	if (line.Trim() == "") {
			    		continue;
			    	} else if (line.StartsWith("<")) {
				    	sb.Append(line); // It's HTML
			    	} else if (line.StartsWith("######")) {
			    		line = line.Replace("######", "").Trim();
				    	sb.AppendFormat("<h6>{0}</h6>", line);
			    	} else if (line.StartsWith("#####")) {
			    		line = line.Replace("#####", "").Trim();
				    	sb.AppendFormat("<h5>{0}</h5>", line);
			    	} else if (line.StartsWith("####")) {
			    		line = line.Replace("####", "").Trim();
				    	sb.AppendFormat("<h4>{0}</h4>", line);
			    	} else if (line.StartsWith("###")) {
			    		line = line.Replace("###", "").Trim();
				    	sb.AppendFormat("<h3>{0}</h3>", line);
			    	} else if (line.StartsWith("##")) {
			    		line = line.Replace("##", "").Trim();
				    	sb.AppendFormat("<h2>{0}</h2>", line);
			    	} else if (line.StartsWith("#")) {
			    		line = line.Replace("#", "").Trim();
				    	sb.AppendFormat("<h1>{0}</h1>", line);
			    	} else if (line.StartsWith(">")) {
			    		ProcessBlock(sr, sb, ">", "blockquote", "", line);
			    	} else if (line.StartsWith("`")) {
			    		ProcessBlock(sr, sb, "`", "code", "", line);
			    	} else if (line.StartsWith("+++") || line.StartsWith("***") || line.StartsWith("---") ) {
			    		sb.Append("<hr/>");
			    	} else if (line.StartsWith("+")) {
			    		ProcessBlock(sr, sb, "+", "ul", "li", line);
			    	} else if (line.StartsWith("-")) {
			    		ProcessBlock(sr, sb, "-", "ul", "li", line);
			    	} else if (line.StartsWith("*")) {
			    		ProcessBlock(sr, sb, "*", "ul", "li", line);
			    	} else if (line.StartsWith("1.")) {
			    		ProcessBlock(sr, sb, "1.", "ol", "li", line);
			    	} else if (line.StartsWith("|")) {
			    		ProcessTable(sr, sb, "|", line);
			    	} else if (line.StartsWith("[")) {
			    		ProcessAnchor(sr, sb, line);
			    	} else if (line.StartsWith("!")) {
			    		ProcessImage(sr, sb, line);
				    } else {
			    		ProcessBlock(sr, sb, " ", "p", "", line);
				    }
			    }
			    Content = sb.ToString();
			}
		} catch (Exception ex) {
			ReportError(templateFile, contentFile, ex);
		}
	}

}
