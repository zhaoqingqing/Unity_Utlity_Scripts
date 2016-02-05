using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Diagnostics;
using SimpleJson;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class NumberHelper
{
	/// <summary>
	/// 人性化数字显示，百万，千万，亿
	/// </summary>
	/// <param name="number"></param>
	/// <returns></returns>
	public static string HumanizeNumber(int number)
	{
		if (number > 100000000)
		{
			return string.Format("{0}{1}", number / 100000000, "亿");
		}
		else if (number > 10000000)
		{
			return string.Format("{0}{1}", number / 10000000, "千万");
		}
		else if (number > 1000000)
		{
			return string.Format("{0}{1}", number / 1000000, "百万");
		}
		else if (number > 10000)
		{
			return string.Format("{0}{1}", number / 10000, "万");
		}

		return number.ToString();
	}
}