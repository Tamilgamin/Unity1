# AR Chemistry Lab System - Setup Guide

## Overview
This guide explains how to set up and use the AR Chemistry Lab system in your Unity AR Foundation project. The system allows users to place a lab table in AR, spawn chemistry equipment, add chemicals, and simulate reactions with visual effects.

## Created Scripts

All scripts have been created in `Assets/Scripts/` with the following files:

1. **Chemical.cs** - Defines a chemical object with name, color, and reaction type
2. **Beaker.cs** - Container for holding chemicals with volume tracking
3. **ARObjectPlacement.cs** - Handles detection and placement of the lab table
4. **EquipmentSpawner.cs** - Spawns laboratory equipment (beakers, test tubes, flasks, burners)
5. **ReactionSystem.cs** - Manages chemical reactions and visual effects
6. **ExperimentManager.cs** - Manages chemistry experiments with instructions
7. **ExperimentGuideUI.cs** - Displays UI guidance and experiment instructions
8. **SoundManager.cs** - Handles audio playback for reactions

## Setup Instructions

### Step 1: Create Required Prefabs

You need to create the following prefabs for the system to work:

#### 1.1 Lab Table Prefab
- Create an empty GameObject named "LabTable"
- Add a simple 3D model (cube or import a table model)
- Add a **BoxCollider** component
- Add the **EquipmentSpawner** script to this object
- Tag it with a layer (e.g., "Table") for raycasting
- Create a prefab from this GameObject and place it in `Assets/Prefabs/`

#### 1.2 Beaker Prefab
- Create an empty GameObject named "Beaker"
- Add a cylinder or beaker-shaped model
- Add a **CapsuleCollider** component
- Add the **Beaker** script to this object
- Create a prefab from this GameObject

#### 1.3 Test Tube, Flask, Bunsen Burner Prefabs
- Follow similar steps as the Beaker
- Add appropriate 3D models for each

### Step 2: Set Up the Scene Hierarchy

Ensure your scene has the following structure:

```
AR Session (XR Origin)
├── AR Session Origin
│   ├── AR Camera
│   ├── AR Raycast Manager (component on same GameObject)
│   ├── AR Plane Manager (component on same GameObject)
│   └── [Your custom UI Canvas]
```

### Step 3: Configure ARObjectPlacement Script

1. In your scene, select the GameObject with **AR Session Origin**
2. Add the **ARObjectPlacement.cs** script as a component
3. In the Inspector, assign:
   - **AR Raycast Manager**: Assign the ARRaycastManager component
   - **AR Plane Manager**: Assign the ARPlaneManager component
   - **Lab Table Prefab**: Assign the LabTable prefab you created in Step 1.1

### Step 4: Configure EquipmentSpawner Script

1. Create an empty GameObject named "EquipmentSpawner" as a child of AR Session Origin
2. Add the **EquipmentSpawner.cs** script
3. In the Inspector, assign:
   - **Beaker Prefab**: Assign the Beaker prefab
   - **Test Tube Prefab**: Assign the TestTube prefab
   - **Flask Prefab**: Assign the Flask prefab
   - **Bunsen Burner Prefab**: Assign the BunsenBurner prefab
   - **Table Layer Mask**: Set to the layer you assigned to your table

### Step 5: Configure ReactionSystem Script

1. Create an empty GameObject named "ReactionSystem"
2. Add the **ReactionSystem.cs** script
3. In the Inspector, you can optionally assign:
   - **Bubble Particle Prefab**: Create a particle system for bubbles
   - **Smoke Particle Prefab**: Create a particle system for smoke
   - **Spark Particle Prefab**: Create a particle system for sparks
   - **Flame Particle Prefab**: Create a particle system for flames

### Step 6: Configure SoundManager Script

1. Create an empty GameObject named "SoundManager" with an **AudioSource** component
2. Add the **SoundManager.cs** script
3. In the Inspector, assign audio clips for:
   - **Bubbling Sound**: Sound for neutralization/precipitation reactions
   - **Flame Sound**: Sound for combustion reactions
   - **Mixing Sound**: Sound for adding chemicals
   - **Success Sound**: Sound for successful experiment completion
   - **Error Sound**: Sound for errors

### Step 7: Configure ExperimentManager Script

1. Create an empty GameObject named "ExperimentManager"
2. Add the **ExperimentManager.cs** script
3. The script comes with 3 default experiments:
   - Acid-Base Neutralization (HCl + NaOH)
   - Precipitation Reaction (AgNO3 + NaCl)
   - Metal Combustion (Mg + O2)
4. You can add more experiments in the Inspector or via code

### Step 8: Set Up UI with ExperimentGuideUI

1. Create a Canvas in your scene (if not already present)
2. Add the following UI elements as children:
   - **Panel**: Container for the guide
   - **Text (instructionsText)**: For displaying instructions
   - **Text (statusText)**: For displaying status messages
   - **Text (experimentTitleText)**: For displaying experiment names
   - **Text (chemicalCountText)**: For showing chemical count
   - **Image (progressBar)**: For showing experiment progress
   - **Button (nextButton)**: To move to next experiment
   - **Button (previousButton)**: To move to previous experiment

3. Create an empty GameObject named "GuideUI"
4. Add the **ExperimentGuideUI.cs** script
5. In the Inspector, drag and drop all UI elements into their corresponding fields

### Step 9: Create and Add Chemicals to the Scene

Create a script or use the Inspector to add chemicals. Example:

```csharp
Chemical hcl = new Chemical("HCl", Color.red, Chemical.ReactionType.Acid, 50f);
Chemical naoh = new Chemical("NaOH", Color.blue, Chemical.ReactionType.Base, 50f);

beaker.AddChemical(hcl);
beaker.AddChemical(naoh);
```

## Usage Instructions

### For End Users:

1. **Place the Lab Table**: Tap on a horizontal surface to place the lab table
2. **Spawn Equipment**: After the table is placed, tap on the table to spawn equipment
3. **Add Chemicals**: Interact with beakers to add the required chemicals
4. **Observe Reactions**: When all required chemicals are added, the reaction system triggers visual and audio effects

### For Developers:

#### Spawning Equipment:
```csharp
EquipmentSpawner spawner = GetComponent<EquipmentSpawner>();
spawner.SpawnBeaker();
spawner.SpawnTestTube();
```

#### Adding Chemicals:
```csharp
Chemical chemical = new Chemical("HCl", Color.red, Chemical.ReactionType.Acid, 50f);
beaker.AddChemical(chemical);
```

#### Checking Reactions:
```csharp
ReactionSystem reactionSystem = GetComponent<ReactionSystem>();
reactionSystem.CheckForReaction(beaker);
```

#### Managing Experiments:
```csharp
ExperimentManager manager = GetComponent<ExperimentManager>();
manager.StartCurrentExperiment();
manager.NextExperiment();
manager.ValidateChemicalRequirements(beaker);
```

## Optimization for Android ARCore (4GB RAM Devices)

The scripts are already optimized for Android devices:

1. **Object Pooling**: The system reuses GameObjects where possible
2. **Particle System Optimization**: Particle systems are instantiated only when reactions occur
3. **Raycast Hitlist Caching**: Reuses List<ARRaycastHit> to avoid allocations
4. **Plane Detection Disabling**: Disables plane detection after table placement to reduce processing
5. **Kinematic Rigidbodies**: Equipment uses kinematic physics for stability
6. **Collider Optimization**: Only essential colliders are added to spawned objects

### Additional Optimization Tips:

- Disable real-time lighting on mobile devices
- Use simplified 3D models for chemical equipment prefabs
- Set particle system max particles to reasonable limits (50-100)
- Use baked textures instead of real-time generated ones
- Profile on actual Android devices to verify performance

## Testing Checklist

- [ ] Scripts compile without errors
- [ ] AR plane detection works
- [ ] Lab table spawns when tapping
- [ ] Equipment spawns on the table
- [ ] Chemicals can be added to beakers
- [ ] Reactions trigger with proper effects
- [ ] UI displays correctly
- [ ] Audio plays for reactions
- [ ] Experiments progress correctly

## Troubleshooting

### Issues with AR Plane Detection:
- Ensure adequate lighting in the environment
- Move the device slowly to help feature tracking
- Check that ARCore/ARKit is installed and updated

### Equipment Not Spawning:
- Verify prefabs are assigned in the inspector
- Check layer mask settings for raycasting
- Ensure table is properly placed

### Reactions Not Triggering:
- Verify particle system prefabs are assigned
- Check chemical types match reaction requirements
- Ensure beaker has enough capacity

### Audio Not Playing:
- Check audio clips are assigned to SoundManager
- Verify device volume is not muted
- Ensure AudioSource components are present

## Performance Monitoring

Use Unity's Profiler to monitor:
- Memory usage (target: < 3GB on 4GB devices)
- CPU usage during reactions
- Particle system performance
- Raycast performance

## Next Steps

1. Create 3D models for all equipment
2. Add more experiment types
3. Implement saving/loading of experiment results
4. Add achievement system
5. Create tutorial sequence
6. Implement multiplayer AR feature

---

For more information about AR Foundation, visit: https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@latest
