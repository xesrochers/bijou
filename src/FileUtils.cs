using System;
using System.IO;
using System.Text;

/**************************************************
 * <summary>
 * FileUtils
 * </summary>
 *************************************************/
public class FileUtils  { 

  public static void CreateFolder(string folder){
    if(!Directory.Exists(folder)) {
      if (Bijou.Verbose) Console.WriteLine("Creating "+folder);     
      Directory.CreateDirectory(folder);
    }
  }

  public static void CreateScafolding(string folder){
    CreateFolder(folder+"/content");
    CreateFolder(folder+"/template");
    //CreateFolder(folder+"/site");
    /* CreateTemplateHeader();
    CreateTemplateFooter();
    CreateTemplateHeader();*/
  }


  public static string SharedRead(string filename) {
    string result = "";
    using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
      using (var textReader = new StreamReader(fileStream)) {
          result = textReader.ReadToEnd();
      }
      fileStream.Close();
    }
    return result;
  }
  

  public static void WriteFile(string filename, string content) {
    using (StreamWriter sw = File.CreateText(filename)) {
          sw.WriteLine(content);
      }
  } 

  public static void WriteFile(string filename, string template, string content) {
    string title = BijouUtils.ParsePageTitle(filename);
    using (StreamWriter sw = File.CreateText(filename)) {
      SubstitutionEngine se = SubstitutionEngine.GetSubstitutionEngine(title, content);
      if (SearchProcessor.SearchData.Length > 0) {
        se.Add("search", SearchProcessor.SearchData.ToString());
      }
      string text = se.Substitute(template);
          sw.WriteLine(text);
      }
  } 

  public static void HtmlClone(string contentFolder, string siteFolder, string filename, string ext, string content) {
    string templateFile = BaseProcessor.GetTemplateFilename(filename.Replace(ext, ".html"), ".html");
    string siteFile = siteFolder + "/index.html";
    string template = FileUtils.SharedRead(templateFile);
    WriteFile(siteFile, template, content);
  }

  public static string AppendFilename(string folder, string filename) { 
    return (folder.EndsWith("/")) ? folder + filename : folder + "/" + filename;
  }

  public static string AppendFolder(string folder, string subFolder) { 
    string result = "";
    if (folder.EndsWith("/")) {
      result = folder + subFolder + "/";
    } else if (subFolder.StartsWith("/")) {
      result = folder + subFolder;
    } else {
      result = folder + "/" + subFolder;
    }
    return result;
  }
}
