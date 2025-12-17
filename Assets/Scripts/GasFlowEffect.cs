using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasFlowEffect : MonoBehaviour
{
    [Header("Particle System")]
    public ParticleSystem gasParticleSystem;

    [Header("Flow Settings")]
    [Tooltip("Tốc độ dòng khí (m/s)")]
    [Range(0.1f, 20f)]
    public float flowSpeed = 5f;

    [Tooltip("Lượng khí phát ra (particles/second)")]
    [Range(10, 1000)]
    public int emissionRate = 100;

    [Tooltip("Kích thước hạt khí")]
    [Range(0.01f, 1f)]
    public float particleSize = 0.1f;

    [Header("Pipe Settings")]
    [Tooltip("Đường kính ống")]
    [Range(0.1f, 5f)]
    public float pipeDiameter = 1f;

    [Tooltip("Chiều dài ống")]
    [Range(1f, 50f)]
    public float pipeLength = 10f;

    [Header("Visual Settings")]
    [Tooltip("Màu dòng khí")]
    public Color gasColor = new Color(0.8f, 0.8f, 1f, 0.3f);

    [Tooltip("Bật/tắt dòng khí")]
    public bool isFlowing = true;

    [Header("Advanced")]
    [Tooltip("Độ loạn lưu (turbulence)")]
    [Range(0f, 2f)]
    public float turbulence = 0.2f;

    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.ShapeModule shapeModule;
    private ParticleSystem.ColorOverLifetimeModule colorModule;
    private ParticleSystem.NoiseModule noiseModule;

    void Start()
    {
        // Tạo Particle System nếu chưa có
        if (gasParticleSystem == null)
        {
            GameObject particleObj = new GameObject("GasParticles");
            particleObj.transform.SetParent(transform);
            particleObj.transform.localPosition = Vector3.zero;
            gasParticleSystem = particleObj.AddComponent<ParticleSystem>();
        }

        SetupParticleSystem();
    }

    void SetupParticleSystem()
    {
        // Main Module
        mainModule = gasParticleSystem.main;
        mainModule.startSpeed = flowSpeed;
        mainModule.startSize = particleSize;
        mainModule.startColor = gasColor;
        mainModule.startLifetime = pipeLength / flowSpeed; // Thời gian sống = chiều dài / tốc độ
        mainModule.maxParticles = 5000;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;

        // Emission Module
        emissionModule = gasParticleSystem.emission;
        emissionModule.rateOverTime = emissionRate;

        // Shape Module - Phát từ hình tròn (đầu ống)
        shapeModule = gasParticleSystem.shape;
        shapeModule.enabled = true;
        shapeModule.shapeType = ParticleSystemShapeType.Circle;
        shapeModule.radius = pipeDiameter / 2f;
        shapeModule.radiusThickness = 0.9f; // Phát từ vùng gần mép ống

        // Color Over Lifetime - Mờ dần
        colorModule = gasParticleSystem.colorOverLifetime;
        colorModule.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(gasColor, 0.0f),
                new GradientColorKey(gasColor, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(gasColor.a, 0.0f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );
        colorModule.color = gradient;

        // Noise Module - Tạo hiệu ứng loạn lưu
        noiseModule = gasParticleSystem.noise;
        noiseModule.enabled = true;
        noiseModule.strength = turbulence;
        noiseModule.frequency = 0.5f;
        noiseModule.scrollSpeed = 1f;
        noiseModule.damping = true;

        // Renderer
        var renderer = gasParticleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
    }

    void Update()
    {
        // Cập nhật settings real-time
        if (gasParticleSystem != null)
        {
            mainModule.startSpeed = flowSpeed;
            mainModule.startSize = particleSize;
            mainModule.startColor = gasColor;
            mainModule.startLifetime = pipeLength / flowSpeed;

            emissionModule.rateOverTime = emissionRate;

            shapeModule.radius = pipeDiameter / 2f;

            noiseModule.strength = turbulence;

            // Bật/tắt
            if (isFlowing && !gasParticleSystem.isPlaying)
            {
                gasParticleSystem.Play();
            }
            else if (!isFlowing && gasParticleSystem.isPlaying)
            {
                gasParticleSystem.Stop();
            }
        }
    }

    // Phương thức public để control từ code khác
    public void StartFlow()
    {
        isFlowing = true;
    }

    public void StopFlow()
    {
        isFlowing = false;
    }

    public void SetFlowSpeed(float speed)
    {
        flowSpeed = speed;
    }

    public void SetEmissionRate(int rate)
    {
        emissionRate = rate;
    }

    // Vẽ Gizmos để visualize trong Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        // Vẽ đường ống
        Vector3 start = transform.position;
        Vector3 end = transform.position + transform.forward * pipeLength;

        // Vẽ circle ở đầu ống
        DrawCircle(start, transform.forward, pipeDiameter / 2f);

        // Vẽ đường trục
        Gizmos.DrawLine(start, end);

        // Vẽ circle ở cuối ống
        DrawCircle(end, transform.forward, pipeDiameter / 2f);
    }

    void DrawCircle(Vector3 center, Vector3 normal, float radius)
    {
        Vector3 right = Vector3.Cross(normal, Vector3.up).normalized;
        if (right == Vector3.zero) right = Vector3.Cross(normal, Vector3.forward).normalized;
        Vector3 up = Vector3.Cross(right, normal).normalized;

        int segments = 32;
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

            Vector3 point1 = center + (right * Mathf.Cos(angle1) + up * Mathf.Sin(angle1)) * radius;
            Vector3 point2 = center + (right * Mathf.Cos(angle2) + up * Mathf.Sin(angle2)) * radius;

            Gizmos.DrawLine(point1, point2);
        }
    }
}
