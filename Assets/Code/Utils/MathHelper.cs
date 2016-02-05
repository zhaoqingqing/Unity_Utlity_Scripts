using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MathHelper
{
	/// <summary>
	/// 获取一个范围
	/// </summary>
	/// <param name="startDirection"></param>
	/// <param name="nNum"></param>
	/// <param name="pAnchorPos"></param>
	/// <param name="fAngle"></param>
	/// <param name="nRadius"></param>
	/// <returns></returns>
	public static Vector3[] GetSmartNpcPoints(Vector3 startDirection, int nNum, Vector3 pAnchorPos, float fAngle, float nRadius)
	{
		bool bPlural = nNum % 2 == 0 ? true : false; // 是否复数模式
		Vector3 vDir = startDirection;
		int nMidNum = bPlural ? nNum / 2 : nNum / 2 + 1; // 中间数, 循环过了中间数后，另一个方向起排布
		Vector3 vRPos = vDir * nRadius; //// 计算直线在圆形上的顶点 半径是表现距离
		Vector3[] targetPos = new Vector3[nNum];
		for (int i = 1; i <= nNum; i++)
		{
			float nAddAngle = 0;

			if (bPlural) // 复数模式
			{
				if (i > nMidNum)
					nAddAngle = fAngle * ((i % nMidNum) + 1) - fAngle / 2;
				else
					nAddAngle = -fAngle * ((i % nMidNum) + 1) + fAngle / 2; // 除以2，是为了顶端NPC均匀排布 by KK
			}
			else // 单数模式
			{
				// 判断是否过了中间数
				if (i > nMidNum)
				{
					nAddAngle = fAngle * (i % nMidNum); // 添加NPC的角度
				}
				else if (i < nMidNum) // 非复数模式， 中间数NPC 放在正方向
				{
					nAddAngle = -fAngle * (i % nMidNum); // 反方向角度
				}
				else
					nAddAngle = 0; // 正方向
			}

			Vector3 vTargetPos = pAnchorPos + Quaternion.AngleAxis(nAddAngle, Vector3.forward) * vRPos;
			targetPos[i - 1] = vTargetPos;
		}
		return targetPos;
	}
	
			/// <summary>
		/// 判断字符串是否Int
		/// </summary>
		/// <param name="strTemp"></param>
		/// <returns></returns>
		public static bool IsInt(string strTemp)
		{
			if (string.IsNullOrEmpty(strTemp))
			{
				return false;
			}

			var ret = Regex.IsMatch(strTemp, @"^\d+$");
			return ret;
		}

		/// <summary>
		/// 判断字符串是否float
		/// </summary>
		/// <param name="strTemp"></param>
		/// <returns></returns>
		public static bool IsDoubleOrFloat(string strTemp)
		{
			if (string.IsNullOrEmpty(strTemp))
			{
				return false;
			}

			double num;
			var ret = double.TryParse(strTemp, System.Globalization.NumberStyles.Float,
				System.Globalization.NumberFormatInfo.InvariantInfo, out num);
			return ret;
		}
}