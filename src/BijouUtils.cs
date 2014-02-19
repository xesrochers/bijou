using System;
using System.IO;

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
}


