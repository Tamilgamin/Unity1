using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Utility script to generate particle system prefabs for the AR Chemistry Lab.
/// Place this script in Assets/Editor/ folder.
/// 
/// Usage: In Unity Editor, go to Tools > AR Chemistry Lab > Generate Particle Prefabs
/// </summary>
public class ParticleSystemGenerator
{
    static string PREFABS_PATH = "Assets/Prefabs/";
    static string PARTICLES_PATH = "Assets/Prefabs/Particles/";

    [MenuItem("Tools/AR Chemistry Lab/Generate Particle Prefabs")]
    public static void GenerateAllParticles()
    {
        // Create Prefabs/Particles directory
        if (!AssetDatabase.IsValidFolder(PARTICLES_PATH))
        {
            if (!AssetDatabase.IsValidFolder(PREFABS_PATH))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }
            AssetDatabase.CreateFolder(PREFABS_PATH.TrimEnd('/'), "Particles");
        }

        // Generate each particle system
        GenerateBubbleParticles();
        GenerateSmokeParticles();
        GenerateSparkParticles();
        GenerateFlameParticles();

        // Refresh and log
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ All particle systems generated successfully in " + PARTICLES_PATH);
        EditorUtility.DisplayDialog("Success", "All particle system prefabs have been generated!\n\nLocation: " + PARTICLES_PATH, "OK");
    }

    /// <summary>
    /// Generates bubble particle system
    /// </summary>
    static void GenerateBubbleParticles()
    {
        GameObject particles = new GameObject("BubbleParticles");
        ParticleSystem ps = particles.AddComponent<ParticleSystem>();

        // Configure particle system
        ParticleSystem.MainModule main = ps.main;
        main.duration = 3f;
        main.loop = false;
        main.startLifetime = 2f;
        main.startSpeed = 1.5f;
        main.startSize = 0.1f;
        main.startColor = new Color(0.7f, 0.9f, 1f, 0.6f);

        // Emission
        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 30f;

        // Shape
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        // Velocity over lifetime
        ParticleSystem.VelocityOverLifetimeModule velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.y = new ParticleSystem.MinMaxCurve(0.5f, 2f);

        // Add renderer
        ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        // Save prefab
        SaveParticlePrefab(particles, "BubbleParticles");
    }

    /// <summary>
    /// Generates smoke particle system
    /// </summary>
    static void GenerateSmokeParticles()
    {
        GameObject particles = new GameObject("SmokeParticles");
        ParticleSystem ps = particles.AddComponent<ParticleSystem>();

        // Configure particle system
        ParticleSystem.MainModule main = ps.main;
        main.duration = 4f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(2f, 4f);
        main.startSpeed = 0.5f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.5f);
        main.startColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
        main.gravityModifier = -0.2f;

        // Emission
        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 20f;

        // Shape
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 30f;
        shape.radius = 0.2f;

        // Size over lifetime
        ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = AnimationCurve.EaseInOut(0, 0.2f, 1, 1f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // Renderer
        ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        // Save prefab
        SaveParticlePrefab(particles, "SmokeParticles");
    }

    /// <summary>
    /// Generates spark particle system
    /// </summary>
    static void GenerateSparkParticles()
    {
        GameObject particles = new GameObject("SparkParticles");
        ParticleSystem ps = particles.AddComponent<ParticleSystem>();

        // Configure particle system
        ParticleSystem.MainModule main = ps.main;
        main.duration = 2f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(1f, 2f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 8f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        main.startColor = new Color(1f, 0.8f, 0.2f, 0.8f);
        main.gravityModifier = 0.5f;

        // Emission
        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 100f;

        // Shape - burst outward
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        // Velocity over lifetime
        ParticleSystem.VelocityOverLifetimeModule velocity = ps.velocityOverLifetime;
        velocity.enabled = true;

        // Renderer
        ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        // Save prefab
        SaveParticlePrefab(particles, "SparkParticles");
    }

    /// <summary>
    /// Generates flame particle system
    /// </summary>
    static void GenerateFlameParticles()
    {
        GameObject particles = new GameObject("FlameParticles");
        ParticleSystem ps = particles.AddComponent<ParticleSystem>();

        // Configure particle system
        ParticleSystem.MainModule main = ps.main;
        main.duration = 3f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(1f, 2.5f);
        main.startSpeed = 2f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.8f);
        main.startColor = Color.yellow;
        main.gravityModifier = -0.3f;

        // Emission
        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 50f;

        // Shape
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 45f;
        shape.radius = 0.2f;

        // Color over lifetime (yellow to red to transparent)
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.yellow, 0f),
                new GradientColorKey(Color.red, 0.7f),
                new GradientColorKey(new Color(1f, 0.5f, 0f, 0f), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.5f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        // Size over lifetime
        ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = AnimationCurve.EaseInOut(0, 0.3f, 1, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // Renderer
        ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        // Save prefab
        SaveParticlePrefab(particles, "FlameParticles");
    }

    /// <summary>
    /// Saves a particle system as a prefab
    /// </summary>
    static void SaveParticlePrefab(GameObject gameObject, string prefabName)
    {
        string prefabPath = PARTICLES_PATH + prefabName + ".prefab";
        
        // Save the prefab
        PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath);
        
        // Destroy the instance
        Object.DestroyImmediate(gameObject);
        
        Debug.Log("✓ Generated particle prefab: " + prefabName);
    }
}
#endif
