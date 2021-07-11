using UnityEditor;
using UnityEngine;

public class ComplexScriptCreateWindow : EditorWindow {

    [MenuItem("Assets/Create/Create Complex Script")]
    private static void AddWindow() {
        Rect windowRect = new Rect(400, 300, 300, 100);
        ComplexScriptCreateWindow window = (ComplexScriptCreateWindow)GetWindowWithRect(typeof(ComplexScriptCreateWindow), windowRect, true, "脚本创建参数设置");
        window.Show();
    }

    private string scriptName;
    private string inheriteClassName;
    private string interfaceName;

    private void OnGUI() {
        var options = new[] { GUILayout.Width(160), GUILayout.Height(20) };
        scriptName = EditorGUILayout.TextField("输入脚本名称:", scriptName);
        inheriteClassName = EditorGUILayout.TextField("输入继承类名:", inheriteClassName);
        interfaceName = EditorGUILayout.TextField("输入接口名:", interfaceName);
        EditorGUILayout.LabelField("*所有接口名之间用逗号区分", options);
        if (GUILayout.Button("创建")) {
            CreateScript(scriptName, inheriteClassName, interfaceName);
            Close();
        }
    }

    private static void CreateScript(string scriptName, string inheriteClassName, string interfaceName) {
        var resourceFile = Application.dataPath + "/Plugins/MyPlugins/GeneralScriptTemplate.cs.txt";
        Texture2D csIcon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
        var endNameEditAction = CreateInstance<ComplexScriptCreateAction>();
        endNameEditAction.SetupNames(inheriteClassName, interfaceName);
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, endNameEditAction, scriptName + ".cs", csIcon, resourceFile);
    }
}
