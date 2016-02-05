using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public static class Extensions
{
static public Int32 ToInt32(this string str)
	{
		var ret = 0;
		if (!string.IsNullOrEmpty(str))
			ret=Convert.ToInt32(str);

		return ret;
	}

	static public Int32 ToInt32(this object obj)
	{
		var ret = 0;
		if (obj!=null)
			ret= Convert.ToInt32(obj);

		return ret;
	}
	static public string ToString(this object obj)
	{
		var ret = "";
		if (obj != null)
			ret=obj.ToString();
		return ret;
	}

	static public bool SetActive(this Transform trans,bool active)
	{
		var ret = false;
		if (trans != null)
		{
			trans.gameObject.SetActive(active);
			ret = true;
		}
		return ret;
	}
	static public bool SetActiveX(this GameObject obj, bool active)
	{
		var ret = false;
		if (obj != null)
		{
			obj.SetActive(active);
			ret = true;
		}
		return ret;
	}

	public static void SetPositionX(this Transform t, float newX)
	{
		t.position = new Vector3(newX, t.position.y, t.position.z);
	}

	public static void SetPositionY(this Transform t, float newY)
	{
		t.position = new Vector3(t.position.x, newY, t.position.z);
	}

	public static void SetLocalPositionX(this Transform t, float newX)
	{
		t.localPosition = new Vector3(newX, t.localPosition.y, t.localPosition.z);
	}
	public static void SetLocalPositionY(this Transform t, float newY)
	{
		t.localPosition = new Vector3(t.localPosition.x, newY, t.localPosition.z);
	}
	/// <summary>
	/// Y鍧愭爣+
	/// </summary>
	public static void SetLocalPositionYAdd(this Transform t, float newY)
	{
		t.localPosition = new Vector3(t.localPosition.x, t.GetLocalPositionY() + newY, t.localPosition.z);
	}
	public static void SetPositionZ(this Transform t, float newZ)
	{
		t.position = new Vector3(t.position.x, t.position.y, newZ);
	}
	public static void SetLocalPositionZ(this Transform t, float newZ)
	{
		t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, newZ);
	}

	public static void SetLocalScale(this Transform t, Vector3 newScale)
	{
		t.localScale = newScale;
	}
	public static void SetLocalScaleZero(this Transform t)
	{
		t.localScale = Vector3.zero;
	}
	public static float GetPositionX(this Transform t)
	{
		return t.position.x;
	}

	public static float GetPositionY(this Transform t)
	{
		return t.position.y;
	}

	public static float GetPositionZ(this Transform t)
	{
		return t.position.z;
	}

	public static float GetLocalPositionX(this Transform t)
	{
		return t.localPosition.x;
	}

	public static float GetLocalPositionY(this Transform t)
	{
		return t.localPosition.y;
	}

	public static float GetLocalPositionZ(this Transform t)
	{
		return t.localPosition.z;
	}
	public static bool HasRigidbody(this GameObject gobj)
	{
		return (gobj.GetComponent<Rigidbody>() != null);
	}

	public static bool HasAnimation(this GameObject gobj)
	{
		return (gobj.GetComponent<Animation>() != null);
	}

	public static void SetSpeed(this Animation anim, float newSpeed)
	{
		anim[anim.clip.name].speed = newSpeed;
	}

	public static Vector2 ToVector2(this Vector3 vec)
	{
		return new Vector2(vec.x, vec.y);
	}
	public static byte ToByte(this string val)
	{
		return string.IsNullOrEmpty(val) ? (byte)0 : Convert.ToByte(val);
	}

	public static long ToInt64(this string val)
	{
		return string.IsNullOrEmpty(val) ? 0 : Convert.ToInt64(val);
	}
	public static float ToFloat(this string val)
	{
		return string.IsNullOrEmpty(val) ? 0f : Convert.ToSingle(val);
	}

	/// <summary>
	/// Get from object Array
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="openArgs"></param>
	/// <param name="offset"></param>
	/// <param name="isLog"></param>
	/// <returns></returns>
	public static T Get<T>(this object[] openArgs, int offset, bool isLog = true)
	{
		T ret;
		if ((openArgs.Length - 1) >= offset)
		{
			var arrElement = openArgs[offset];
			if (arrElement == null)
				ret = default(T);
			else
			{
				try
				{

					ret = (T)Convert.ChangeType(arrElement, typeof(T));
				}
				catch (Exception)
				{
					if (arrElement is string && string.IsNullOrEmpty(arrElement as string))
						ret = default(T);
					else
					{
						//LogWriter.WriteError("[Error get from object[],  '{0}' change to type {1}", arrElement, typeof(T));
						ret = default(T);
					}
				}

			}
		}
		else
		{
			ret = default(T);
			if (isLog)
			{
				//Logger.WriteError("[GetArg] {0} args - offset: {1}", openArgs, offset);
			}
		}

		return ret;
	}

	//public static void ResizeBoxCollider(this BoxCollider boxCollider, UIWidget widget)
	//{
	//	if (widget != null)
	//	{
	//		boxCollider.size = new Vector3(widget.width, widget.height, 0);
	//	}
	//}
}