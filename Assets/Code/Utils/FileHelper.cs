using System;
using System.Collections.Generic;
using System.IO;

public class FileHelper
{
	/// <summary>
	/// 递归获取所有的目录
	/// </summary>
	/// <param name="strPath"></param>
	/// <param name="lstDirect"></param>
	public static void GetAllDirectorys(string strPath, ref List<string> lstDirect)
	{
		if (Directory.Exists(strPath) == false)
		{
			Console.WriteLine("请检查，路径不存在：{0}",strPath);
			return;
		}
		DirectoryInfo diFliles = new DirectoryInfo(strPath);
		DirectoryInfo[] directories = diFliles.GetDirectories();
		var max = directories.Length;
		for (int dirIdx = 0; dirIdx < max; dirIdx++)
		{
			try
			{
				var dir = directories[dirIdx];
				//dir.FullName是某个子目录的绝对地址，把它记录起来
				lstDirect.Add(dir.FullName);
				GetAllDirectorys(dir.FullName, ref lstDirect);
			}
			catch
			{
				continue;
			}
		}
	}

	/// <summary>  
	/// 遍历当前目录及子目录，获取所有文件 
	/// </summary>  
	/// <param name="strPath">文件路径</param>  
	/// <returns>所有文件</returns>  
	public static IList<FileInfo> GetAllFiles(string strPath)
	{
		List<FileInfo> lstFiles = new List<FileInfo>();
		List<string> lstDirect = new List<string>();
		lstDirect.Add(strPath);
		DirectoryInfo diFliles = null;
		GetAllDirectorys(strPath, ref lstDirect);

		var max = lstDirect.Count;
		for (int idx = 0; idx < max; idx++)
		{
			try
			{
				diFliles = new DirectoryInfo(lstDirect[idx]);
				lstFiles.AddRange(diFliles.GetFiles());
			}
			catch
			{
				continue;
			}
		}
		return lstFiles;
	}
}