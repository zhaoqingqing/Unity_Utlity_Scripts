// Copyright © 2011, Zero and One Computing Inc. All rights reserved.
// Distributed under terms of the Modified BSD License, see license.txt for details.
//
// ExaminerVisitorDump.cs
//
// Visits game objects, components, etc. for dumping to file or string.
//
// History:
// version 1.0 - January 2010 - Yossarian King

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEditor;

namespace ZeroAndOne.UnityEditor
{
	public class ExaminerVisitorDump : SceneVisitor
	{
        public const string cDefaultOutputFile = "unity-scene-dump.txt";
    
        private const string cIndentString = "  ";
        private TextWriter mWriter = null;

        public void DumpToFile(IEnumerable objects, SceneVisitor.Options options, string filename)
        {
            // Make sure we have a filename to output to.
            if (String.IsNullOrEmpty(filename))
            {
                filename = cDefaultOutputFile;
            }

            // If no folder specified then output to the Assets folder.
            if (!Path.IsPathRooted(filename))
            {
                filename = Path.Combine(Application.dataPath, filename);
            }
            
            // Normalize slashes in path.
            filename = filename.Replace(@"\", @"/");

            using (mWriter = new StreamWriter(filename))
            {
                Traverse(objects, options);
            }
            mWriter = null;
        }
        
        public string DumpToString(IEnumerable objects, SceneVisitor.Options options)
        {
            string results = null;
            using (StringWriter stringWriter = new StringWriter())
            {
                mWriter = stringWriter;
                Traverse(objects, options);
                results = mWriter.ToString();
            }
            
            mWriter = null;
            
            return results;
        }

        private void DumpLine(string line)
        {
            for (int i = 0; i < this.Depth; ++i)
            {
                mWriter.Write(cIndentString);
            }
            mWriter.WriteLine(line);
        }

        private void DumpObject(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                DumpLine("Missing");
            }
            else
            {
                DumpLine(ObjectNames.GetInspectorTitle(obj));
            }
        }

        private void DumpField(string name, object val)
        {
            name = ObjectNames.NicifyVariableName(name);
            if (val == null)
            {
                val = "null";
            }
            if ((val as string) == String.Empty)
            {
                val = "\"\"";
            }

            UnityEngine.Object unityObjectVal = val as UnityEngine.Object;
            if (unityObjectVal != null)
            {
                // If asset, then display its path. (TextAssets get messy real quick!!)
                string path = AssetDatabase.GetAssetPath(unityObjectVal);
                if (!String.IsNullOrEmpty(path))
                {
                    val = path;
                }
                else
                {
                    // For objects from the Hierarchy, show the name.
                    val = String.Format("{0}({1})", unityObjectVal.name, unityObjectVal.GetType().Name);
                }
            }
     
            // Nothing fancy here, just rely on each type to do its own string conversion.
            DumpLine(String.Format("{0}={1}", name, val.ToString()));
        }

        protected override bool Arrive(GameObject gameObject)
        {
            DumpObject(gameObject);
            return true;
        }

        protected override bool Arrive(Component component, GameObject owner)
        {
            DumpObject(component);
            return true;
        }
        
        protected override void Arrive(FieldInfo fi, object obj, object val)
        {
            DumpField(fi.Name, val);
        }
        
        protected override void Arrive(PropertyInfo pi, object obj, object val)
        {
            DumpField(pi.Name, val);
        }
    }
}
