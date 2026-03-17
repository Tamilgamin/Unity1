# AR Chemistry Lab - API Reference

## Quick Reference for All Scripts

### Chemical.cs

Represents a chemical compound.

**Constructor:**
```csharp
Chemical(string name, Color color, ReactionType type, float volume = 50f)
```

**Properties:**
- `chemicalName` - Name of the chemical (string)
- `color` - Visual color representation (Color)
- `reactionType` - Type: Acid, Base, Salt, Oxide, Metal (ReactionType)
- `volumeMl` - Volume in milliliters (float)

**Methods:**
- `ToString()` - Returns string representation

---

### Beaker.cs

Container for holding and mixing chemicals.

**Events:**
- `OnChemicalAdded` - Fires when a chemical is added
- `OnChemicalRemoved` - Fires when a chemical is removed

**Methods:**
- `bool AddChemical(Chemical chemical)` - Adds a chemical, returns success
- `bool RemoveChemical(Chemical chemical)` - Removes a chemical
- `void Empty()` - Empties all chemicals
- `List<Chemical> GetChemicals()` - Returns list of chemicals
- `float GetCurrentVolume()` - Returns current volume in ml
- `int GetChemicalCount()` - Returns number of chemicals
- `Color GetMixedColor()` - Returns average color of chemicals
- `float GetCapacity()` - Returns max capacity
- `bool IsEmpty()` - Checks if beaker is empty
- `bool IsFull()` - Checks if beaker is full

---

### ARObjectPlacement.cs

Handles AR plane detection and lab table placement.

**Events:**
- `OnTablePlaced` - Fires when table is successfully placed
- `OnPlacementFailed` - Fires when placement fails

**Methods:**
- `GameObject GetPlacedLabTable()` - Returns placed table reference
- `bool IsTablePlaced()` - Returns placement status
- `void RemoveLabTable()` - Removes placed table and allows replacement

---

### EquipmentSpawner.cs

Spawns laboratory equipment on the table.

**Enum EquipmentType:**
- `Beaker`
- `TestTube`
- `Flask`
- `BunsenBurner`

**Events:**
- `OnEquipmentSpawned` - Fires when equipment is spawned
- `OnEquipmentRemoved` - Fires when equipment is removed

**Methods:**
- `void SpawnBeaker()` - Spawns a beaker
- `void SpawnTestTube()` - Spawns a test tube
- `void SpawnFlask()` - Spawns a flask
- `void SpawnBunsenBurner()` - Spawns a Bunsen burner
- `void RemoveEquipment(GameObject equipment)` - Removes specific equipment
- `void ClearAllEquipment()` - Clears all spawned equipment
- `int GetEquipmentCount()` - Returns count of spawned equipment
- `List<GameObject> GetSpawnedEquipment()` - Returns list of equipments

---

### ReactionSystem.cs

Manages chemical reactions and visual effects.

**Enum ReactionType:**
- `Neutralization` - Acid + Base
- `Precipitation` - Salt formation
- `Combustion` - Metal + Oxygen
- `None` - No reaction

**Class ReactionData:**
- `ReactionType type` - The reaction type
- `string productName` - Product name
- `Color resultColor` - Resulting color
- `string description` - Description of reaction

**Events:**
- `OnReactionOccurred` - Fires when a reaction happens

**Methods:**
- `void CheckForReaction(Beaker beaker)` - Checks and triggers reaction if possible
- `ReactionData GetActiveReaction(Beaker beaker)` - Gets active reaction
- `bool HasActiveReaction(Beaker beaker)` - Checks if beaker has active reaction
- `void ClearAllReactions()` - Clears all active reactions

---

### SoundManager.cs

Manages audio playback for reactions.

**Methods:**
- `void PlayReactionSound(ReactionSystem.ReactionType type)` - Plays reaction sound
- `void PlaySound(AudioClip clip, float volumeMultiplier = 1f)` - Plays any sound
- `void PlayMixingSound()` - Plays mixing sound
- `void PlaySuccessSound()` - Plays success sound
- `void PlayErrorSound()` - Plays error sound
- `void SetMasterVolume(float volume)` - Sets volume (0-1)
- `float GetMasterVolume()` - Gets current volume
- `void MuteAll()` - Mutes all sounds
- `void UnmuteAll()` - Unmutes all sounds
- `void StopAllSounds()` - Stops all playback

---

### ExperimentManager.cs

Manages chemistry experiments.

**Class Experiment:**
- `string experimentName` - Name of experiment
- `string description` - Description
- `List<string> requiredChemicals` - List of required chemical names
- `string instructions` - Step-by-step instructions
- `string expectedReaction` - Expected reaction description
- `Sprite experimentImage` - Optional image
- `bool isCompleted` - Completion status

**Events:**
- `OnExperimentStarted` - Fires when experiment starts
- `OnExperimentCompleted` - Fires when completed
- `OnExperimentProgress` - Fires on progress update

**Methods:**
- `Experiment GetCurrentExperiment()` - Gets current experiment
- `void StartCurrentExperiment()` - Starts current experiment
- `bool NextExperiment()` - Moves to next experiment
- `bool PreviousExperiment()` - Moves to previous experiment
- `void SetExperiment(int index)` - Sets experiment by index
- `void CompleteCurrentExperiment()` - Marks as completed
- `bool IsExperimentComplete()` - Checks completion status
- `bool ValidateChemicalRequirements(Beaker beaker)` - Validates required chemicals
- `float GetCurrentProgress(Beaker beaker)` - Gets progress (0-1)
- `List<Experiment> GetAllExperiments()` - Gets all experiments
- `int GetExperimentCount()` - Gets total count
- `int GetCompletedExperimentCount()` - Gets completed count
- `void AddExperiment(Experiment experiment)` - Adds new experiment
- `void ResetAllExperiments()` - Resets all experiments

---

### ExperimentGuideUI.cs

Displays experiment guidance on UI.

**Methods:**
- `void UpdateInstructions(string text)` - Updates instruction text
- `void UpdateStatus(string text)` - Updates status text
- `void ShowTemporaryMessage(string message)` - Shows temporary message
- `void SetCurrentBeaker(Beaker beaker)` - Sets beaker for tracking
- `void ShowHelp()` - Displays context-sensitive help
- `void ToggleGuidePanel()` - Toggles panel visibility

---

## Common Usage Patterns

### Add a Chemical to a Beaker
```csharp
Chemical hcl = new Chemical("HCl", Color.red, Chemical.ReactionType.Acid, 50f);
beaker.AddChemical(hcl);
```

### Check for Reactions
```csharp
ReactionSystem reactionSystem = GetComponent<ReactionSystem>();
reactionSystem.CheckForReaction(beaker);
```

### Get Experiment Progress
```csharp
ExperimentManager manager = GetComponent<ExperimentManager>();
float progress = manager.GetCurrentProgress(beaker);
```

### Spawn Equipment
```csharp
EquipmentSpawner spawner = GetComponent<EquipmentSpawner>();
spawner.SpawnBeaker();
```

### Play a Sound
```csharp
SoundManager soundManager = GetComponent<SoundManager>();
soundManager.PlayMixingSound();
```

---

## Event Subscription Example

```csharp
void Start()
{
    Beaker beaker = GetComponent<Beaker>();
    beaker.OnChemicalAdded += OnChemicalAdded;
    beaker.OnChemicalRemoved += OnChemicalRemoved;
}

void OnChemicalAdded(Chemical chemical)
{
    Debug.Log($"Chemical added: {chemical.chemicalName}");
}

void OnChemicalRemoved(Chemical chemical)
{
    Debug.Log($"Chemical removed: {chemical.chemicalName}");
}
```

---

## System Architecture Diagram

```
ARObjectPlacement
    ↓
    └─→ Places LabTable (parent for equipment)
         ↓
         └─→ EquipmentSpawner
              ↓
              └─→ Spawns Equipment (Beaker, TestTube, Flask, BunsenBurner)
                   ↓
                   └─→ Beaker containing Chemicals
                        ↓
                        └─→ ReactionSystem (monitors for reactions)
                             ↓
                             ├─→ Visual Effects (Particles)
                             └─→ Audio (SoundManager)

ExperimentManager
    ↓
    ├─→ Manages experiment state
    └─→ Communicates with ExperimentGuideUI

ExperimentGuideUI
    ↓
    └─→ Provides user feedback
```

---

## Reaction Examples

### Neutralization (Acid + Base)
- Input: HCl (Red, Acid) + NaOH (Blue, Base)
- Product: Neutral Solution (Gray)
- Effect: Bubbles, soft sound

### Precipitation (Acid/Base + Salt)
- Input: AgNO3 (Colorless) + NaCl (Colorless)
- Product: Precipitate (Off-white)
- Effect: More bubbles, smoke

### Combustion (Metal + Oxide)
- Input: Mg (Silver) + O2 (Colorless)
- Product: Metal Oxide (Orange)
- Effect: Flames, sparks, smoke, hot sound

---

## Performance Considerations

- Particle systems only spawn during reactions (not persistent)
- Raycast hit list is cached to avoid allocations
- Plane detection disables after table placement
- Equipment count is limited (default: 20 items)
- All physics objects are kinematic for stability

---

For detailed setup instructions, see SETUP_GUIDE.md
