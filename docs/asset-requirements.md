# Asset Requirements for Havenwood Hollow

> Comprehensive list of all digital assets needed, organized by category.  
> Each asset includes a detailed AI image generation prompt.  
> **Note:** AI (Claude) cannot directly create image, audio, or 3D assets.  
> All visual/audio assets listed here must be created using external tools.

---

## Asset Creation Tools

| Tool | Purpose |
|------|---------|
| **Midjourney v6 / DALL-E 3 / Stable Diffusion** | Initial pixel art generation from prompts below |
| **Aseprite** | Sprite cleanup, animation frames, palette enforcement, tile slicing |
| **Audacity / FMOD** | Sound effect editing and audio mixing |
| **SpriteLamp / Laigter** | Normal map generation for 2D sprites |
| **Unity Tile Palette** | Importing and arranging tilesets |

---

## Shared Art Specifications

| Property | Value |
|----------|-------|
| **Tile Size** | 16×16 pixels (recommended, see Design Questions Q7.1) |
| **Character Sprite Size** | 16×32 pixels (1×2 tiles) or 32×32 pixels |
| **Color Palette** | Gothic palette — muted purples, deep greens, charcoal greys, blood reds, bone whites, sickly yellows |
| **Art Style** | 16-bit pixel art, top-down perspective (slight ¾ angle for characters) |
| **Animation Frame Rate** | 8-12 FPS for character animations |
| **Transparency** | All sprites on transparent backgrounds |

---

## 1. Tilesets

### 1.1 Farm Tileset
**Files Needed:** 1 seamless tileset image (256×256 or larger), sliced to 16×16 tiles  
**Tiles Required:**
- Dirt (dry, tilled, watered) — 3 variants + edges
- Grass (short, tall, dead) — 3 variants
- Path (cobblestone, dirt path) — 2 types with edges/corners
- Fences (wood, iron, bone) — 3 types with posts/rails
- Water (pond edges, animated ripples) — 4 frames

**AI Prompt:**
> `16-bit pixel art tileset, top-down perspective, gothic farm terrain, dark rich soil, withered grass, cracked cobblestone paths, rusty iron fences and bone-white picket fences, stagnant pond water, muted purple and grey color palette, seamless tiling, flat 2d game asset, transparent background, 16x16 pixel grid --v 6.0 --ar 1:1`

### 1.2 Town Tileset
**Files Needed:** 1 seamless tileset (256×256+)  
**Tiles Required:**
- Stone roads (cobblestone with moss) — with edges/corners
- Building walls (stone, wood, brick) — exterior facing
- Rooftops (slate, thatch) — 2 styles
- Doors and windows — 3 styles each
- Lampposts, benches, market stalls — decorative objects

**AI Prompt:**
> `16-bit pixel art tileset, top-down gothic victorian village, cobblestone streets with moss and cracks, dark stone buildings with pointed roofs, iron lampposts with flickering glow, wooden market stalls, eerie purple fog undertone, dark moody atmosphere, flat 2d game asset, seamless tiling, 16x16 pixel grid --v 6.0 --ar 1:1`

### 1.3 Forest Tileset
**Files Needed:** 1 seamless tileset (256×256+)  
**Tiles Required:**
- Dense trees (gnarled oaks, dead trees, mushroom-covered stumps) — 5+ variants
- Underbrush, brambles, thorns — ground decoration
- Forest floor (leaf litter, mud, moss) — 3 variants
- Forageable spawn points (berries, herbs, mushrooms)
- Fallen logs, boulders — obstacles

**AI Prompt:**
> `16-bit pixel art tileset, top-down dark enchanted forest, gnarled twisted oak trees, dead leafless branches, glowing mushrooms on stumps, thorny brambles, mossy forest floor with scattered leaves, eerie green and purple bioluminescence, dark gothic atmosphere, flat 2d game asset, seamless tiling, 16x16 pixel grid --v 6.0 --ar 1:1`

### 1.4 Mine/Dungeon Tileset
**Files Needed:** 1 seamless tileset (256×256+)  
**Tiles Required:**
- Stone walls (rough, carved, crumbling) — 3 variants with edges
- Floor (stone slab, gravel, blood-stained) — 3 variants
- Mine cart tracks — straight, curved, T-junction
- Ore deposits (copper, iron, crystal, eldritch) — 4 types
- Doors (wooden, iron, sealed) — 3 types
- Staircases up/down

**AI Prompt:**
> `16-bit pixel art tileset, top-down dark mine dungeon, rough hewn stone walls, crumbling ancient brick, glowing crystal ore deposits in walls, rusty mine cart tracks, stone slab flooring with cracks, dripping water, torch sconce holders, gothic underground crypt aesthetic, flat 2d game asset, seamless tiling, 16x16 pixel grid --v 6.0 --ar 1:1`

### 1.5 Interior Tileset
**Files Needed:** 1 seamless tileset (256×256+)  
**Tiles Required:**
- Wood plank flooring — 2 variants
- Stone flooring — 1 variant
- Carpet/rug sections — decorative
- Walls (interior wood, stone, wallpaper) — 3 types
- Furniture tiles (see Section 3 below)

**AI Prompt:**
> `16-bit pixel art tileset, top-down gothic interior rooms, dark wooden plank flooring, grey stone walls, faded Victorian wallpaper, worn carpet, cobwebs in corners, candle-lit atmosphere, moody warm amber and cool shadow tones, flat 2d game asset, seamless tiling, 16x16 pixel grid --v 6.0 --ar 1:1`

---

## 2. Character Sprites

### 2.1 Player Character
**Files Needed:** Sprite sheet per body variant  
**Animations Required (each in 4 directions: up, down, left, right):**
- Idle (2-4 frames)
- Walk (4-6 frames)
- Run (6-8 frames)
- Tool swing (hoe, axe, pickaxe — 4-6 frames)
- Watering can pour (4 frames)
- Sword slash (4-6 frames)
- Fishing cast + reel (6-8 frames)
- Eat/consume (4 frames)
- Stagger/damage (2 frames)
- Sleep/collapse (2 frames)

**AI Prompt (base sprite):**
> `pixel art sprite sheet, gothic farmer character, top-down RPG, dark cloak and worn boots, pale complexion, 4 directional walk cycle, idle pose, 16x32 pixel character on 16x16 grid, muted earth tones with purple undertones, transparent background, sequential animation frames, retro 16-bit style --v 6.0 --ar 16:9`

### 2.2 NPC Sprites (per NPC — estimate 10-20 needed)
**Files Needed:** 1 sprite sheet per NPC  
**Animations Required (4 directions each):**
- Idle (2 frames)
- Walk (4 frames)
- Unique action (e.g., blacksmith hammering, herbalist grinding, priest praying — 4 frames)

**AI Prompts (sample NPCs):**

**Blacksmith:**
> `pixel art sprite sheet, gothic village blacksmith character, top-down RPG, muscular build, leather apron, soot-covered face, heavy hammer, 4 directional walk cycle and hammering animation, 16x32 pixels, dark metal and fire tones, transparent background, retro 16-bit style --v 6.0`

**Herbalist/Witch:**
> `pixel art sprite sheet, gothic herbalist witch character, top-down RPG, hooded green cloak, glass vials and dried herbs, mysterious aura, 4 directional walk cycle and potion-mixing animation, 16x32 pixels, deep green and purple tones, transparent background, retro 16-bit style --v 6.0`

**Tavern Keeper:**
> `pixel art sprite sheet, gothic tavern keeper character, top-down RPG, stout figure, stained white shirt and dark vest, friendly but weary expression, 4 directional walk cycle and pouring drink animation, 16x32 pixels, warm amber and brown tones, transparent background, retro 16-bit style --v 6.0`

**Priest/Occultist:**
> `pixel art sprite sheet, gothic village priest character, top-down RPG, dark robes with silver holy symbols, gaunt figure, holding ancient tome, 4 directional walk cycle and praying animation, 16x32 pixels, black and silver tones with candlelight glow, transparent background, retro 16-bit style --v 6.0`

**Gravedigger:**
> `pixel art sprite sheet, gothic gravedigger character, top-down RPG, hunched posture, old tattered coat, carrying shovel, lantern on belt, 4 directional walk cycle and digging animation, 16x32 pixels, dark earth and grey tones, transparent background, retro 16-bit style --v 6.0`

### 2.3 NPC Portraits (for dialogue UI)
**Files Needed:** 1 portrait per NPC (96×96 or 128×128 pixels)  
**Expressions:** Neutral, happy, sad, angry, surprised, scared (6 per NPC)

**AI Prompt (template):**
> `pixel art character portrait, [CHARACTER NAME AND DESCRIPTION], gothic fantasy style, expressive face showing [EMOTION], ornate dark frame border, detailed shading, 128x128 pixels, moody dramatic lighting, dark background, retro RPG dialogue portrait style --v 6.0 --ar 1:1`

---

## 3. Furniture & Building Sprites

### 3.1 Farm Buildings
- Player house (3 upgrade levels: shack → cottage → manor)
- Barn (for creatures)
- Coop (for smaller creatures)
- Greenhouse (glass and iron frame)
- Silo (grain storage)
- Well (water source)

**AI Prompt (Player House — Level 1):**
> `pixel art building sprite, top-down gothic farmhouse, small wooden shack with crooked chimney, broken shutter windows, overgrown vines, dark wood and moss, Victorian Gothic style, 48x48 pixels, moody atmosphere, transparent background, retro 16-bit game asset --v 6.0`

### 3.2 Interior Furniture
- Bed, dresser, mirror, wardrobe
- Kitchen: stove, table, chairs, cabinet
- Crafting: workbench, forge, alchemy table, loom, occult altar
- Decorative: bookshelves, paintings, rugs, curtains, candelabras
- Storage: chests (wood, iron, eldritch)

**AI Prompt (Crafting Stations):**
> `pixel art game furniture sprites, top-down view, gothic alchemy workbench with bubbling potions and skulls, iron forge with glowing embers, dark wooden crafting table with tools, occult altar with candles and runes, each item 32x32 pixels, dark moody aesthetic, transparent background, retro 16-bit style --v 6.0`

---

## 4. Crop & Plant Sprites

### 4.1 Crops (per crop type, ~15-20 crops needed)
**Files Needed:** 1 sprite strip per crop showing all growth phases  
**Phases per crop:** 4-6 phases (seed → sprout → growing → mature → harvest-ready)

**Crop List:**
| Crop | Season | Phases | Theme |
|------|--------|--------|-------|
| Nightshade Berries | Spring | 5 | Purple berries on thorny vine |
| Blood Root | Spring | 4 | Red root vegetable, bleeds when harvested |
| Corpse Lily | Summer | 6 | Large putrid flower, valuable |
| Shadow Melon | Summer | 5 | Dark-skinned melon with glowing interior |
| Bone Wheat | Fall | 4 | White skeletal wheat stalks |
| Grave Pumpkin | Fall | 5 | Jack-o-lantern pumpkin with face |
| Frost Moss | Winter | 3 | Crystal-covered moss (greenhouse only) |
| Witch's Cabbage | Spring/Fall | 4 | Purple cabbage with spiral pattern |
| Ghoul Pepper | Summer | 5 | Fiery orange pepper that glows |
| Moon Potato | Fall/Winter | 4 | Luminescent pale tuber |
| Spirit Corn | Summer/Fall | 6 | Translucent husks, ghostly glow |
| Hemlock Herb | Spring | 3 | Medicinal but dangerous herb |
| Dread Tomato | Summer | 5 | Pulsating red fruit |
| Wither Rose | Fall | 5 | Beautiful but wilting Gothic rose |
| Eternal Turnip | All Seasons | 4 | Regrows after harvest, common starter |

**AI Prompt (crop growth strip):**
> `pixel art crop growth stages, top-down farming game, [CROP NAME]: [DESCRIPTION], 5 sequential growth phases from seed to harvest, planted in dark tilled soil, gothic dark aesthetic, each phase 16x16 pixels, arranged left to right, transparent background, retro 16-bit style --v 6.0 --ar 5:1`

---

## 5. Creature Sprites

### 5.1 Base Creatures (~8-12 species)
**Files Needed:** 1 sprite sheet per creature  
**Animations (4 directions):** Idle (2f), Walk (4f), Attack (4f), Special/Trait (4f), Death (4f)

**Creature List:**
| Creature | Description | Size |
|----------|-------------|------|
| Shade Hound | Shadow-wolf with glowing red eyes | 16×16 |
| Crystal Beetle | Armored insect with gem-like shell | 16×16 |
| Wraith Moth | Ethereal giant moth with luminescent wings | 24×24 |
| Bone Serpent | Skeletal snake with venomous fangs | 16×32 |
| Grave Toad | Bloated toad that croaks ominously | 16×16 |
| Phantom Cat | Semi-transparent feline, bioluminescent | 16×16 |
| Iron Raven | Metallic bird, sparks when it flaps | 16×16 |
| Swamp Leech | Writhing parasite, grows when feeding | 16×16 |
| Mire Salamander | Amphibian with toxic skin glow | 16×16 |
| Elder Owl | Large owl with ancient wisdom aura | 24×24 |

**AI Prompt (template):**
> `pixel art sprite sheet, [CREATURE NAME], [DESCRIPTION], top-down gothic fantasy game, idle walk and attack animations, 4 directional, [SIZE] pixels, dark eerie color palette with [ACCENT COLOR] glow effects, transparent background, sequential animation frames, retro 16-bit creature design --v 6.0 --ar 16:9`

### 5.2 Genetic Trait Overlays
**Files Needed:** 1 overlay sprite per trait per creature  
These are drawn on top of the base creature to show expressed genetic traits.

| Trait | Visual Overlay |
|-------|---------------|
| NightVision | Glowing eyes overlay (red/green) |
| Wings | Small wing pair attached to body |
| HardenedScale | Armor plate texture overlay |
| Venomous | Dripping green particles from mouth |
| Bioluminescence | Pulsing glow outline (2-frame animation) |
| ShadowForm | Distortion/transparency overlay |

**AI Prompt:**
> `pixel art creature trait overlays, top-down game sprites, set of 6 genetic modifications: glowing red eyes, small bat wings, armored scale plates, green venom drip, bioluminescent glow aura, shadow distortion effect, each overlay 16x16 pixels, transparent background, gothic dark aesthetic, retro 16-bit style --v 6.0`

---

## 6. Enemy Sprites

### 6.1 Raid Enemies
**Files Needed:** 1 sprite sheet per enemy  
**Animations (4 directions):** Idle (2f), Walk (4f), Attack (4f), Death (4f)

| Enemy | Description | Size |
|-------|-------------|------|
| Shambling Ghoul | Undead humanoid, slow but persistent | 16×32 |
| Spectral Wisp | Floating orb of ghostly light, ranged attacker | 16×16 |
| Gravestone Golem | Animated graveyard stones, heavy tank | 32×32 |
| Phantasm | Near-invisible enemy with distortion shader | 16×32 |
| Bone Crawler | Multi-legged skeletal horror, fast | 24×16 |
| Hollow Knight | Armored undead warrior, mid-boss | 16×32 |

**AI Prompt (Shambling Ghoul):**
> `pixel art sprite sheet, shambling ghoul undead enemy, top-down gothic RPG, rotting flesh, torn burial clothes, shambling walk cycle, clawing attack animation, death collapse, 4 directional, 16x32 pixels, dark putrid green and grey tones, transparent background, retro 16-bit horror game style --v 6.0 --ar 16:9`

### 6.2 Raid Boss
**Files Needed:** 1 large sprite sheet  
**Animations:** Idle (4f), Walk (4f), Attack patterns ×3 (6f each), Phase transition (4f), Death (8f)

**The Hollow King:**
> `pixel art boss sprite sheet, The Hollow King, massive undead monarch, top-down gothic RPG, crown of twisted iron and bone, flowing tattered royal robes, skeletal hands wielding a dark scepter, glowing void in chest cavity, imposing 48x48 pixel sprite, multiple attack animations, dark purple and black with gold accents, transparent background, detailed retro pixel art boss design --v 6.0 --ar 16:9`

---

## 7. UI Assets

### 7.1 HUD Elements
- Health bar frame + fill (red gradient)
- Stamina bar frame + fill (green gradient)
- Experience bar (purple gradient)
- Clock/time display frame
- Season indicator icons (4 seasons)
- Currency icon
- Hotbar slot frame (selected + unselected states)

**AI Prompt:**
> `pixel art game UI elements, gothic RPG HUD, ornate dark iron health bar frame with red fill, stamina bar with green fill, decorative hotbar slots with dark filigree borders, clock icon with moon phases, season icons showing spring flower summer sun fall leaf winter snowflake, gold coin currency icon, all on transparent background, retro 16-bit gothic aesthetic, dark and elegant --v 6.0`

### 7.2 Inventory & Menu UI
- Inventory panel background (dark wood/leather texture)
- Item slot frames (normal, selected, locked)
- Button frames (normal, hover, pressed, disabled)
- Tab indicators (equipment, items, crafting, quests)
- Scroll bar and arrows
- Tooltip background frame
- Window close button

**AI Prompt:**
> `pixel art RPG inventory menu UI kit, gothic dark style, aged leather and dark wood panel backgrounds, ornate iron-framed item slots, Victorian-style button designs with hover and pressed states, decorative scroll elements, tab icons for equipment sword items potion crafting hammer quests scroll, dark elegant color scheme, retro 16-bit game UI, transparent background --v 6.0`

### 7.3 Dialogue UI
- Dialogue box background (parchment/dark panel)
- Character name plate
- Choice button frames
- Text advance indicator (animated arrow)
- Portrait frame border

**AI Prompt:**
> `pixel art dialogue box UI, gothic RPG style, aged parchment text box with dark ornate border, Victorian nameplate for speaker name, choice buttons with iron frame styling, small animated arrow indicator, portrait frame with Gothic arch design, dark moody color scheme, retro 16-bit game UI elements, transparent background --v 6.0`

### 7.4 Shop UI
- Shop panel background
- Buy/sell tabs
- Price tags with currency icon
- Quantity selector arrows
- Confirm/cancel buttons

### 7.5 Quest Log UI
- Quest log panel background
- Quest entry frames (active, completed, failed)
- Objective checkboxes
- Quest category tabs (main, side, bounty)
- Reward preview frames

---

## 8. Effects & Particles

### 8.1 Environmental Effects
- Rain particles (normal, blood rain during raids)
- Fog/mist sprites (rolling ground fog)
- Firefly particles (forest ambience)
- Falling leaves (autumn)
- Snow particles (winter)
- Dust motes (interior)

### 8.2 Combat Effects
- Slash trail (tool/weapon swing)
- Hit spark (damage dealt)
- Heal shimmer (potion use)
- Poison drip (venomous attack)
- Shield flash (HardenedScale block)
- Death poof (enemy defeat)

### 8.3 Farming Effects
- Water splash (watering can)
- Soil till puff (hoe use)
- Harvest sparkle (crop ready)
- Growth shimmer (fertilizer applied)
- Seed plant puff (planting)

**AI Prompt (Effects Sheet):**
> `pixel art VFX sprite sheet, gothic RPG game effects, sword slash trail, impact spark, healing green shimmer, poison purple drip, water splash, soil dust puff, harvest golden sparkle, death smoke poof, each effect 4-6 animation frames, 16x16 pixels per frame, transparent background, retro 16-bit style, dark moody color palette --v 6.0 --ar 16:9`

---

## 9. Audio Assets

> **Note:** Audio cannot be generated by Claude. Use tools like FMOD, Audacity, sfxr, or license from asset stores.

### 9.1 Music Tracks
| Track | Duration | Description |
|-------|----------|-------------|
| Main Theme | 2-3 min | Melancholic piano + strings, Gothic waltz feel |
| Farm Day | 3-4 min | Gentle acoustic guitar, birdsong, pastoral but eerie undertone |
| Farm Night | 3-4 min | Music box melody, owl hoots, creaking wood, tense |
| Town Theme | 3-4 min | Warm pub folk music with minor key, accordion |
| Forest Theme | 3-4 min | Ambient drones, wind, distant whispers, harp arpeggios |
| Mine Theme | 3-4 min | Echoing percussion, dripping water, low strings |
| Raid Battle | 2-3 min | Intense orchestral, fast percussion, choir stabs |
| Boss Battle | 3-4 min | Epic organ + full orchestra, escalating intensity |
| Season - Spring | 3-4 min | Hopeful but tinged with sorrow, flute lead |
| Season - Summer | 3-4 min | Warm but oppressive, heavy cicada sounds, slow tempo |
| Season - Fall | 3-4 min | Nostalgic, wind instruments, falling leaf sound design |
| Season - Winter | 3-4 min | Sparse, cold, glass-like chimes, minimal |

### 9.2 Sound Effects
| Category | Effects Needed |
|----------|---------------|
| **Player** | Footsteps (grass, stone, wood, mud — 3 variants each), eat/drink, damage taken, level up |
| **Tools** | Hoe dig, axe chop, pickaxe mine, watering pour, sword swing, fishing cast + reel |
| **Farming** | Crop plant, crop harvest, crop wither, fertilizer apply |
| **UI** | Menu open/close, item pickup, item drop, slot select, button click, quest complete, gold clink |
| **Combat** | Hit impact, miss whoosh, enemy death, shield block, poison sizzle, heal shimmer |
| **Environment** | Door open/close, wind, rain, thunder, fire crackle, water flow, owl hoot, crow caw |
| **Creatures** | Per-creature cry (idle, happy, distressed, attack — 3 variants each) |
| **Raid** | Warning horn, gate breaking, raid start fanfare, raid victory fanfare |

### 9.3 Ambient Loops
| Location | Ambience Description |
|----------|---------------------|
| Farm (Day) | Distant birdsong, gentle wind, occasional chicken cluck |
| Farm (Night) | Crickets, distant wolf howl, creaking timber |
| Town | Murmured voices, footsteps, cart wheels, blacksmith hammer |
| Forest | Wind through leaves, owl hoots, twig snaps, insect buzz |
| Mine | Dripping water, distant rumbles, echo of footsteps |
| During Raid | Thunder, distant screams, breaking wood, fire crackle |

---

## 10. Miscellaneous Assets

### 10.1 Title Screen
- Logo: "Havenwood Hollow" in Gothic serif font with vine/thorn decorations
- Background: Atmospheric farm scene at twilight, mansion silhouette on hill

**AI Prompt:**
> `pixel art title screen, "Havenwood Hollow" gothic horror farming game, dark twilight scene, silhouette of Victorian mansion on misty hill, barren farm in foreground, gnarled dead tree, full moon partially obscured by clouds, flock of crows, moody purple and deep blue color palette, atmospheric fog, retro 16-bit game title screen, detailed pixel art --v 6.0 --ar 16:9`

### 10.2 Season Transition Screens
- Spring: Flowers pushing through dark soil, pale green tint
- Summer: Oppressive sun over withered fields, amber tint
- Fall: Swirling dead leaves, orange/brown tint
- Winter: Snow on gravestones, blue/white tint

### 10.3 Map/Minimap Icons
- Player position marker
- Building icons (house, barn, shop, church)
- NPC markers
- Quest objective markers
- Resource node indicators

---

## Asset Count Summary

| Category | Estimated Count |
|----------|----------------|
| Tilesets | 5 major sets (~500+ individual tiles) |
| Player Sprites | 1 base + variants (~200 frames) |
| NPC Sprites | 10-20 characters (~80-160 frames each) |
| NPC Portraits | 10-20 × 6 expressions (~60-120 portraits) |
| Crop Sprites | 15-20 crops × 5 phases (~75-100 sprites) |
| Creature Sprites | 8-12 species (~80-120 frames each) |
| Enemy Sprites | 6 types + 1 boss (~100-150 frames each) |
| Trait Overlays | 6 traits × 10 creatures (~60 sprites) |
| Building Sprites | ~15 buildings |
| Furniture Sprites | ~40 items |
| UI Elements | ~80 individual frames |
| Effects/Particles | ~60 animation frames |
| Music Tracks | 12 tracks |
| Sound Effects | ~80 individual sounds |
| Ambient Loops | 6 loops |
| **Total Estimated** | **~1,500-2,000 visual assets + ~100 audio assets** |
