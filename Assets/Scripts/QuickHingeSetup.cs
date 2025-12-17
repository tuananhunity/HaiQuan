using UnityEngine;

/// <summary>
/// Helper để setup nhanh các loại khớp phổ biến
/// </summary>
public class QuickHingeSetup : MonoBehaviour
{
    public enum HingeType
    {
        Door,           // Cửa: 0-90 độ, trục Y
        Valve,          // Van xoay: 0-90 độ, trục Z
        Lever,          // Cần gạt: -45 đến 45, trục X
        Lid,            // Nắp hộp: -90 đến 0, trục X
        Wheel,          // Bánh xe: -180 đến 180, trục Y
        Custom          // Tùy chỉnh
    }

    [Header("Quick Setup")]
    public HingeType hingeType = HingeType.Valve;

    [ContextMenu("Apply Quick Setup")]
    public void ApplyQuickSetup()
    {
        CustomHingeJoint hinge = GetComponent<CustomHingeJoint>();

        if (hinge == null)
        {
            hinge = gameObject.AddComponent<CustomHingeJoint>();
            Debug.Log("Added CustomHingeJoint component");
        }

        switch (hingeType)
        {
            case HingeType.Door:
                hinge.axisDirection = Vector3.up; // Y
                hinge.minAngle = 0f;
                hinge.maxAngle = 90f;
                hinge.useLimits = true;
                hinge.rotationSpeed = 90f;
                break;

            case HingeType.Valve:
                hinge.axisDirection = Vector3.forward; // Z
                hinge.minAngle = 0f;
                hinge.maxAngle = 90f;
                hinge.useLimits = true;
                hinge.rotationSpeed = 45f;
                break;

            case HingeType.Lever:
                hinge.axisDirection = Vector3.right; // X
                hinge.minAngle = -45f;
                hinge.maxAngle = 45f;
                hinge.useLimits = true;
                hinge.rotationSpeed = 120f;
                break;

            case HingeType.Lid:
                hinge.axisDirection = Vector3.right; // X
                hinge.minAngle = -90f;
                hinge.maxAngle = 0f;
                hinge.useLimits = true;
                hinge.rotationSpeed = 60f;
                break;

            case HingeType.Wheel:
                hinge.axisDirection = Vector3.up; // Y
                hinge.minAngle = -180f;
                hinge.maxAngle = 180f;
                hinge.useLimits = false; // Xoay tự do
                hinge.rotationSpeed = 180f;
                break;
        }

        Debug.Log($"Applied {hingeType} setup to {gameObject.name}");
    }

    [ContextMenu("Test - Open")]
    public void TestOpen()
    {
        CustomHingeJoint hinge = GetComponent<CustomHingeJoint>();
        if (hinge != null) hinge.Open();
    }

    [ContextMenu("Test - Close")]
    public void TestClose()
    {
        CustomHingeJoint hinge = GetComponent<CustomHingeJoint>();
        if (hinge != null) hinge.Close();
    }

    [ContextMenu("Test - Reset")]
    public void TestReset()
    {
        CustomHingeJoint hinge = GetComponent<CustomHingeJoint>();
        if (hinge != null) hinge.Reset();
    }
}
