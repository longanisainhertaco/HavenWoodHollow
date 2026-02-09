# Open Design Questions for Havenwood Hollow

> Decisions that need to be made before or during Unity development.  
> Each question includes context, options, and what depends on the answer.  
> ✅ = Resolved | ❓ = Still Open

---

## 1. World & Setting

### Q1.1: How large is the player's farm? ✅ RESOLVED
**Decision:** Same as Stardew Valley — **~80×65 tiles** (~5,200 plantable tiles).  
**Rationale:** Matches the proven Stardew Valley standard farm layout. Large enough for meaningful farming with Job System optimization, familiar scope for genre fans.

~~**Options:**~~
- ~~(A) Small: 40×40 tiles~~
- **(B) Medium: ~80×65 tiles — matching Stardew Valley** ✅
- ~~(C) Large: 100×100 tiles~~

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

### Q2.1: How long is a single in-game day in real minutes? ✅ RESOLVED
**Decision:** Same as Stardew Valley — **14 real-time minutes** per in-game day (840 seconds).  
**Rationale:** Proven pacing that balances farming tasks, exploration, and social interactions within a single day.

~~**Options:**~~
- **(A) 14 minutes (Stardew Valley standard)** ✅
- ~~(B) 20 minutes~~
- ~~(C) Configurable~~

### Q2.2: How many days per season? ✅ RESOLVED
**Decision:** Same as Stardew Valley — **28 days per season** (4 weeks).  
**Rationale:** Standard farming game pacing. Aligns with existing SeasonManager default. Provides enough time for crop cycles and seasonal events.

~~**Options:**~~
- **(A) 28 days (Stardew standard, 4 weeks)** ✅
- ~~(B) 14 days~~
- ~~(C) 30 days~~

### Q2.3: What happens in Winter?
**Context:** Most farming games restrict farming in winter. Gothic theme could make winter uniquely dangerous.  
**Options:**
- (A) No farming at all — focus on mining, combat, socializing
- (B) Limited greenhouse farming with special winter-only crops
- (C) Winter is "eternal night" with continuous raid threat and survival mechanics  

**Depends on:** Crop data `Seasons` field, gameplay loop variety, difficulty curve.

---

## 3. Combat & Raids

### Q3.1: Is combat real-time action or turn-based? ✅ RESOLVED
**Decision:** **Simplified Stardew-style combat** — single attack button, auto-aim nearest enemy, simple hitboxes.  
**Rationale:** Reduces animation complexity, keeps focus on farming/breeding loop, accessible to casual players. Weapons use the same Physics2D.OverlapCircle system as tools.

~~**Options:**~~
- ~~(A) Real-time action combat~~
- **(B) Stardew-style simplified swing** ✅
- ~~(C) Hybrid~~

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

### Q4.1: How many NPCs are there? ✅ RESOLVED
**Decision:** **30-50 NPCs** — a large, bustling Gothic village exceeding Stardew's roster.  
**Rationale:** Supports the ambition of a rich Gothic community. Stardew Valley has ~30 named NPCs. We aim for 30-50 with unique schedules, dialogue, and GOAP behaviors. This is a significant art and writing investment but creates a living world.

~~**Options:**~~
- ~~(A) 8-10 core NPCs~~
- ~~(B) 15-20 NPCs~~
- **(D) 30-50 NPCs (ambitious Gothic village)** ✅

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

### Q6.2: What do creatures do for the player? ✅ RESOLVED
**Decision:** **Both combat and farming utility** based on genetic trait expression.  
**Rationale:** Maximizes the value of the breeding system. Creatures with Wings/HardenedScale excel in combat; creatures with Bioluminescence/NightVision help on the farm. Trait expression determines role, giving breeding strategic depth.

**Gameplay Effects by Trait:**
| Trait | Combat Utility | Farm Utility |
|-------|---------------|--------------|
| NightVision | Scouts enemies in dark | Harvests at night automatically |
| Wings | Dodges attacks, aerial strikes | Pollinates crops (growth boost) |
| HardenedScale | Absorbs damage (tank) | Breaks rocks on farm |
| Venomous | Poison DOT on enemies | Pest control (prevents crop disease) |
| Bioluminescence | Reveals invisible enemies | Acts as portable light source |
| ShadowForm | Stealth flanking attacks | Scares away crows/pests |

~~**Options:**~~
- ~~(A) Combat companions~~
- ~~(B) Farm helpers~~
- **(C) Both combat and farming utility** ✅
- ~~(D) Collectibles~~

### Q6.3: Can creatures die permanently?
**Context:** Not addressed.  
**Options:**
- (A) Yes — death is permanent, emphasizing careful breeding
- (B) No — creatures faint and recover after a day
- (C) Yes, but can be resurrected at the Occult Altar (with mutation risk)  

**Depends on:** Difficulty, emotional investment, breeding strategy depth.

---

## 7. Technical Decisions

### Q7.1: What is the target resolution and pixel scale? ✅ RESOLVED
**Decision:** **16×16 tile size, 320×180 base resolution** (classic 16-bit pixel art).  
**Rationale:** Lower asset complexity enables faster iteration with the 30-50 NPC target. Classic pixel art style matches the Gothic horror aesthetic. All assets in `asset-requirements.md` are calibrated to this size.

**Confirmed Specs:**
- Tile size: 16×16 pixels
- Character sprite size: 16×32 pixels (1×2 tiles)
- Base resolution: 320×180 (16:9, scales to 1920×1080 at 6x)
- Pixels per unit: 16

~~**Options:**~~
- **(A) 16×16 tile size, 320×180 base resolution** ✅
- ~~(B) 32×32 tile size~~
- ~~(C) Mixed~~

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

## Priority Decisions — ✅ ALL RESOLVED

All 6 priority decisions have been made. Unity development can proceed.

| # | Question | Decision | Impact |
|---|----------|----------|--------|
| 1 | **Q7.1** — Tile/pixel resolution | **16×16 tiles, 320×180 base** | All art assets calibrated to 16px grid |
| 2 | **Q2.1 & Q2.2** — Day/season length | **14 min days, 28 days/season** | Time-based balancing locked |
| 3 | **Q3.1** — Combat style | **Simplified Stardew-style** | Minimal combat animation needs |
| 4 | **Q4.1** — NPC count | **30-50 NPCs** | Major art/writing investment |
| 5 | **Q6.2** — Creature utility | **Both combat + farming** | Trait system has dual purpose |
| 6 | **Q1.1** — Farm size | **~80×65 tiles (Stardew-scale)** | Tilemap and camera configured |
