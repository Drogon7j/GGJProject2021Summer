// File create date:2019/7/22
using System.Text;
using UnityEditor;
using UnityEngine;
// Created By Yu.Liu
public class HierarchyPathWindow : EditorWindow {

    [MenuItem("Window/General/Hierarchy Path Display")]
    private static void AddWindow() {
        Rect windowRect = new Rect(400, 300, 300, 100);
        HierarchyPathWindow window = (HierarchyPathWindow)GetWindowWithRect(typeof(HierarchyPathWindow), windowRect, false, "选择对象路径查看");
        window.Show();
    }

    private GameObject currentSelection;
    private string objectFullpath;
    private StringBuilder pathBuilder = new StringBuilder();

    private void OnGUI() {
        objectFullpath = EditorGUILayout.TextField("对象完整路径:", objectFullpath);
    }

    private void OnSelectionChange() {
        currentSelection = Selection.activeGameObject;
        pathBuilder.Clear();
        if (currentSelection != null) {
            Transform tp = currentSelection.transform;
            pathBuilder.Insert(0, tp.name);
            tp = tp.parent;
            while (tp != null) {
                pathBuilder.Insert(0, string.Format("{0}/", tp.name));
                tp = tp.parent;
            }
        }
        objectFullpath = pathBuilder.ToString();
        Repaint();
    }
}
