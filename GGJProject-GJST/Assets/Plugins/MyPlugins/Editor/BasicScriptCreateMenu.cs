using UnityEngine;
using UnityEditor;

public class BasicScriptCreateMenu : ScriptableObject {
    [MenuItem("Assets/Create/Create Basic Script")]
    private static void CreateScript() {
        var resourceFile = Application.dataPath + "/Plugins/MyPlugins/BasicScriptTemplate.cs.txt";
        Texture2D csIcon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
        var endNameEditAction = CreateInstance<BasicScriptCreateAction>();
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, endNameEditAction,  "BasicScript.cs", csIcon, resourceFile);
    }
}
