using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Manages chemical reactions when multiple chemicals are mixed in a beaker.
/// Detects reaction types and triggers appropriate visual effects.
/// </summary>
public class ReactionSystem : MonoBehaviour
{
    /// <summary>
    /// Enum for reaction types
    /// </summary>
    public enum ReactionType
    {
        Neutralization,  // Acid + Base
        Precipitation,   // Salt formation
        Combustion,      // Metal + Oxygen
        None             // No reaction
    }

    /// <summary>
    /// Data class for reaction information
    /// </summary>
    [System.Serializable]
    public class ReactionData
    {
        public ReactionType type;
        public string productName;
        public Color resultColor;
        public string description;
    }

    /// <summary>
    /// Particle effect prefab for bubbling
    /// </summary>
    [SerializeField]
    private GameObject bubbleParticlePrefab;

    /// <summary>
    /// Particle effect prefab for smoke
    /// </summary>
    [SerializeField]
    private GameObject smokeParticlePrefab;

    /// <summary>
    /// Particle effect prefab for sparks
    /// </summary>
    [SerializeField]
    private GameObject sparkParticlePrefab;

    /// <summary>
    /// Particle effect prefab for flame
    /// </summary>
    [SerializeField]
    private GameObject flameParticlePrefab;

    /// <summary>
    /// Reference to the sound manager for reaction sounds
    /// </summary>
    private SoundManager soundManager;

    /// <summary>
    /// Dictionary to store active reactions to avoid duplicate triggers
    /// </summary>
    private Dictionary<Beaker, ReactionData> activeReactions = new Dictionary<Beaker, ReactionData>();

    /// <summary>
    /// Event for when a reaction occurs
    /// </summary>
    public delegate void ReactionOccurredDelegate(Beaker beaker, ReactionData reaction);
    public event ReactionOccurredDelegate OnReactionOccurred;

    private void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        if (soundManager == null)
        {
            Debug.LogWarning("SoundManager not found in scene. Audio effects will be disabled.");
        }
    }

    /// <summary>
    /// Checks if chemicals in a beaker can react and triggers the reaction
    /// </summary>
    public void CheckForReaction(Beaker beaker)
    {
        if (beaker == null || beaker.GetChemicalCount() < 2)
            return;

        // Skip if this beaker already has an active reaction
        if (activeReactions.ContainsKey(beaker))
            return;

        List<Chemical> chemicals = beaker.GetChemicals();

        // Check for possible reactions
        ReactionData reaction = DetermineReaction(chemicals);

        if (reaction != null && reaction.type != ReactionType.None)
        {
            TriggerReaction(beaker, reaction);
        }
    }

    /// <summary>
    /// Determines what reaction occurs between the given chemicals
    /// </summary>
    private ReactionData DetermineReaction(List<Chemical> chemicals)
    {
        if (chemicals.Count < 2)
            return null;

        // Get the types of chemicals present
        bool hasAcid = false;
        bool hasBase = false;
        bool hasSalt = false;
        bool hasOxide = false;
        bool hasMetal = false;

        foreach (Chemical chem in chemicals)
        {
            switch (chem.reactionType)
            {
                case Chemical.ReactionType.Acid:
                    hasAcid = true;
                    break;
                case Chemical.ReactionType.Base:
                    hasBase = true;
                    break;
                case Chemical.ReactionType.Salt:
                    hasSalt = true;
                    break;
                case Chemical.ReactionType.Oxide:
                    hasOxide = true;
                    break;
                case Chemical.ReactionType.Metal:
                    hasMetal = true;
                    break;
            }
        }

        // Determine reaction type based on chemical combinations
        if (hasAcid && hasBase)
        {
            // HCl + NaOH → Neutralization
            return new ReactionData
            {
                type = ReactionType.Neutralization,
                productName = "Neutral Solution",
                resultColor = new Color(0.7f, 0.7f, 0.7f), // Gray
                description = "Acid-Base Neutralization"
            };
        }
        else if ((hasAcid || hasBase) && hasSalt)
        {
            // AgNO3 + NaCl → Precipitation
            return new ReactionData
            {
                type = ReactionType.Precipitation,
                productName = "Precipitate",
                resultColor = new Color(1f, 1f, 0.9f), // Off-white
                description = "Precipitation Reaction"
            };
        }
        else if (hasMetal && hasOxide)
        {
            // Mg + O2 → Combustion
            return new ReactionData
            {
                type = ReactionType.Combustion,
                productName = "Metal Oxide",
                resultColor = new Color(1f, 0.5f, 0f), // Orange
                description = "Combustion Reaction"
            };
        }

        return new ReactionData { type = ReactionType.None };
    }

    /// <summary>
    /// Triggers the visual and audio effects for a reaction
    /// </summary>
    private void TriggerReaction(Beaker beaker, ReactionData reaction)
    {
        // Mark this beaker as having an active reaction
        activeReactions[beaker] = reaction;

        Debug.Log($"Reaction triggered in beaker: {reaction.description}");

        // Get beaker position for particle effects
        Vector3 effectPosition = beaker.transform.position + Vector3.up * 0.3f;

        // Trigger effects based on reaction type
        switch (reaction.type)
        {
            case ReactionType.Neutralization:
                TriggerNeutralizationEffects(effectPosition);
                break;
            case ReactionType.Precipitation:
                TriggerPrecipitationEffects(effectPosition);
                break;
            case ReactionType.Combustion:
                TriggerCombustionEffects(effectPosition);
                break;
        }

        // Update beaker color
        if (beaker.GetComponent<Image>() != null)
        {
            beaker.GetComponent<Image>().color = reaction.resultColor;
        }

        // Trigger event
        OnReactionOccurred?.Invoke(beaker, reaction);

        // Play reaction sound
        if (soundManager != null)
        {
            soundManager.PlayReactionSound(reaction.type);
        }

        // Schedule cleanup of active reaction after a delay
        StartCoroutine(ClearReactionAfterDelay(beaker, 5f));
    }

    /// <summary>
    /// Triggers neutralization effects (minor bubbling)
    /// </summary>
    private void TriggerNeutralizationEffects(Vector3 position)
    {
        // Spawn bubble particles
        if (bubbleParticlePrefab != null)
        {
            GameObject bubblesObj = Instantiate(bubbleParticlePrefab, position, Quaternion.identity);
            ParticleSystem bubbles = bubblesObj.GetComponent<ParticleSystem>();
            if (bubbles != null)
            {
                bubbles.Play();
                Destroy(bubblesObj, 3f);
            }
        }
    }

    /// <summary>
    /// Triggers precipitation effects (bubbles and smoke)
    /// </summary>
    private void TriggerPrecipitationEffects(Vector3 position)
    {
        // Spawn bubble particles
        if (bubbleParticlePrefab != null)
        {
            GameObject bubblesObj = Instantiate(bubbleParticlePrefab, position, Quaternion.identity);
            ParticleSystem bubbles = bubblesObj.GetComponent<ParticleSystem>();
            if (bubbles != null)
            {
                var emission = bubbles.emission;
                emission.rateOverTime = 50f;
                bubbles.Play();
                Destroy(bubblesObj, 4f);
            }
        }

        // Spawn smoke particles
        if (smokeParticlePrefab != null)
        {
            GameObject smokeObj = Instantiate(smokeParticlePrefab, position, Quaternion.identity);
            ParticleSystem smoke = smokeObj.GetComponent<ParticleSystem>();
            if (smoke != null)
            {
                smoke.Play();
                Destroy(smokeObj, 5f);
            }
        }
    }

    /// <summary>
    /// Triggers combustion effects (flames, sparks, and smoke)
    /// </summary>
    private void TriggerCombustionEffects(Vector3 position)
    {
        // Spawn flame particles
        if (flameParticlePrefab != null)
        {
            GameObject flameObj = Instantiate(flameParticlePrefab, position, Quaternion.identity);
            ParticleSystem flame = flameObj.GetComponent<ParticleSystem>();
            if (flame != null)
            {
                flame.Play();
                Destroy(flameObj, 4f);
            }
        }

        // Spawn spark particles
        if (sparkParticlePrefab != null)
        {
            GameObject sparksObj = Instantiate(sparkParticlePrefab, position, Quaternion.identity);
            ParticleSystem sparks = sparksObj.GetComponent<ParticleSystem>();
            if (sparks != null)
            {
                var emission = sparks.emission;
                emission.rateOverTime = 100f;
                sparks.Play();
                Destroy(sparksObj, 3f);
            }
        }

        // Spawn smoke particles
        if (smokeParticlePrefab != null)
        {
            GameObject smokeObj = Instantiate(smokeParticlePrefab, position, Quaternion.identity);
            ParticleSystem smoke = smokeObj.GetComponent<ParticleSystem>();
            if (smoke != null)
            {
                var emission = smoke.emission;
                emission.rateOverTime = 60f;
                smoke.Play();
                Destroy(smokeObj, 5f);
            }
        }
    }

    /// <summary>
    /// Clears the active reaction from a beaker after a delay
    /// </summary>
    private System.Collections.IEnumerator ClearReactionAfterDelay(Beaker beaker, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        if (activeReactions.ContainsKey(beaker))
        {
            activeReactions.Remove(beaker);
        }
    }

    /// <summary>
    /// Gets the active reaction data for a beaker
    /// </summary>
    public ReactionData GetActiveReaction(Beaker beaker)
    {
        if (activeReactions.TryGetValue(beaker, out ReactionData reaction))
        {
            return reaction;
        }
        return null;
    }

    /// <summary>
    /// Checks if a beaker currently has an active reaction
    /// </summary>
    public bool HasActiveReaction(Beaker beaker)
    {
        return activeReactions.ContainsKey(beaker);
    }

    /// <summary>
    /// Clears all active reactions (for reset/cleanup)
    /// </summary>
    public void ClearAllReactions()
    {
        activeReactions.Clear();
        Debug.Log("All active reactions cleared.");
    }
}
