# Quick Start Guide - Tanx Clone

This guide will help you get the Tanx Clone Unity project up and running quickly.

## Prerequisites

- Unity 2021.3 LTS or newer
- Basic Unity knowledge
- A pixel art tool (optional - for creating custom sprites)

## Step-by-Step Setup

### 1. Open Project in Unity

1. Launch Unity Hub
2. Click "Open" or "Add"
3. Navigate to the `TanxClone` folder
4. Select the folder and open

### 2. Import TextMeshPro

- If prompted, import TMP Essentials
- If not prompted: Window → TextMeshPro → Import TMP Essential Resources

### 3. Project Overview

All C# scripts are complete and located in `Assets/Scripts/`:

**Core Systems:**
- `GameConfig.cs` - Game settings and constants
- `GameManager.cs` - Main game controller
- `TerrainGenerator.cs` - Procedural terrain generation
- `TankController.cs` - Tank behavior
- `Projectile.cs` - Projectile physics
- `InteractiveObject.cs` - Targets, Fans, Pushers, Pullers
- `CameraController.cs` - Camera scrolling
- `AudioManager.cs` - Sound system

**UI Scripts:**
- `MainMenuUI.cs` - Main menu and options
- `GameHUD.cs` - In-game controls
- `StatusScreen.cs` - Results screen

**Effects:**
- `ExplosionEffect.cs` - Explosion visuals

### 4. What You Need to Create

The scripts are complete, but you'll need to create the Unity scene assets:

#### Required GameObjects & Prefabs

**Tank Prefab** (`Assets/Prefabs/Tank.prefab`):
- GameObject hierarchy:
  ```
  Tank (TankController, BoxCollider2D)
  ├── Body (SpriteRenderer)
  ├── Barrel (empty pivot)
  │   └── BarrelSprite (SpriteRenderer)
  └── FirePoint (empty marker at barrel tip)
  ```
- Tag as "Tank"
- Assign references in TankController inspector

**Projectile Prefab** (`Assets/Prefabs/Projectile.prefab`):
- Components: Rigidbody2D (kinematic), CircleCollider2D (trigger), TrailRenderer, Projectile script
- Tag as "Projectile"
- Set explosion radius to 30

**Interactive Objects** (`Assets/Prefabs/`):
- `Target.prefab` - Add Target component
- `Fan.prefab` - Add Fan component with FanBlades child
- `Pusher.prefab` - Add Pusher component
- `Puller.prefab` - Add Puller component

**Explosion Prefab** (`Assets/Prefabs/Explosion.prefab`):
- SpriteRenderer + ParticleSystem (optional)
- ExplosionEffect component
- Lifetime: 1 second

#### Main Scene Setup

**Create Scene** (`Assets/Scenes/MainScene.unity`):

1. **GameManager** (empty GameObject)
   - Add GameManager component
   - Assign all prefabs in inspector

2. **Terrain** (empty GameObject)
   - Add: MeshFilter, MeshRenderer, PolygonCollider2D, TerrainGenerator
   - Create and assign a material (any solid color)
   - Assign terrain material in TerrainGenerator

3. **Main Camera**
   - Add CameraController component
   - Set to Orthographic
   - Size: 300
   - Background: Sky blue or black

4. **AudioManager** (empty GameObject)
   - Add AudioManager component
   - (Optional) Import and assign audio clips

5. **UI Canvas** (Canvas - Screen Space Overlay)
   - Create three child panels:
     - MainMenuPanel (attach MainMenuUI.cs)
     - GameHUDPanel (attach GameHUD.cs)
     - StatusPanel (attach StatusScreen.cs)

### 5. Minimal UI Setup

For testing without full UI:

**Simple MainMenuUI:**
- Panel with "Play" button
- Wire Play button to MainMenuUI script
- Two InputFields for player names (optional, will use defaults)

**Simple GameHUD:**
- Panel with:
  - Angle slider (range: -90 to 150)
  - Velocity slider (range: 0 to 199)
  - Fire button
  - Left/Right tank movement buttons
- Wire all to GameHUD script

**Simple StatusScreen:**
- Panel with:
  - Winner text
  - Play Again button
  - Main Menu button

### 6. Sprites (Placeholder or Pixel Art)

**Quick Placeholder Sprites:**

You can use Unity's built-in sprites temporarily:
- Tank Body: Create a blue/red square sprite (32x24)
- Tank Barrel: Create a small rectangle (24x4)
- Projectile: Create a small circle (6x6)
- Target: Create a circle or crosshair
- Fan: Create an arrow
- Pusher: Create a triangle pointing up
- Puller: Create a triangle pointing down
- Explosion: Create a red circle

**Sprite Import Settings:**
- Filter Mode: Point (no filter) for pixel art
- Compression: None
- Pixels Per Unit: 16

### 7. Testing

1. Press Play in Unity
2. Click title screen to see main menu
3. Click Play to start game
4. Use sliders to adjust angle and velocity
5. Click Fire to shoot
6. Test all features

### 8. Optional Enhancements

**Audio:**
- Import free sound effects from freesound.org
- Assign to AudioManager

**Better Graphics:**
- Create pixel art sprites in Aseprite or similar
- Import with Point filter mode

**Particle Effects:**
- Add ParticleSystem to Explosion prefab
- Configure for debris/smoke

## Common Issues

**Scripts not compiling:**
- Make sure all scripts are in proper folders
- Check for missing using statements

**Terrain not appearing:**
- Check TerrainGenerator has material assigned
- Verify terrain mesh is being generated (check console)

**Tanks not spawning:**
- Ensure Tank prefab is assigned in GameManager
- Check terrain is generated first

**UI not responding:**
- Verify EventSystem exists in scene (auto-created with Canvas)
- Check button OnClick events are wired up

**LeanTween errors:**
- LeanTween is optional - you can comment out those lines or install LeanTween from Asset Store

## Next Steps

1. **Create placeholder sprites** to test gameplay
2. **Build prefabs** with components as described
3. **Set up main scene** with all GameObjects
4. **Wire up UI** with basic buttons and sliders
5. **Test core gameplay** - shooting, terrain, physics
6. **Polish visuals** - replace placeholders with pixel art
7. **Add audio** - import and assign sound effects

## Quick Test Checklist

- [ ] Unity project opens without errors
- [ ] All scripts compile successfully
- [ ] Main scene created
- [ ] GameManager set up with prefabs
- [ ] Terrain generates on play
- [ ] Tanks spawn on terrain
- [ ] UI appears and responds
- [ ] Can adjust angle and velocity
- [ ] Can fire projectile
- [ ] Projectile follows physics
- [ ] Terrain deforms on impact
- [ ] Winner detected correctly
- [ ] Can play multiple rounds

## Getting Help

Check the main README.md for detailed setup instructions on each component.

## Have Fun!

Once everything is wired up, you'll have a fully functional Tanx clone with all the features of the original Amiga game. Enjoy!
