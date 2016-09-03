// Copyright © 2011, Zero and One Computing Inc. All rights reserved.
// Distributed under terms of the Modified BSD License, see license.txt for details.
//
// ExaminerWindow.cs - detailed inspection of Unity scenes
//
// History:
// version 1.0 - December 2010 - Yossarian King (SceneDumper)
// version 2.0 - December 2010 - Yossarian King - converted to editor window; rewrote most of it

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

using UnityEngine;
using UnityEditor;

// ExaminerWindow adds a custom panel to the Editor user interface.
public class ExaminerWindow : ZeroAndOne.UnityEditor.ExaminerWindow
{
    [MenuItem("Tools/³ÌÐò/²é¿´/¶ÔÏó²é¿´Æ÷ %#E")]
    static void Init()
    {
        // Create and/or focus the window.
        EditorWindow.GetWindow(typeof(ExaminerWindow), false, "¶ÔÏó²é¿´Æ÷");
    }
}

// You can't put an EditorWindow in a namespace, but you can put all the implementation in one!
namespace ZeroAndOne.UnityEditor
{
    public class ExaminerWindow : EditorWindow
    {
        // Objects currently being dumped -- generally same as Selection.objects.
        private List<object> mObjectsToExamine = new List<object>();
        
        // Other options.
        private SceneVisitor.Options mSceneVisitorOptions = SceneVisitor.Options.Default | SceneVisitor.Options.OtherUnityObjects;
        private bool mFollowSelection = true;
        private string mOutputFile = ExaminerVisitorDump.cDefaultOutputFile;
        private Vector2 mScrollViewPosition = Vector2.zero;

        // Scene visitors for GUI construction and dumping to file.
        private ExaminerVisitorGUI mGUIVisitor = new ExaminerVisitorGUI();
        private ExaminerVisitorDump mDumpVisitor = new ExaminerVisitorDump();
        
        public ExaminerWindow() : base()
        {
            OnSelectionChange();
        }
 
        void OnSelectionChange()
        {
            if (mFollowSelection)
            {
                Clear();
                Add(Selection.objects);
                Repaint();
            }
        }
        
        void Clear()
        {
            mObjectsToExamine.Clear();
            mGUIVisitor.Reset();
        }
        
        void Add(IEnumerable objects)
        {
			Add(objects, false);
		}
		
        void Add(IEnumerable objects, bool expand)
        {
            foreach (object obj in objects)
            {
				if (!mObjectsToExamine.Contains(obj))
				{
                	mObjectsToExamine.Add(obj);
				}
				mGUIVisitor.Expand(obj, expand);
            }
        }

        void OnGUI()
        {
            if ((Event.current.type == EventType.DragUpdated) || (Event.current.type == EventType.DragPerform))
            {
                DoDragDrop(DragAndDrop.objectReferences);
            }
            else
            {
                DoUI();
            }
        }
        
        private void DoDragDrop(UnityEngine.Object[] objects)
        {
            if (Event.current.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy; // Show a drag-add icon on the mouse cursor.
                Event.current.Use();
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                Event.current.Use();

                Add(objects);
            }
        }

        private void DoUI()
        {
            DoUIToolbar();
            
            if (mObjectsToExamine.Count == 0)
            {
                GUILayout.Label("\n\n    You can drag game objects or assets onto here.");
                return;
            }

            // Scrolling output in GUI.
            mScrollViewPosition = EditorGUILayout.BeginScrollView(mScrollViewPosition);
            
            // GUI visitor puts everything to the UI.
            mGUIVisitor.Traverse(mObjectsToExamine, mSceneVisitorOptions);
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DoUIToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            // If not enough space for full text of toolbar buttons then shorten them.
            // TODO: are pixel sizes consistent across versions and platforms?
            // Nope: toolbar takes less space in Unity 2.6 than in 3.1 (on Windows, not sure about Mac)
            bool useShortNames = (Screen.width < 530);

            if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                Clear();
            }

            GUILayout.Label(String.Empty, EditorStyles.toolbarButton, GUILayout.Width(8));  // Pseudo-separator.

            bool wasFollowing = mFollowSelection;
            mFollowSelection = GUILayout.Toggle(mFollowSelection, (useShortNames ? "Follow" : "Follow selection"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
            if (mFollowSelection && !wasFollowing)
            {
                OnSelectionChange();
            }

            GUILayout.Label(String.Empty, EditorStyles.toolbarButton, GUILayout.Width(8));  // Pseudo-separator.

            SceneVisitorOptionToggle((useShortNames ? "O" : "Objects"), SceneVisitor.Options.GameObjects);
            SceneVisitorOptionToggle((useShortNames ? "C" : "Components"), SceneVisitor.Options.Components);
            SceneVisitorOptionToggle((useShortNames ? "A" : "Assets"), SceneVisitor.Options.OtherUnityObjects);
            SceneVisitorOptionToggle((useShortNames ? "V" : "Variables"), SceneVisitor.Options.Fields);
            SceneVisitorOptionToggle((useShortNames ? "D" : "Debug"), SceneVisitor.Options.Properties);

            EditorGUILayout.Separator();

            if (GUILayout.Button((useShortNames ? "Dump" : "Dump to file"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                DumpToOutputFile();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void SceneVisitorOptionToggle(string name, SceneVisitor.Options option)
        {
            bool toggle = SceneVisitor.IsSet(mSceneVisitorOptions, option);
            toggle = GUILayout.Toggle(toggle, name, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
            if (toggle)
            {
                mSceneVisitorOptions |= option;
            }
            else
            {
                mSceneVisitorOptions &= ~option;
            }
        }
		
        private void DumpToOutputFile()
        {
            string folder = Path.GetDirectoryName(mOutputFile);
            if (String.IsNullOrEmpty(folder))
            {
                folder = Application.dataPath;
            }
            string selected = EditorUtility.SaveFilePanel("Select Output File", folder, mOutputFile, "txt");
            if (!string.IsNullOrEmpty(selected))
            {
                mDumpVisitor.DumpToFile(mObjectsToExamine, mSceneVisitorOptions, selected);

                // Show relative to asset folder.
                if (selected.StartsWith(Application.dataPath))
                {
                    selected = selected.Substring(Application.dataPath.Length + 1); // Strip off path and slash.
                }
                mOutputFile = selected;
                
                Debug.Log("Scene dumped to " + mOutputFile);
            }
        }
    }
}
