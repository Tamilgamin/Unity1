using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Represents a beaker container that can hold chemicals.
/// Tracks the chemicals inside and notifies listeners when chemicals are added.
/// </summary>
public class Beaker : MonoBehaviour
{
    /// <summary>
    /// Maximum volume capacity of this beaker in milliliters
    /// </summary>
    [SerializeField]
    private float maxCapacityMl = 500f;

    /// <summary>
    /// Current volume of chemicals in the beaker
    /// </summary>
    private float currentVolumeMl = 0f;

    /// <summary>
    /// List of chemicals currently inside the beaker
    /// </summary>
    private List<Chemical> chemicalsInside = new List<Chemical>();

    /// <summary>
    /// Delegate for chemical added event
    /// </summary>
    public delegate void ChemicalAddedDelegate(Chemical chemical);

    /// <summary>
    /// Event triggered when a chemical is added to the beaker
    /// </summary>
    public event ChemicalAddedDelegate OnChemicalAdded;

    /// <summary>
    /// Delegate for chemical removed event
    /// </summary>
    public delegate void ChemicalRemovedDelegate(Chemical chemical);

    /// <summary>
    /// Event triggered when a chemical is removed from the beaker
    /// </summary>
    public event ChemicalRemovedDelegate OnChemicalRemoved;

    /// <summary>
    /// Visual indicator for the beaker's current fill level
    /// </summary>
    private Image fillImage;

    private void Start()
    {
        // Try to find a UI Image component that represents the fill level
        fillImage = GetComponentInChildren<Image>();
    }

    /// <summary>
    /// Adds a chemical to the beaker if there's enough capacity
    /// </summary>
    public bool AddChemical(Chemical chemical)
    {
        if (chemical == null)
        {
            Debug.LogWarning("Attempted to add null chemical to beaker");
            return false;
        }

        // Check if adding this chemical would exceed capacity
        if (currentVolumeMl + chemical.volumeMl > maxCapacityMl)
        {
            Debug.LogWarning($"Cannot add {chemical.chemicalName}: exceeds capacity. Current: {currentVolumeMl}ml, Max: {maxCapacityMl}ml");
            return false;
        }

        chemicalsInside.Add(chemical);
        currentVolumeMl += chemical.volumeMl;

        // Update visual fill level
        UpdateFillLevel();

        // Notify listeners
        OnChemicalAdded?.Invoke(chemical);

        Debug.Log($"Added {chemical.chemicalName} to beaker. Current volume: {currentVolumeMl}ml");
        return true;
    }

    /// <summary>
    /// Removes a specific chemical from the beaker
    /// </summary>
    public bool RemoveChemical(Chemical chemical)
    {
        if (chemicalsInside.Remove(chemical))
        {
            currentVolumeMl -= chemical.volumeMl;
            UpdateFillLevel();
            OnChemicalRemoved?.Invoke(chemical);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Empties all chemicals from the beaker
    /// </summary>
    public void Empty()
    {
        chemicalsInside.Clear();
        currentVolumeMl = 0f;
        UpdateFillLevel();
    }

    /// <summary>
    /// Gets the list of chemicals currently in the beaker
    /// </summary>
    public List<Chemical> GetChemicals()
    {
        return new List<Chemical>(chemicalsInside);
    }

    /// <summary>
    /// Gets the current volume of liquid in the beaker
    /// </summary>
    public float GetCurrentVolume()
    {
        return currentVolumeMl;
    }

    /// <summary>
    /// Gets the number of chemicals in the beaker
    /// </summary>
    public int GetChemicalCount()
    {
        return chemicalsInside.Count;
    }

    /// <summary>
    /// Gets the average color of all chemicals in the beaker
    /// </summary>
    public Color GetMixedColor()
    {
        if (chemicalsInside.Count == 0)
            return Color.white;

        Color mixedColor = Color.black;
        for (int i = 0; i < chemicalsInside.Count; i++)
        {
            mixedColor += chemicalsInside[i].color;
        }
        mixedColor /= chemicalsInside.Count;
        return mixedColor;
    }

    /// <summary>
    /// Updates the visual fill level of the beaker
    /// </summary>
    private void UpdateFillLevel()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = currentVolumeMl / maxCapacityMl;
            fillImage.color = GetMixedColor();
        }
    }

    /// <summary>
    /// Gets the beaker capacity
    /// </summary>
    public float GetCapacity()
    {
        return maxCapacityMl;
    }

    /// <summary>
    /// Checks if beaker is empty
    /// </summary>
    public bool IsEmpty()
    {
        return chemicalsInside.Count == 0;
    }

    /// <summary>
    /// Checks if beaker is full
    /// </summary>
    public bool IsFull()
    {
        return currentVolumeMl >= maxCapacityMl;
    }
}
