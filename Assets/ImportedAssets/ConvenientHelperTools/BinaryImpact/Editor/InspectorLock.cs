using System.Reflection;
using UnityEditor;

public class InspectorLock : Editor
{
    [MenuItem("Tools/Toggle Inspector lock %#l")]
    static public void ToggleInspectorLock()
    {
        var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        var isLocked = inspectorType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public);

        var inspectorWindow = EditorWindow.GetWindow(inspectorType);
        var state = isLocked.GetGetMethod().Invoke(inspectorWindow, new object[] { });

        isLocked.GetSetMethod().Invoke(inspectorWindow, new object[] { !(bool)state });
    }
}