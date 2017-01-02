// Copyright © 2011, Zero and One Computing Inc. All rights reserved.
// Distributed under terms of the Modified BSD License, see license.txt for details.
//
// SceneVisitor.cs
//
// Visits game objects, components, other objects, and their properties.
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
    // SceneVisitor handles scene/object traversal, and provides virtual
    // Arrive methods for derived classes to override.
	public abstract class SceneVisitor
	{
        [Flags]
        public enum Options
        {
            GameObjects         = 1 << 0,       // Traverse the game objects.
            Components          = 1 << 1,       // Traverse the components of game objects.
            OtherUnityObjects   = 1 << 2,       // Traverse other UnityEngine Objects (such as assets).
            SystemObjects       = 1 << 3,       // Traverse any C# object.

            Fields              = 1 << 4,       // Traverse the fields of objects.
            Properties          = 1 << 5,       // Traverse the properties of objects.
            Methods             = 1 << 6,       // Traverse the methods of objects.

            Recursive           = 1 << 7,       // Traverse hierarchy.

            // UnityScript terminology.
            Variables = Fields,
            Functions = Methods,
            
            AllUnityObjects = GameObjects | Components | OtherUnityObjects,
            AllObjects = AllUnityObjects | SystemObjects,
            AllMembers = Fields | Properties | Methods,
            
            All = AllObjects | AllMembers | Recursive,
            
            Default = GameObjects | Components | Fields | Recursive     // Default is equivalent to what you can see in Hierarchy + Inspector.
        }
        
        // BindingFlags controls which fields/methods/properties will be visited.
        public BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.Instance;
        
        // Tracks hierarchy depth scene is visited.
        protected int Depth = 0;

        public void TraverseHierarchy()
        {
            TraverseHierarchy(Options.Default);
        }
        
        public void TraverseHierarchy(Options options)
        {
            Traverse(ObjectHelper.FindRootGameObjects(), options);
        }

        public void TraverseSelection()
        {
            TraverseSelection(Options.Default);
        }

        public void TraverseSelection(Options options)
        {
            Traverse(Selection.objects, options);
        }
        
        public void Traverse(IEnumerable objects)
        {
            Traverse(objects, Options.Default);
        }

        public void Traverse(IEnumerable objects, Options options)
        {
            foreach (object obj in objects)
            {
                TraverseObject(obj, options);
            }
        }

        public void TraverseObject(object obj, Options options)
        {
            if (obj is GameObject)
            {
                if (IsSet(options, Options.GameObjects))
                {
                    TraverseGameObject(obj as GameObject, options);
                }
            }
            else if (obj is Component)
            {
                if (IsSet(options, Options.Components))
                {
                    Component component = obj as Component;
                    TraverseComponent(component, component.gameObject, options);
                }
            }
            else if (obj is UnityEngine.Object)
            {
                if (IsSet(options, Options.OtherUnityObjects))
                {
                    TraverseOtherUnityObject(obj as UnityEngine.Object, options);
                }
            }
            else
            {
                if (IsSet(options, Options.SystemObjects))
                {
                    TraverseSystemObject(obj, options);
                }
            }
        }


        #region // private -- traversal implementation
        private void TraverseGameObject(GameObject gameObject, Options options)
        {
            // Does visitor want to stay?
            if (!Arrive(gameObject))
                return;

            ++Depth;
            
            if (IsSet(options, Options.Components))
            {
                foreach (Component component in gameObject.GetComponents<Component>())
                {
                    TraverseComponent(component, gameObject, options);
                }
            }
            
            if (IsSet(options, Options.Recursive))
            {
                foreach (Transform child in gameObject.transform)
                {
                    TraverseGameObject(child.gameObject, options);
                }
            }
            
            --Depth;
        }
        
        private void TraverseComponent(Component component, GameObject owner, Options options)
        {
            // Does visitor want to stay?
            if (!Arrive(component, owner))
                return;

            ++Depth;
            TraverseObjectMembers(component, options);
            --Depth;
        }
        
        private void TraverseOtherUnityObject(UnityEngine.Object unityObject, Options options)
        {
            // Does visitor want to stay?
            if (!Arrive(unityObject))
                return;

            ++Depth;
            TraverseObjectMembers(unityObject, options);
            --Depth;
        }
        
        private void TraverseSystemObject(object obj, Options options)
        {
            // Does visitor want to stay?
            if (!Arrive(obj))
                return;

            ++Depth;
            TraverseObjectMembers(obj, options);
            --Depth;
        }
        
        private void TraverseObjectMembers(object obj, Options options)
        {
            if (IsSet(options, Options.Fields))
            {
                foreach (FieldInfo fi in obj.GetType().GetFields(this.BindingFlags | BindingFlags.GetField))
                {
                    try
                    {
                        Arrive(fi, obj, fi.GetValue(obj));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.ToString());
                    }
                }
            }

            if (IsSet(options, Options.Properties))
            {
                foreach (PropertyInfo pi in obj.GetType().GetProperties(this.BindingFlags | BindingFlags.GetProperty))
                {
                    object val = null;
                    
                    try
                    {
                        // For non-indexed properties we get the value.
                        if (pi.GetIndexParameters().Length == 0)
                        {
                            val = pi.GetValue(obj, null);
                        }
                        Arrive(pi, obj, val);
                    }
                    catch (Exception ex)
                    {
                        // TODO: Seems like we never get here -- Unity's already logged the exception?
                        Arrive(pi, obj, ex.Message);
                    }
                }
            }

            if (IsSet(options, Options.Methods))
            {
                foreach (MethodInfo mi in obj.GetType().GetMethods(this.BindingFlags))
                {
                    Arrive(mi, obj);
                }
            }
        }
        #endregion
        
        #region // protected -- concrete visitors override what they're interested in
        protected virtual bool Arrive(GameObject gameObject) { return true; }
        protected virtual bool Arrive(Component component, GameObject owner) { return true; }
        protected virtual bool Arrive(UnityEngine.Object unityObject) { return true; }
        protected virtual bool Arrive(object obj) { return true; }

        protected virtual void Arrive(FieldInfo fi, object owner, object val) {}
        protected virtual void Arrive(PropertyInfo pi, object owner, object val) {}
        protected virtual void Arrive(MethodInfo mi, object owner) {}
        #endregion
        
        // Helper method for testing option flags.
        public static bool IsSet(Options options, Options option)
        {
            return (options & option) == option;
        }
	}
}
