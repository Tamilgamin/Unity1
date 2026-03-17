using UnityEngine;

/// <summary>
/// Manages audio playback for chemistry reactions and UI interactions.
/// Plays appropriate sounds based on reaction type and user actions.
/// </summary>
public class SoundManager : MonoBehaviour
{
    /// <summary>
    /// Audio clips for different reaction types
    /// </summary>
    [SerializeField]
    private AudioClip bubblingSound;

    [SerializeField]
    private AudioClip flameSound;

    [SerializeField]
    private AudioClip mixingSound;

    [SerializeField]
    private AudioClip successSound;

    [SerializeField]
    private AudioClip errorSound;

    /// <summary>
    /// Master volume control
    /// </summary>
    [SerializeField]
    [Range(0f, 1f)]
    private float masterVolume = 0.8f;

    /// <summary>
    /// Audio source for sound effects
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Audio source for background ambience
    /// </summary>
    private AudioSource ambienceSource;

    private void Start()
    {
        // Get or create an audio source for sound effects
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.volume = masterVolume;
        audioSource.playOnAwake = false;

        // Create a second audio source for ambience if needed
        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length < 2)
        {
            ambienceSource = gameObject.AddComponent<AudioSource>();
            ambienceSource.playOnAwake = false;
        }
        else
        {
            ambienceSource = sources[1];
        }

        ambienceSource.volume = masterVolume * 0.5f;
    }

    /// <summary>
    /// Plays a sound for the given reaction type
    /// </summary>
    public void PlayReactionSound(ReactionSystem.ReactionType reactionType)
    {
        AudioClip clip = null;

        switch (reactionType)
        {
            case ReactionSystem.ReactionType.Neutralization:
                clip = bubblingSound;
                break;
            case ReactionSystem.ReactionType.Precipitation:
                clip = bubblingSound;
                break;
            case ReactionSystem.ReactionType.Combustion:
                clip = flameSound;
                break;
        }

        if (clip != null)
        {
            PlaySound(clip, 1f);
        }
        else
        {
            Debug.LogWarning($"No sound clip assigned for reaction type: {reactionType}");
        }
    }

    /// <summary>
    /// Plays a sound effect at the specified volume
    /// </summary>
    public void PlaySound(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("Attempted to play null audio clip.");
            return;
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not initialized.");
            return;
        }

        audioSource.volume = masterVolume * volumeMultiplier;
        audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Plays the mixing sound for when chemicals are added
    /// </summary>
    public void PlayMixingSound()
    {
        if (mixingSound != null)
        {
            PlaySound(mixingSound, 0.7f);
        }
    }

    /// <summary>
    /// Plays a success sound for positive feedback
    /// </summary>
    public void PlaySuccessSound()
    {
        if (successSound != null)
        {
            PlaySound(successSound, 0.8f);
        }
    }

    /// <summary>
    /// Plays an error sound for negative feedback
    /// </summary>
    public void PlayErrorSound()
    {
        if (errorSound != null)
        {
            PlaySound(errorSound, 0.8f);
        }
    }

    /// <summary>
    /// Sets the master volume for all sounds
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        
        if (audioSource != null)
        {
            audioSource.volume = masterVolume;
        }

        if (ambienceSource != null)
        {
            ambienceSource.volume = masterVolume * 0.5f;
        }
    }

    /// <summary>
    /// Gets the current master volume
    /// </summary>
    public float GetMasterVolume()
    {
        return masterVolume;
    }

    /// <summary>
    /// Mutes all sounds
    /// </summary>
    public void MuteAll()
    {
        if (audioSource != null)
        {
            audioSource.mute = true;
        }

        if (ambienceSource != null)
        {
            ambienceSource.mute = true;
        }
    }

    /// <summary>
    /// Unmutes all sounds
    /// </summary>
    public void UnmuteAll()
    {
        if (audioSource != null)
        {
            audioSource.mute = false;
        }

        if (ambienceSource != null)
        {
            ambienceSource.mute = false;
        }
    }

    /// <summary>
    /// Stops all audio playback
    /// </summary>
    public void StopAllSounds()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (ambienceSource != null)
        {
            ambienceSource.Stop();
        }
    }
}
