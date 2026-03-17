
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

/// <summary>
/// Handles AR plane detection and lab table placement.
/// Allows users to tap on detected horizontal planes to place a lab table prefab.
/// Only one table can be placed at a time.
/// </summary>
public class ARObjectPlacement : MonoBehaviour
{
    /// <summary>
    /// Reference to the ARRaycastManager for raycasting against AR planes
    /// </summary>
    [SerializeField]
    private ARRaycastManager arRaycastManager;

    /// <summary>
    /// Reference to the ARPlaneManager for managing detected planes
    /// </summary>
    [SerializeField]
    private ARPlaneManager arPlaneManager;

    /// <summary>
    /// Prefab of the lab table to instantiate
    /// </summary>
    [SerializeField]
    private GameObject labTablePrefab;

    /// <summary>
    /// Reference to the currently placed lab table
    /// </summary>
    private GameObject placedLabTable;

    /// <summary>
    /// Indicates if a table has already been placed
    /// </summary>
    private bool isTablePlaced = false;

    /// <summary>
    /// Reusable list for raycast hits to avoid allocating memory each frame
    /// </summary>
    private static List<ARRaycastHit> rayCastHits = new List<ARRaycastHit>();

    /// <summary>
    /// Events for UI feedback
    /// </summary>
    public delegate void TablePlacedDelegate(Vector3 position);
    public event TablePlacedDelegate OnTablePlaced;

    public delegate void PlacementFailedDelegate(string reason);
    public event PlacementFailedDelegate OnPlacementFailed;

    private void Start()
    {
        // Validate required references
        if (arRaycastManager == null)
        {
            arRaycastManager = GetComponent<ARRaycastManager>();
        }

        if (arRaycastManager == null)
        {
            Debug.LogError("ARRaycastManager not found or assigned. AR placement will not work.");
        }

        if (arPlaneManager == null)
        {
            arPlaneManager = GetComponent<ARPlaneManager>();
        }

        if (arPlaneManager == null)
        {
            Debug.LogError("ARPlaneManager not found or assigned. Plane detection will not work.");
        }

        if (labTablePrefab == null)
        {
            Debug.LogError("Lab table prefab is not assigned in the inspector.");
        }
    }

    private void Update()
    {
        HandleTouchInput();
    }

    /// <summary>
    /// Handles touch input for placing the lab table
    /// </summary>
    private void HandleTouchInput()
    {
        // Only process if a table hasn't been placed yet
        if (isTablePlaced)
            return;

        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Only process touch began phase
            if (touch.phase == TouchPhase.Began)
            {
                HandleTap(touch.position);
            }
        }
        // For editor testing, also support mouse click
        else if (Input.GetMouseButtonDown(0))
        {
            HandleTap(Input.mousePosition);
        }
    }

    /// <summary>
    /// Processes a tap/click at the given screen position
    /// </summary>
    private void HandleTap(Vector2 screenPosition)
    {
        // Raycast against AR planes
        if (arRaycastManager.Raycast(screenPosition, rayCastHits, TrackableType.PlaneWithinPolygon))
        {
            // Get the first hit (closest plane)
            ARRaycastHit hit = rayCastHits[0];

            // Validate the hit plane is horizontal
            if (arPlaneManager != null)
            {
                ARPlane plane = arPlaneManager.GetPlane(hit.trackableId);
                if (plane != null && (plane.alignment == PlaneAlignment.HorizontalUp || plane.alignment == PlaneAlignment.HorizontalDown))
                {
                    PlaceLabTable(hit.pose.position, hit.pose.rotation);
                }
                else
                {
                    OnPlacementFailed?.Invoke("Please tap on a horizontal surface");
                    Debug.Log("Hit plane is not horizontal. Please tap on a horizontal surface.");
                }
            }
            else
            {
                // No plane manager, just place the table
                PlaceLabTable(hit.pose.position, hit.pose.rotation);
            }
        }
        else
        {
            Debug.Log("Raycast did not hit any plane. Try tapping on a detected plane surface.");
            OnPlacementFailed?.Invoke("No plane detected at this location");
        }
    }

    /// <summary>
    /// Places the lab table at the specified position and rotation
    /// </summary>
    private void PlaceLabTable(Vector3 position, Quaternion rotation)
    {
        if (labTablePrefab == null)
        {
            Debug.LogError("Lab table prefab is not set. Cannot place table.");
            OnPlacementFailed?.Invoke("Lab table prefab not available");
            return;
        }

        // Instantiate the lab table
        placedLabTable = Instantiate(labTablePrefab, position, rotation);

        if (placedLabTable == null)
        {
            Debug.LogError("Failed to instantiate lab table prefab.");
            OnPlacementFailed?.Invoke("Failed to instantiate lab table");
            return;
        }

        // Mark that a table has been placed
        isTablePlaced = true;

        // Disable plane detection to reduce processing
        if (arPlaneManager != null)
        {
            foreach (ARPlane plane in arPlaneManager.GetComponentsInChildren<ARPlane>())
            {
                plane.gameObject.SetActive(false);
            }
            arPlaneManager.enabled = false;
        }

        Debug.Log($"Lab table placed at position: {position}");
        OnTablePlaced?.Invoke(position);
    }

    /// <summary>
    /// Gets the currently placed lab table
    /// </summary>
    public GameObject GetPlacedLabTable()
    {
        return placedLabTable;
    }

    /// <summary>
    /// Checks if a table has been placed
    /// </summary>
    public bool IsTablePlaced()
    {
        return isTablePlaced;
    }

    /// <summary>
    /// Removes the placed lab table and allows placement again
    /// </summary>
    public void RemoveLabTable()
    {
        if (placedLabTable != null)
        {
            Destroy(placedLabTable);
            placedLabTable = null;
        }

        isTablePlaced = false;

        // Re-enable plane detection
        if (arPlaneManager != null)
        {
            arPlaneManager.enabled = true;
            foreach (ARPlane plane in arPlaneManager.GetComponentsInChildren<ARPlane>())
            {
                plane.gameObject.SetActive(true);
            }
        }

        Debug.Log("Lab table removed. Plane detection re-enabled.");
    }
}
