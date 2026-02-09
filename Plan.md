# **Clean-Room Porting Plan: Technical Specification for 'Havenwood Hollow' on Unity 6**

## **1\. Executive Summary**

The following document serves as the master technical and legal blueprint for the development of 'Havenwood Hollow,' a simulation role-playing game (RPG) designed to replicate the mechanical depth of *Stardew Valley* while migrating the codebase to the Unity 6 engine and introducing a distinct Gothic Horror thematic overlay. This report mandates a "Clean-Room" development methodology to ensure legal immunity from copyright infringement claims associated with the reference material, specifically the decompiled *Stardew Valley* repository.  
The engineering challenge is twofold: first, to deconstruct the functional logic of the reference title—specifically its movement physics, crop growth algorithms, and interaction systems—without accessing or copying the original source code during the implementation phase. Second, to leverage the advanced capabilities of Unity 6, including the Universal Render Pipeline (URP) for 2D dynamic lighting, the CoreCLR runtime for performance optimization, and the job system for complex simulations, to surpass the technical limitations of the reference's XNA/MonoGame framework.  
Furthermore, 'Havenwood Hollow' will diverge significantly in its gameplay loop by integrating "Town Defense" mechanics driven by Goal-Oriented Action Planning (GOAP) artificial intelligence, a genetic breeding system for eldritch creatures utilizing bitwise inheritance logic, and a stealth mechanics suite relying on dynamic lighting and shader-based environmental cues. This report provides a granular, line-by-line implementation strategy for these systems, ensuring that the final product is not merely a clone, but a technological evolution of the genre.

## ---

**2\. Legal Framework: The Clean-Room Protocol**

Copyright law, particularly in the context of software development, protects the specific expression of ideas—the actual lines of code—but not the functional concepts or mechanics themselves. To successfully port the mechanics of *Stardew Valley* without infringing on the intellectual property of ConcernedApe, the development process must adhere to a strict Clean-Room Reverse Engineering protocol. This necessitates the creation of a "Chinese Wall" between two distinct engineering teams.

### **2.1 Operational Separation of Teams**

The integrity of the Clean-Room process relies on the absolute separation of the "Analysis Team" and the "Implementation Team." Any breach of this separation, however minor, compromises the legal defensibility of the project.

#### **2.1.1 The Analysis Team (Dirty Room)**

This group acts as the research unit. They are granted authorization to study the reference material, including the WeDias/StardewValley repository, decompiled C\# binaries, and existing community documentation.1 Their mandate is strictly limited to analysis.

* **Responsibilities:** They must analyze the reference code to understand the underlying algorithms (e.g., how daily crop growth is calculated, how movement speed modifiers stack, how NPC schedules are parsed).  
* **Deliverables:** They produce non-code documentation, such as white papers, flowcharts, logic tables, and pseudo-code specifications. These documents must describe *what* the system does, not *how* it is coded in the original source.3  
* **Restrictions:** Members of this team are permanently barred from writing any code for the 'Havenwood Hollow' repository. They may not discuss implementation details with the clean team, only functional requirements.

#### **2.1.2 The Implementation Team (Clean Room)**

This group is responsible for building 'Havenwood Hollow' in Unity 6\. They work in a pristine environment, isolated from the reference material.

* **Access Rights:** They must never view the *Stardew Valley* source code, decompiled assets, or the WeDias repository. Their only source of information is the functional specification provided by the Analysis Team and standard Unity documentation.3  
* **Verification:** All code committed to the repository must be original. If a function in 'Havenwood Hollow' looks identical to *Stardew Valley*, it must be provable that this similarity is a result of functional necessity (e.g., a standard A\* pathfinding implementation) rather than copying.

### **2.2 Intellectual Property & Asset Hygiene**

A critical component of legal safety is the handling of assets. The "Dirty Room" analysis indicates that *Stardew Valley* relies on specific XNA libraries and sprite batching techniques.2 While the clean team creates new code, they must also ensure no visual or audio assets are carried over.

* **Asset Prohibition:** No sprites, sound effects, or data files (XML/JSON) from the reference game may be imported into the Unity project, even as placeholders. Placeholder art must be primitive shapes or original creations.  
* **Clean Data Structures:** While the Reference Specification might describe a crop having "5 growth stages," the Implementation Team must define their own data structures (e.g., ScriptableObjects in Unity) to store this data, ensuring the file formats and internal schemas are distinct from the reference's XNA serialization.5

## ---

**3\. Core Mechanics Analysis & Unity Implementation**

The following sections detail the translation of specific *Stardew Valley* mechanics into Unity 6, contrasting the "Reference Logic" (derived from research) with the "Clean Room Implementation" (proposed for 'Havenwood Hollow').

### **3.1 Character Locomotion and Physics**

The "feel" of a game is largely defined by its movement logic. The reference game, built on XNA, utilizes an integer-based coordinate system for precise pixel alignment, resulting in "snappy" movement with no inertia. Unity's default physics engine, Box2D, is continuous and floating-point based, which can lead to a "floaty" feeling if not properly constrained.

#### **3.1.1 Reference Logic Analysis**

Analysis of the reference movement mechanics reveals a deterministic system. The player's base walking speed is identified as 2 pixels per frame, and running speed is 5 pixels per frame.6 These values are modified by additive buffs:

* **Coffee:** \+1 Speed  
* **Horse:** \+1.6 Speed (Resulting in 6.6 or 7 approx)  
* **Pathing:** \+0.1 Speed on specific tiles  
* **Terrain Penalties:** \-1 Speed for grass or crops (unless running).7

The movement logic likely polls for input and directly modifies the transform position, performing collision checks against the GameLocation tile grid before applying the move. This results in instant acceleration and deceleration.8

#### **3.1.2 Unity 6 Clean Implementation**

To replicate this in Unity 6 without copying code, we must bypass the default AddForce physics method, which introduces unwanted acceleration curves and inertia. Instead, we utilize a Kinematic Rigidbody2D.  
**Component Configuration:**

* **Rigidbody2D:** Body Type set to Kinematic. This gives the script total control over movement while allowing the physics engine to handle collision resolution.  
* **Collision Detection:** set to Continuous to prevent tunneling at high speeds.  
* **Interpolation:** Enabled to ensure smooth visual movement between fixed physics ticks, preventing jitter on high-refresh-rate monitors.10

**Velocity Calculation Logic:**  
The Implementation Team must construct a PlayerController that calculates velocity based on the specifications provided.

C\#

// Pseudo-code for Clean Room Specification  
void FixedUpdate() {  
    // 1\. Input Processing  
    Vector2 rawInput \= InputSystem.ReadValue\<Vector2\>();  
      
    // Normalize to prevent diagonal speed boost (Pythagorean theorem)  
    // Reference game may or may not normalize; modern standards dictate we should unless attempting bug-for-bug compatibility.  
    // However, exact replication requires checking if (1,1) moves faster than (1,0).   
    // Research implies Stardew diagonal movement might be normalized in later patches or handled via specific clamping.  
    // We will normalize for consistency.  
    Vector2 moveDirection \= rawInput.normalized;

    // 2\. Speed Calculation  
    float baseSpeed \= IsRunning? 5.0f : 2.0f; // Pixels per tick equivalent  
    float modifiers \= CalculateBuffs(); // Coffee (+1), Pathing (+0.1)  
    float activeSpeed \= baseSpeed \+ modifiers;

    // 3\. Application via RigidBody API  
    // MovePosition is critical here. It teleports the RB to the new spot but sweeps for collisions along the way.  
    // This replicates the integer-collision logic of the reference.  
    Vector2 targetPosition \= rb.position \+ (moveDirection \* activeSpeed \* Time.fixedDeltaTime);  
    rb.MovePosition(targetPosition);  
}

**Collision Handling:** The reference uses a custom tile collision system. In Unity 6, we leverage the CompositeCollider2D on the Tilemap. This merges individual tile colliders into a single geometry, preventing the player from getting "stuck" on the seams between adjacent wall tiles—a common issue in Unity 2D development.12

### **3.2 The Agricultural Simulation Engine**

The core loop of farming requires a robust data management system. Unlike the reference, which uses XML serialization for save states 5, Unity 6 favors ScriptableObjects for static data and binary or JSON serialization for dynamic save data.

#### **3.2.1 Crop Growth Logic Specification**

Research into the reference logic highlights a specific phase-based growth system. Crops do not grow linearly; they progress through distinct visual stages defined by integer durations.

* **Growth Phases:** Defined as an array of integers representing days. E.g., \[2, 3, 2, 3, 3\] means Phase 1 lasts 2 days, Phase 2 lasts 3 days, etc..14  
* **Watering Dependency:** Growth is strictly conditional. If IsWatered \== false at the nightly tick, the DaysInCurrentPhase counter does not increment.  
* **Fertilizer Math:** Modifiers like "Speed-Gro" (10% or 25%) are applied to the total growth time or per-phase time. The logic often involves rounding to ensure integer days.  
  * *Formula Spec:* DaysSaved \= ceil(TotalDays \* Modifier). This value is then subtracted from the growth phases, typically removing days from the later phases first or distributed evenly.14

**Table 1: Proposed Unity Data Structure for Crops**

| Field Name | Data Type | Description | Reference Equivalent Logic |
| :---- | :---- | :---- | :---- |
| ID | string | Unique identifier (e.g., "crop\_parsnip") | Matches Object ID logic 5 |
| PhaseDurations | int | Days required for each visual stage | phaseDays array in reference 15 |
| Regrows | bool | True if crop persists after harvest | regrowAfterHarvest logic |
| RegrowPhaseIndex | int | The phase index to revert to after harvest | \- |
| Seasons | Season | Enum flags for valid growing seasons | Season validation logic |
| HarvestItem | ItemSO | Reference to the item yielded | indexOfHarvest |

#### **3.2.2 Unity 6 Implementation Strategy**

To optimize for thousands of crops, we must avoid creating a GameObject for every plant. Instead, we utilize a specialized Tilemap manager.

* **Data Storage:** A Dictionary\<Vector3Int, CropState\> stores the dynamic data (current phase, watered status) for each coordinate. CropState is a lightweight struct.  
* **Rendering:** The CropManager updates the Tilemap visuals directly using Tilemap.SetTile(pos, phaseSprite).  
* **Simulation Tick:** The growth logic is decoupled from Update. It runs only when the "Sleep" event is triggered.  
  * *Optimization:* Utilizing Unity's C\# Job System, the nightly update can process 10,000+ crop tiles in parallel, calculating growth, checking watering status, and applying fertilizer logic in milliseconds.17

### **3.3 Inventory and Tool Interaction**

The reference game uses a unified Object class hierarchy where tools, items, and furniture share common parents. Unity's component-based architecture suggests a different approach utilizing Interfaces.

#### **3.3.1 Clean Room Tool Logic**

Instead of inheritance (e.g., Tool \-\> Axe \-\> CopperAxe), we use a Composition model.

* **Interfaces:** IInteractable, ITool, IConsumable.  
* **Tool Action:** When the player uses a tool, the system performs a Physics2D.OverlapCircle or raycast at the target grid coordinate.  
  * If the hit object implements IDamageable (e.g., a rock, a monster), the tool applies damage.  
  * If the hit object interacts with a specific tool type (e.g., Hoe on Dirt), the terrain modification is triggered.5

## ---

**4\. Artificial Intelligence: Goal-Oriented Action Planning (GOAP)**

A critical divergence from the reference material is the "Town Defense" mechanic. While *Stardew Valley* features basic state-machine AI (e.g., "Move towards player," "Attack"), 'Havenwood Hollow' requires NPCs to exhibit complex behaviors: farming during the day, socializing in the evening, and defending the town against raids at night. This complexity exceeds the capabilities of simple Finite State Machines (FSMs) or Behavior Trees. We will implement Goal-Oriented Action Planning (GOAP).

### **4.1 Why GOAP?**

Behavior Trees (BTs) require the developer to explicitly define every transition and branch. In a raid scenario where an NPC might need to find a weapon, barricade a door, or heal an ally depending on dynamic conditions, a BT becomes unwieldy and brittle. GOAP allows the agent to dynamically formulate a plan based on a desired Goal and the current World State.18

### **4.2 Unity 6 GOAP Architecture**

We will utilize the CrashKonijn/GOAP library logic (recreated or imported as a verified third-party tool) which leverages Unity's Job System for multi-threaded planning.17

#### **4.2.1 World State Definition**

The AI perception system updates a "Blackboard" or World State with boolean or integer values derived from sensors.

* IsNight (Global)  
* RaidActive (Global)  
* HasWeapon (Local to Agent)  
* HealthLow (Local)  
* EnemyVisible (Local)

#### **4.2.2 Goals and Actions**

The planner works backwards from a Goal to find a sequence of Actions that satisfy it.  
**Table 2: GOAP Configuration for 'Village Guard'**

| Goal | Priority | Condition | Actions to Satisfy |
| :---- | :---- | :---- | :---- |
| **Survive** | 100 (Critical) | HealthLow \== true | UsePotion, FleeToBunker |
| **DefendTown** | 80 (High) | RaidActive \== true | PatrolPerimeter, AttackEnemy, BarricadeGate |
| **MaintainEnergy** | 40 (Medium) | Stamina \< 20 | EatFood, Sleep |
| **Socialize** | 20 (Low) | IsEvening \== true | GoToTavern, TalkToVillager |

* **Example Plan:** If a raid starts (RaidActive \= true) but the guard has no weapon (HasWeapon \= false):  
  * Goal: DefendTown (Requires EnemyDead).  
  * Action: AttackEnemy (Requires HasWeapon).  
  * Action: RetrieveSword (Effect: HasWeapon \= true).  
  * **Resulting Plan:** RetrieveSword \-\> AttackEnemy.20

### **4.3 Procedural Raid Generation with Adaptive Probe Volumes**

To keep the defense mechanic engaging, raids cannot be static. We will use Unity 6's **Adaptive Probe Volumes (APV)**—typically used for lighting—to drive spawning logic. APVs store data about the scene's geometry and lighting. We can query the APV structure to find valid spawn points that are:

1. **Navigable:** Valid on the NavMesh.  
2. **Dark:** Low light intensity (spawn in shadows).  
3. **Hidden:** Occluded from the player's camera view. This ensures monsters emerge from the darkness organically, leveraging the lighting data for gameplay mechanics.22

## ---

**5\. Visual Pipeline: The Gothic Horror Aesthetic**

The visual identity of 'Havenwood Hollow' relies on a "Gothic Pixel Art" style. This requires a sophisticated rendering pipeline that combines the charm of 16-bit art with modern dynamic lighting to create tension and atmosphere.

### **5.1 Universal Render Pipeline (URP) 2D Lighting**

Unity 6's URP 2D Renderer is the cornerstone of this visual upgrade. Unlike the reference game's simple ambient tinting, URP allows for physically plausible 2D lighting.23

#### **5.1.1 Lighting Components**

* **Global Light 2D:** This drives the Day/Night cycle. A script TimeManager interpolates the global light color over time.  
  * *Day (12:00):* Pure White (1, 1, 1\) intensity 1.0.  
  * *Dusk (18:00):* Orange/Purple tint (0.8, 0.5, 0.6) intensity 0.8.  
  * *Night (00:00):* Deep Blue/Black (0.1, 0.1, 0.25) intensity 0.3. This darkness is crucial for the horror mechanics, forcing players to use light sources.25  
* **Point Light 2D:** Attached to torches, lanterns, and bioluminescent enemies.  
* **Shadow Caster 2D:** Attached to walls, trees, and buildings. These cast dynamic hard shadows, creating "safe zones" or "danger zones" for the stealth mechanic.

#### **5.1.2 Normal Maps for Pixel Art**

To prevent the lighting from looking "flat," all sprites will have associated Normal Maps.

* **Generation:** A secondary pipeline (potentially AI-assisted or using tools like SpriteLamp) generates normal maps that define the "slope" of each pixel.  
* **Effect:** When a 2D light passes over a sprite with a normal map, it illuminates the "facing" pixels and darkens the "away" pixels, creating volume. This is essential for the Gothic aesthetic, allowing gargoyles and crypt walls to look imposing and three-dimensional.26

#### **5.1.3 Light Blend Styles**

URP allows for custom Blend Styles. We will define:

* **Standard:** Multiplies light color with sprite color (Standard illumination).  
* **Additive:** Adds light color (Magic spells, ghost auras).  
* **Subtractive:** Removes light (Void zones, curses). This creates areas of "anti-light" where torches are dampened, a key horror mechanic.27

### **5.2 Stealth Mechanics: The Invisible Enemy**

Research suggests that "Invisible Person" mechanics rely on environmental feedback.28 We will implement a shader-based stealth system.

* **Distortion Shader:** Enemies like the "Phantasm" utilize a custom Shader Graph shader that samples the screen color behind the sprite and applies a UV offset based on a noise texture. This creates a "Predator" shimmer effect rather than total invisibility.30  
* **Particle Interaction:** Invisible enemies emit "Footprint" particles when moving. This requires decoupling the particle emitter from the sprite renderer so footprints appear even if the sprite is disabled.  
* **Light Detection Logic:** A script on the Player calculates the aggregate light intensity at their position using Light2D.GetIntensity().  
  * If Intensity \< StealthThreshold, the Player enters "Stealth Mode," becoming invisible to enemies (reduced aggro radius).  
  * This forces players to destroy light sources or stick to shadows during stealth segments.31

## ---

**6\. Genetic Breeding System**

'Havenwood Hollow' introduces a "Monster Ranching" element where players breed eldritch creatures. This requires a genetic inheritance algorithm significantly more complex than the reference game's animal reproduction.

### **6.1 Bitwise Genetics Architecture**

We utilize C\# Bitmasks (Flags Enum) to store and manipulate genetic traits efficiently. This allows for millions of combinations with minimal memory overhead.32  
**Table 3: Genetic Traits Bitmask Structure**

| Bit Index | Hex Value | Trait Name | Gameplay Effect | Dominance |
| :---- | :---- | :---- | :---- | :---- |
| 0 | 0x01 | NightVision | Can see in dark without lantern | Recessive |
| 1 | 0x02 | Wings | Can fly over obstacles | Dominant |
| 2 | 0x04 | HardenedScale | \+5 Defense | Dominant |
| 3 | 0x08 | Venomous | Attacks apply poison | Recessive |
| 4 | 0x10 | Bioluminescence | Acts as a light source | Recessive |
| 5 | 0x20 | ShadowForm | Invisible in darkness | Ultra-Rare |

### **6.2 The Inheritance Algorithm (Punnett Simulator)**

The breeding logic simulates a Punnett Square.

1. **Genome Storage:** Each creature has two sets of genes (Alleles) for each trait slot: GeneA and GeneB.  
2. **Expression:** The phenotype (visible trait) is determined by dominance. If Wings is dominant, Wings/Empty results in Wings.  
3. **Crossover:** When breeding, the offspring receives one allele from each parent randomly.  
   C\#  
   // Concept Logic  
   Trait alleleFromFather \= (Random.value \> 0.5f)? father.GeneA : father.GeneB;  
   Trait alleleFromMother \= (Random.value \> 0.5f)? mother.GeneA : mother.GeneB;  
   child.GeneA \= alleleFromFather;  
   child.GeneB \= alleleFromMother;

   // Mutation Check  
   if (Random.value \< MutationRate) {  
       child.GeneA \= GenerateRandomMutation();  
   }

4. **Visual Application:** The CreatureVisualizer script reads the expressed traits and swaps sprite parts using Unity's **Sprite Library Asset** system (see Section 7.2).34

## ---

**7\. Modular Character System (Frankenstein Mechanic)**

Aligning with the Gothic theme, players can modify characters (or themselves) by swapping body parts—a "Frankenstein" mechanic.

### **7.1 2D Animation and Rigging**

Unity 6's **2D Animation Package** allows for skeletal animation of sprites. We create a "Master Rig" containing all possible bones (Head, Torso, Arm\_L, Arm\_R, Leg\_L, Leg\_R, Wing\_L, Wing\_R, Tail).

### **7.2 Runtime Sprite Swapping**

We utilize SpriteLibraryAsset and SpriteResolver components.

* **Asset Structure:** A Library Asset is created with categories corresponding to body parts (e.g., Category: "LeftArm").  
* **Labels:** Within "LeftArm", we have labels: "Human", "Skeleton", "Werewolf", "Mechanical".  
* **Swap Logic:** To equip a "Werewolf Arm," the code executes:  
  C\#  
  spriteResolver.SetCategoryAndLabel("LeftArm", "Werewolf");

  This swaps the visual sprite while maintaining the bone weights and IK targets, allowing the character to perform standard farming animations with a monstrous limb.36

## ---

**8\. Asset Generation Strategy: AI and Clean Room**

Since we cannot use the reference assets, we must generate a massive volume of original art. We will use Generative AI (Midjourney v6) as a force multiplier for the art team.

### **8.1 Prompt Engineering for Pixel Art**

Midjourney v6 is capable of high-quality pixel art generation if prompted correctly.

* **Tilesets:** 16-bit pixel art tileset, top-down game perspective, gothic victorian cobblestone streets, dark moody atmosphere, purple and grey palette, seamless texture, flat 2d \--v 6.0.38  
* **Interiors:** Isometric pixel art room, alchemist lab, potions, skulls, dark wood furniture, dim lighting, detailed, 32-bit style \--ar 16:9.39  
* **Sprites:** Pixel art sprite sheet, gothic vampire character, walking animation, idle animation, attack animation, 4 directions, transparent background, sequential frames \--v 6.0.40

**Processing:** AI outputs are raster images. They must be processed in tools like Aseprite to fix tiling errors, ensure color palette consistency (using a shared palette file), and slice into Unity-ready grids.

## ---

**9\. Development Roadmap**

To ensure a structured Clean-Room implementation, the project is divided into distinct phases.  
**Phase 1: The Foundation (Months 1-3)**

* **Dirty Team:** Analyze reference movement and crop growth formulas. Deliver specifications.  
* **Clean Team:** Set up Unity 6 project with CoreCLR and URP 2D. Implement the Grid system and Rigidbody2D Kinematic controller. Build the TimeManager.

**Phase 2: The Simulation Loop (Months 4-6)**

* **Dirty Team:** Analyze Item/Inventory data structures and Tool interactions.  
* **Clean Team:** Implement CropManager using Job System. Build Inventory UI using UI Toolkit. Create ScriptableObject database for items.

**Phase 3: The Gothic Divergence (Months 7-9)**

* **Clean Team:** Implement GOAP AI for Raid Defense. Build the Lighting System (Day/Night, Normal Maps). Create the Genetic Breeding algorithm logic.  
* **Art Team:** Generate and refine Gothic pixel art assets.

**Phase 4: Polish & Integration (Months 10-12)**

* **Clean Team:** Integrate Dialogue System (using Ink). Finalize Stealth mechanics. Performance profiling and optimization (Memory management, Garbage Collection).  
* **Audit:** Legal review to ensure no code or assets infringe on the reference.

## ---

**10\. Conclusion**

'Havenwood Hollow' represents a technically ambitious project that respects the legal boundaries of game development while pushing the genre forward. By strictly adhering to the **Clean Room** protocol, we neutralize the risk of copyright infringement. By leveraging **Unity 6**, **URP 2D**, and **GOAP AI**, we transform the pastoral farming loop into a tense, atmospheric Gothic survival experience. The detailed specifications provided herein—from the math of fertilizer to the bitwise logic of monster genetics—provide the engineering team with a clear, actionable path to execution. The result will be a familiar yet distinctly unique product, standing on the shoulders of giants without stepping on their toes.

#### **Works cited**

1. WeDias/StardewValley: Decompiled Stardew Valley source ... \- GitHub, accessed February 8, 2026, [https://github.com/WeDias/StardewValley](https://github.com/WeDias/StardewValley)  
2. veywrn/StardewValley: Decompiled Stardew Valley ... \- GitHub, accessed February 8, 2026, [https://github.com/veywrn/StardewValley](https://github.com/veywrn/StardewValley)  
3. Legality of Reverse Engineering & Clean Room Reversing, accessed February 8, 2026, [https://www.retroreversing.com/clean-room-reversing](https://www.retroreversing.com/clean-room-reversing)  
4. accessed December 31, 1969, [https://www.gamedev.net/articles/programming/general-programming/clean-room-design-in-game-development-r5472/](https://www.gamedev.net/articles/programming/general-programming/clean-room-design-in-game-development-r5472/)  
5. OrderObjective.cs \- WeDias/StardewValley \- GitHub, accessed February 8, 2026, [https://github.com/WeDias/StardewValley/blob/main/OrderObjective.cs](https://github.com/WeDias/StardewValley/blob/main/OrderObjective.cs)  
6. In Depth Description on the Maximum Speed Possible in Stardew, accessed February 8, 2026, [https://www.reddit.com/r/StardewValley/comments/1gsfczk/in\_depth\_description\_on\_the\_maximum\_speed/](https://www.reddit.com/r/StardewValley/comments/1gsfczk/in_depth_description_on_the_maximum_speed/)  
7. Stardew valley movement speed \- Webflow, accessed February 8, 2026, [https://uploads-ssl.webflow.com/685c40cc4dc3968b41a2e1cb/68af8d36e221534b6908a6dc\_lijiki.pdf](https://uploads-ssl.webflow.com/685c40cc4dc3968b41a2e1cb/68af8d36e221534b6908a6dc_lijiki.pdf)  
8. Stardew Valley Modding Part 2: Run Faster \- Piffany's Musings, accessed February 8, 2026, [https://piffanysmusings.wordpress.com/2018/12/21/stardew-valley-modding-part-2-run-faster/](https://piffanysmusings.wordpress.com/2018/12/21/stardew-valley-modding-part-2-run-faster/)  
9. How do you get oldschool 2D platformer feel? (Classic physics), accessed February 8, 2026, [https://www.reddit.com/r/Unity2D/comments/uh1zri/how\_do\_you\_get\_oldschool\_2d\_platformer\_feel/](https://www.reddit.com/r/Unity2D/comments/uh1zri/how_do_you_get_oldschool_2d_platformer_feel/)  
10. 2D Movement & Physics?? : r/Unity2D \- Reddit, accessed February 8, 2026, [https://www.reddit.com/r/Unity2D/comments/ebagsf/2d\_movement\_physics/](https://www.reddit.com/r/Unity2D/comments/ebagsf/2d_movement_physics/)  
11. 2D Movement \[Rigidbody vs Transform\] Mastery Tutorial Unity (2021, accessed February 8, 2026, [https://www.youtube.com/watch?v=fcKGqxUuENk](https://www.youtube.com/watch?v=fcKGqxUuENk)  
12. \[Unity\] Useful tips to make the most out of TileMaps. \- Tee, accessed February 8, 2026, [https://killertee.wordpress.com/2023/01/18/unity-useful-tips-to-make-the-most-out-of-tilemaps/](https://killertee.wordpress.com/2023/01/18/unity-useful-tips-to-make-the-most-out-of-tilemaps/)  
13. Unity Tilemaps Best practices \- Game Development Stack Exchange, accessed February 8, 2026, [https://gamedev.stackexchange.com/questions/188658/unity-tilemaps-best-practices](https://gamedev.stackexchange.com/questions/188658/unity-tilemaps-best-practices)  
14. I can't math \- Stardew Valley Forums, accessed February 8, 2026, [https://forums.stardewvalley.net/threads/i-cant-math.9209/](https://forums.stardewvalley.net/threads/i-cant-math.9209/)  
15. Crops \- Stardew Valley Wiki, accessed February 8, 2026, [https://stardewvalleywiki.com/Crops](https://stardewvalleywiki.com/Crops)  
16. Help me understand crop growth speed | Chucklefish Forums, accessed February 8, 2026, [https://community.playstarbound.com/threads/help-me-understand-crop-growth-speed.135702/](https://community.playstarbound.com/threads/help-me-understand-crop-growth-speed.135702/)  
17. crashkonijn/GOAP: A multi-threaded GOAP system for Unity \- GitHub, accessed February 8, 2026, [https://github.com/crashkonijn/GOAP](https://github.com/crashkonijn/GOAP)  
18. GOBT: A Synergistic Approach to Game AI Using Goal-Oriented and, accessed February 8, 2026, [https://www.jmis.org/archive/view\_article?pid=jmis-10-4-321](https://www.jmis.org/archive/view_article?pid=jmis-10-4-321)  
19. Game AI Planning: GOAP, Utility, and Behavior Trees, accessed February 8, 2026, [https://tonogameconsultants.com/game-ai-planning/](https://tonogameconsultants.com/game-ai-planning/)  
20. GOAP \- Goal Oriented Action Planning AI in Unity \- GitHub, accessed February 8, 2026, [https://github.com/matieme/GOAP](https://github.com/matieme/GOAP)  
21. 1\. Getting Started | GOAP, accessed February 8, 2026, [https://goap.crashkonijn.com/readme/tutorial/gettingstarted](https://goap.crashkonijn.com/readme/tutorial/gettingstarted)  
22. New lighting features and workflows in Unity 6 \- YouTube, accessed February 8, 2026, [https://www.youtube.com/watch?v=IpVuIZYFRg4](https://www.youtube.com/watch?v=IpVuIZYFRg4)  
23. Introduction to the 2D lighting system in URP \- Unity \- Manual, accessed February 8, 2026, [https://docs.unity3d.com/6000.1/Documentation/Manual/urp/Lights-2D-intro.html](https://docs.unity3d.com/6000.1/Documentation/Manual/urp/Lights-2D-intro.html)  
24. Unity 2D light with Universal render pipeline \- VionixStudio, accessed February 8, 2026, [https://vionixstudio.com/2021/07/21/unity-2d-lights-tutorial/](https://vionixstudio.com/2021/07/21/unity-2d-lights-tutorial/)  
25. 2D Day/Night Cycles : r/gamedev \- Reddit, accessed February 8, 2026, [https://www.reddit.com/r/gamedev/comments/7cti1q/2d\_daynight\_cycles/](https://www.reddit.com/r/gamedev/comments/7cti1q/2d_daynight_cycles/)  
26. Enhance a top-down game with URP 2D lights | Unity at GDC 2023, accessed February 8, 2026, [https://www.youtube.com/watch?v=YhrwKF\_i-BI](https://www.youtube.com/watch?v=YhrwKF_i-BI)  
27. Blend Modes \- Unity \- Manual, accessed February 8, 2026, [https://docs.unity3d.com/6000.3/Documentation/Manual/urp/2d-light-blend-modes.html](https://docs.unity3d.com/6000.3/Documentation/Manual/urp/2d-light-blend-modes.html)  
28. How to design to stealth around invisible enemies? Horror game, accessed February 8, 2026, [https://www.reddit.com/r/gamedesign/comments/pbdmfk/how\_to\_design\_to\_stealth\_around\_invisible\_enemies/](https://www.reddit.com/r/gamedesign/comments/pbdmfk/how_to_design_to_stealth_around_invisible_enemies/)  
29. Ways that Mummy Could Become Fun : r/dcss \- Reddit, accessed February 8, 2026, [https://www.reddit.com/r/dcss/comments/vmx49l/ways\_that\_mummy\_could\_become\_fun/](https://www.reddit.com/r/dcss/comments/vmx49l/ways_that_mummy_could_become_fun/)  
30. Create 2D Invisibility easily with Unity Shader Graph \- YouTube, accessed February 8, 2026, [https://www.youtube.com/watch?v=2ydpOQ6e2Cg](https://www.youtube.com/watch?v=2ydpOQ6e2Cg)  
31. Is there a way to hide a player who is in the shadow in Unity 2D, accessed February 8, 2026, [https://stackoverflow.com/questions/65696260/is-there-a-way-to-hide-a-player-who-is-in-the-shadow-in-unity-2d-light-system](https://stackoverflow.com/questions/65696260/is-there-a-way-to-hide-a-player-who-is-in-the-shadow-in-unity-2d-light-system)  
32. c\# \- Creating a bit mask for relevant bits over multiple bytes, accessed February 8, 2026, [https://stackoverflow.com/questions/50196897/creating-a-bit-mask-for-relevant-bits-over-multiple-bytes-programmatically](https://stackoverflow.com/questions/50196897/creating-a-bit-mask-for-relevant-bits-over-multiple-bytes-programmatically)  
33. Using a bitmask in C\# \- Stack Overflow, accessed February 8, 2026, [https://stackoverflow.com/questions/3261451/using-a-bitmask-in-c-sharp](https://stackoverflow.com/questions/3261451/using-a-bitmask-in-c-sharp)  
34. Inheritance Mechanics for Creature Generation : r/gamedesign, accessed February 8, 2026, [https://www.reddit.com/r/gamedesign/comments/t8dqsu/inheritance\_mechanics\_for\_creature\_generation/](https://www.reddit.com/r/gamedesign/comments/t8dqsu/inheritance_mechanics_for_creature_generation/)  
35. How would i implement a genetic traits system : r/unrealengine, accessed February 8, 2026, [https://www.reddit.com/r/unrealengine/comments/1dqcy4i/how\_would\_i\_implement\_a\_genetic\_traits\_system/](https://www.reddit.com/r/unrealengine/comments/1dqcy4i/how_would_i_implement_a_genetic_traits_system/)  
36. How to SWAP your Character\! (Change Mesh Visual ... \- YouTube, accessed February 8, 2026, [https://www.youtube.com/watch?v=AO1vw-b8Qzw](https://www.youtube.com/watch?v=AO1vw-b8Qzw)  
37. Unity 2D Swappable Rig\! \- YouTube, accessed February 8, 2026, [https://www.youtube.com/watch?v=hUERvxcKt\_I](https://www.youtube.com/watch?v=hUERvxcKt_I)  
38. Best Midjourney Prompt for Pixel Art \- Art & Design, accessed February 8, 2026, [https://www.godofprompt.ai/mj-art-design/pixel-art-06d08](https://www.godofprompt.ai/mj-art-design/pixel-art-06d08)  
39. Pixel Art Retro Rooms (Prompts Included) : r/midjourney \- Reddit, accessed February 8, 2026, [https://www.reddit.com/r/midjourney/comments/1izjexg/pixel\_art\_retro\_rooms\_prompts\_included/](https://www.reddit.com/r/midjourney/comments/1izjexg/pixel_art_retro_rooms_prompts_included/)  
40. The Best 25 Midjourney Prompts for Pixel Art \- OpenArt, accessed February 8, 2026, [https://openart.ai/blog/post/midjourney-prompts-for-pixel-art](https://openart.ai/blog/post/midjourney-prompts-for-pixel-art)
