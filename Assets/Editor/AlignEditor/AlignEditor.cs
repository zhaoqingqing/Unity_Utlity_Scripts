using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

/**
 * version 1.0 by kk 
 * version 1.1 by zhaoqingqing
 */

/// <summary>
/// 对齐扩展
/// </summary>
public class AlignEditor : EditorWindow
{
	//对齐
	public float alignX = 0f, alignY = 0f, alignZ = 0f;

	[MenuItem("Window/AlignEditor")]
	static void Init()
	{
		//Init Editor Window
		var alignEditor = EditorWindow.GetWindow(typeof(AlignEditor));
		alignEditor.title = "对齐小工具";
		alignEditor.Show();

	}

	void OnGUI()
	{
		GUILayout.BeginHorizontal("HelpBox");
		GUILayout.Label("Tips:请在Hierarchy选中你要对齐Gameobject", "label");
		GUILayout.EndHorizontal();

		GUILayout.Label("设置间距");
		alignX = EditorGUILayout.FloatField("X", alignX);
		alignY = EditorGUILayout.FloatField("Y", alignY);
		alignZ = EditorGUILayout.FloatField("Z", alignZ);
		//对齐
		if (GUILayout.Button("设置间距"))
		{
			var gameObjects = getSortedGameObjects();

			Vector3 firstObjPos = Vector3.zero;//第1个的坐标
			for (int i = 0; i < gameObjects.Count; i++)
			{
				if (i == 0)
				{
					firstObjPos = gameObjects[i].transform.localPosition;
					continue;
				}

				gameObjects[i].transform.localPosition = new Vector3(
					firstObjPos.x + alignX * i,
					firstObjPos.y + alignY * i,
					firstObjPos.z + alignZ * i);
			}
		}

		GUILayout.Label("Other Align");

		GUILayout.BeginHorizontal();

		if (GUILayout.Button("左对齐"))
		{
			this.AlignSelection(AlignType.XAlign, false);
		}
		if (GUILayout.Button("右对齐"))
		{
			this.AlignSelection(AlignType.XAlign, true);
		}
		if (GUILayout.Button("顶对齐"))
		{
			AlignSelection(AlignType.YAlign, true);
		}
		if (GUILayout.Button("底对齐"))
		{
			AlignSelection(AlignType.YAlign, false);
		}
		GUILayout.EndHorizontal();

	}

	enum AlignType
	{
		YAlign,
		XAlign
	}


	//根据名字比较
	private int CompareGameObjectsByName(GameObject a, GameObject b)
	{
		return a.name.CompareTo(b.name);
	}

	/// <summary>
	/// 获取选中的gameobjects
	/// </summary>
	/// <returns></returns>
	private List<GameObject> getSortedGameObjects()
	{
		List<GameObject> gameObjects = new List<GameObject>(Selection.gameObjects);
		//gameObjects.Sort(this.CompareGameObjectsByName);
		return gameObjects;
	}

	/// <summary>
	/// 设置位置
	/// </summary>
	/// <param name="alignType"></param>
	/// <param name="fromSmallToLarge"></param>
	void AlignSelection(AlignType alignType, bool fromSmallToLarge)
	{
		var gameObjects = getSortedGameObjects();
		if (gameObjects.Count <= 0)
		{
			Debug.Log("select gameobject null");
			return;
		}

		if (alignType == AlignType.XAlign)
		{
			//按从小到大排序
			gameObjects.Sort((ls, rs) =>
			{
				return ls.transform.localPosition.x.CompareTo(rs.transform.localPosition.x);

			});
			float alignX = 0f;

			if (fromSmallToLarge) alignX = gameObjects.LastOrDefault().transform.localPosition.x;
			else alignX = gameObjects.FirstOrDefault().transform.localPosition.x;

			foreach (GameObject obj in gameObjects)
			{
				float selfY = obj.transform.localPosition.y;
				float selfZ = obj.transform.localPosition.z;
				//注册操作记录
				Undo.RecordObject(obj,string.Concat("SetPosX",obj.name));
				obj.transform.localPosition = new Vector3(alignX, selfY, selfZ);
				
			}
		}
		else if (alignType == AlignType.YAlign)
		{
			//按从小到大排序
			gameObjects.Sort((ls, rs) =>
			{
				return ls.transform.localPosition.y.CompareTo(rs.transform.localPosition.y);

			});
			float alignY = 0f;

			if (fromSmallToLarge) alignY = gameObjects.LastOrDefault().transform.localPosition.y;
			else alignY = gameObjects.FirstOrDefault().transform.localPosition.y;

			foreach (GameObject obj in gameObjects)
			{
				float selfX = obj.transform.localPosition.x;
				float selfZ = obj.transform.localPosition.z;

				obj.transform.localPosition = new Vector3(selfX, alignY, selfZ);
			}
		}
	}

}
