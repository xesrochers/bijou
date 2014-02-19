using System;
using System.IO;
using System.Text;


/**************************************************
 * <summary>
 * IcsProcessor
 * </summary>
 *************************************************/
public class IcsProcessor : BaseProcessor { 

	private const int ICS_START    = 0;
	private const int ICS_TITLE    = 1;
	private const int ICS_LOCATION = 2;
	private const int ICS_DATE     = 3;
	private const int ICS_ID       = 4;
	private const int ICS_SKIP     = 5;
	private const int ICS_DESC     = 6;

	private static void StartIcsEvent(StringBuilder ics, StringBuilder htm) {
		DateTime now = DateTime.Now;
		string date = string.Format("{0:yyyyMMdd}", now); 
		string time = string.Format("{0:hhmmss}", now);
		ics.Append("BEGIN:VEVENT\n");
		ics.Append("CREATED:19000101T120000Z\n");
		ics.Append("SEQUENCE:0\n");
		ics.Append("STATUS:CONFIRMED\n");
		ics.AppendFormat("DTSTAMP:{0}T{1}Z\n", date,time);
		
		htm.Append("<div class='event'>");
	}

	public void Consume(string contentFolder, string siteFolder, string filename, string ext) {
		string contentFile = contentFolder + "/" + filename;
		string templateFile = GetTemplateFilename(filename, ext);
		SiteFile = siteFolder + "/index.ics";

		try {
			Template = BijouUtils.SharedRead(templateFile);
		    StringBuilder ics = new StringBuilder();
		    StringBuilder htm = new StringBuilder();
			using (StreamReader sr = new StreamReader(contentFile)) {
			    string line;
			    int state = ICS_START;
			    while ((line = sr.ReadLine()) != null) {
		    		switch (state) {
		    			case ICS_START:
					    	if (line.StartsWith("---")) {
					    		StartIcsEvent(ics, htm);
						    	state = ICS_TITLE;
						    }
					    	break;
		    			case ICS_TITLE:
						    	ics.AppendFormat("SUMMARY:{0}\n", line);
						    	htm.AppendFormat("<h2>{0}</h2>", line);
						    	state = ICS_LOCATION;
					    	break;
		    			case ICS_LOCATION:
						    	ics.AppendFormat("LOCATION:{0}\n", line);
						    	htm.AppendFormat("<div class='location'>{0}</div>", line);
						    	state = ICS_DATE;
					    	break;
		    			case ICS_DATE:
		    					if (line.Contains(":")) {
		    						string[] date = line.Replace("(","").Replace(")","").Trim().Split(' ');
		    						if (date.Length == 2) {
			    						string[] time = date[1].Replace(":","").Split('-');
			    						if (time.Length == 2) {
									    	ics.AppendFormat("DTSTART:{0}T{1}00Z\n", date[0].Replace("/","").Trim(), time[0].Trim());
									    	ics.AppendFormat("DTEND:{0}T{1}00Z\n", date[0].Replace("/","").Trim(), time[1].Trim());
									    	htm.AppendFormat("<div class='date-time'>{0} ({1}-{2})</div>", date[0], time[0], time[1]);
									    }
								    }
		    					} else {
		    						string[] date = line.Split('-');
		    						if (date.Length == 2) {
								    	ics.AppendFormat("DTSTART;VALUE=DATE:{0}\n", date[0].Replace("/","").Trim());
								    	ics.AppendFormat("DTEND;VALUE=DATE:{0}\n", date[1].Replace("/","").Trim());
								    	htm.AppendFormat("<div class='date-time'>{0} - {1}</div>", date[0], date[1]);
								    }
		    					}
						    	state = ICS_ID;
					    	break;
		    			case ICS_ID:
						    	if (!line.StartsWith("---")) {
						    		ics.AppendFormat("UID:{0}\n", line);
						    		htm.AppendFormat("UID:{0}\n", line);
						    		state = ICS_SKIP;
					    		} else {
							    	ics.Append("DESCRIPTION:");
							    	htm.Append("<p>");
							    	state = ICS_DESC;
					    		}
					    	break;
		    			case ICS_SKIP:
						    	ics.Append("DESCRIPTION:");
						    	htm.Append("<p>");
						    	state = ICS_DESC;
					    	break;
		    			case ICS_DESC:
					    	if (line.StartsWith("---")) {
						    	ics.Append("\nTRANSP:OPAQUE\n");
						    	ics.Append("END:VEVENT\n");
						    	htm.Append("</p></div>");
						    	StartIcsEvent(ics, htm);
					    		state = ICS_TITLE;
					    	} else {
						    	ics.Append(line);
						    	htm.Append(line);
						    }
					    	break;
			    		
			    	}
			    }
			}

			if (ics.Length > 0) ics.Append("\nTRANSP:OPAQUE\nEND:VEVENT");
			if (htm.Length > 0) htm.Append("</p></div>");

			Content = ics.ToString();
			Clone = htm.ToString();
		} catch (Exception ex) {
			ReportError(templateFile, contentFile, ex);
		}		
	}
}
