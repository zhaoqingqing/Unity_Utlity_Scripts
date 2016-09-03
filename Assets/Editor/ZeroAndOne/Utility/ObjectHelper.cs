// Copyright © 2011, Zero and One Computing Inc. All rights reserved.
// Distributed under terms of the Modified BSD License, see license.txt for details.
//
// ObjectHelper.cs - various scene querying utilities
//
// History:
// version 1.0 - January 2011 - Yossarian King

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace ZeroAndOne.UnityEditor
{
    // Static class with various helper methods for scene traversal and object finding.
    public static class ObjectHelper
    {
        // Find all game objects at the root of the scene.
        public static GameObject[] FindRootGameObjects()
        {
            List<GameObject> rootObjects = new List<GameObject>();
            foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (obj.transform.parent == null)
                {
                    rootObjects.Add(obj);
                }
            }
            
            return rootObjects.ToArray();
        }
        
        // Get all of the specified objects which are component MonoScript assets (i.e. those which
        // implement MonoBehaviour).
        public static MonoScript[] GetComponentMonoScripts(IEnumerable<UnityEngine.Object> objects)
        {
            List<MonoScript> scripts = new List<MonoScript>();

			foreach (UnityEngine.Object unityObject in objects)
			{
                MonoScript script = unityObject as MonoScript;
				if (script == null)
					continue;
				
                Type componentType = script.GetClass();
                if (componentType == null)
                    continue;
                
                scripts.Add(script);
            }
            
            return scripts.ToArray();
        }
        
        // Find all game objects in the scene which use the specified scripts.
		public static GameObject[] FindGameObjectsUsingScripts(IEnumerable<UnityEngine.Object> scriptObjects)
		{
            List<GameObject> gameObjectsUsingScripts = new List<GameObject>();
            GameObject[] sceneRoots = FindRootGameObjects();

			foreach (MonoScript script in GetComponentMonoScripts(scriptObjects))
			{
                Type componentType = script.GetClass();

				// Look for dependencies, starting with all objects at root of scene.
                foreach (GameObject gameObj in sceneRoots)
                {
                    foreach (Component component in gameObj.GetComponentsInChildren(componentType, true))
                    {
                        if (!gameObjectsUsingScripts.Contains(component.gameObject))
                        {
                            gameObjectsUsingScripts.Add(component.gameObject);
                        }
                    }
                }
            }
                
            return gameObjectsUsingScripts.ToArray();
		}

        // Find all game objects in the scene that refer to scripts which no longer exist.
		public static GameObject[] FindGameObjectsWithMissingScripts()
		{
            return FindGameObjectsWithMissingScripts(FindRootGameObjects());
        }

        // Find all game objects in the specified list (or descendants) that refer to scripts which no longer exist.
		public static GameObject[] FindGameObjectsWithMissingScripts(IEnumerable<GameObject> gameObjects)
		{
            MissingComponentFinder finder = new MissingComponentFinder();
            finder.Traverse(gameObjects);
            return finder.GameObjectsWithMissingComponents;
        }
        
        // Scene visitor that accumulates list of objects with missing components.
        private class MissingComponentFinder : SceneVisitor
        {
            private List<GameObject> mGameObjectsWithMissingComponents = new List<GameObject>();
            
            public GameObject[] GameObjectsWithMissingComponents
            {
                get
                {
                    return mGameObjectsWithMissingComponents.ToArray();
                }
            }
            
            protected override bool Arrive(Component component, GameObject owner)
            {
                if (component == null)
                {
                    if (!mGameObjectsWithMissingComponents.Contains(owner))
                    {
                        mGameObjectsWithMissingComponents.Add(owner);
                    }
                }
                
                return false;   // Don't dig down into properties.
            }
        }
    }
}
