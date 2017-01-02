// Copyright © 2011, Zero and One Computing Inc. All rights reserved.
// Distributed under terms of the Modified BSD License, see license.txt for details.
//
// ExaminerVisitorGUI.cs
//
// Visits game objects, components, etc. for display in the Examiner window.
//
// History:
// version 1.0 - January 2010 - Yossarian King

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEditor;

namespace ZeroAndOne.UnityEditor
{
	public class ExaminerVisitorGUI : SceneVisitor
	{
        // Lookup from object to foldout state, for controlling the view of the tree.
        private Dictionary<object, bool> mObjectFoldoutState = new Dictionary<object, bool>();

        public void Reset()
        {
            mObjectFoldoutState.Clear();
        }
		
		public void Expand(object obj, bool expand)
		{
			mObjectFoldoutState[obj] = expand;
		}
 
        private bool FoldoutObject(UnityEngine.Object obj)
        {
            // Early out for null object -- e.g. missing component.
            if (obj == null)
            {
                Foldout(false, new GUIContent("Missing"));
                return false;
            }

            // Get name and icon for this object.
            GUIContent content = EditorGUIUtility.ObjectContent(obj, null);
            
            if (obj is GameObject)
            {
                // Skip icons for game objects, for consistency with Hierarchy view
                content.image = null;
            }
            else
            {
                // Get prettier name for Components.
                content.text = ObjectNames.GetInspectorTitle(obj);
            }

            if ((obj != null) && !mObjectFoldoutState.ContainsKey(obj))
            {
                mObjectFoldoutState[obj] = false;
            }

            mObjectFoldoutState[obj] = Foldout(mObjectFoldoutState[obj], content);
            return mObjectFoldoutState[obj];
        }

        private bool Foldout(bool state, GUIContent content)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(this.Depth * 12);
            state = EditorGUILayout.Foldout(state, content);
            GUILayout.EndHorizontal();

            return state;
        }
        
        private void ShowField(string name, System.Type type, object val)
        {
            name = ObjectNames.NicifyVariableName(name);

            GUIContent content = null;
            if (val == null)
            {
                content = new GUIContent("null");
            }
            else if ((val as string) == String.Empty)
            {
                // Show "" instead of empty space.
                content = new GUIContent("\"\"");
            }
            else
            {
                UnityEngine.Object unityObjectVal = val as UnityEngine.Object;
                if (unityObjectVal != null)
                {
                    content = EditorGUIUtility.ObjectContent(unityObjectVal, null);

                    // If asset, then display its path. (TextAssets get messy real quick!!)
                    string path = AssetDatabase.GetAssetPath(unityObjectVal);
                    if (!String.IsNullOrEmpty(path))
                    {
                        content.text = path;
                    }
                    else
                    {
                        // For objects from the Hierarchy, show the name.
                        content.text = ObjectNames.GetInspectorTitle(unityObjectVal);
                    }
                }
                else
                {
                    content = new GUIContent(val.ToString());
                }
            }

            // Nothing fancy here, just rely on each type to do its own string conversion.
            EditorGUILayout.BeginHorizontal();
            int space = (this.Depth + 1) * 12;   // Extra indent to get below component name, not just foldout triangle.
            GUILayout.Space(space);
            GUILayout.Label(name, GUILayout.Width((Screen.width - space) * 0.3f));
            GUILayout.Label(content);
            EditorGUILayout.EndHorizontal();
        }

        protected override bool Arrive(GameObject gameObject)
        {
            return FoldoutObject(gameObject);
        }

        protected override bool Arrive(Component component, GameObject owner)
        {
            return FoldoutObject(component);
        }
        
        protected override bool Arrive(UnityEngine.Object unityObject)
        {
            return FoldoutObject(unityObject);
        }
        
        protected override void Arrive(FieldInfo fi, object obj, object val)
        {
            ShowField(fi.Name, fi.FieldType, val);
        }
        
        protected override void Arrive(PropertyInfo pi, object obj, object val)
        {
            ShowField(pi.Name, pi.PropertyType, val);
        }
    }
}
