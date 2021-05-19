using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

// Tagging a class with the EditorTool attribute and no target type registers a global tool. Global tools are valid for any selection, and are accessible through the top left toolbar in the editor.
[EditorTool("Scene Camera Relative")]
class SceneCameraRelativeTool : EditorTool
{
    // Serialize this value to set a default value in the Inspector.
    [SerializeField]
    Texture2D m_ToolIcon;

    GUIContent m_IconContent;

    void OnEnable()
    {
        m_IconContent = new GUIContent()
        {
            image = m_ToolIcon,
            text = "Scene Camera Relative",
            tooltip = "Scene Camera Relative"
        };
    }

    public override GUIContent toolbarIcon
    {
        get { return m_IconContent; }
    }

    // This is called for each window that your tool is active in. Put the functionality of your tool here.
    public override void OnToolGUI(EditorWindow window)
    {
        EditorGUI.BeginChangeCheck();

        Vector3 position = Tools.handlePosition;

        Matrix4x4 cm = SceneView.lastActiveSceneView.camera.cameraToWorldMatrix;
        Vector3 up = cm * Vector3.up;
        Vector3 right = cm * Vector3.right;

        using (new Handles.DrawingScope(Color.red))
        {
            position = Handles.Slider(position, right);
        }
        using (new Handles.DrawingScope(Color.green))
        {
            position = Handles.Slider(position, up);
        }
        using (new Handles.DrawingScope(new Color(0,0,1,.3f)))
        {
            float size = HandleUtility.GetHandleSize(position) * .15f;
            Vector3 offset = (up * size) + (right * size);
            position = Handles.FreeMoveHandle(position + offset, Quaternion.identity, size, Vector3.zero, Handles.DotHandleCap);
            position -= offset;
        }

        if (EditorGUI.EndChangeCheck())
        {
            Vector3 delta = position - Tools.handlePosition;

            Undo.RecordObjects(Selection.transforms, "Move Transform");

            foreach (var transform in Selection.transforms)
                transform.position += delta;
        }
    }
}