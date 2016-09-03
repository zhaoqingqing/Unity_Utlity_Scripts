using UnityEngine;
using UnityEditor;

public class GenScriptableObject
{
    [MenuItem("Tools/程序/生成/模板配置表")]
    static void Generate()
    {
		var asset = ScriptableObject.CreateInstance<ScriptableObject>();
        ProjectWindowUtil.CreateAsset(asset, "templatetable.asset");
    }
}