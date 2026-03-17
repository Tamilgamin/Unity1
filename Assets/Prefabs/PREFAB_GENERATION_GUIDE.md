# Prefab Generation - Quick Start Guide

## 🎯 What You Have

You now have **2 automated prefab generator scripts** that will create all the prefabs you need with a single click!

### Files Created:
- `Assets/Editor/PrefabGenerator.cs` - Generates equipment prefabs
- `Assets/Editor/ParticleSystemGenerator.cs` - Generates particle effect prefabs

---

## 🚀 How to Use

### Step 1: Launch Unity Editor
Open your AR Chemistry Lab project in Unity (any recent version with AR Foundation)

### Step 2: Generate Equipment Prefabs

1. In the top menu bar, go to: **Tools > AR Chemistry Lab > Generate All Prefabs**

   ![Menu Location](assets/menu-prefabs.png)

2. Click on it and watch as all prefabs are automatically generated!

3. You'll see a **success dialog** confirming generation

4. Prefabs will be created in: **Assets/Prefabs/**

**Prefabs generated:**
- ✅ LabTable.prefab
- ✅ Beaker.prefab
- ✅ TestTube.prefab
- ✅ Flask.prefab
- ✅ BunsenBurner.prefab

### Step 3: Generate Particle System Prefabs

1. In the top menu bar, go to: **Tools > AR Chemistry Lab > Generate Particle Prefabs**

2. Click and wait for completion

3. Particle prefabs will be created in: **Assets/Prefabs/Particles/**

**Particle prefabs generated:**
- ✅ BubbleParticles.prefab
- ✅ SmokeParticles.prefab
- ✅ SparkParticles.prefab
- ✅ FlameParticles.prefab

---

## ✅ What Gets Generated

### Equipment Prefabs
Each equipment prefab includes:
- ✅ 3D geometry (simple primitives)
- ✅ Colliders for physics/interaction
- ✅ Material (colored/transparent)
- ✅ Components (Beaker script for beakers)
- ✅ Ready to assign in Inspector

### Particle Prefabs
Each particle system includes:
- ✅ Pre-configured emission
- ✅ Appropriate colors and sizes
- ✅ Physics (gravity, velocity)
- ✅ Proper lifetime and effects
- ✅ Billboard rendering

---

## 📂 File Structure After Generation

```
Assets/
├── Prefabs/
│   ├── LabTable.prefab
│   ├── Beaker.prefab
│   ├── TestTube.prefab
│   ├── Flask.prefab
│   ├── BunsenBurner.prefab
│   └── Particles/
│       ├── BubbleParticles.prefab
│       ├── SmokeParticles.prefab
│       ├── SparkParticles.prefab
│       └── FlameParticles.prefab
└── Scripts/
    ├── ARObjectPlacement.cs
    ├── EquipmentSpawner.cs
    └── ... (other scripts)
```

---

## 🔧 Next: Assign Prefabs to Scripts

After generating prefabs, follow these steps to connect everything:

### 1. Assign Equipment Prefabs to EquipmentSpawner

1. In your scene, select the **ARSessionOrigin** (or similar)
2. Find the **EquipmentSpawner** script component
3. In the Inspector, assign these prefabs:
   - **Beaker Prefab** → Select `Assets/Prefabs/Beaker.prefab`
   - **Test Tube Prefab** → Select `Assets/Prefabs/TestTube.prefab`
   - **Flask Prefab** → Select `Assets/Prefabs/Flask.prefab`
   - **Bunsen Burner Prefab** → Select `Assets/Prefabs/BunsenBurner.prefab`

### 2. Assign Lab Table Prefab to ARObjectPlacement

1. Select the **ARSessionOrigin**
2. Find the **ARObjectPlacement** script component
3. In the Inspector:
   - **Lab Table Prefab** → Select `Assets/Prefabs/LabTable.prefab`

### 3. Assign Particle Prefabs to ReactionSystem

1. In your scene, select the **ReactionSystem** GameObject
2. In the Inspector, assign:
   - **Bubble Particle Prefab** → `Assets/Prefabs/Particles/BubbleParticles.prefab`
   - **Smoke Particle Prefab** → `Assets/Prefabs/Particles/SmokeParticles.prefab`
   - **Spark Particle Prefab** → `Assets/Prefabs/Particles/SparkParticles.prefab`
   - **Flame Particle Prefab** → `Assets/Prefabs/Particles/FlameParticles.prefab`

---

## 🎨 Customizing Prefabs

All generated prefabs can be customized in the Unity Editor:

### Change Appearance
1. Open a prefab (double-click it in Project folder)
2. Edit the 3D geometry, materials, colors
3. Save and close

### Change Physics
1. Select child objects in the prefab
2. Adjust Collider components
3. Save the prefab

### Add 3D Models
1. Open a prefab
2. Delete the primitive geometry
3. Drag your 3D model into the prefab
4. Adjust scale and position
5. Save the prefab

---

## ✨ Features of Generated Prefabs

### LabTable
- 1x Table surface (large flat cube)
- 4x Table legs
- BoxCollider for raycasting
- Tagged as "LabTable"

### Beaker
- Cylindrical body with glass-like transparency
- Ring-shaped rim
- CapsuleCollider for interaction
- **Beaker.cs script attached** - ready to add chemicals

### TestTube
- Thin tall cylinder for narrow tube look
- Cap at top
- CapsuleCollider

### Flask
- Spherical main body
- Thin neck
- Cap on top
- SphereCollider

### BunsenBurner
- Base foundation
- Vertical support pole
- Burner head (dark gray)
- BoxCollider for triggering heat effects

### Particles
- **Bubbles**: Upward-floating translucent spheres
- **Smoke**: Light gray expanding clouds
- **Sparks**: Yellow-orange fast particles
- **Flames**: Yellow-to-red ascending particles

---

## 🐛 Troubleshooting

### Error: "Tools menu not showing"
- Make sure both script files are in `Assets/Editor/` folder
- Right-click on the Editor folder → Reimport
- Restart Unity if needed

### Error: "Prefab creation failed"
- Check that Assets/Prefabs/ folder exists (generator creates it automatically)
- Ensure you have write permissions in the Assets folder
- Close any open prefabs in the editor

### Prefabs look wrong after generation
- Don't worry! You can edit them easily:
  - Delete the primitive geometry
  - Add your own 3D models
  - Adjust colors and materials

### Missing particle effects
- Check that all particle prefabs are assigned in ReactionSystem script
- Verify prefab paths are correct in Inspector

---

## 📸 What It Looks Like

After generation, you can see:

1. **In Project Folder**: All prefab files with Unity icon
2. **In Scene**: When you place table, all equipment spawns with physics
3. **During Reactions**: Particle systems play with appropriate effects

---

## 🎓 Next Steps

1. ✅ Run equipment prefab generator
2. ✅ Run particle system generator
3. ✅ Assign all prefabs to their respective scripts
4. ✅ Play the scene and test
5. 🎨 Customize prefabs with your own 3D models
6. 🔊 Add audio clips to SoundManager
7. 🎬 Create a UI Canvas with ExperimentGuideUI

---

## 💡 Tips

- **Backup**: Unity auto-saves, but consider saving your scene before generating
- **Iteration**: You can run generators multiple times (they'll update existing prefabs)
- **Scale**: All prefabs are designed at 1 unit = 1 meter for AR
- **Models**: Replace the primitive geometry with your own models later
- **Materials**: Use the glass material on beakers/flasks for realistic look

---

## ✅ Verification Checklist

After generation, verify you have:

- [ ] `Assets/Prefabs/` folder created
- [ ] 5 equipment prefabs created
- [ ] `Assets/Prefabs/Particles/` folder created
- [ ] 4 particle prefabs created
- [ ] All prefabs have correct colliders
- [ ] No errors in Console
- [ ] EquipmentSpawner has prefabs assigned
- [ ] ARObjectPlacement has lab table assigned
- [ ] ReactionSystem has particle prefabs assigned

---

For more details, see:
- `Assets/Scripts/SETUP_GUIDE.md` - Full setup guide
- `Assets/Scripts/API_REFERENCE.md` - Script API documentation
- `Assets/Scripts/README.md` - Project overview

**Happy AR Chemistry Lab building! 🧪🔬**
