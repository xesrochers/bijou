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


}
