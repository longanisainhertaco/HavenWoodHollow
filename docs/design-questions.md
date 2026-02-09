# Open Design Questions for Havenwood Hollow

> Decisions that need to be made before or during Unity development.  
> Each question includes context, options, and what depends on the answer.

---

## 1. World & Setting

### Q1.1: How large is the player's farm?
**Context:** The Plan specifies Job System optimization for 10,000+ crops, but doesn't define actual farm dimensions.  
**Options:**
- (A) Small: 40×40 tiles (~1,600 plantable tiles) — cozy, manageable
- (B) Medium: 64×64 tiles (~4,096 plantable tiles) — comparable to Stardew
- (C) Large: 100×100 tiles (~10,000 plantable tiles) — sprawling Gothic estate  

**Depends on:** Tilemap size, camera zoom level, movement speed feel, crop balancing.

### Q1.2: How many distinct locations/scenes exist?
**Context:** Plan mentions Farm, Town, Mine/Dungeon, Forest. Are there more?  
**Options:**
- (A) 4 core areas (Farm, Town, Forest, Mine)
- (B) 6 areas (add Graveyard, Swamp)
- (C) 8+ areas (add Castle Ruins, Underground Lake, Witch's Tower, etc.)  

**Depends on:** Asset workload, NPC schedules, quest variety, tileset count.

### Q1.3: Is the world connected or scene-based?
**Context:** Stardew uses discrete screens. Some farming games use seamless worlds.  
**Options:**
- (A) Discrete scenes with fade transitions (simpler, Stardew-like)
- (B) Connected tilemap zones with streaming (more immersive, more complex)  

**Depends on:** Scene management architecture, camera system, NPC pathfinding across zones.

---

## 2. Time & Seasons

### Q2.1: How long is a single in-game day in real minutes?
**Context:** Plan defines a 24-hour cycle with TimeManager but not the real-time duration.  
**Options:**
- (A) 14 minutes (Stardew Valley standard)
- (B) 20 minutes (more relaxed pacing for Gothic exploration)
- (C) Configurable via settings  

**Depends on:** Crop growth balance, stamina management, raid timing, player experience.

### Q2.2: How many days per season?
**Context:** Plan doesn't specify.  
**Options:**
- (A) 28 days (Stardew standard, 4 weeks)
- (B) 14 days (faster progression, more compact)
- (C) 30 days (calendar month)  

**Depends on:** Crop growth tables, seasonal events, economy pacing.

### Q2.3: What happens in Winter?
**Context:** Most farming games restrict farming in winter. Gothic theme could make winter uniquely dangerous.  
**Options:**
- (A) No farming at all — focus on mining, combat, socializing
- (B) Limited greenhouse farming with special winter-only crops
- (C) Winter is "eternal night" with continuous raid threat and survival mechanics  

**Depends on:** Crop data `Seasons` field, gameplay loop variety, difficulty curve.

---

## 3. Combat & Raids

### Q3.1: Is combat real-time action or turn-based?
**Context:** Plan implies real-time (Physics2D overlap, GOAP actions like AttackEnemy), but doesn't explicitly state it.  
**Options:**
- (A) Real-time action combat (click/button to swing, dodge, use items)
- (B) Stardew-style simplified swing (single attack button, auto-aim nearest)
- (C) Hybrid: normal enemies are real-time, bosses have turn-based phases  

**Depends on:** Animation requirements, hitbox design, tool/weapon system, difficulty tuning.

### Q3.2: How frequent are raids?
**Context:** Plan mentions nightly raids driven by GOAP, but doesn't define frequency.  
**Options:**
- (A) Every night (high tension, core loop)
- (B) Periodic (every 3-5 nights, escalating)
- (C) Triggered by player actions (e.g., moon phase, corruption level, story events)  

**Depends on:** Raid Manager wave tables, NPC defense schedules, resource economy.

### Q3.3: Can the player die? What is the death penalty?
**Context:** Not addressed in the Plan.  
**Options:**
- (A) Wake up at home, lose some gold and items (Stardew-style)
- (B) Permadeath for creatures only, player respawns
- (C) "Corruption" mechanic — death adds mutation effects to the player  

**Depends on:** Save system, player stats, difficulty settings, horror atmosphere.

### Q3.4: What enemy types exist?
**Context:** Plan mentions "eldritch" creatures and phantasms but doesn't list specific enemies.  
**Minimum needed for launch:**
- Melee grunt (e.g., Shambling Ghoul)
- Ranged attacker (e.g., Spectral Wisp)
- Tank/bruiser (e.g., Gravestone Golem)
- Stealth enemy using distortion shader (Phantasm)
- Raid boss (e.g., The Hollow King)  

**Depends on:** Sprite requirements, AI action variety, creature breeding integration.

---

## 4. Characters & NPCs

### Q4.1: How many NPCs are there?
**Context:** Plan mentions villagers with GOAP behaviors but doesn't list characters.  
**Options:**
- (A) 8-10 core NPCs (small village, easier to develop)
- (B) 15-20 NPCs (more diverse community)
- (C) 25+ NPCs (full Stardew-scale town)  

**Depends on:** Dialogue writing workload, portrait assets, schedule complexity, romance options.

### Q4.2: Is there a romance/relationship system?
**Context:** Plan references "Socialize" as a GOAP goal but doesn't define relationships.  
**Options:**
- (A) Friendship levels only (gifts increase friendship, unlock dialogue/events)
- (B) Romance with marriage candidates (Stardew-style, 4-6 candidates)
- (C) Gothic romance with supernatural elements (vampire suitor, ghost companion, etc.)  

**Depends on:** NPC data, dialogue trees, event scripting, gift item design.

### Q4.3: What is the player character's backstory?
**Context:** Not defined in the Plan. Important for narrative coherence.  
**Options:**
- (A) Inherited a cursed farm from a mysterious relative (Stardew parallel)
- (B) Exiled alchemist seeking redemption in a forgotten village
- (C) Amnesia — wake up in the Hollow with no memory, piecing together the story
- (D) Custom backstory chosen at character creation affecting starting stats  

**Depends on:** Opening sequence, dialogue branching, quest motivations.

### Q4.4: What does character creation look like?
**Context:** Plan mentions Frankenstein body swapping as a mechanic, but not initial creation.  
**Options:**
- (A) Choose from preset appearances (simple, fast)
- (B) Full customization (hair, face, skin tone, body type)
- (C) Minimal start — body customization IS the progression (start plain, add parts)  

**Depends on:** Sprite sheet requirements, SpriteLibraryAsset categories, UI work.

---

## 5. Economy & Progression

### Q5.1: What is the currency?
**Context:** ItemData has `buyPrice` and `sellPrice` but the currency isn't named.  
**Options:**
- (A) Gold/Coins (standard)
- (B) "Ichor" or "Eldritch Tokens" (thematic)
- (C) Dual currency: Gold for mundane goods, Ether for magical items  

**Depends on:** UI display, shop system, reward balancing.

### Q5.2: How does tool upgrading work?
**Context:** ITool has `ToolTier` but the upgrade path isn't specified.  
**Options:**
- (A) Blacksmith NPC upgrades tools for gold + materials (Stardew-style)
- (B) Crafting system allows player to forge upgrades directly
- (C) Tools evolve by absorbing monster essence (Gothic theme)  

**Depends on:** Crafting recipes, NPC shop design, material item definitions.

### Q5.3: What are the crafting station requirements?
**Context:** CraftingManager supports station-based crafting, but stations aren't defined.  
**Options:**
- Workbench (basic recipes)
- Forge (tools, weapons)
- Alchemy Table (potions, fertilizers)
- Loom (clothing, body wrappings)
- Occult Altar (creature enhancements, curses)  

**Depends on:** Recipe data, furniture placement, building system.

---

## 6. Creature System

### Q6.1: How many base creature species exist?
**Context:** Plan defines genetics but not the creature roster.  
**Options:**
- (A) 5-8 base species (manageable for initial release)
- (B) 12-15 species (good variety)
- (C) 20+ species (ambitious)  

**Depends on:** Sprite workload (each needs idle, walk, attack, special animations × trait variants).

### Q6.2: What do creatures do for the player?
**Context:** Plan mentions breeding but not utility.  
**Options:**
- (A) Combat companions that fight alongside the player in raids
- (B) Farm helpers (auto-water, auto-harvest based on traits)
- (C) Both combat and farming utility based on trait expression
- (D) Collectibles with exhibition/contest mechanics  

**Depends on:** Creature AI, trait gameplay effects, UI for creature management.

### Q6.3: Can creatures die permanently?
**Context:** Not addressed.  
**Options:**
- (A) Yes — death is permanent, emphasizing careful breeding
- (B) No — creatures faint and recover after a day
- (C) Yes, but can be resurrected at the Occult Altar (with mutation risk)  

**Depends on:** Difficulty, emotional investment, breeding strategy depth.

---

## 7. Technical Decisions

### Q7.1: What is the target resolution and pixel scale?
**Context:** Plan says "16-bit pixel art" and "32-bit style" in different sections.  
**Options:**
- (A) 16×16 tile size, 320×180 base resolution (classic 16-bit)
- (B) 32×32 tile size, 640×360 base resolution (higher detail, more asset work)
- (C) 16×16 tiles with 32×32 characters (mixed, common in indie games)  

**Depends on:** Every sprite asset, tileset design, camera configuration, UI scaling.

### Q7.2: What save format should be used?
**Context:** SaveSystem.cs currently uses JSON. Plan mentions binary.  
**Options:**
- (A) JSON (human-readable, easy to debug, mod-friendly)
- (B) Binary (smaller, faster, harder to tamper)
- (C) JSON for development, binary for release builds  

**Depends on:** Modding support plans, save file size, load time requirements.

### Q7.3: What platforms are targeted?
**Context:** Not specified in Plan. Affects build settings and input system.  
**Options:**
- (A) PC only (Windows/Mac/Linux)
- (B) PC + Console (Switch, Steam Deck)
- (C) PC + Console + Mobile  

**Depends on:** Input system configuration, UI scaling, performance budgets, build pipeline.

---

## Priority Decisions Needed

The following questions **must** be answered before meaningful Unity work begins:

1. **Q7.1** — Tile/pixel resolution (affects ALL art assets)
2. **Q2.1 & Q2.2** — Day length and season length (affects all time-based balancing)
3. **Q3.1** — Combat style (affects animation requirements and tool system)
4. **Q4.1** — NPC count (affects portrait/sprite workload)
5. **Q6.2** — Creature utility (affects creature AI and farm integration)
6. **Q1.1** — Farm size (affects tilemap setup and camera zoom)
