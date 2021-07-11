using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;

public class BasicScriptCreateAction : EndNameEditAction {
    public override void Action(int instanceId, string pathName, string resourceFile) {
        var content = File.ReadAllText(resourceFile);
        var className = Path.GetFileNameWithoutExtension(pathName);
        //清除空格并替换模板内容
        className = className.Replace(" ", "");
        content = content.Replace("#SCRIPTNAME#", className);
        content = "// File create date:" + DateTime.Now.Date.ToShortDateString() + "\n" + content;
        //utf8编码
        var encoding = new UTF8Encoding(true, false);
        File.WriteAllText(pathName, content, encoding);
        AssetDatabase.ImportAsset(pathName);
        var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
        ProjectWindowUtil.ShowCreatedAsset(asset);
    }
}
