using System;
using System.IO;
using System.Text;

/**************************************************
 * <summary>
 * BijouUtils
 * </summary>
 *************************************************/
public class BijouUtils  { 

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

	public static string BuildRelativePath(int level) {
		string result = "";
		if (level > 0){
			for(int i=0; i<level; i++){
				if (!string.IsNullOrEmpty(result)) result += "/";
				result += "..";
			}
		} else {
			result = ".";
		}
		return result;
	}

	public static string BuildRootPath(int level) {
		string result = string.Empty;
		if (Bijou.Index) {
			result = BuildRelativePath(level);
		} else {
			result = Bijou.WebRoot;
		}
		return result;
	}

	public static void BuildLink(StringBuilder stream, string currentPath, DirectoryInfo di) {
		string stripped = StripPrefix(di.Name, '.');
		string displayName = ParseDisplayName(stripped); 
		string strippedPath = StripPrefix(currentPath);

		if (Bijou.Index) {
			string root = BuildRelativePath(Bijou.Level);
			stream.AppendFormat("<a href='{0}{1}index.html'>{2}</a>", root, strippedPath, displayName);
		} else if (Bijou.WebRoot == "") {
			stream.AppendFormat("<a href='{0}'>{1}</a>", strippedPath, displayName);
		} else {
			stream.AppendFormat("<a href='{0}/{1}'>{2}</a>", Bijou.WebRoot, strippedPath, displayName);					
		}
	}

	public static string StripPrefix(string contentPath) {
		StringBuilder result = new StringBuilder("/");
		contentPath = contentPath.Replace("./content", "");
		string[] tokens = contentPath.Split('/');
		foreach (string item in tokens) {
			if (!string.IsNullOrEmpty(item)) {
				result.AppendFormat("{0}/", StripPrefix(item, '.'));
			}
		}
		// result.Append("/");

 		if (Bijou.Debug) Console.WriteLine("StripPath " + result);

		return result.ToString();
	}


	public static string StripPrefix(string text, char separator) {
		string result = text;
		if (!string.IsNullOrEmpty(text)) {
			string[] tokens = text.Split(separator);
			if (tokens.Length >= 2) {
				result = tokens[1];
			}
		}
		return result;
	}


	public static string ParseDisplayName(string text) {
		string result = text;

		if (!string.IsNullOrEmpty(text)) {
			StringBuilder sb = new StringBuilder();
			foreach(char c in text) {
				if (char.IsUpper(c) && (sb.Length > 0)) sb.Append(" ");
				sb.Append(c);
			}
			result = sb.ToString();
		}
		return result;
	}

	public static string ParsePageTitle(string filename) {
		string result = filename;
		if (!string.IsNullOrEmpty(filename)) {
			string[] tokens = filename.Split('/');
			if (tokens.Length >= 2) {
				result = tokens[tokens.Length-2];
			}
		}
		if (result == "site") result = "";
		return ParseDisplayName(result);
	}


	public static bool IsNavigation(string folder) {
		return folder.Contains(".");
	} 

	public static bool IsInvisible(string folder) {
		return folder.StartsWith("0.");
	} 


}


