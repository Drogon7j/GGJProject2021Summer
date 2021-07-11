using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;

public class ComplexScriptCreateAction : EndNameEditAction {

    private string inheriteClassName;
    private string interfaceName;

    public void SetupNames(string clsName, string iName) {
        inheriteClassName = clsName;
        interfaceName = iName;
    }

    public override void Action(int instanceId, string pathName, string resourceFile) {
        var content = File.ReadAllText(resourceFile);
        var className = Path.GetFileNameWithoutExtension(pathName);
        //清除空格并替换模板内容
        className = className.Replace(" ", "");
        bool classFlag = false;
        if (inheriteClassName != null && inheriteClassName.Length > 0) {
            inheriteClassName = " : " + inheriteClassName;
            classFlag = true;
        }
        if (interfaceName != null && interfaceName.Length > 0) {
            if (classFlag) {
                interfaceName = "," + interfaceName;
            } else {
                interfaceName = " : " + interfaceName;
            }
            interfaceName = interfaceName.Replace(",", ", ");
        }
        content = content.Replace("#SCRIPTNAME#", className);
        content = content.Replace("#PARENTNAME#", inheriteClassName);
        content = content.Replace("#INTERFACENAME#", interfaceName);
        content = "// File create date:" + DateTime.Now.Date.ToShortDateString() + "\n" + content;
        //utf8编码
        var encoding = new UTF8Encoding(true, false);
        File.WriteAllText(pathName, content, encoding);
        AssetDatabase.ImportAsset(pathName);
        var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
        ProjectWindowUtil.ShowCreatedAsset(asset);
    }
}
