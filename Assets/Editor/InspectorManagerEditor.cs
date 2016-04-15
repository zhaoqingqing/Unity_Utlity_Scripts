using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System;

/// <summary>
/// 描述：全部 展开/收起 Inspector面板的所有组件、
/// todo 还可以深度定制，添加两个按钮切换
/// 参考自：http://blog.csdn.net/u010019717/article/details/50268111
/// </summary>
//[CustomEditor(typeof(Transform))]
public class InspectorManagerEditor : Editor
{
	[ContextMenu("Tools/InspectorManager/全部展开组件... %#&m")]
	static void Expansion()
	{
		var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
		var window = EditorWindow.GetWindow(type);
		FieldInfo info = type.GetField("m_Tracker", BindingFlags.NonPublic | BindingFlags.Instance);
		ActiveEditorTracker tracker = info.GetValue(window) as ActiveEditorTracker;

		for (int i = 0; i < tracker.activeEditors.Length; i++)
		{
			////可以通过名子单独判断组件展开或不展开
			//if (tracker.activeEditors[i].target.GetType().Name != "NewBehaviourScript")
			//{
				//这里1就是展开，0就是合起来
				tracker.SetVisible(i, 1);
			//}
		}
	}

	[ContextMenu("Tools/InspectorManager/全部收起组件... %#&n")]
	static void Shrinkage()
	{
		var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
		var window = EditorWindow.GetWindow(type);
		FieldInfo info = type.GetField("m_Tracker", BindingFlags.NonPublic | BindingFlags.Instance);
		ActiveEditorTracker tracker = info.GetValue(window) as ActiveEditorTracker;

		for (int i = 0; i < tracker.activeEditors.Length; i++)
		{
			//这里1就是展开，0就是合起来
			tracker.SetVisible(i, 0);
		}
	}
}
