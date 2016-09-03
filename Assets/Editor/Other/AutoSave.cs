using UnityEngine;
using UnityEditor;
using System;
 
public class AutoSave : EditorWindow {
 
	private bool autoSaveScene = true;
	private bool showMessage = true;
	private bool isStarted = false;
	private int intervalScene;	
	private DateTime lastSaveTimeScene = DateTime.Now;
 
	private string projectPath = Application.dataPath;
	private string scenePath;
 
	[MenuItem ("Tools/程序/保存/自动保存场景")]
	static void Init () {
		AutoSave saveWindow = (AutoSave)EditorWindow.GetWindow (typeof (AutoSave));
		saveWindow.Show();
	}
 
	void OnGUI () {	
		GUILayout.Label ("Info:", EditorStyles.boldLabel);
		EditorGUILayout.LabelField ("Saving to:", ""+projectPath);
		EditorGUILayout.LabelField ("Saving scene:", ""+scenePath);
		GUILayout.Label ("Options:", EditorStyles.boldLabel);
		autoSaveScene = EditorGUILayout.BeginToggleGroup ("Auto save", autoSaveScene);
		intervalScene = EditorGUILayout.IntSlider ("Interval (minutes)", intervalScene, 1, 10);
		if(isStarted) {
			EditorGUILayout.LabelField ("Last save:", ""+lastSaveTimeScene);
		}
		EditorGUILayout.EndToggleGroup();
		showMessage = EditorGUILayout.BeginToggleGroup ("Show Message", showMessage);
		EditorGUILayout.EndToggleGroup ();
	}
 
 
	void Update(){
		scenePath = EditorApplication.currentScene;
		if(autoSaveScene) {
			if(DateTime.Now.Minute >= (lastSaveTimeScene.Minute+intervalScene) || DateTime.Now.Minute == 59 && DateTime.Now.Second == 59){
				saveScene();
			}
		} else {
			isStarted = false;
		}
 
	}
 
	void saveScene() {
		EditorApplication.SaveScene(scenePath);
		lastSaveTimeScene = DateTime.Now;
		isStarted = true;
		if(showMessage){
			Debug.Log("AutoSave saved: "+scenePath+" on "+lastSaveTimeScene);
		}
		AutoSave repaintSaveWindow = (AutoSave)EditorWindow.GetWindow (typeof (AutoSave));
		repaintSaveWindow.Repaint();
	}
}