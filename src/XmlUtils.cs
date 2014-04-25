using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;


/**************************************************
 * <summary>
 * XmlUtils
 * </summary>
 *************************************************/
public class XmlUtils  {

  public static string Transform(string xsltFile, string xmlFile) {
    XsltArgumentList xslArg = new XsltArgumentList();
    return Transform(xsltFile, xmlFile, xslArg);
  }


  public static string Transform(string xsltFile, string xml, XsltArgumentList xslArg) {
    string result = "";
    try {

        XslCompiledTransform xslt = new XslCompiledTransform();
        xslt.Load(xsltFile); 
        // XmlWriterSettings.OmitXmlDeclaration

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(xml));

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.OmitXmlDeclaration = true;
        settings.NewLineOnAttributes = true;

        using (StringWriter sw = new StringWriter()) {
        using (XmlWriter writer = XmlWriter.Create(sw, settings)) {
            xslt.Transform(xmlDoc, xslArg, writer);
        }
        sw.Flush();
        result = sw.ToString();
      }
    } catch (Exception ex) {
        Console.WriteLine(ex.ToString());
    }
    return result;
  }

}
