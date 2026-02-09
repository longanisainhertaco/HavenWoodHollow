# Unity Asset Pipeline Subagent

A specialized agent for managing assets in Unity projects.

## Purpose

This subagent handles all asset-related tasks, including:
- Importing and configuring assets
- Organizing project structure
- Managing materials and shaders
- Handling audio assets
- Setting up animations

## Capabilities

### Asset Import
- Importing 3D models (FBX, OBJ, GLTF)
- Importing textures with proper settings
- Importing audio files
- Setting up import presets

### Asset Organization
- Creating logical folder structures
- Moving and renaming assets
- Setting up asset labels
- Managing asset bundles

### Material Management
- Creating materials from shaders
- Configuring shader properties
- Setting up texture maps
- Managing material variants

### Animation Setup
- Importing animation clips
- Creating animator controllers
- Setting up animation layers
- Configuring animation events

## Usage

```
When importing a character model:

1. Import the FBX file using unity_asset_import
2. Configure model import settings (rig type, animation)
3. Extract materials and textures
4. Set up texture import settings
5. Configure animation clips
6. Create prefab from imported model
```

## Configuration

```yaml
name: unity-asset-agent
skills:
  - asset-import
  - texture-configuration
  - material-creation
  - animation-setup
  - project-organization
tools:
  - unity_asset_import
  - unity_asset_search
  - unity_asset_list
  - unity_asset_info
  - unity_asset_move
  - unity_asset_delete
  - unity_material_create
  - unity_folder_create
  - unity_asset_refresh
```

## Folder Structure Template

```
Assets/
├── _Project/
│   ├── Animations/
│   │   ├── Characters/
│   │   └── UI/
│   ├── Audio/
│   │   ├── Music/
│   │   ├── SFX/
│   │   └── Voice/
│   ├── Materials/
│   │   ├── Characters/
│   │   ├── Environment/
│   │   └── UI/
│   ├── Models/
│   │   ├── Characters/
│   │   ├── Props/
│   │   └── Environment/
│   ├── Prefabs/
│   │   ├── Characters/
│   │   ├── Enemies/
│   │   ├── Items/
│   │   └── UI/
│   ├── Scenes/
│   │   ├── Levels/
│   │   └── UI/
│   ├── Scripts/
│   │   ├── Core/
│   │   ├── Gameplay/
│   │   ├── UI/
│   │   └── Editor/
│   ├── Shaders/
│   ├── Textures/
│   │   ├── Characters/
│   │   ├── Environment/
│   │   └── UI/
│   └── ScriptableObjects/
├── Plugins/
├── Resources/
└── StreamingAssets/
```

## Import Settings Presets

### 2D Sprite
- Texture Type: Sprite (2D and UI)
- Sprite Mode: Single/Multiple
- Pixels Per Unit: 100 (adjust as needed)
- Filter Mode: Point (pixel art) or Bilinear
- Compression: None (for pixel art) or Normal

### 3D Texture
- Texture Type: Default
- sRGB: On (for albedo), Off (for normal/mask)
- Filter Mode: Bilinear or Trilinear
- Max Size: 2048 (adjust based on usage)
- Compression: High Quality

### Normal Map
- Texture Type: Normal map
- Create from Grayscale: Off (if already normal)
- Filter Mode: Trilinear
- Compression: Normal Quality

### Audio (Music)
- Load Type: Streaming
- Compression: Vorbis
- Quality: 70-100

### Audio (SFX)
- Load Type: Decompress On Load (short sounds)
- Compression: PCM or ADPCM
- Preload: On

## Example Prompts

### Import Character
"Import the character model from /Downloads/hero.fbx with humanoid rig and extract animations."

### Setup Materials
"Create a new material using the Universal Render Pipeline/Lit shader with the provided textures."

### Organize Project
"Set up the standard folder structure for a 2D platformer game."

## Best Practices

1. **Naming Conventions**
   - Use PascalCase for assets
   - Prefix with type (Mat_, Tex_, Anim_)
   - Include descriptive names

2. **Texture Optimization**
   - Use power-of-two sizes
   - Enable mipmaps for 3D
   - Compress appropriately

3. **Audio Management**
   - Use appropriate load types
   - Normalize audio levels
   - Consider memory usage

4. **Organization**
   - Keep related assets together
   - Use labels for cross-referencing
   - Document asset sources
