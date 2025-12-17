using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Chuyển động khớp/bản lề chính xác - Xoay quanh 1 trục
/// </summary>
[ExecuteInEditMode]
public class CustomHingeJoint : MonoBehaviour
{
    [Header("Hinge Settings")]
    [Tooltip("Transform làm trục xoay (để trống = dùng object này)")]
    public Transform hingeAxis;

    [Tooltip("Hướng trục xoay (Local Space)")]
    public Vector3 axisDirection = Vector3.up; // Y-axis mặc định

    [Header("Rotation Limits")]
    [Tooltip("Góc tối thiểu (độ)")]
    [Range(-180f, 180f)]
    public float minAngle = -90f;

    [Tooltip("Góc tối đa (độ)")]
    [Range(-180f, 180f)]
    public float maxAngle = 90f;

    [Tooltip("Giới hạn góc xoay")]
    public bool useLimits = true;

    [Header("Motion Settings")]
    [Tooltip("Tốc độ xoay (độ/giây)")]
    [Range(1f, 500f)]
    public float rotationSpeed = 90f;

    [Tooltip("Smooth rotation")]
    public bool smoothRotation = true;

    [Tooltip("Độ mượt (càng nhỏ càng mượt)")]
    [Range(0.01f, 1f)]
    public float smoothTime = 0.1f;

    [Header("Current State")]
    [Tooltip("Góc hiện tại (độ)")]
    public float currentAngle = 0f;

    [Tooltip("Góc mục tiêu (độ)")]
    public float targetAngle = 0f;

    [Header("Debug")]
    public bool showGizmos = true;
    public float gizmoSize = 0.5f;

    private Quaternion initialRotation;
    private float rotationVelocity = 0f;
    private Vector3 worldAxis;

    void Start()
    {
        // Lưu rotation ban đầu
        initialRotation = transform.localRotation;

        // Nếu không set hingeAxis, dùng chính object này
        if (hingeAxis == null)
            hingeAxis = transform;

        // Normalize axis
        axisDirection = axisDirection.normalized;
    }

    void Update()
    {
        // Tính world axis
        worldAxis = hingeAxis.TransformDirection(axisDirection);

        // Smooth hoặc instant rotation
        if (smoothRotation)
        {
            currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref rotationVelocity, smoothTime);
        }
        else
        {
            float step = rotationSpeed * Time.deltaTime;
            currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, step);
        }

        // Áp dụng giới hạn
        if (useLimits)
        {
            currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);
            targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);
        }

        // Apply rotation
        ApplyRotation();
    }

    void ApplyRotation()
    {
        // Xoay quanh trục
        Quaternion rotation = Quaternion.AngleAxis(currentAngle, axisDirection);
        transform.localRotation = initialRotation * rotation;
    }

    /// <summary>
    /// Set góc mục tiêu (độ)
    /// </summary>
    public void SetTargetAngle(float angle)
    {
        targetAngle = angle;
    }

    /// <summary>
    /// Set góc ngay lập tức (không smooth)
    /// </summary>
    public void SetAngleImmediate(float angle)
    {
        if (useLimits)
            angle = Mathf.Clamp(angle, minAngle, maxAngle);

        currentAngle = angle;
        targetAngle = angle;
        ApplyRotation();
    }

    /// <summary>
    /// Xoay thêm một lượng góc
    /// </summary>
    public void Rotate(float deltaAngle)
    {
        SetTargetAngle(targetAngle + deltaAngle);
    }

    /// <summary>
    /// Mở (đến góc max)
    /// </summary>
    public void Open()
    {
        SetTargetAngle(maxAngle);
    }

    /// <summary>
    /// Đóng (về góc min)
    /// </summary>
    public void Close()
    {
        SetTargetAngle(minAngle);
    }

    /// <summary>
    /// Reset về vị trí ban đầu (0 độ)
    /// </summary>
    public void Reset()
    {
        SetTargetAngle(0f);
    }

    /// <summary>
    /// Kiểm tra đã đến góc mục tiêu chưa
    /// </summary>
    public bool IsAtTarget()
    {
        return Mathf.Abs(currentAngle - targetAngle) < 0.1f;
    }

    /// <summary>
    /// Lấy % mở (0-1)
    /// </summary>
    public float GetOpenPercentage()
    {
        if (maxAngle == minAngle) return 0f;
        return Mathf.InverseLerp(minAngle, maxAngle, currentAngle);
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Transform pivot = hingeAxis != null ? hingeAxis : transform;
        Vector3 axis = axisDirection.normalized;

        // Vẽ trục xoay
        Gizmos.color = Color.green;
        Vector3 worldAxisDir = pivot.TransformDirection(axis);
        Gizmos.DrawLine(pivot.position - worldAxisDir * gizmoSize,
                       pivot.position + worldAxisDir * gizmoSize);

        // Vẽ góc giới hạn
        if (useLimits)
        {
            Gizmos.color = Color.red;
            Vector3 perpendicular = Vector3.Cross(axis, Vector3.up);
            if (perpendicular.magnitude < 0.01f)
                perpendicular = Vector3.Cross(axis, Vector3.right);
            perpendicular = pivot.TransformDirection(perpendicular.normalized);

            // Min angle
            Vector3 minDir = Quaternion.AngleAxis(minAngle, worldAxisDir) * perpendicular;
            Gizmos.DrawLine(pivot.position, pivot.position + minDir * gizmoSize);

            // Max angle
            Vector3 maxDir = Quaternion.AngleAxis(maxAngle, worldAxisDir) * perpendicular;
            Gizmos.DrawLine(pivot.position, pivot.position + maxDir * gizmoSize);

            // Current angle
            Gizmos.color = Color.yellow;
            if (Application.isPlaying)
            {
                Vector3 currentDir = Quaternion.AngleAxis(currentAngle, worldAxisDir) * perpendicular;
                Gizmos.DrawLine(pivot.position, pivot.position + currentDir * gizmoSize * 1.2f);
            }
        }

        // Vẽ sphere tại điểm xoay
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(pivot.position, 0.05f);
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ arc của góc giới hạn
        if (!showGizmos || !useLimits) return;

        Transform pivot = hingeAxis != null ? hingeAxis : transform;
        Vector3 axis = axisDirection.normalized;
        Vector3 worldAxisDir = pivot.TransformDirection(axis);

        Vector3 perpendicular = Vector3.Cross(axis, Vector3.up);
        if (perpendicular.magnitude < 0.01f)
            perpendicular = Vector3.Cross(axis, Vector3.right);
        perpendicular = pivot.TransformDirection(perpendicular.normalized);

        // Vẽ arc
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        int segments = 20;
        float angleStep = (maxAngle - minAngle) / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = minAngle + angleStep * i;
            float angle2 = minAngle + angleStep * (i + 1);

            Vector3 dir1 = Quaternion.AngleAxis(angle1, worldAxisDir) * perpendicular;
            Vector3 dir2 = Quaternion.AngleAxis(angle2, worldAxisDir) * perpendicular;

            Gizmos.DrawLine(pivot.position + dir1 * gizmoSize * 0.8f,
                           pivot.position + dir2 * gizmoSize * 0.8f);
        }
    }
}
