// Copyright © 2011, Zero and One Computing Inc. All rights reserved.
// Distributed under terms of the Modified BSD License, see license.txt for details.
//
// Find.SelectedScript.cs - find game objects using selected script(s)
//
// History:
// version 1.0 - January 2011 - Yossarian King

using System;
using System.Text;

using UnityEngine;
using UnityEditor;

namespace ZeroAndOne.UnityEditor
{
    public static class FindScriptMenuItem
    {
        // For each script selected in the Project view, find and select
        // the game objects that use it in the Hierarchy view. Log a message
        // to the console with the details.
        [MenuItem("Tools/≥Ã–Ú/≤È’“/≥°æ∞÷– π”√—°÷–Ω≈±æµƒ∂‘œÛ &#f")]
        private static void FindScriptMenuItemHandler()
        {
            if ((Selection.objects == null) || (Selection.objects.Length == 0))
            {
                return;
            }
            
            string description = "one of these scripts";
            if (Selection.objects.Length == 1)
            {
                description = Selection.objects[0].name;
            }

            // Find all game objects using any currently selected scripts.
            GameObject[] scriptUsers = ObjectHelper.FindGameObjectsUsingScripts(Selection.objects);

            if (scriptUsers.Length == 0)
            {
                StringBuilder message = new StringBuilder();
                message.AppendFormat("Found 0 objects using {0} ...\n", description);
                foreach (UnityEngine.Object obj in Selection.objects)
                {
                    message.AppendFormat("    {0}({1})\n", obj.name, obj.GetType().Name);
                }

                message.AppendLine("Be sure to select script(s) in the Project view, and note that only scripts which implement MonoBehaviour will be found.");
                Debug.LogWarning(message.ToString());
            }
            else
            {
                // Select the identified objects.
                Selection.objects = scriptUsers;

                foreach (GameObject gameObject in scriptUsers)
                {
                    Debug.Log(String.Format("{0} uses {1}.", gameObject.name, description), gameObject);
                }
            }
        }
    }
}
