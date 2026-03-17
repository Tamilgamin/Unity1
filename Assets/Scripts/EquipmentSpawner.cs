using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles spawning laboratory equipment on the lab table.
/// Supports spawning Beakers, Test Tubes, Flasks, and Bunsen Burners.
/// Uses raycasting from touch input to determine spawn position.
/// </summary>
public class EquipmentSpawner : MonoBehaviour
{
    /// <summary>
    /// Enum for equipment types
    /// </summary>
    public enum EquipmentType
    {
        Beaker,
        TestTube,
        Flask,
        BunsenBurner
    }

    /// <summary>
    /// Prefabs for each equipment type
    /// </summary>
    [SerializeField]
    private GameObject beakerPrefab;

    [SerializeField]
    private GameObject testTubePrefab;

    [SerializeField]
    private GameObject flaskPrefab;

    [SerializeField]
    private GameObject bunsenBurnerPrefab;

    /// <summary>
    /// Reference to the lab table transform for positioning equipment relative to it
    /// </summary>
    private Transform labTableTransform;

    /// <summary>
    /// List to track spawned equipment for cleanup
    /// </summary>
    private List<GameObject> spawnedEquipment = new List<GameObject>();

    /// <summary>
    /// Reference to the AR Object Placement script to get table reference
    /// </summary>
    private ARObjectPlacement arObjectPlacement;

    /// <summary>
    /// Layer mask for raycasting on the lab table
    /// </summary>
    [SerializeField]
    private LayerMask tableLayerMask = -1;

    /// <summary>
    /// Maximum number of equipment pieces allowed on the table
    /// </summary>
    [SerializeField]
    private int maxEquipmentCount = 20;

    /// <summary>
    /// Spacing between spawned equipment to avoid overlap
    /// </summary>
    [SerializeField]
    private float equipmentSpacing = 0.1f;

    /// <summary>
    /// Events for equipment spawning
    /// </summary>
    public delegate void EquipmentSpawnedDelegate(GameObject equipment, EquipmentType type);
    public event EquipmentSpawnedDelegate OnEquipmentSpawned;

    public delegate void EquipmentRemovedDelegate(GameObject equipment, EquipmentType type);
    public event EquipmentRemovedDelegate OnEquipmentRemoved;

    private void Start()
    {
        // Try to find the ARObjectPlacement script
        arObjectPlacement = FindObjectOfType<ARObjectPlacement>();

        // Validate equipment prefabs
        List<string> missingPrefabs = new List<string>();
        if (beakerPrefab == null) missingPrefabs.Add("Beaker");
        if (testTubePrefab == null) missingPrefabs.Add("TestTube");
        if (flaskPrefab == null) missingPrefabs.Add("Flask");
        if (bunsenBurnerPrefab == null) missingPrefabs.Add("BunsenBurner");

        if (missingPrefabs.Count > 0)
        {
            Debug.LogWarning($"Missing prefabs: {string.Join(", ", missingPrefabs)}");
        }
    }

    private void Update()
    {
        HandleEquipmentSpawning();
    }

    /// <summary>
    /// Handles touch input for spawning equipment
    /// </summary>
    private void HandleEquipmentSpawning()
    {
        // Get the lab table reference
        if (labTableTransform == null && arObjectPlacement != null)
        {
            GameObject table = arObjectPlacement.GetPlacedLabTable();
            if (table != null)
            {
                labTableTransform = table.transform;
            }
        }

        // Only spawn if we have a table
        if (labTableTransform == null)
            return;

        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                HandleEquipmentTap(touch.position);
            }
        }
        // For editor testing
        else if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnBeaker();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnTestTube();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            SpawnFlask();
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            SpawnBunsenBurner();
        }
    }

    /// <summary>
    /// Handles a tap to detect where equipment should be spawned
    /// </summary>
    private void HandleEquipmentTap(Vector2 screenPosition)
    {
        if (labTableTransform == null)
            return;

        // Raycast from the camera to the table surface
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 1000f, tableLayerMask))
        {
            // Check if we hit the table
            if (hitInfo.transform.IsChildOf(labTableTransform) || hitInfo.transform == labTableTransform)
            {
                // Spawn a beaker at the hit position for demo purposes
                SpawnEquipmentAtPosition(EquipmentType.Beaker, hitInfo.point, hitInfo.normal);
            }
        }
    }

    /// <summary>
    /// Spawns a beaker at the default table position
    /// </summary>
    public void SpawnBeaker()
    {
        SpawnEquipment(EquipmentType.Beaker, beakerPrefab);
    }

    /// <summary>
    /// Spawns a test tube at the default table position
    /// </summary>
    public void SpawnTestTube()
    {
        SpawnEquipment(EquipmentType.TestTube, testTubePrefab);
    }

    /// <summary>
    /// Spawns a flask at the default table position
    /// </summary>
    public void SpawnFlask()
    {
        SpawnEquipment(EquipmentType.Flask, flaskPrefab);
    }

    /// <summary>
    /// Spawns a Bunsen burner at the default table position
    /// </summary>
    public void SpawnBunsenBurner()
    {
        SpawnEquipment(EquipmentType.BunsenBurner, bunsenBurnerPrefab);
    }

    /// <summary>
    /// Internal method to spawn equipment with default positioning
    /// </summary>
    private void SpawnEquipment(EquipmentType type, GameObject prefab)
    {
        if (labTableTransform == null)
        {
            Debug.LogWarning("Lab table not placed yet. Cannot spawn equipment.");
            return;
        }

        // Calculate spawn position on the table
        Vector3 spawnPosition = labTableTransform.position + 
                              Vector3.up * 0.5f + 
                              labTableTransform.forward * (0.2f + spawnedEquipment.Count * equipmentSpacing);

        SpawnEquipmentAtPosition(type, spawnPosition, labTableTransform.forward);
    }

    /// <summary>
    /// Spawns equipment at a specific position
    /// </summary>
    private void SpawnEquipmentAtPosition(EquipmentType type, Vector3 position, Vector3 normal)
    {
        if (spawnedEquipment.Count >= maxEquipmentCount)
        {
            Debug.LogWarning("Maximum equipment limit reached on the table.");
            return;
        }

        // Get the appropriate prefab
        GameObject prefab = GetPrefabForType(type);

        if (prefab == null)
        {
            Debug.LogError($"Prefab for {type} is not set in the inspector.");
            return;
        }

        // Align rotation so the equipment sits upright on the surface
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);
        rotation = Quaternion.identity; // For simplicity, keep upright in world space

        // Instantiate the equipment
        GameObject equipment = Instantiate(prefab, position, rotation);

        if (equipment != null)
        {
            // Parent to the table for easier management
            equipment.transform.SetParent(labTableTransform, true);

            // Add a collider if it doesn't have one
            if (equipment.GetComponent<Collider>() == null)
            {
                equipment.AddComponent<BoxCollider>();
            }

            // Add rigidbody if it doesn't have one (for physics interactions)
            if (equipment.GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = equipment.AddComponent<Rigidbody>();
                rb.isKinematic = true; // Keep it kinematic for AR stability
            }

            // Track the spawned equipment
            spawnedEquipment.Add(equipment);

            Debug.Log($"Spawned {type} at position {position}");
            OnEquipmentSpawned?.Invoke(equipment, type);
        }
    }

    /// <summary>
    /// Gets the prefab for a given equipment type
    /// </summary>
    private GameObject GetPrefabForType(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Beaker:
                return beakerPrefab;
            case EquipmentType.TestTube:
                return testTubePrefab;
            case EquipmentType.Flask:
                return flaskPrefab;
            case EquipmentType.BunsenBurner:
                return bunsenBurnerPrefab;
            default:
                return null;
        }
    }

    /// <summary>
    /// Removes spawned equipment from the scene
    /// </summary>
    public void RemoveEquipment(GameObject equipment)
    {
        if (spawnedEquipment.Remove(equipment))
        {
            // Try to determine equipment type from name
            EquipmentType type = EquipmentType.Beaker;
            if (equipment.name.Contains("TestTube")) type = EquipmentType.TestTube;
            else if (equipment.name.Contains("Flask")) type = EquipmentType.Flask;
            else if (equipment.name.Contains("Burner")) type = EquipmentType.BunsenBurner;
            
            Destroy(equipment);
            OnEquipmentRemoved?.Invoke(equipment, type);
            Debug.Log("Equipment removed.");
        }
    }

    /// <summary>
    /// Clears all spawned equipment
    /// </summary>
    public void ClearAllEquipment()
    {
        foreach (GameObject equipment in spawnedEquipment)
        {
            if (equipment != null)
            {
                Destroy(equipment);
            }
        }
        spawnedEquipment.Clear();
        Debug.Log("All equipment cleared.");
    }

    /// <summary>
    /// Gets the count of spawned equipment
    /// </summary>
    public int GetEquipmentCount()
    {
        return spawnedEquipment.Count;
    }

    /// <summary>
    /// Gets the list of spawned equipment
    /// </summary>
    public List<GameObject> GetSpawnedEquipment()
    {
        return new List<GameObject>(spawnedEquipment);
    }
}
