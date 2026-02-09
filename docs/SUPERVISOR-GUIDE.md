# Human-in-the-Loop Supervisor Guide

## The "Built by AI" Experiment — HavenWoodHollow

**Last updated:** 2026-02-09
**Audience:** The human supervisor overseeing Claude Cowork and Claude Code during in-person development sessions.

---

## 1. Your Role

You are the **safety checkpoint** between Claude's AI capabilities and the live Unity project. Claude (via Cowork or Code) can read, write, and modify files across the entire project — but it **cannot** run Unity, test gameplay, or verify visual correctness. That's your job.

### What You Verify

| Check | Why |
|---|---|
| **Does the game compile?** | Claude can write C# scripts but cannot run the Unity compiler. You must check the Console for errors after each change. |
| **Does the scene look right?** | Claude can create/modify GameObjects but cannot see the Game or Scene view. You confirm visual correctness. |
| **Does gameplay feel correct?** | Claude can set movement speeds, physics values, and timers but cannot play-test. You click Play and verify. |
| **Are assets in the right place?** | Claude can create folders and move files but cannot see the Project window. You confirm organization. |
| **Is the Git state clean?** | Claude commits via tools. You verify the PR diff is minimal and expected before merging. |

---

## 2. What Claude CAN Do (via MCP Plugin)

The MCP plugin gives Claude the following **verified, working capabilities** (commands with Unity-side handlers implemented):

### Fully Working Commands

| Category | Command | What It Does |
|---|---|---|
| **Connection** | `unity_connect` | Connects to running Unity Editor on port 8090 |
| **Connection** | `unity_disconnect` | Disconnects from Unity |
| **Connection** | `unity_status` | Checks connection status |
| **Scene** | `unity_scene_create` | Creates a new scene |
| **Scene** | `unity_scene_load` | Loads an existing scene |
| **Scene** | `unity_scene_save` | Saves the current scene |
| **Scene** | `unity_scene_info` | Gets info about the active scene |
| **Scene** | `unity_hierarchy_view` | Lists all GameObjects in the scene |
| **Scene** | `unity_play_mode` | Enter/exit/pause Play Mode |
| **GameObject** | `unity_gameobject_create` | Creates GameObjects (primitives, empty) |
| **GameObject** | `unity_gameobject_find` | Finds GameObjects by name/tag/component |
| **GameObject** | `unity_gameobject_modify` | Changes position, rotation, scale, name, active state |
| **GameObject** | `unity_gameobject_delete` | Deletes a GameObject (with Undo support) |
| **GameObject** | `unity_gameobject_duplicate` | Duplicates a GameObject |
| **Component** | `unity_component_add` | Adds a component (Rigidbody, Collider, custom scripts) |
| **Component** | `unity_component_modify` | Modifies component properties via reflection |
| **Component** | `unity_component_remove` | Removes a component |
| **Component** | `unity_component_get` | Reads component properties |
| **Script** | `unity_script_create` | Creates new C# files with templates |
| **Script** | `unity_script_read` | Reads script file contents |
| **Script** | `unity_script_edit` | Edits script files (full replace) |
| **Script** | `unity_script_delete` | Deletes script files |
| **Script** | `unity_console_clear` | Clears the Unity Console |
| **Asset** | `unity_asset_list` | Lists assets in a folder |
| **Asset** | `unity_asset_search` | Searches assets by name/type |
| **Asset** | `unity_folder_create` | Creates folders in Assets |
| **Asset** | `unity_asset_refresh` | Refreshes the AssetDatabase |
| **Prefab** | `unity_prefab_create` | Saves a GameObject as a prefab |
| **Prefab** | `unity_prefab_instantiate` | Instantiates a prefab in the scene |
| **Build** | `unity_build` | Builds the project for a target platform |
| **Build** | `unity_build_settings` | Gets current build settings |
| **Build** | `unity_switch_platform` | Switches the active build target |
| **Project** | `unity_project_info` | Gets project name, path, Unity version |

### Total: ~33 working commands

---

## 3. What Claude CANNOT Do

### Commands That Will Fail (Missing Unity Handlers)

These tools are defined on the MCP server but have **no Unity-side implementation**. If Claude tries to use them, Unity will return "Unknown method" errors.

| Command | Reason It Fails |
|---|---|
| `unity_asset_import` | No `importAsset` handler |
| `unity_asset_move` | No `moveAsset` handler |
| `unity_asset_delete` | No `deleteAsset` handler |
| `unity_asset_duplicate` | No `duplicateAsset` handler |
| `unity_asset_info` | No `getAssetInfo` handler |
| `unity_script_search` | No `searchScripts` handler |
| `unity_script_analyze` | No `analyzeScript` handler |
| `unity_prefab_unpack` | No `unpackPrefab` handler |
| `unity_material_create` | No `createMaterial` handler |
| `unity_scene_list` | No `listScenes` handler |
| `unity_console_read` | Returns empty array (stub) |
| `unity_execute_code` | Disabled for security (returns error) |
| `unity_packages_list` | No `listPackages` handler |
| `unity_package_add` | No `addPackage` handler |
| `unity_package_remove` | No `removePackage` handler |
| `unity_tags_layers` | No `tagsAndLayers` handler |
| `unity_input_settings` | No `inputSettings` handler |
| `unity_physics_settings` | No `physicsSettings` handler |
| `unity_time_settings` | No `timeSettings` handler |
| `unity_editor_preferences` | No `editorPreferences` handler |
| `unity_quality_settings` | No `qualitySettings` handler |
| `unity_player_settings` | No `playerSettings` handler |
| `unity_addressables` | No `addressables` handler |
| `unity_undo` | No `undoOperations` handler |
| `unity_selection` | No `selection` handler |

### Fundamental Limitations

| Limitation | Explanation | Supervisor Action |
|---|---|---|
| **Cannot see the screen** | Claude has zero visual feedback from Unity. It cannot see the Scene view, Game view, Inspector, or Console. | Describe what you see. Copy-paste error messages. Take screenshots and describe them. |
| **Cannot play-test** | Claude can enter Play Mode but cannot observe or interact with the running game. | You play-test and report results. "Player moves correctly" or "NPC got stuck at (x,y)". |
| **Cannot create visual assets** | Claude cannot produce sprites, 3D models, textures, or audio files. | Provide placeholder art or use asset generation tools (Stable Diffusion, DALL-E). |
| **Cannot configure Unity Preferences/Settings** | Most settings handlers are unimplemented. | You manually adjust Physics, Quality, Input, Tags, Layers in the Unity Editor. |
| **Cannot install Unity packages** | Package Manager handlers not implemented. | You install packages via `Window > Package Manager` in Unity. |
| **Cannot undo** | The `unity_undo` handler is not implemented. | Use `Ctrl+Z` / `Cmd+Z` in Unity to undo Claude's changes. All operations register Undo. |
| **Cannot read console logs** | The `getConsoleLogs` handler is a stub. | Copy-paste console errors to Claude manually. |
| **Cannot manage materials** | `createMaterial` handler not implemented. | Create materials manually or ask Claude to create material via `unity_script_create` (workaround). |

---

## 4. Typical Workflow

### Session Start

1. **Open Unity** with the HavenWoodHollow project.
2. **Verify MCP Bridge** is running: check Unity console for `[MCP Bridge] Initialized on port 8090`.
3. **Start MCP Server**: Run `cd unity-mcp-plugin/server && npm start` (or use `scripts/start-server.sh`).
4. **Open Claude** (Desktop, Cowork, or Code) and begin the session.
5. **Tell Claude to connect**: "Connect to Unity" → Claude calls `unity_connect`.

### During Development

1. **Claude writes code** → Scripts auto-compile in Unity → **You check for compile errors**.
2. **Claude creates GameObjects** → **You verify in Scene view** (position, components, hierarchy).
3. **Claude modifies scenes** → **You save the scene** (Claude may forget, or you may prefer manual save).
4. **Claude reports completion** → **You play-test** and report results back to Claude.
5. **If errors occur** → **Copy-paste the error message** to Claude. It will fix the code.

### Session End

1. **Save all scenes** in Unity.
2. **Review the Git diff** for Claude's changes. Verify scope is minimal and expected.
3. **Approve or request changes** on the PR.

---

## 5. Troubleshooting

### "Not connected to Unity"
- Is Unity running with the HavenWoodHollow project open?
- Check Unity console for `[MCP Bridge] Initialized on port 8090`.
- Try: `unity_connect` with port 8090.

### "Unknown method: XXX"
- Claude tried to use a command that has no Unity-side handler (see Section 3).
- **Workaround:** Perform the action manually in Unity and tell Claude it's done.

### Unity domain reload disconnects Claude
- This is expected! When scripts recompile, Unity reloads. The MCP Bridge automatically restarts via `[InitializeOnLoad]` and the server auto-reconnects via polling.
- Wait 5-10 seconds after compilation. Claude will resume automatically.

### Script compilation errors
- Claude cannot see the Console. **Copy the full error message** and paste it to Claude.
- Claude will modify the script to fix the error and trigger another recompile.

### Performance issues
- If Unity becomes sluggish, Claude may be sending too many rapid requests.
- Tell Claude to pause and process one operation at a time.

---

## 6. What's Ready for Sprint 1

### Fully Operational Systems (Code Complete)

These game systems have complete C# implementations and are ready for Unity Editor setup:

- ✅ **Player Movement** — Kinematic RB2D with buff stacking (coffee, horse, terrain)
- ✅ **Player Stats** — Health, stamina, regen, events, IDamageable
- ✅ **Player Interaction** — Physics2D.OverlapCircle detection
- ✅ **Inventory System** — 36 slots, stacking, hotbar, events
- ✅ **Farming System** — Tilemap-based crop storage, growth phases, watering
- ✅ **Creature Breeding** — Punnett square genetics, mutation, trait expression
- ✅ **GOAP AI** — Full planner, 8 action types, WorldState, agent framework
- ✅ **NPC System** — Schedules, movement, state machine
- ✅ **Raid System** — Wave spawning, escalation, night raids
- ✅ **Crafting** — Recipe management, station filtering, ingredient refund
- ✅ **Economy/Shops** — Buy/sell, daily stock rotation
- ✅ **Dialogue** — Typewriter effect, branching choices
- ✅ **Quest System** — Multi-objective tracking, prerequisites, rewards
- ✅ **Save/Load** — JSON-based persistence for player, calendar, inventory
- ✅ **Time/Seasons** — Day/night cycle, season transitions, sleep events
- ✅ **Audio** — Music crossfade, SFX pooling, spatial audio
- ✅ **Stealth** — Light2D-based detection, stealth meter
- ✅ **Visual FX** — Distortion shader controller, footprint particles
- ✅ **Character Customization** — Body part swapper system

### Requires Human Setup in Unity Editor

| Task | Why Claude Can't Do It |
|---|---|
| Create ScriptableObject instances (ItemData, CropData, CreatureData, etc.) | Requires Inspector interaction |
| Set up Tilemap for farming grid | Visual/spatial task |
| Configure URP 2D Renderer and Light2D | Settings panel navigation |
| Import/create placeholder sprites | Cannot generate images |
| Wire up UI Canvas elements | Visual layout task |
| Set up Input Actions asset | Unity Input System editor UI |
| Create animation controllers | Animator window interaction |
| Configure sorting layers and physics layers | Tags & Layers settings (no handler) |

---

## 7. Risk Register

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| Claude writes code that doesn't compile | Medium | Low | You see errors immediately. Claude fixes them when you report back. |
| Claude creates GameObjects in wrong positions | Medium | Low | You visually verify and either correct or ask Claude to adjust. |
| Claude misunderstands a design requirement | Medium | Medium | Provide clear, specific instructions. Reference Plan.md section numbers. |
| TCP connection drops during session | Low | Low | Auto-reconnect handles this. Wait 5-10 seconds. |
| Claude modifies files outside its scope | Low | High | Review every PR diff before merging. Use branch protection rules. |
| Data loss from incorrect script edits | Low | Medium | All operations have Undo support. Use Ctrl+Z immediately if something goes wrong. |

---

## 8. Communication Tips for Working with Claude

### Be Specific
❌ "Make the player move"
✅ "Set the PlayerController walkSpeed to 3.0 and runSpeed to 6.0, then enter Play Mode so I can test"

### Report What You See
❌ "It doesn't work"
✅ "The player moves but gets stuck on the left wall. Console shows: `MissingComponentException: Rigidbody2D on Player`"

### Reference the Plan
❌ "Add the farming system"
✅ "Following Plan.md Section 3.3.2, create a CropManager with a Dictionary<Vector2Int, CropState> for the tilemap grid"

### One Task at a Time
Claude works best with clear, atomic tasks. Don't ask it to "set up the whole game" — instead, walk through the workflow step by step.

---

## 9. Emergency Procedures

### If Claude Makes a Destructive Change
1. **Press Ctrl+Z** (Cmd+Z on Mac) repeatedly in Unity — all MCP operations support Undo.
2. If files were modified on disk, use `git checkout -- <file>` to restore from the last commit.
3. If an entire scene is corrupted, reload from the last saved version.

### If the MCP Bridge Crashes
1. Check Unity console for `[MCP Bridge] Listener error: ...`.
2. Restart Unity (the bridge auto-starts via `[InitializeOnLoad]`).
3. In the MCP server terminal, restart with `npm start`.

### If Claude Enters an Error Loop
1. Tell Claude to stop and explain the current state.
2. Provide the exact error message.
3. If Claude keeps making the same mistake, manually fix the issue and tell Claude what you did.
