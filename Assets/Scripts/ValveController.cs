using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller cho van - điều khiển nhiều khớp cùng lúc
/// </summary>
public class ValveController : MonoBehaviour
{
    [Header("Valve Components")]
    [Tooltip("Các khớp cần control")]
    public CustomHingeJoint[] joints;

    [Header("Control Settings")]
    [Tooltip("Control bằng phím")]
    public bool enableKeyboardControl = true;

    [Tooltip("Tốc độ khi dùng phím (độ/giây)")]
    public float keyboardRotationSpeed = 45f;

    [Header("Input Keys")]
    public KeyCode openKey = KeyCode.O;
    public KeyCode closeKey = KeyCode.C;
    public KeyCode resetKey = KeyCode.R;

    [Header("Status")]
    public float openPercentage = 0f; // 0-100%

    void Update()
    {
        if (enableKeyboardControl && joints.Length > 0)
        {
            HandleKeyboardInput();
        }

        // Update trạng thái
        UpdateStatus();
    }

    void HandleKeyboardInput()
    {
        // Open
        if (Input.GetKey(openKey))
        {
            foreach (var joint in joints)
            {
                if (joint != null)
                    joint.Rotate(keyboardRotationSpeed * Time.deltaTime);
            }
        }

        // Close
        if (Input.GetKey(closeKey))
        {
            foreach (var joint in joints)
            {
                if (joint != null)
                    joint.Rotate(-keyboardRotationSpeed * Time.deltaTime);
            }
        }

        // Reset
        if (Input.GetKeyDown(resetKey))
        {
            ResetAll();
        }
    }

    void UpdateStatus()
    {
        if (joints.Length == 0) return;

        // Tính % mở trung bình
        float totalPercentage = 0f;
        int validJoints = 0;

        foreach (var joint in joints)
        {
            if (joint != null)
            {
                totalPercentage += joint.GetOpenPercentage();
                validJoints++;
            }
        }

        openPercentage = validJoints > 0 ? (totalPercentage / validJoints) * 100f : 0f;
    }

    // Public methods để gọi từ UI hoặc code khác
    public void OpenAll()
    {
        foreach (var joint in joints)
        {
            if (joint != null)
                joint.Open();
        }
    }

    public void CloseAll()
    {
        foreach (var joint in joints)
        {
            if (joint != null)
                joint.Close();
        }
    }

    public void ResetAll()
    {
        foreach (var joint in joints)
        {
            if (joint != null)
                joint.Reset();
        }
    }

    public void SetAllAngles(float angle)
    {
        foreach (var joint in joints)
        {
            if (joint != null)
                joint.SetTargetAngle(angle);
        }
    }

    public void SetAllAnglesImmediate(float angle)
    {
        foreach (var joint in joints)
        {
            if (joint != null)
                joint.SetAngleImmediate(angle);
        }
    }

    /// <summary>
    /// Set theo % mở (0-100)
    /// </summary>
    public void SetOpenPercentage(float percentage)
    {
        percentage = Mathf.Clamp(percentage, 0f, 100f);

        foreach (var joint in joints)
        {
            if (joint != null)
            {
                float angle = Mathf.Lerp(joint.minAngle, joint.maxAngle, percentage / 100f);
                joint.SetTargetAngle(angle);
            }
        }
    }

    /// <summary>
    /// Kiểm tra tất cả joints đã đến vị trí mục tiêu chưa
    /// </summary>
    public bool AllJointsAtTarget()
    {
        foreach (var joint in joints)
        {
            if (joint != null && !joint.IsAtTarget())
                return false;
        }
        return true;
    }
}
