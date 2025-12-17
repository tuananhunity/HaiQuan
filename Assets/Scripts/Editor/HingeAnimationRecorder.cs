using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Editor Window để setup và record animation cho hinges
/// </summary>
public class HingeAnimationRecorder : EditorWindow
{
    private CustomHingeJoint[] selectedHinges;
    private bool isRecording = false;
    private AnimationClip recordedClip;
    private string clipName = "NewHingeAnimation";

    private float recordedTime = 0f;
    private Dictionary<CustomHingeJoint, List<Keyframe>> recordedKeyframes = new Dictionary<CustomHingeJoint, List<Keyframe>>();

    [MenuItem("Tools/Hinge Animation Recorder")]
    public static void ShowWindow()
    {
        GetWindow<HingeAnimationRecorder>("Hinge Recorder");
    }

    void OnEnable()
    {
        RefreshHinges();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Hinge Animation Recorder", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Selection
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("1. Select Hinges", EditorStyles.miniBoldLabel);

        if (GUILayout.Button("Find All Hinges in Selection"))
        {
            RefreshHinges();
        }

        if (selectedHinges != null && selectedHinges.Length > 0)
        {
            EditorGUILayout.LabelField($"Found {selectedHinges.Length} hinge(s):");
            foreach (var hinge in selectedHinges)
            {
                EditorGUILayout.LabelField($"  • {hinge.gameObject.name}");
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No hinges selected. Select GameObjects with CustomHingeJoint components.", MessageType.Warning);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // Manual Control
        if (!Application.isPlaying && selectedHinges != null && selectedHinges.Length > 0)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("2. Manual Control (Edit Mode)", EditorStyles.miniBoldLabel);

            foreach (var hinge in selectedHinges)
            {
                if (hinge != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(hinge.gameObject.name, GUILayout.Width(150));

                    EditorGUI.BeginChangeCheck();
                    float angle = EditorGUILayout.Slider(hinge.currentAngle, hinge.minAngle, hinge.maxAngle);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(hinge.transform, "Adjust Hinge");
                        SetHingeAngleInEditor(hinge, angle);
                        EditorUtility.SetDirty(hinge.transform);
                    }

                    if (GUILayout.Button("0", GUILayout.Width(30)))
                    {
                        SetHingeAngleInEditor(hinge, 0f);
                    }
                    if (GUILayout.Button("Max", GUILayout.Width(40)))
                    {
                        SetHingeAngleInEditor(hinge, hinge.maxAngle);
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("All to Min"))
            {
                foreach (var h in selectedHinges)
                    if (h) SetHingeAngleInEditor(h, h.minAngle);
            }
            if (GUILayout.Button("All to Zero"))
            {
                foreach (var h in selectedHinges)
                    if (h) SetHingeAngleInEditor(h, 0f);
            }
            if (GUILayout.Button("All to Max"))
            {
                foreach (var h in selectedHinges)
                    if (h) SetHingeAngleInEditor(h, h.maxAngle);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        // Animation Recording
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("3. Create Animation (Legacy)", EditorStyles.miniBoldLabel);

        clipName = EditorGUILayout.TextField("Animation Name:", clipName);

        EditorGUILayout.HelpBox(
            "Animation recording is complex in Editor mode.\n\n" +
            "Alternative methods:\n" +
            "1. Use Unity's Animation Window (Window > Animation)\n" +
            "2. Record in Play mode using Unity Recorder\n" +
            "3. Use Timeline for complex sequences",
            MessageType.Info);

        if (GUILayout.Button("Open Animation Window"))
        {
            EditorApplication.ExecuteMenuItem("Window/Animation/Animation");
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // Preset Positions
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("4. Quick Presets", EditorStyles.miniBoldLabel);

        if (!Application.isPlaying && selectedHinges != null && selectedHinges.Length > 0)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Closed Position"))
            {
                ApplyPreset(0f);
            }
            if (GUILayout.Button("Half Open"))
            {
                ApplyPreset(0.5f);
            }
            if (GUILayout.Button("Fully Open"))
            {
                ApplyPreset(1f);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    void RefreshHinges()
    {
        if (Selection.gameObjects.Length > 0)
        {
            List<CustomHingeJoint> hinges = new List<CustomHingeJoint>();

            foreach (var go in Selection.gameObjects)
            {
                // Get hinges from selected object and all children
                hinges.AddRange(go.GetComponentsInChildren<CustomHingeJoint>());
            }

            selectedHinges = hinges.ToArray();
        }
    }

    void SetHingeAngleInEditor(CustomHingeJoint hinge, float angle)
    {
        if (hinge.useLimits)
            angle = Mathf.Clamp(angle, hinge.minAngle, hinge.maxAngle);

        hinge.currentAngle = angle;
        hinge.targetAngle = angle;

        // Apply rotation directly in editor
        Transform pivot = hinge.hingeAxis != null ? hinge.hingeAxis : hinge.transform;
        Quaternion rotation = Quaternion.AngleAxis(angle, hinge.axisDirection);
        hinge.transform.localRotation = rotation;

        EditorUtility.SetDirty(hinge.transform);
        SceneView.RepaintAll();
    }

    void ApplyPreset(float percentage)
    {
        foreach (var hinge in selectedHinges)
        {
            if (hinge != null)
            {
                float angle = Mathf.Lerp(hinge.minAngle, hinge.maxAngle, percentage);
                SetHingeAngleInEditor(hinge, angle);
            }
        }
    }
}
