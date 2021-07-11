// File create date:2020/3/18
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
// Created By Yu.Liu
public class SpriteAtlasMapWindow : EditorWindow {

    [MenuItem("Window/Sprite Atlas Mapping")]
    private static void NewWindow() {
        Rect windowRect = new Rect(400, 300, 400, 400);
        SpriteAtlasMapWindow window = (SpriteAtlasMapWindow)GetWindowWithRect(typeof(SpriteAtlasMapWindow), windowRect, true, "图片资源映射");
        window.Show();
    }

    public SpriteAtlas targetAtlas;
    private SerializedObject windowObject;
    private SerializedProperty atlasProperty;
    private string atlasNameStr;
    private List<string> spriteNameList = new List<string>();

    private Dictionary<string, List<string>> spriteMapping;

    private Vector2 spriteListPos;

    private void OnEnable() {
        windowObject = new SerializedObject(this);
        atlasProperty = windowObject.FindProperty("targetAtlas");
        spriteMapping = new Dictionary<string, List<string>>();
    }

    private void OnGUI() {
        windowObject.Update();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(atlasProperty);
        CompileListData();
        DrawSpriteList();
        DrawFunc();
        EditorGUILayout.EndVertical();
        windowObject.ApplyModifiedProperties();
    }

    private void CompileListData() {
        if (targetAtlas != null) {
            atlasNameStr = targetAtlas.name;
            spriteNameList.Clear();
            int count = targetAtlas.spriteCount;
            Sprite[] allSprite = new Sprite[count];
            targetAtlas.GetSprites(allSprite);
            for (int i = 0; i < allSprite.Length; i++) {
                string fullName = allSprite[i].name;
                int endIndex = fullName.LastIndexOf('(');
                spriteNameList.Add(fullName.Substring(0, endIndex));
            }
        }
    }

    private void DrawSpriteList() {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        EditorGUILayout.LabelField("", GUILayout.Width(2f));
        EditorGUILayout.LabelField("名称");
        EditorGUILayout.EndHorizontal();
        spriteListPos = EditorGUILayout.BeginScrollView(spriteListPos, GUILayout.Height(300f));
        for (int i = 0; i < spriteNameList.Count; i++) {
            DrawSingleItem(spriteNameList[i]);
        }
        if (spriteNameList == null || spriteNameList.Count == 0) {
            EditorGUILayout.HelpBox("没有图片资源", MessageType.Info);
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawSingleItem(string name) {
        using (EditorGUILayout.VerticalScope vScope = new EditorGUILayout.VerticalScope(GUI.skin.box)) {
            EditorGUILayout.LabelField(name);
        }
    }

    private void DrawFunc() {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("编译到Resources目录")) {
            BuildMappingFile();
        }
        EditorGUILayout.EndVertical();
    }

    private string CheckMappingFile() {
        if (!Directory.Exists("Assets/Resources")) {
            Directory.CreateDirectory("Assets/Resources");
        }
        string filePath = "Assets/Resources/SpriteMap.json";
        if (!File.Exists(filePath)) {
            File.Create(filePath).Close();
            spriteMapping.Clear();
        } else {
            string json = File.ReadAllText(filePath);
            spriteMapping = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
        }
        return filePath;
    }

    private void BuildMappingFile() {
        //Debug.Log($"Atlas Name: {atlasNameStr}");
        //foreach (string name in spriteNameList) {
        //    Debug.Log($"Sprite Name: {name}");
        //}
        string path = CheckMappingFile();
        spriteMapping[atlasNameStr] = spriteNameList;
        File.WriteAllText(path, JsonConvert.SerializeObject(spriteMapping), new UTF8Encoding(true, false));
        AssetDatabase.ImportAsset(path);
    }

    //private void BuildMappingToStream() {
    //    Dictionary<string, string> map = CompileSpriteMapping();
    //    if (!Directory.Exists("Assets/StreamingAssets")) {
    //        Directory.CreateDirectory("Assets/StreamingAssets");
    //    }
    //    string filePath = "Assets/StreamingAssets/AssetMap.json";
    //    File.WriteAllText(filePath, JsonConvert.SerializeObject(map), new UTF8Encoding(true, false));
    //    AssetDatabase.ImportAsset(filePath);
    //}

    //private void BuildMappingToResources() {
    //    Dictionary<string, string> map = CompileSpriteMapping();
    //    if (!Directory.Exists("Assets/Resources")) {
    //        Directory.CreateDirectory("Assets/Resources");
    //    }
    //    string filePath = "Assets/Resources/AssetMap.json";
    //    File.WriteAllText(filePath, JsonConvert.SerializeObject(map), new UTF8Encoding(true, false));
    //    AssetDatabase.ImportAsset(filePath);
    //}
}
