// File create date:2019/11/18
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
// Created By Yu.Liu
public class ViewCodeGenerateWindow : EditorWindow {

    [MenuItem("Assets/View Code Generate")]
    private static void NewWindow() {
        Rect windowRect = new Rect(400, 300, 960, 690);
        ViewCodeGenerateWindow window = (ViewCodeGenerateWindow)GetWindowWithRect(typeof(ViewCodeGenerateWindow), windowRect, true, "脚本自动生成");
        window.Show();
    }

    [MenuItem("GameObject/View Code Generate", false, 10)]
    private static void AddWindow(MenuCommand menuCommand) {
        Rect windowRect = new Rect(400, 300, 960, 690);
        ViewCodeGenerateWindow window = (ViewCodeGenerateWindow)GetWindowWithRect(typeof(ViewCodeGenerateWindow), windowRect, true, "脚本自动生成");
        window.SetupViewRoot(menuCommand.context as GameObject);
        window.Show();
    }

    private static ViewCodeGenerateWindow codeWindow;
    private static string cacheScriptPath;

    private SerializedObject serializedObj;
    private GameObject lastRoot;
    private GameObject viewRoot;
    private string[] viewType = { "BaseView", "ViewWindow" };
    private string selectType = "BaseView";
    private List<UIWidgetItem> widgetList = new List<UIWidgetItem>();
    private Vector2 widgetListPos;
    private List<UIWidgetItem> objectList = new List<UIWidgetItem>();
    private Vector2 objectListPos;
    private Color selectItemColor;
    private Color unselectItemColor;

    private string scriptName;
    private string scriptInterface;

    private string scriptPath;

    private StringBuilder codeBuilder = new StringBuilder();
    private List<UIWidgetItem> indexWidgets = new List<UIWidgetItem>();

    private void OnEnable() {
        serializedObj = new SerializedObject(this);
        selectItemColor = Color.blue;
        selectItemColor.a = 0.25f;
        unselectItemColor = Color.red;
        unselectItemColor.a = 0.25f;
    }

    private void OnGUI() {
        serializedObj.Update();
        if (codeWindow == null) {
            codeWindow = GetWindow<ViewCodeGenerateWindow>();
        }
        DrawTitleArea();
        using (new EditorGUILayout.VerticalScope()) {
            GUI.backgroundColor = Color.white;
            EditorGUILayout.LabelField("系统控件列表：");
            DrawWidgetList();
            EditorGUILayout.LabelField("其它对象列表：");
            DrawObjectList();
            EditorGUILayout.Space();
            DrawFunctionArea();
        }
    }

    public void SetupViewRoot(GameObject root) {
        viewRoot = root;
    }

    private void DrawTitleArea() {
        // 标题部分，包括根节点，脚本名称，父类以及接口
        using (EditorGUILayout.VerticalScope vScope = new EditorGUILayout.VerticalScope(GUI.skin.box)) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("生成代码的根节点:", GUILayout.Width(100f));
            viewRoot = EditorGUILayout.ObjectField(viewRoot, typeof(GameObject), true) as GameObject;
            if (viewRoot != null && lastRoot != viewRoot) {
                // 不同的根节点，刷新数据
                UpdateDataList();
                scriptName = viewRoot.name;
                scriptInterface = "";
                lastRoot = viewRoot;
            }
            scriptPath = cacheScriptPath;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("脚本名称:", GUILayout.Width(100f));
            scriptName = EditorGUILayout.TextField(scriptName);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("上级类型:", GUILayout.Width(100f));
            //scriptParent = EditorGUILayout.TextField(scriptParent);
            GUIContent content = new GUIContent(selectType);
            if (EditorGUILayout.DropdownButton(content, FocusType.Keyboard)) {
                GenericMenu menu = new GenericMenu();
                foreach (string item in viewType) {
                    GUIContent itemContent = new GUIContent(item);
                    menu.AddItem(itemContent, selectType.Equals(item), OnValueSelected, item);
                }
                menu.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("实现接口:", GUILayout.Width(100f));
            scriptInterface = EditorGUILayout.TextField(scriptInterface);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
    }

    private void OnValueSelected(object value) {
        selectType = value.ToString();
    }

    private void UpdateDataList() {
        widgetList.Clear();
        objectList.Clear();
        Queue<SearchCell> searchQueue = new Queue<SearchCell>();
        if (viewRoot.transform.childCount > 0) {
            for (int i = 0; i < viewRoot.transform.childCount; i++) {
                searchQueue.Enqueue(new SearchCell(viewRoot.transform.GetChild(i), ""));
            }
        }
        while (searchQueue.Count > 0) {
            SearchCell cursorCell = searchQueue.Dequeue();
            UIWidgetItem widget;
            if (GenerateSingleWidget(cursorCell, out widget)) {
                // 有后继节点
                for (int i = 0; i < cursorCell.root.childCount; i++) {
                    searchQueue.Enqueue(new SearchCell(cursorCell.root.GetChild(i), widget.path));
                }
            }
            if (widget.type == ViewCodeConfigs.TYPE_NAME_GAME_OBJECT) {
                // 这部分没有控件或者没有系统控件的对象放到另一个表
                objectList.Add(widget);
            } else {
                widgetList.Add(widget);
            }
        }
    }

    private bool GenerateSingleWidget(SearchCell cell, out UIWidgetItem widget) {
        widget = new UIWidgetItem();
        widget.isSelected = false;
        widget.genEvent = false;
        widget.name = cell.root.name;
        widget.path = $"{cell.parentPath}/{cell.root.name}";
        if (cell.root.GetComponent(ViewCodeConfigs.TYPE_BUTTON) != null) {
            // 存在Button控件
            widget.type = ViewCodeConfigs.TYPE_NAME_BUTTON;
        } else if (cell.root.GetComponent(ViewCodeConfigs.TYPE_TOGGLE) != null) {
            // 存在Toggle控件
            widget.type = ViewCodeConfigs.TYPE_NAME_TOGGLE;
        } else if (cell.root.GetComponent(ViewCodeConfigs.TYPE_DROPDOWN) != null) {
            // 存在Dropdown控件
            widget.type = ViewCodeConfigs.TYPE_NAME_DROPDOWN;
        } else if (cell.root.GetComponent(ViewCodeConfigs.TYPE_SLIDER) != null) {
            // 存在Slider控件
            widget.type = ViewCodeConfigs.TYPE_NAME_SLIDER;
        } else if (cell.root.GetComponent(ViewCodeConfigs.TYPE_INPUT_FIELD) != null) {
            // 存在InputField控件
            widget.type = ViewCodeConfigs.TYPE_NAME_INPUT_FIELD;
        } else if (cell.root.GetComponent(ViewCodeConfigs.TYPE_SCROLL_RECT) != null) {
            // 存在ScrollRect控件
            widget.type = ViewCodeConfigs.TYPE_NAME_SCROLL_RECT;
        } else if (cell.root.GetComponent(ViewCodeConfigs.TYPE_IMAGE) != null) {
            // 存在Image控件
            widget.type = ViewCodeConfigs.TYPE_NAME_IMAGE;
        } else if (cell.root.GetComponent(ViewCodeConfigs.TYPE_RAW_IMAGE) != null) {
            // 存在RawImage控件
            widget.type = ViewCodeConfigs.TYPE_NAME_RAW_IMAGE;
        } else if (cell.root.GetComponent(ViewCodeConfigs.TYPE_TEXT) != null) {
            // 存在Text控件
            widget.type = ViewCodeConfigs.TYPE_NAME_TEXT;
        } else if (cell.root.GetComponent(ViewCodeConfigs.TYPE_CANVAS_GROUP) != null) {
            // 存在CanvasGroup控件
            widget.type = ViewCodeConfigs.TYPE_NAME_CANVAS_GROUP;
        } else {
            // 没有控件，就是普通对象了，如果有特殊需求自行设置
            widget.type = ViewCodeConfigs.TYPE_NAME_GAME_OBJECT;
        }
        widget.varName = $"{widget.type.ToLower()}_{cell.root.name}";
        return cell.root.childCount > 0;
    }

    private void DrawWidgetList() {
        // 控件列表
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        EditorGUILayout.LabelField("", GUILayout.Width(2f));
        EditorGUILayout.LabelField("S", GUILayout.Width(16f));
        EditorGUILayout.LabelField("名称", GUILayout.Width(100f));
        EditorGUILayout.LabelField("变量名", GUILayout.Width(100f));
        EditorGUILayout.LabelField("类型", GUILayout.Width(100f));
        EditorGUILayout.LabelField("跳转", GUILayout.Width(40f));
        EditorGUILayout.LabelField("路径");
        EditorGUILayout.EndHorizontal();
        widgetListPos = EditorGUILayout.BeginScrollView(widgetListPos, GUILayout.Height(180f));
        for (int i = 0; i < widgetList.Count; i++) {
            DrawSingleWidget(widgetList[i]);
        }
        if (viewRoot == null) {
            EditorGUILayout.HelpBox("没有根节点", MessageType.Info);
        } else {
            if (widgetList.Count <= 0) {
                EditorGUILayout.HelpBox("找不到指定物体", MessageType.Info);
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        GUI.backgroundColor = Color.white;
    }

    private void DrawObjectList() {
        // 对象列表
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        EditorGUILayout.LabelField("", GUILayout.Width(2f));
        EditorGUILayout.LabelField("S", GUILayout.Width(16f));
        EditorGUILayout.LabelField("名称", GUILayout.Width(100f));
        EditorGUILayout.LabelField("变量名", GUILayout.Width(100f));
        EditorGUILayout.LabelField("类型", GUILayout.Width(100f));
        EditorGUILayout.LabelField("跳转", GUILayout.Width(40f));
        EditorGUILayout.LabelField("路径");
        EditorGUILayout.EndHorizontal();
        objectListPos = EditorGUILayout.BeginScrollView(objectListPos, GUILayout.Height(180f));
        for (int i = 0; i < objectList.Count; i++) {
            DrawSingleObject(objectList[i]);
        }
        if (viewRoot == null) {
            EditorGUILayout.HelpBox("没有根节点", MessageType.Info);
        } else {
            if (objectList.Count <= 0) {
                EditorGUILayout.HelpBox("找不到指定物体", MessageType.Info);
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        GUI.backgroundColor = Color.white;
    }

    private void DrawSingleWidget(UIWidgetItem item) {
        // 绘制单个控件信息
        GUI.backgroundColor = item.isSelected ? selectItemColor : unselectItemColor;
        using (EditorGUILayout.VerticalScope vScope = new EditorGUILayout.VerticalScope(GUI.skin.box)) {
            GUI.backgroundColor = Color.white;
            EditorGUILayout.BeginHorizontal();
            item.isSelected = EditorGUILayout.Toggle(item.isSelected, GUILayout.Width(16f));
            EditorGUILayout.LabelField(item.name, GUILayout.Width(100f));
            item.varName = EditorGUILayout.TextField(item.varName, GUILayout.Width(100f));
            EditorGUILayout.LabelField(item.type, GUILayout.Width(100f));
            if (GUILayout.Button("转到", GUILayout.Width(40f))) {
                GameObject target = viewRoot.transform.Find(item.path.Substring(1)).gameObject;
                EditorGUIUtility.PingObject(target);
                Selection.activeObject = target;
            }
            EditorGUILayout.LabelField(item.path);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;
            item.genEvent = EditorGUILayout.Toggle(item.genEvent, GUILayout.Width(26f));
            EditorGUILayout.LabelField("生成UI事件", GUILayout.Width(80f));
            item.isCustomType = EditorGUILayout.Toggle(item.isCustomType, GUILayout.Width(26f));
            EditorGUILayout.LabelField("是否自定义类型", GUILayout.Width(100f));
            if (item.isCustomType) {
                item.type = EditorGUILayout.TextField(item.type, GUILayout.Width(140f));
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawSingleObject(UIWidgetItem item) {
        // 绘制单个对象信息
        GUI.backgroundColor = item.isSelected ? selectItemColor : unselectItemColor;
        using (EditorGUILayout.VerticalScope vScope = new EditorGUILayout.VerticalScope(GUI.skin.box)) {
            GUI.backgroundColor = Color.white;
            EditorGUILayout.BeginHorizontal();
            item.isSelected = EditorGUILayout.Toggle(item.isSelected, GUILayout.Width(16f));
            EditorGUILayout.LabelField(item.name, GUILayout.Width(100f));
            item.varName = EditorGUILayout.TextField(item.varName, GUILayout.Width(100f));
            EditorGUILayout.LabelField(item.type, GUILayout.Width(100f));
            if (GUILayout.Button("转到", GUILayout.Width(40f))) {
                GameObject target = viewRoot.transform.Find(item.path.Substring(1)).gameObject;
                EditorGUIUtility.PingObject(target);
                Selection.activeObject = target;
            }
            EditorGUILayout.LabelField(item.path);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("自定义类型:", GUILayout.Width(80f));
            item.type = EditorGUILayout.TextField(item.type, GUILayout.Width(140f));
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawFunctionArea() {
        // 功能区域
        using (new EditorGUILayout.VerticalScope()) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("生成路径:", GUILayout.Width(60f));
            scriptPath = EditorGUILayout.TextField(scriptPath);
            cacheScriptPath = scriptPath;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("预览代码")) {
                CompileScriptCode();
                Rect windowRect = new Rect(400, 300, 600, 800);
                ViewCodePreviewWindow previewWindow = GetWindowWithRect<ViewCodePreviewWindow>(windowRect, true, "代码预览");
                previewWindow.SetupCode(codeBuilder.ToString());
                previewWindow.Show();

            }
            if (GUILayout.Button("生成代码文件")) {
                CompileScriptCode();
                GenerateScriptFile();
            }
            if (string.IsNullOrEmpty(scriptPath)) {
                EditorGUILayout.HelpBox("未设置路径，代码会被默认保存到Asset目录下", MessageType.Warning);
            } else {
                string filePath = GetScriptFilePath();
                if (File.Exists(filePath)) {
                    EditorGUILayout.HelpBox("指定代码文件已经存在，请修改类名或者路径", MessageType.Error);
                }
            }
        }
    }

    private void GenerateScriptFile() {
        string filePath = GetScriptFilePath();
        if (!File.Exists(filePath)) {
            File.WriteAllText(filePath, codeBuilder.ToString(), new UTF8Encoding(true, false));
            AssetDatabase.ImportAsset(filePath);
            var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(filePath);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }

    private string GetScriptFilePath() {
        StringBuilder pathBuilder = new StringBuilder(scriptPath);
        if (pathBuilder.Length > 0) {
            if (pathBuilder[0] == '/') {
                pathBuilder.Remove(0, 1);
            }
            if (pathBuilder[pathBuilder.Length - 1] != '/') {
                pathBuilder.Append('/');
            }
        }
        string filePath = pathBuilder.ToString();
        if (filePath.IndexOf("Assets") == 0) {
            // 头部发现一个Assets根目录
            filePath += $"{scriptName}.cs";
        } else {
            // 头部没有Assets根目录，添加一个
            filePath = $"Assets/{filePath}{scriptName}.cs";
        }
        return filePath;
    }

    private void CompileScriptCode() {
        // 按步骤一行一行插入代码即可
        codeBuilder.Clear();
        indexWidgets.Clear();
        AppendScriptHeader(codeBuilder);
        AppendScriptClass(codeBuilder);
    }

    private void AppendScriptHeader(StringBuilder builder) {
        builder.AppendLine($"// Generated at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        builder.AppendLine("using RoachGame;");
        builder.AppendLine("using RoachGame.Services;");
        builder.AppendLine("using RoachGame.Services.Broadcast;");
        builder.AppendLine("using RoachGame.Services.Events;");
        builder.AppendLine("using RoachGame.Utils;");
        builder.AppendLine("using RoachGame.Views;");
        builder.AppendLine("using RoachGame.Widgets;");
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using UnityEngine;");
        builder.AppendLine("using UnityEngine.UI;");
        builder.AppendLine("// Generated By View Code Generator");
        builder.AppendLine();
    }

    private void AppendScriptClass(StringBuilder builder) {
        if (!string.IsNullOrEmpty(scriptInterface)) {
            builder.AppendLine($"public class {scriptName} : {selectType},{scriptInterface} {{");
        } else {
            builder.AppendLine($"public class {scriptName} : {selectType} {{");
        }
        builder.AppendLine();
        if (selectType == "ViewWindow") {
            builder.AppendLine($"public const string WINDOW_IDENTIFIER = \"{scriptName}\";");
            builder.AppendLine();
        }
        if (widgetList.Count > 0 || objectList.Count > 0) {
            AppendScriptVariables(builder);
            builder.AppendLine();
        }
        AppendPreLoadSegment(builder);
        builder.AppendLine();
        AppendLoadMembersSegment(builder);
        builder.AppendLine();
        AppendLoadViewsSegment(builder);
        builder.AppendLine();
        AppendPostLoadSegment(builder);
        builder.AppendLine();
        AppendHandleEventSegment(builder);
        builder.AppendLine();
        AppendUpdateViewSegment(builder);
        builder.AppendLine();
        for (int i = 0; i < indexWidgets.Count; i++) {
            UIWidgetItem item = indexWidgets[i];
            if (item.type == ViewCodeConfigs.TYPE_NAME_BUTTON) {
                AppendButtonClickEvent(builder, item.varName);
                builder.AppendLine();
            }
        }
        AppendBroadcastReceiver(builder);
        builder.AppendLine("}");
    }

    private void AppendScriptVariables(StringBuilder builder) {
        for (int i = 0; i < widgetList.Count; i++) {
            UIWidgetItem item = widgetList[i];
            if (item.isSelected) {
                // 选中的才处理
                builder.AppendLine($"\tprivate {item.type} {item.varName};");
                indexWidgets.Add(item);
            }
        }
        builder.AppendLine();
        for (int i = 0; i < objectList.Count; i++) {
            UIWidgetItem item = objectList[i];
            if (item.isSelected) {
                // 选中才处理
                builder.AppendLine($"\tprivate {item.type} {item.varName};");
                indexWidgets.Add(item);
            }
        }
    }

    private void AppendPreLoadSegment(StringBuilder builder) {
        builder.AppendLine("\tprotected override void PreLoad() {");
        builder.AppendLine("\t\tbase.PreLoad();");
        builder.AppendLine("\t}");
    }

    private void AppendLoadMembersSegment(StringBuilder builder) {
        builder.AppendLine("\tprotected override void LoadMembers() {");
        builder.AppendLine("\t\tbase.LoadMembers();");
        builder.AppendLine("\t}");
    }

    private void AppendLoadViewsSegment(StringBuilder builder) {
        builder.AppendLine("\tprotected override void LoadViews() {");
        builder.AppendLine("\t\tbase.LoadViews();");
        for (int i = 0; i < indexWidgets.Count; i++) {
            UIWidgetItem item = indexWidgets[i];
            if (item.type == ViewCodeConfigs.TYPE_NAME_GAME_OBJECT) {
                builder.AppendLine($"\t\t{item.varName} = FindGameObject(\"{item.path.Substring(1)}\");");
            } else {
                builder.AppendLine($"\t\t{item.varName} = FindComponent<{item.type}>(\"{item.path.Substring(1)}\");");
            }
        }
        builder.AppendLine();
        for (int i = 0; i < indexWidgets.Count; i++) {
            UIWidgetItem item = indexWidgets[i];
            if (item.type == ViewCodeConfigs.TYPE_NAME_BUTTON) {
                // 按钮的回调事件
                builder.AppendLine($"\t\t{item.varName}.SetupButtonListener(On{CapitalFirstCharacter(item.varName)}Click);");
            }
        }
        builder.AppendLine("\t}");
    }

    private void AppendPostLoadSegment(StringBuilder builder) {
        builder.AppendLine("\tprotected override void PostLoad() {");
        builder.AppendLine("\t\tbase.PostLoad();");
        builder.AppendLine("\t}");
    }

    private void AppendHandleEventSegment(StringBuilder builder) {
        builder.AppendLine("\tprotected override void HandleEvent(EventMessage e) {");
        builder.AppendLine("\t\tbase.HandleEvent(e);");
        builder.AppendLine("\t}");
    }

    private void AppendUpdateViewSegment(StringBuilder builder) {
        builder.AppendLine("\tpublic override void UpdateViews() {");
        builder.AppendLine("\t\tbase.UpdateViews();");
        builder.AppendLine("\t}");
    }

    private void AppendButtonClickEvent(StringBuilder builder, string varName) {
        builder.AppendLine($"\tprivate void On{CapitalFirstCharacter(varName)}Click() {{");
        builder.AppendLine("\t\t// Click Callback");
        builder.AppendLine("\t}");
    }

    private void AppendBroadcastReceiver(StringBuilder builder) {
        builder.AppendLine("\tprivate void ReceiveBroadcast(BroadcastInformation msg) {");
        builder.AppendLine("\t\t// Broadcast Process");
        builder.AppendLine("\t}");
    }

    private string CapitalFirstCharacter(string str) {
        return $"{char.ToUpper(str[0])}{str.Substring(1)}";
    }

    public class UIWidgetItem {
        public bool isSelected;
        public string name;
        public string varName;
        public bool isCustomType;
        public string type;
        public string path;
        public bool genEvent;
    }

    public class SearchCell {
        public Transform root;
        public string parentPath;
        public SearchCell(Transform root, string path) {
            this.root = root;
            parentPath = path;
        }
    }

    public static class ViewCodeConfigs {
        // --- 系统控件名称
        public const string TYPE_NAME_TEXT = "Text";
        public const string TYPE_NAME_IMAGE = "Image";
        public const string TYPE_NAME_RAW_IMAGE = "RawImage";
        public const string TYPE_NAME_BUTTON = "Button";
        public const string TYPE_NAME_TOGGLE = "Toggle";
        public const string TYPE_NAME_DROPDOWN = "Dropdown";
        public const string TYPE_NAME_SLIDER = "Slider";
        public const string TYPE_NAME_INPUT_FIELD = "InputField";
        public const string TYPE_NAME_SCROLL_RECT = "ScrollRect";
        public const string TYPE_NAME_CANVAS_GROUP = "CanvasGroup";
        public const string TYPE_NAME_SCRIPT = "Script";
        public const string TYPE_NAME_GAME_OBJECT = "GameObject";
        // --- 系统控件类型枚举
        public readonly static Type TYPE_TEXT = typeof(Text);
        public readonly static Type TYPE_IMAGE = typeof(Image);
        public readonly static Type TYPE_RAW_IMAGE = typeof(RawImage);
        public readonly static Type TYPE_BUTTON = typeof(Button);
        public readonly static Type TYPE_TOGGLE = typeof(Toggle);
        public readonly static Type TYPE_DROPDOWN = typeof(Dropdown);
        public readonly static Type TYPE_SLIDER = typeof(Slider);
        public readonly static Type TYPE_INPUT_FIELD = typeof(InputField);
        public readonly static Type TYPE_SCROLL_RECT = typeof(ScrollRect);
        public readonly static Type TYPE_CANVAS_GROUP = typeof(CanvasGroup);
    }
}
