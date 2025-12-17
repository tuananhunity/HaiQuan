using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasFlowPathEffect : MonoBehaviour
{
    [Header("Path Settings")]
    [Tooltip("Các điểm tạo thành đường ống (thêm/xóa để tạo đường cong)")]
    public Transform[] pathPoints;

    [Tooltip("Tự động tạo path points từ children")]
    public bool useChildrenAsPath = false;

    [Tooltip("Độ mượt của đường cong (càng cao càng mượt)")]
    [Range(10, 100)]
    public int pathResolution = 50;

    [Header("Flow Settings")]
    [Range(0.1f, 20f)]
    public float flowSpeed = 5f;

    [Range(10, 500)]
    public int emissionRate = 100;

    [Range(0.01f, 0.5f)]
    public float particleSize = 0.15f;

    [Tooltip("Biến thiên kích thước (0 = size cố định, 1 = random)")]
    [Range(0f, 1f)]
    public float sizeVariation = 0.5f;

    [Header("Pipe Settings")]
    [Range(0.1f, 2f)]
    public float pipeDiameter = 0.5f;

    [Header("Visual Settings - Gas")]
    public Color gasColor = new Color(0.9f, 0.9f, 0.95f, 0.15f); // Nhạt hơn, trong suốt hơn
    public bool isFlowing = true;

    [Tooltip("Particles lớn lên khi chảy (giống khí lan tỏa)")]
    [Range(1f, 3f)]
    public float sizeGrowth = 1.8f;

    [Tooltip("Xoay particles (tạo cảm giác động)")]
    public bool enableRotation = true;

    [Range(0f, 1f)]
    public float turbulence = 0.3f; // Tăng mặc định cho khí

    [Tooltip("Blend mode: Additive = sáng, Alpha = mờ tự nhiên")]
    public enum BlendMode { AlphaBlend, Additive }
    public BlendMode blendMode = BlendMode.AlphaBlend;

    [Header("Debug")]
    public bool showPath = true;
    public Color pathColor = Color.cyan;

    private ParticleSystem gasParticleSystem;
    private ParticleSystem.Particle[] particles;
    private List<Vector3> smoothPath;
    private float totalPathLength;

    void Start()
    {
        SetupPath();
        SetupParticleSystem();
    }

    void SetupPath()
    {
        // Nếu dùng children làm path points
        if (useChildrenAsPath)
        {
            List<Transform> childPoints = new List<Transform>();
            foreach (Transform child in transform)
            {
                childPoints.Add(child);
            }
            pathPoints = childPoints.ToArray();
        }

        // Tạo smooth path từ path points
        if (pathPoints != null && pathPoints.Length >= 2)
        {
            smoothPath = GenerateSmoothPath(pathPoints, pathResolution);
            totalPathLength = CalculatePathLength(smoothPath);
        }
        else
        {
            Debug.LogWarning("GasFlowPathEffect: Cần ít nhất 2 điểm để tạo path!");
            smoothPath = new List<Vector3> { transform.position, transform.position + transform.forward * 5f };
            totalPathLength = 5f;
        }
    }

    void SetupParticleSystem()
    {
        GameObject particleObj = new GameObject("GasParticles");
        particleObj.transform.SetParent(transform);
        particleObj.transform.localPosition = Vector3.zero;
        gasParticleSystem = particleObj.AddComponent<ParticleSystem>();

        // === MAIN MODULE ===
        var main = gasParticleSystem.main;
        main.startSpeed = 0; // Particles sẽ được control bởi script

        // Size với variation (khí không đồng đều)
        main.startSize = new ParticleSystem.MinMaxCurve(
            particleSize * (1f - sizeVariation),
            particleSize * (1f + sizeVariation)
        );

        main.startColor = gasColor;
        main.startLifetime = totalPathLength / flowSpeed;
        main.maxParticles = emissionRate * (int)(totalPathLength / flowSpeed);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.loop = true;

        // Rotation cho khí
        if (enableRotation)
        {
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
        }

        // === EMISSION MODULE ===
        var emission = gasParticleSystem.emission;
        emission.rateOverTime = emissionRate;

        // === SHAPE MODULE ===
        var shape = gasParticleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = pipeDiameter / 4f;

        // === SIZE OVER LIFETIME - Khí lan tỏa ===
        var sizeOverLifetime = gasParticleSystem.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 1f);           // Bắt đầu: size bình thường
        sizeCurve.AddKey(0.3f, 1.2f);       // Giữa đường: lớn lên một chút
        sizeCurve.AddKey(1f, sizeGrowth);   // Cuối: lớn ra (khí lan tỏa)
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // === ROTATION OVER LIFETIME ===
        if (enableRotation)
        {
            var rotationOverLifetime = gasParticleSystem.rotationOverLifetime;
            rotationOverLifetime.enabled = true;
            rotationOverLifetime.z = new ParticleSystem.MinMaxCurve(-90f * Mathf.Deg2Rad, 90f * Mathf.Deg2Rad);
        }

        // === COLOR OVER LIFETIME - Mờ dần ===
        var colorOverLifetime = gasParticleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(gasColor, 0.0f),
                new GradientColorKey(gasColor, 0.5f),
                new GradientColorKey(new Color(gasColor.r, gasColor.g, gasColor.b, 0), 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f, 0.0f),         // Bắt đầu trong suốt
                new GradientAlphaKey(gasColor.a, 0.2f), // Hiện rõ dần
                new GradientAlphaKey(gasColor.a, 0.7f), // Giữ opacity
                new GradientAlphaKey(0f, 1.0f)          // Mờ hẳn cuối đường
            }
        );
        colorOverLifetime.color = gradient;

        // === NOISE/TURBULENCE - Quan trọng cho khí! ===
        var noise = gasParticleSystem.noise;
        noise.enabled = true;
        noise.strength = turbulence * 2f; // Turbulence mạnh hơn cho khí
        noise.frequency = 0.8f;
        noise.scrollSpeed = 0.5f;
        noise.damping = true;
        noise.octaveCount = 2; // Thêm detail

        // === RENDERER ===
        var renderer = gasParticleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortMode = ParticleSystemSortMode.Distance;

        // Shader phù hợp với khí
        string shaderName = blendMode == BlendMode.Additive
            ? "Particles/Additive"
            : "Particles/Standard Unlit";
        renderer.material = new Material(Shader.Find(shaderName));
        renderer.material.SetFloat("_SoftParticlesEnabled", 1f);

        // Khởi tạo particle array
        particles = new ParticleSystem.Particle[main.maxParticles];
    }

    void LateUpdate()
    {
        if (gasParticleSystem == null || smoothPath == null || smoothPath.Count < 2)
            return;

        if (!isFlowing)
        {
            if (gasParticleSystem.isPlaying)
                gasParticleSystem.Stop();
            return;
        }
        else if (!gasParticleSystem.isPlaying)
        {
            gasParticleSystem.Play();
        }

        // Update particles theo path
        int particleCount = gasParticleSystem.GetParticles(particles);

        for (int i = 0; i < particleCount; i++)
        {
            // Tính % lifetime (0 -> 1)
            float lifetimePercent = 1f - (particles[i].remainingLifetime / particles[i].startLifetime);

            // Map lifetime sang path position
            Vector3 targetPosition = GetPositionOnPath(lifetimePercent);
            Vector3 targetDirection = GetDirectionOnPath(lifetimePercent);

            // Smooth movement
            particles[i].position = targetPosition;

            // Thêm turbulence offset
            if (turbulence > 0)
            {
                Vector3 turbulenceOffset = new Vector3(
                    Mathf.PerlinNoise(Time.time + i * 0.1f, 0) - 0.5f,
                    Mathf.PerlinNoise(Time.time + i * 0.1f, 100) - 0.5f,
                    Mathf.PerlinNoise(Time.time + i * 0.1f, 200) - 0.5f
                ) * turbulence * pipeDiameter;

                particles[i].position += turbulenceOffset;
            }

            // Set velocity để tạo motion blur
            particles[i].velocity = targetDirection * flowSpeed;
        }

        gasParticleSystem.SetParticles(particles, particleCount);
    }

    Vector3 GetPositionOnPath(float t)
    {
        t = Mathf.Clamp01(t);
        float totalLength = 0f;
        float targetLength = t * totalPathLength;

        for (int i = 0; i < smoothPath.Count - 1; i++)
        {
            float segmentLength = Vector3.Distance(smoothPath[i], smoothPath[i + 1]);

            if (totalLength + segmentLength >= targetLength)
            {
                float segmentT = (targetLength - totalLength) / segmentLength;
                return Vector3.Lerp(smoothPath[i], smoothPath[i + 1], segmentT);
            }

            totalLength += segmentLength;
        }

        return smoothPath[smoothPath.Count - 1];
    }

    Vector3 GetDirectionOnPath(float t)
    {
        float epsilon = 0.01f;
        Vector3 pos1 = GetPositionOnPath(t);
        Vector3 pos2 = GetPositionOnPath(Mathf.Clamp01(t + epsilon));
        return (pos2 - pos1).normalized;
    }

    List<Vector3> GenerateSmoothPath(Transform[] points, int resolution)
    {
        List<Vector3> path = new List<Vector3>();

        if (points.Length < 2) return path;

        // Catmull-Rom spline cho đường cong mượt
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 p0 = i > 0 ? points[i - 1].position : points[i].position;
            Vector3 p1 = points[i].position;
            Vector3 p2 = points[i + 1].position;
            Vector3 p3 = i < points.Length - 2 ? points[i + 2].position : points[i + 1].position;

            int segmentResolution = resolution / (points.Length - 1);

            for (int j = 0; j < segmentResolution; j++)
            {
                float t = j / (float)segmentResolution;
                Vector3 point = CalculateCatmullRom(p0, p1, p2, p3, t);
                path.Add(point);
            }
        }

        path.Add(points[points.Length - 1].position);

        return path;
    }

    Vector3 CalculateCatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        Vector3 result = 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );

        return result;
    }

    float CalculatePathLength(List<Vector3> path)
    {
        float length = 0f;
        for (int i = 0; i < path.Count - 1; i++)
        {
            length += Vector3.Distance(path[i], path[i + 1]);
        }
        return length;
    }

    // Public methods
    public void StartFlow() => isFlowing = true;
    public void StopFlow() => isFlowing = false;
    public void SetFlowSpeed(float speed) => flowSpeed = speed;

    // Gizmos
    void OnDrawGizmos()
    {
        if (!showPath) return;

        // Vẽ path points
        if (pathPoints != null && pathPoints.Length > 0)
        {
            Gizmos.color = Color.yellow;
            foreach (var point in pathPoints)
            {
                if (point != null)
                    Gizmos.DrawSphere(point.position, 0.1f);
            }

            // Vẽ đường nối
            Gizmos.color = Color.red;
            for (int i = 0; i < pathPoints.Length - 1; i++)
            {
                if (pathPoints[i] != null && pathPoints[i + 1] != null)
                    Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
            }
        }

        // Vẽ smooth path
        if (smoothPath != null && smoothPath.Count > 1)
        {
            Gizmos.color = pathColor;
            for (int i = 0; i < smoothPath.Count - 1; i++)
            {
                Gizmos.DrawLine(smoothPath[i], smoothPath[i + 1]);

                // Vẽ pipe diameter
                if (i % 10 == 0)
                {
                    DrawWireSphere(smoothPath[i], pipeDiameter / 2f);
                }
            }
        }
    }

    void DrawWireSphere(Vector3 center, float radius)
    {
        int segments = 16;
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * radius;
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * radius;

            Gizmos.DrawLine(point1, point2);
        }
    }

    void OnValidate()
    {
        // Update path khi thay đổi settings trong Editor
        if (Application.isPlaying && pathPoints != null && pathPoints.Length >= 2)
        {
            SetupPath();
            if (gasParticleSystem != null)
            {
                var main = gasParticleSystem.main;
                main.startLifetime = totalPathLength / flowSpeed;
            }
        }
    }
}
