# Tanx Clone - Unity Recreation

A complete recreation of the classic Amiga game **Tanx** (1991) by Gary Roberts, built in Unity with retro pixel art styling.

## About the Original

Tanx is a classic 2-player turn-based artillery game where players control tanks on a randomly generated landscape and attempt to destroy each other using angle and velocity-based projectile physics.

The original Amiga version featured:
- Randomly generated landscapes
- Variable gravity and wind physics
- Interactive objects affecting projectile flight
- Full stereo sound
- 50 FPS screen updates
- Over 100 colors on screen with parallax scrolling

## Features

This Unity recreation includes **all original features**:

### Core Gameplay
- ✅ 2-player turn-based combat
- ✅ Angle control (-90° to 150°)
- ✅ Velocity control (0-199)
- ✅ Realistic projectile physics
- ✅ Destructible terrain

### Landscape Generation
- ✅ **Mountains** - Steep peaks with large valleys
- ✅ **Foothills** - Shallow valleys with small hills
- ✅ **Random** - Completely randomized terrain

### Physics Systems
- ✅ **Gravity** - Light, Medium, Strong, or Random
- ✅ **Wind** - None, Light, Medium, Strong, or Random strength
- ✅ **Wind Direction** - Fixed per game or randomly changing

### Interactive Objects
- ✅ **Targets** - Destroyable for bonus shots
- ✅ **Fans** - Blow projectiles in their facing direction
- ✅ **Pushers** - Push projectiles upward
- ✅ **Pullers** - Pull projectiles downward

### Visual & Audio
- ✅ Retro pixel art style
- ✅ Stereo audio with positional panning
- ✅ Explosion effects
- ✅ Camera scrolling across 2-screen-wide landscape
- ✅ Smooth animations

### Game Features
- ✅ Full options/settings menu
- ✅ Player name customization (3 characters)
- ✅ Score tracking across multiple games
- ✅ Status screen with match results
- ✅ All original game parameters

## Project Structure

```
Assets/
├── Scripts/
│   ├── GameConfig.cs                 # Game settings and enums
│   ├── CameraController.cs           # Camera scrolling system
│   ├── Terrain/
│   │   └── TerrainGenerator.cs       # Random terrain generation
│   ├── Tanks/
│   │   └── TankController.cs         # Tank movement and aiming
│   ├── Projectiles/
│   │   └── Projectile.cs             # Projectile physics
│   ├── Objects/
│   │   └── InteractiveObject.cs      # Targets, Fans, Pushers, Pullers
│   ├── Managers/
│   │   └── GameManager.cs            # Core game logic
│   ├── UI/
│   │   ├── MainMenuUI.cs             # Main menu and options
│   │   ├── GameHUD.cs                # In-game controls
│   │   └── StatusScreen.cs           # Results screen
│   ├── Audio/
│   │   └── AudioManager.cs           # Sound system
│   └── Effects/
│       └── ExplosionEffect.cs        # Visual effects
├── Sprites/                          # Pixel art assets
├── Audio/                            # Sound effects
├── Prefabs/                          # Game object prefabs
├── Scenes/                           # Unity scenes
└── Materials/                        # Materials for terrain, etc.
```

## Unity Setup Instructions

### Requirements
- **Unity Version**: 2021.3 LTS or newer
- **TextMeshPro**: Included in Unity (will auto-import on first use)
- **LeanTween**: Optional for smooth animations (can be replaced with Unity's built-in tweening)

### Initial Setup

1. **Open the project in Unity**
   - File → Open Project
   - Select the `TanxClone` folder

2. **Import TextMeshPro**
   - When prompted, import TMP Essentials
   - Window → TextMeshPro → Import TMP Essential Resources

3. **Create the Main Scene**
   - Create new scene: `Assets/Scenes/MainScene.unity`

4. **Set up the Game Manager**
   - Create empty GameObject: `GameManager`
   - Add `GameManager.cs` component
   - Create and assign prefabs (see below)

5. **Create Prefabs** (detailed instructions below)
   - Tank Prefab
   - Projectile Prefab
   - Interactive Object Prefabs (Target, Fan, Pusher, Puller)
   - Explosion Prefab

6. **Set up the Camera**
   - Add `CameraController.cs` to Main Camera
   - Set camera to Orthographic
   - Set size to 300

7. **Create UI Canvas**
   - Create Canvas (Screen Space - Overlay)
   - Add UI components for MainMenuUI, GameHUD, StatusScreen

## Creating Prefabs

### Tank Prefab

1. Create empty GameObject: `Tank`
2. Add components:
   - `TankController.cs`
   - `BoxCollider2D` (is Trigger)
   - Tag: "Tank"
3. Create child objects:
   - `Body` (SpriteRenderer for tank body)
   - `Barrel` (empty GameObject for pivot point)
     - `BarrelSprite` (SpriteRenderer, child of Barrel)
   - `FirePoint` (empty GameObject, positioned at barrel tip)
4. Assign references in TankController

### Projectile Prefab

1. Create GameObject: `Projectile`
2. Add components:
   - `Rigidbody2D` (Kinematic, no gravity)
   - `CircleCollider2D` (is Trigger, radius: 2)
   - `TrailRenderer` (for visual trail)
   - `Projectile.cs`
   - Tag: "Projectile"
3. Configure TrailRenderer with desired look

### Interactive Object Prefabs

**Target:**
1. GameObject: `Target`
2. Components: `SpriteRenderer`, `BoxCollider2D` (Trigger)
3. Add `Target` component (from InteractiveObject.cs)
4. Tag: "Target"

**Fan:**
1. GameObject: `Fan`
2. Child: `FanBlades` (will rotate)
3. Components: `SpriteRenderer`, `Fan` component
4. Create sprite showing direction arrow

**Pusher:**
1. GameObject: `Pusher`
2. Components: `SpriteRenderer`, `Pusher` component
3. Red pyramid sprite

**Puller:**
1. GameObject: `Puller`
2. Components: `SpriteRenderer`, `Puller` component
3. Jaw sprite

### Explosion Prefab

1. GameObject: `Explosion`
2. Components:
   - `SpriteRenderer` (explosion sprite/circle)
   - `ParticleSystem` (optional, for debris)
   - `ExplosionEffect.cs`
3. Configure particle system for debris/smoke

## Creating the Terrain

The terrain is generated at runtime, but you need a GameObject:

1. Create empty GameObject: `Terrain`
2. Add components:
   - `MeshFilter`
   - `MeshRenderer`
   - `PolygonCollider2D`
   - `TerrainGenerator.cs`
3. Create material for terrain (simple colored material)
4. Assign to TerrainGenerator

## UI Setup

### Main Menu Canvas

Create UI hierarchy:
```
Canvas
├── TitlePanel
│   └── TitleImage
├── MainMenuPanel
│   ├── PlayerInputs
│   │   ├── Player1Input (TMP_InputField)
│   │   └── Player2Input (TMP_InputField)
│   ├── OptionsGroup
│   │   ├── WindSettings (Toggle Group)
│   │   ├── GravitySettings (Toggle Group)
│   │   ├── LandscapeSettings (Toggle Group)
│   │   └── ObjectToggles
│   └── Buttons
│       ├── PlayButton
│       ├── AboutButton
│       └── QuitButton
└── AboutPanel
```

Attach `MainMenuUI.cs` and wire up all references.

### Game HUD Canvas

```
Canvas
└── ControlPanel
    ├── PlayerDisplay
    ├── AngleControls (Slider + Buttons + Text)
    ├── VelocityControls (Slider + Buttons + Text)
    ├── ActionButtons (Fire, Quit)
    ├── TankMovement (Left/Right buttons)
    ├── CameraScroll (Arrow buttons)
    └── WindDisplay (Text + Arrow + Meter)
```

Attach `GameHUD.cs` and wire up references.

### Status Screen Canvas

```
Canvas
└── StatusPanel
    ├── WinnerDisplay
    ├── ScoreDisplay
    │   ├── Player1Score
    │   └── Player2Score
    └── Buttons
        ├── PlayAgainButton
        └── MainMenuButton
```

Attach `StatusScreen.cs` and wire up references.

## Creating Retro Pixel Art Assets

### Recommended Sprite Sizes
- **Tank Body**: 32x24 pixels
- **Tank Barrel**: 24x4 pixels
- **Projectile**: 6x6 pixels
- **Target**: 16x24 pixels
- **Fan**: 24x24 pixels
- **Pusher**: 20x20 pixels (pyramid)
- **Puller**: 24x16 pixels (jaws)
- **Explosion**: 64x64 pixels

### Sprite Import Settings
- Texture Type: Sprite (2D and UI)
- Pixels Per Unit: 16 (for pixel perfect rendering)
- Filter Mode: Point (no filter)
- Compression: None
- Max Size: 256 or 512

### Color Palette (Retro Amiga Style)
- Use limited color palette (16-32 colors)
- Primary colors: Blues, Reds, Greens, Browns
- High contrast for visibility

## Audio Setup

1. **Import or create sound effects:**
   - Fire sound (shooting)
   - Explosion sound
   - Target hit sound
   - Tank movement sound
   - Menu click sound
   - Wind ambient sound (looping)

2. **Audio Import Settings:**
   - Load Type: Decompress On Load (for short SFX)
   - Compression Format: Vorbis or PCM
   - Quality: Medium to High

3. **Assign to AudioManager:**
   - Drag clips to AudioManager component in scene

## Build Settings

1. File → Build Settings
2. Add scene: `Assets/Scenes/MainScene`
3. Platform: PC/Mac/Linux Standalone
4. Target: Your preferred platform
5. Build and Run

## Controls

### In-Game
- **Angle**: Use slider or arrow buttons
- **Velocity**: Use slider or arrow buttons
- **Fire**: Click Fire button
- **Move Tank**: Left/Right buttons
- **Scroll Camera**: Arrow buttons or keyboard arrows
- **Quit**: White flag button

### Camera
- **Arrow Keys / WASD**: Scroll camera
- **Mouse edges**: Edge scrolling (optional)

## Game Tips

From the original documentation:

1. Start with default settings (no wind, medium gravity, foothills)
2. Try tunneling by firing at -4° or -5° angle with 100+ velocity near mountains
3. For hardest difficulty, enable all options and set everything to Random
4. Watch the wind meter before each shot
5. Targets grant bonus shots when destroyed

## Credits

**Original Game:**
- Gary Roberts (1991)
- Written for Amiga Computing

**Unity Recreation:**
- Based on original Amiga Tanx game mechanics
- Recreated from original documentation

## License

This is a fan recreation for educational purposes. Original game concept and mechanics © Gary Roberts 1991.

## Next Steps

1. **Create all sprites** in your preferred pixel art tool (Aseprite, Photoshop, etc.)
2. **Import audio files** for sound effects
3. **Build all prefabs** as described above
4. **Set up UI canvases** with proper layout
5. **Wire up all references** in inspector
6. **Test and iterate** on gameplay balance

Enjoy recreating this classic game!
