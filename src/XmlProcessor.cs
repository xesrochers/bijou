using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

/**************************************************
 * <summary>
 * HtmlProcessor
 * </summary>
 *************************************************/
public class XmlProcessor : BaseProcessor { 
	public XsltArgumentList XslArgs {get; set;}

	public void Consume(string contentFolder, string siteFolder, string filename, string ext) {
		string contentFile = contentFolder + "/" + filename;
		string templateFile = GetTemplateFilename(filename, ext);
		SiteFile = siteFolder + "/index.html";

		try {
				//XsltArgumentList xslArg = BuildXsltArgumentList();
        XslCompiledTransform xslt = new XslCompiledTransform();
        xslt.Load(templateFile); 

        using (StringWriter sw = new StringWriter()) {
			  using (XmlWriter writer = XmlWriter.Create(sw)) {
	      	xslt.Transform(contentFile, XslArgs, writer);
			  }
			  sw.Flush();
			  Content = sw.ToString();
			}
		} catch (Exception ex) {
			ReportError(templateFile, contentFile, ex);
		}
	}

	public static XsltArgumentList BuildXsltArgumentList(string title) {
		SubstitutionEngine se = SubstitutionEngine.GetSubstitutionEngine(title,"");
		return se.ToXsltArgumentList();
	}

}
