# Pre-Unity Development Checklist

> What can be done **now** without the Unity engine, and what must wait.

## ✅ Completable Without Unity

These items can be created, reviewed, and refined in any text editor or IDE. They are pure C# logic, data definitions, or documentation that will compile and integrate directly once the Unity project is opened.

### C# Scripts (Pure Logic — No Unity Runtime Required to Author)

| System | Script | Status | Notes |
|--------|--------|--------|-------|
| **Crafting** | `CraftingRecipe.cs` | ✅ Created | ScriptableObject recipe definitions |
| **Crafting** | `CraftingManager.cs` | ✅ Created | Runtime crafting logic, inventory integration |
| **Quests** | `QuestData.cs` | ✅ Created | ScriptableObject quest definitions with objectives |
| **Quests** | `QuestManager.cs` | ✅ Created | Quest tracking, progress, and completion |
| **Economy** | `ShopData.cs` | ✅ Created | ScriptableObject shop/NPC merchant definitions |
| **Economy** | `ShopManager.cs` | ✅ Created | Buy/sell logic with inventory integration |
| **Audio** | `AudioManager.cs` | ✅ Created | Singleton audio system with music/SFX/ambience layers |

### Game Data Definitions (JSON)

| Data File | Status | Purpose |
|-----------|--------|---------|
| `Data/items.json` | ✅ Created | Master item database (seeds, crops, tools, materials, consumables) |
| `Data/crops.json` | ✅ Created | Crop growth phases, seasons, harvest yields |
| `Data/creatures.json` | ✅ Created | Creature species, base stats, default genetics |
| `Data/recipes.json` | ✅ Created | Crafting recipes with ingredients and outputs |
| `Data/shops.json` | ✅ Created | Shop inventories and NPC merchant data |
| `Data/npc-schedules.json` | ✅ Created | NPC daily schedules, locations, and dialogue triggers |

### Documentation

| Document | Status | Purpose |
|----------|--------|---------|
| `docs/pre-unity-checklist.md` | ✅ This file | Tracks what can be done now |
| `docs/asset-requirements.md` | ✅ Created | Every art/audio asset needed with AI prompts |
| `docs/design-questions.md` | ✅ Updated | 6 priority decisions RESOLVED; remaining questions still open |

### Design Decisions (Confirmed)

| Decision | Answer | Reference |
|----------|--------|-----------|
| Tile/pixel resolution | **16×16 tiles, 320×180 base** | Q7.1 |
| Day length | **14 minutes (840s)** | Q2.1 |
| Season length | **28 days per season** | Q2.2 |
| Combat style | **Simplified Stardew-style** | Q3.1 |
| NPC count | **30-50 NPCs** | Q4.1 |
| Creature utility | **Both combat + farming** | Q6.2 |
| Farm size | **~80×65 tiles** | Q1.1 |

### Game Constants

| File | Status | Purpose |
|------|--------|---------|
| `Scripts/Core/GameConstants.cs` | ✅ Created | Central reference for all confirmed design values |

---

## ⏳ Requires Unity Engine

These tasks need the Unity Editor running to author, test, or configure.

### Scene & Level Design
- [ ] Create the Farm scene with Tilemap layers (ground, paths, fences, buildings)
- [ ] Create the Town scene with NPC buildings, plaza, shops
- [ ] Create the Mine/Dungeon scene with procedural room layouts
- [ ] Create the Forest scene with forage zones and monster areas
- [ ] Set up Scene transitions (doorways, map edges)

### Tilemap & Collision Setup
- [ ] Import tilesets into Unity Tile Palette
- [ ] Paint ground, wall, water, and decoration layers
- [ ] Configure CompositeCollider2D on wall/obstacle tilemaps
- [ ] Set up sorting layers (Ground, Crops, Characters, UI)

### URP 2D Lighting
- [ ] Configure URP 2D Renderer Asset
- [ ] Set up Global Light 2D for day/night cycle
- [ ] Place Point Light 2D on torches, lanterns, windows
- [ ] Attach Shadow Caster 2D to walls, trees, buildings
- [ ] Configure Light Blend Styles (Standard, Additive, Subtractive)

### Animation
- [ ] Create Player animation controller (idle, walk, run, tool use — 4 directions)
- [ ] Create NPC animation controllers
- [ ] Create creature animation controllers (idle, walk, attack, special)
- [ ] Set up 2D skeletal rig for Frankenstein body part swapping
- [ ] Configure SpriteLibraryAsset for body part categories and labels

### Prefab Assembly
- [ ] Player prefab (controller, stats, interaction, sprite, animator, colliders)
- [ ] NPC prefab template (controller, GOAP agent, sprite, dialogue trigger)
- [ ] Crop tile prefabs (per-phase sprite swap)
- [ ] Creature prefab template (controller, genome, visualizer, sprite)
- [ ] Tool prefabs (hoe, axe, pickaxe, watering can, fishing rod, sword)
- [ ] UI prefabs (HUD, inventory panel, dialogue panel, shop panel, quest log)

### Shader Graph
- [ ] Distortion/shimmer shader for invisible enemies
- [ ] Void zone subtractive light shader
- [ ] Crop growth transition shader (optional polish)
- [ ] Day/night color grading post-process

### ScriptableObject Asset Creation
- [ ] Create ItemData assets from `items.json` definitions
- [ ] Create CropData assets from `crops.json` definitions
- [ ] Create CreatureData assets from `creatures.json` definitions
- [ ] Create CraftingRecipe assets from `recipes.json` definitions
- [ ] Create ShopData assets from `shops.json` definitions
- [ ] Create QuestData assets for storyline and side quests
- [ ] Create DialogueData assets for NPC conversations
- [ ] Create BodyPartData assets for Frankenstein mechanic

### Audio Integration
- [ ] Import audio clips and configure AudioMixer groups
- [ ] Set up ambient sound zones (forest, cave, town)
- [ ] Hook TimeManager to music transitions (day/night/raid)

### Build & Testing
- [ ] Configure build settings for target platforms
- [ ] Playtest movement feel and collision
- [ ] Playtest crop growth cycle end-to-end
- [ ] Playtest inventory/crafting/shop flow
- [ ] Playtest GOAP AI raid behavior
- [ ] Playtest creature breeding and visualization
- [ ] Performance profiling (Job System crop sim, GOAP planning)

---

## 🔄 Partially Completable

These items have code written but need Unity to fully validate and polish.

| Item | Code Status | Unity Work Remaining |
|------|-------------|---------------------|
| GOAP Actions (8 files) | Written | Need NavMesh, animation triggers, physics testing |
| Raid Manager | Written | Need spawn point placement, wave balancing, enemy prefabs |
| Stealth System | Written | Need Light2D integration testing, shader validation |
| Save System | Written | Need full serialization of all new systems (quests, recipes learned) |
| Creature Visualizer | Written | Need SpriteResolver setup, trait overlay sprites |
| Body Part Swapper | Written | Need SpriteLibraryAsset configuration, bone weight testing |
