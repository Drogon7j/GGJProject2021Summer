// File create date:2019/11/19
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
public class ViewCodePreviewWindow : EditorWindow {

    private string previewCode;
    private Vector2 scrollerPos;

    public void SetupCode(string code) {
        previewCode = code;
    }

    private void OnGUI() {
        using (GUILayout.ScrollViewScope scroller = new GUILayout.ScrollViewScope(scrollerPos)) {
            scrollerPos = scroller.scrollPosition;
            if (!string.IsNullOrEmpty(previewCode)) {
                GUILayout.Label(previewCode);
            } else {
                EditorGUILayout.HelpBox("没有可预览的代码", MessageType.Warning);
            }
        }
    }
}
