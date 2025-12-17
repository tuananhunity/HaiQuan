using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomHingeJoint))]
public class CustomHingeJointEditor : Editor
{
    private CustomHingeJoint hinge;
    private float previewAngle = 0f;
    private bool isPreviewMode = false;

    void OnEnable()
    {
        hinge = (CustomHingeJoint)target;
    }

    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Editor Controls", EditorStyles.boldLabel);

        // Preview Mode
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Preview Mode (Edit Mode Only)", EditorStyles.miniBoldLabel);

        if (!Application.isPlaying)
        {
            isPreviewMode = EditorGUILayout.Toggle("Enable Preview", isPreviewMode);

            if (isPreviewMode)
            {
                EditorGUI.BeginChangeCheck();
                previewAngle = EditorGUILayout.Slider("Preview Angle", previewAngle, hinge.minAngle, hinge.maxAngle);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(hinge.transform, "Preview Hinge Rotation");
                    PreviewRotation(previewAngle);
                    EditorUtility.SetDirty(hinge.transform);
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Min Angle"))
                {
                    previewAngle = hinge.minAngle;
                    PreviewRotation(previewAngle);
                }
                if (GUILayout.Button("Zero"))
                {
                    previewAngle = 0f;
                    PreviewRotation(previewAngle);
                }
                if (GUILayout.Button("Max Angle"))
                {
                    previewAngle = hinge.maxAngle;
                    PreviewRotation(previewAngle);
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (GUILayout.Button("Reset to Initial Position"))
                {
                    Undo.RecordObject(hinge.transform, "Reset Hinge");
                    hinge.transform.localRotation = Quaternion.identity;
                    EditorUtility.SetDirty(hinge.transform);
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Preview mode is only available in Edit Mode", MessageType.Info);
        }

        EditorGUILayout.EndVertical();

        // Runtime Controls
        if (Application.isPlaying)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Runtime Controls", EditorStyles.miniBoldLabel);

            EditorGUILayout.LabelField($"Current Angle: {hinge.currentAngle:F1}°");
            EditorGUILayout.LabelField($"Target Angle: {hinge.targetAngle:F1}°");
            EditorGUILayout.LabelField($"Open: {hinge.GetOpenPercentage() * 100f:F1}%");

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open"))
            {
                hinge.Open();
            }
            if (GUILayout.Button("Close"))
            {
                hinge.Close();
            }
            if (GUILayout.Button("Reset"))
            {
                hinge.Reset();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            float newTarget = EditorGUILayout.Slider("Set Target Angle", hinge.targetAngle, hinge.minAngle, hinge.maxAngle);
            if (newTarget != hinge.targetAngle)
            {
                hinge.SetTargetAngle(newTarget);
            }

            EditorGUILayout.EndVertical();
        }

        // Quick Actions
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Quick Setup", EditorStyles.miniBoldLabel);

        if (GUILayout.Button("Auto-detect Axis Direction"))
        {
            AutoDetectAxis();
        }

        EditorGUILayout.EndVertical();
    }

    private void PreviewRotation(float angle)
    {
        Transform pivot = hinge.hingeAxis != null ? hinge.hingeAxis : hinge.transform;
        Quaternion rotation = Quaternion.AngleAxis(angle, hinge.axisDirection);
        hinge.transform.localRotation = rotation;
    }

    private void AutoDetectAxis()
    {
        // Try to detect best axis based on parent structure
        EditorUtility.DisplayDialog("Auto-detect Axis",
            "This feature will analyze the object's orientation and suggest the best axis.\n\n" +
            "Currently set to: " + hinge.axisDirection.ToString(),
            "OK");
    }

    private void OnSceneGUI()
    {
        // Draw interactive handles in Scene view
        if (!Application.isPlaying && isPreviewMode)
        {
            Transform pivot = hinge.hingeAxis != null ? hinge.hingeAxis : hinge.transform;
            Vector3 worldAxis = pivot.TransformDirection(hinge.axisDirection);

            // Arc handle for rotation
            Handles.color = Color.yellow;

            Vector3 perpendicular = Vector3.Cross(hinge.axisDirection, Vector3.up);
            if (perpendicular.magnitude < 0.01f)
                perpendicular = Vector3.Cross(hinge.axisDirection, Vector3.right);
            perpendicular = pivot.TransformDirection(perpendicular.normalized);

            float handleSize = HandleUtility.GetHandleSize(pivot.position) * 0.5f;

            // Draw rotation arc handle
            EditorGUI.BeginChangeCheck();
            float newAngle = Handles.Disc(
                Quaternion.AngleAxis(previewAngle, worldAxis),
                pivot.position,
                worldAxis,
                handleSize,
                false,
                0f
            ).eulerAngles.magnitude;

            if (EditorGUI.EndChangeCheck())
            {
                // Update angle based on handle rotation
                // This is simplified - you might need more complex calculation
                Undo.RecordObject(hinge.transform, "Rotate Hinge");
                PreviewRotation(previewAngle);
            }
        }
    }
}
