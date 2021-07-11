using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AssetBundleMapWindow : EditorWindow {

    [MenuItem("Window/Asset Bundle Mapping")]
    private static void NewWindow() {
        Rect windowRect = new Rect(400, 300, 400, 600);
        AssetBundleMapWindow window = (AssetBundleMapWindow)GetWindowWithRect(typeof(AssetBundleMapWindow), windowRect, true, "资源包映射");
        window.Show();
    }

    private string targetPlatform;
    private string manifestPath;
    private AssetBundle manifestBundle;
    private AssetBundleManifest currentManifest;
    private List<AssetBundleItem> assetBundleList = new List<AssetBundleItem>();
    private Vector2 bundleListPos;
    private Color selectItemColor;
    private Color unselectItemColor;

    private string[] platforms = {
        "StandaloneWindows"
    };

    private void OnEnable() {
        targetPlatform = platforms[0];
        selectItemColor = Color.blue;
        selectItemColor.a = 0.25f;
        unselectItemColor = Color.red;
        unselectItemColor.a = 0.25f;
        manifestBundle?.Unload(true);
    }

    private void OnGUI() {
        DrawTitle();
        DrawList();
        DrawFunc();
    }

    private void DrawTitle() {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("目标平台:", GUILayout.Width(100f));
        GUIContent content = new GUIContent(targetPlatform);
        if (EditorGUILayout.DropdownButton(content, FocusType.Keyboard)) {
            GenericMenu menu = new GenericMenu();
            foreach (string item in platforms) {
                GUIContent itemContent = new GUIContent(item);
                menu.AddItem(itemContent, targetPlatform.Equals(item), OnPlatformSelect, item);
            }
            menu.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("读取配置")) {
            string bundleUri = $"{Application.streamingAssetsPath}/{targetPlatform}";
            if (bundleUri != manifestPath) {
                manifestPath = bundleUri;
                manifestBundle = AssetBundle.LoadFromFile(manifestPath);
                currentManifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                UpdateDataList();
                manifestBundle.Unload(false);
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void UpdateDataList() {
        if (currentManifest != null) {
            assetBundleList.Clear();
            string[] bundles = currentManifest.GetAllAssetBundlesWithVariant();
            foreach (string name in bundles) {
                string[] nameSplit = name.Split('.');
                AssetBundleItem item = new AssetBundleItem();
                item.isSelected = true;
                item.name = nameSplit[0];
                item.varName = nameSplit[1];
                item.assetList = new List<string>(AssetDatabase.GetAssetPathsFromAssetBundle(name));
                assetBundleList.Add(item);
            }
        }
    }

    private void DrawList() {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        EditorGUILayout.LabelField("", GUILayout.Width(2f));
        EditorGUILayout.LabelField("S", GUILayout.Width(16f));
        EditorGUILayout.LabelField("名称");
        EditorGUILayout.LabelField("后缀", GUILayout.Width(100f));
        EditorGUILayout.LabelField("数量", GUILayout.Width(100f));
        EditorGUILayout.EndHorizontal();
        bundleListPos = EditorGUILayout.BeginScrollView(bundleListPos, GUILayout.Height(360f));
        for (int i = 0; i < assetBundleList.Count; i++) {
            DrawSingleItem(assetBundleList[i]);
        }
        if (currentManifest == null) {
            EditorGUILayout.HelpBox("没有配置表", MessageType.Info);
        } else {
            if (assetBundleList.Count == 0) {
                EditorGUILayout.HelpBox("找不到资源包", MessageType.Info);
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        GUI.backgroundColor = Color.white;
    }

    private void DrawSingleItem(AssetBundleItem item) {
        GUI.backgroundColor = item.isSelected ? selectItemColor : unselectItemColor;
        using (EditorGUILayout.VerticalScope vScope = new EditorGUILayout.VerticalScope(GUI.skin.box)) {
            GUI.backgroundColor = Color.white;
            EditorGUILayout.BeginHorizontal();
            item.isSelected = EditorGUILayout.Toggle(item.isSelected, GUILayout.Width(16f));
            EditorGUILayout.LabelField(item.name);
            EditorGUILayout.LabelField(item.varName, GUILayout.Width(100f));
            EditorGUILayout.LabelField(item.assetList.Count.ToString(), GUILayout.Width(100f));
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawFunc() {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("编译到StreamingAssets目录")) {
            BuildMappingToStream();
        }
        if (GUILayout.Button("编译到Resources目录")) {
            BuildMappingToResources();
        }
    }

    private Dictionary<string, string> CompileAssetMapping() {
        Dictionary<string, string> map = new Dictionary<string, string>();
        for (int i = 0; i < assetBundleList.Count; i++) {
            if (assetBundleList[i].isSelected) {
                string bundleName = $"{assetBundleList[i].name}.{assetBundleList[i].varName}";
                foreach (string aname in assetBundleList[i].assetList) {
                    int slashIndex = aname.LastIndexOf('/');
                    int pointIndex = aname.LastIndexOf('.');
                    int nameLength = pointIndex - slashIndex - 1;
                    map[aname.Substring(slashIndex + 1, nameLength)] = bundleName;
                }
            }
        }
        return map;
    }

    private void BuildMappingToStream() {
        Dictionary<string, string> map = CompileAssetMapping();
        if (!Directory.Exists("Assets/StreamingAssets")) {
            Directory.CreateDirectory("Assets/StreamingAssets");
        }
        string filePath = "Assets/StreamingAssets/AssetMap.json";
        File.WriteAllText(filePath, JsonConvert.SerializeObject(map), new UTF8Encoding(true, false));
        AssetDatabase.ImportAsset(filePath);
    }

    private void BuildMappingToResources() {
        Dictionary<string, string> map = CompileAssetMapping();
        if (!Directory.Exists("Assets/Resources")) {
            Directory.CreateDirectory("Assets/Resources");
        }
        string filePath = "Assets/Resources/AssetMap.json";
        File.WriteAllText(filePath, JsonConvert.SerializeObject(map), new UTF8Encoding(true, false));
        AssetDatabase.ImportAsset(filePath);
    }

    private void OnPlatformSelect(object value) {
        targetPlatform = value.ToString();
    }

    public class AssetBundleItem {
        public bool isSelected;
        public string name;
        public string varName;
        public List<string> assetList;
    }
}
