using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Utility script to generate all required prefabs for the AR Chemistry Lab system.
/// Place this script in Assets/Editor/ folder.
/// 
/// Usage: In Unity Editor, go to Tools > AR Chemistry Lab > Generate All Prefabs
/// </summary>
public class PrefabGenerator
{
    static string PREFABS_PATH = "Assets/Prefabs/";

    [MenuItem("Tools/AR Chemistry Lab/Generate All Prefabs")]
    public static void GenerateAllPrefabs()
    {
        // Create Prefabs directory if it doesn't exist
        if (!AssetDatabase.IsValidFolder(PREFABS_PATH))
        {
            string guid = AssetDatabase.CreateFolder("Assets", "Prefabs");
            PREFABS_PATH = AssetDatabase.GUIDToAssetPath(guid) + "/";
        }

        // Generate each prefab
        GenerateLabTablePrefab();
        GenerateBeakerPrefab();
        GenerateTestTubePrefab();
        GenerateFlaskPrefab();
        GenerateBunsenBurnerPrefab();

        // Refresh and log completion
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ All prefabs generated successfully in " + PREFABS_PATH);
        EditorUtility.DisplayDialog("Success", "All AR Chemistry Lab prefabs have been generated!\n\nLocation: " + PREFABS_PATH, "OK");
    }

    /// <summary>
    /// Generates the Lab Table Prefab
    /// </summary>
    static void GenerateLabTablePrefab()
    {
        GameObject labTable = new GameObject("LabTable");

        // Create table surface (top)
        GameObject tableSurface = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tableSurface.name = "Surface";
        tableSurface.transform.SetParent(labTable.transform);
        tableSurface.transform.localPosition = Vector3.zero;
        tableSurface.transform.localScale = new Vector3(2f, 0.1f, 1f);
        
        // Remove the primitive collider and re-add it
        Object.DestroyImmediate(tableSurface.GetComponent<Collider>());
        tableSurface.AddComponent<BoxCollider>();

        // Remove renderer for clean mesh
        Object.DestroyImmediate(tableSurface.GetComponent<Renderer>());

        // Create table legs
        for (int i = 0; i < 4; i++)
        {
            GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leg.name = "Leg" + (i + 1);
            leg.transform.SetParent(labTable.transform);
            
            // Position legs at corners
            float xPos = (i % 2 == 0) ? 0.8f : -0.8f;
            float zPos = (i < 2) ? 0.4f : -0.4f;
            
            leg.transform.localPosition = new Vector3(xPos, -0.5f, zPos);
            leg.transform.localScale = new Vector3(0.15f, 1f, 0.15f);
            
            Object.DestroyImmediate(leg.GetComponent<Collider>());
            leg.AddComponent<BoxCollider>();
            Object.DestroyImmediate(leg.GetComponent<Renderer>());
        }

        // Add tag and layer for raycasting
        labTable.tag = "LabTable";
        labTable.layer = LayerMask.NameToLayer("Default");

        // Save as prefab
        SavePrefab(labTable, "LabTable");
    }

    /// <summary>
    /// Generates the Beaker Prefab
    /// </summary>
    static void GenerateBeakerPrefab()
    {
        GameObject beaker = new GameObject("Beaker");

        // Create beaker body (cylinder)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        body.name = "Body";
        body.transform.SetParent(beaker.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = new Vector3(0.4f, 0.6f, 0.4f);

        Object.DestroyImmediate(body.GetComponent<Collider>());
        body.AddComponent<CapsuleCollider>();

        // Change color to look like glass
        Renderer bodyRenderer = body.GetComponent<Renderer>();
        Material glassMaterial = new Material(Shader.Find("Standard"));
        glassMaterial.color = new Color(0.7f, 0.9f, 1f, 0.3f);
        bodyRenderer.material = glassMaterial;

        // Create rim (top ring) using a flattened cylinder
        GameObject rim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        if (rim != null)
        {
            rim.name = "Rim";
            rim.transform.SetParent(beaker.transform);
            rim.transform.localPosition = new Vector3(0, 0.3f, 0);
            rim.transform.localScale = new Vector3(0.42f, 0.02f, 0.42f);
            
            Object.DestroyImmediate(rim.GetComponent<Collider>());
            Object.DestroyImmediate(rim.GetComponent<Renderer>());
        }

        // Add Beaker script
        beaker.AddComponent<Beaker>();

        // Add collider for interaction
        Object.DestroyImmediate(beaker.GetComponent<Collider>());
        beaker.AddComponent<CapsuleCollider>();

        // Save as prefab
        SavePrefab(beaker, "Beaker");
    }

    /// <summary>
    /// Generates the Test Tube Prefab
    /// </summary>
    static void GenerateTestTubePrefab()
    {
        GameObject testTube = new GameObject("TestTube");

        // Create tube body (thin cylinder)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        body.name = "Body";
        body.transform.SetParent(testTube.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = new Vector3(0.2f, 0.8f, 0.2f);

        // Rotate to stand upright
        body.transform.localRotation = Quaternion.identity;

        Object.DestroyImmediate(body.GetComponent<Collider>());
        body.AddComponent<CapsuleCollider>();

        // Glass material
        Renderer bodyRenderer = body.GetComponent<Renderer>();
        Material glassMaterial = new Material(Shader.Find("Standard"));
        glassMaterial.color = new Color(0.7f, 0.9f, 1f, 0.4f);
        bodyRenderer.material = glassMaterial;

        // Create cap (small sphere at top)
        GameObject cap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cap.name = "Cap";
        cap.transform.SetParent(testTube.transform);
        cap.transform.localPosition = new Vector3(0, 0.5f, 0);
        cap.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);

        Object.DestroyImmediate(cap.GetComponent<Collider>());
        Object.DestroyImmediate(cap.GetComponent<Renderer>());

        // Add collider
        Object.DestroyImmediate(testTube.GetComponent<Collider>());
        testTube.AddComponent<CapsuleCollider>();

        // Save as prefab
        SavePrefab(testTube, "TestTube");
    }

    /// <summary>
    /// Generates the Flask Prefab
    /// </summary>
    static void GenerateFlaskPrefab()
    {
        GameObject flask = new GameObject("Flask");

        // Create main body (sphere)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        body.name = "Body";
        body.transform.SetParent(flask.transform);
        body.transform.localPosition = new Vector3(0, -0.1f, 0);
        body.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        Object.DestroyImmediate(body.GetComponent<Collider>());
        body.AddComponent<SphereCollider>();

        // Glass material
        Renderer bodyRenderer = body.GetComponent<Renderer>();
        Material glassMaterial = new Material(Shader.Find("Standard"));
        glassMaterial.color = new Color(0.7f, 0.9f, 1f, 0.3f);
        bodyRenderer.material = glassMaterial;

        // Create neck (thin cylinder)
        GameObject neck = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        neck.name = "Neck";
        neck.transform.SetParent(flask.transform);
        neck.transform.localPosition = new Vector3(0, 0.35f, 0);
        neck.transform.localScale = new Vector3(0.15f, 0.3f, 0.15f);

        Object.DestroyImmediate(neck.GetComponent<Collider>());
        Object.DestroyImmediate(neck.GetComponent<Renderer>());

        // Create cap (small sphere)
        GameObject cap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cap.name = "Cap";
        cap.transform.SetParent(flask.transform);
        cap.transform.localPosition = new Vector3(0, 0.65f, 0);
        cap.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);

        Object.DestroyImmediate(cap.GetComponent<Collider>());
        Object.DestroyImmediate(cap.GetComponent<Renderer>());

        // Add collider
        Object.DestroyImmediate(flask.GetComponent<Collider>());
        flask.AddComponent<SphereCollider>();

        // Save as prefab
        SavePrefab(flask, "Flask");
    }

    /// <summary>
    /// Generates the Bunsen Burner Prefab
    /// </summary>
    static void GenerateBunsenBurnerPrefab()
    {
        GameObject burner = new GameObject("BunsenBurner");

        // Create base (cube)
        GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseObj.name = "Base";
        baseObj.transform.SetParent(burner.transform);
        baseObj.transform.localPosition = new Vector3(0, -0.15f, 0);
        baseObj.transform.localScale = new Vector3(0.3f, 0.1f, 0.3f);

        Object.DestroyImmediate(baseObj.GetComponent<Collider>());
        baseObj.AddComponent<BoxCollider>();

        // Create stand/pole (vertical cylinder)
        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.name = "Pole";
        pole.transform.SetParent(burner.transform);
        pole.transform.localPosition = new Vector3(0, 0.3f, 0);
        pole.transform.localScale = new Vector3(0.08f, 0.6f, 0.08f);

        Object.DestroyImmediate(pole.GetComponent<Collider>());
        Object.DestroyImmediate(pole.GetComponent<Renderer>());

        // Create burner head (orange cylinder for flame effect)
        GameObject burnerHead = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        burnerHead.name = "Head";
        burnerHead.transform.SetParent(burner.transform);
        burnerHead.transform.localPosition = new Vector3(0, 0.65f, 0);
        burnerHead.transform.localScale = new Vector3(0.15f, 0.1f, 0.15f);

        Renderer headRenderer = burnerHead.GetComponent<Renderer>();
        Material metalMaterial = new Material(Shader.Find("Standard"));
        metalMaterial.color = new Color(0.3f, 0.3f, 0.3f); // Dark gray
        headRenderer.material = metalMaterial;

        Object.DestroyImmediate(burnerHead.GetComponent<Collider>());
        burnerHead.AddComponent<BoxCollider>();

        // Add collider to burner
        Object.DestroyImmediate(burner.GetComponent<Collider>());
        burner.AddComponent<BoxCollider>();

        // Save as prefab
        SavePrefab(burner, "BunsenBurner");
    }

    /// <summary>
    /// Saves a GameObject as a prefab and destroys the instance
    /// </summary>
    static void SavePrefab(GameObject gameObject, string prefabName)
    {
        string prefabPath = PREFABS_PATH + prefabName + ".prefab";
        
        // Save the prefab
        PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath);
        
        // Destroy the instance
        Object.DestroyImmediate(gameObject);
        
        Debug.Log("✓ Generated prefab: " + prefabName);
    }
}
#endif
