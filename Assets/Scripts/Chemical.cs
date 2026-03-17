using UnityEngine;

/// <summary>
/// Represents a chemical compound with properties like name, color, and reaction type.
/// This is used to track what chemicals are stored in beakers and containers.
/// </summary>
public class Chemical
{
    /// <summary>
    /// Name of the chemical (e.g., "HCl", "NaOH", "AgNO3")
    /// </summary>
    public string chemicalName;

    /// <summary>
    /// Color representation of the chemical for visual feedback
    /// </summary>
    public Color color;

    /// <summary>
    /// Type of reaction this chemical can undergo (Acid, Base, Salt, Oxide, Metal)
    /// </summary>
    public enum ReactionType
    {
        Acid,
        Base,
        Salt,
        Oxide,
        Metal
    }

    /// <summary>
    /// The reaction type of this chemical
    /// </summary>
    public ReactionType reactionType;

    /// <summary>
    /// Volume of the chemical in milliliters
    /// </summary>
    public float volumeMl;

    /// <summary>
    /// Constructor to create a new chemical
    /// </summary>
    public Chemical(string name, Color color, ReactionType type, float volume = 50f)
    {
        chemicalName = name;
        this.color = color;
        reactionType = type;
        volumeMl = volume;
    }

    /// <summary>
    /// Returns a string representation of the chemical
    /// </summary>
    public override string ToString()
    {
        return $"{chemicalName} ({reactionType}) - {volumeMl}ml";
    }
}
