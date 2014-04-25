using System;
using System.IO;
using System.Collections;
using System.Xml;
using System.Xml.Xsl;


/**************************************************
 * <summary>
 * SubstitutionEngine
 * </summary>
 *************************************************/
public class SubstitutionEngine { 
	private ArrayList mKeys = new ArrayList();
	private ArrayList mVals = new ArrayList();

	public void Add(string key, string val) {
		if (!string.IsNullOrEmpty(key)) {
			mKeys.Add(key);
			mVals.Add(val);
		}
	}

	public XsltArgumentList ToXsltArgumentList() {
		XsltArgumentList result = new XsltArgumentList();
		for(int i=0; i<mKeys.Count; i++){
	        result.AddParam((string)mKeys[i], "", (string)mVals[i]);
		}
		return result;

	}

	public string Substitute(string template) {
		string result = (!string.IsNullOrEmpty(template)) ? template : string.Empty;
		for(int i=0; i<mKeys.Count; i++){
			result = result.Replace("{$"+mKeys[i]+"}", (string)mVals[i]);
		}
		return result;
	}


	public static SubstitutionEngine GetSubstitutionEngine(string title, string content) {
		SubstitutionEngine result = new SubstitutionEngine();
		Bijou.Breadcrumb = NavUtils.BuildBreadcrumb();
		DateTime now = DateTime.Now;
		result.Add("content", content);
		result.Add("title", title);
		result.Add("root", BijouUtils.BuildRootPath(Bijou.Level));
		result.Add("topnav", Bijou.TopNav);
		result.Add("breadcrumb", Bijou.Breadcrumb);
		result.Add("children", Bijou.Children);
		result.Add("url", Bijou.CurrentPageUrl);
		result.Add("date", string.Format("{0:yyyy/MM/dd}", now ));
		result.Add("time", string.Format("{0:hh}:{1:mm}", now, now ));
		result.Add("gmt", string.Format("{0:yyyyMMdd}T{1:hhmmss}Z", now, now));
		return result;
	}

}
