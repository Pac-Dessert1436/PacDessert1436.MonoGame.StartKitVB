# MonoGame 2D StartKit VB.NET

> ## ⚠️ Crash Warning for v1.2.5 Users
> Version 1.2.5 contains a critical **null-reference crash** that occurs when pausing, returning to the menu, and restarting the demo game. _A manual code edit is required if using v1.2.5._
> ### Two Fix Paths:
> 1. **Best Choice**: Update to NuGet package 1.2.5.1 or later, which includes the critical fix (no manual code changes needed).
> 2. **If you cannot upgrade from v1.2.5**: Paste the following snippet at the top of the `Update()` method in `GameMain.vb`:
> ```vb
> With _gameManager
>     If .Player IsNot Nothing AndAlso .Player.Joystick Is Nothing Then
>         .Player.Joystick = _renderer.Joystick
>     End If
> End With
> ```

A **fully-functional** multi-platform game template built with **VB.NET** for [MonoGame](https://www.monogame.net/), featuring the demo game **Seed-Scape: Forest Planting Quest**.

Starting from version 1.2.0, all assets are fully licensed and attribution-ready:
- Main character sprite adapted from CC0-licensed art
- Original graphics created in Aseprite (published to Open Game Art)
- All audio assets are CC0-licensed from Open Game Art
- Font properly licensed for commercial use
- **Blank template available from version 1.2.4** for starting your own projects

> **🚨 Important Notice**: **Versions prior to 1.2.0 have been unlisted and should not be used** due to potential copyright concerns with included assets. Always use version 1.2.0 or later for production projects.

---

## Overview

- **Project Name**: PacDessert1436.MonoGame.StartKitVB
- **Template Short Name**: `mg2dstartkitvb`
- **Game Name**: Seed-Scape: Forest Planting Quest

_PacDessert1436.MonoGame.StartKitVB_ is a **production-ready** game template demonstrating modern MonoGame development practices using VB.NET. 

Designed as a VB.NET alternative to the original C# MonoGame StartKit (`mg2dstartkit`), this template enables VB.NET developers - especially those transitioning from `vbPixelGameEngine` - to leverage MonoGame's powerful game development capabilities.

The package includes **two templates**:

1. **Complete Demo Game**: _**Seed-Scape: Forest Planting Quest**_ - An arcade game where players collect seeds to grow a forest while dodging patrolling insects.
2. **Blank Template (`mgblank2dstartkitvb`)**: A clean starting point for your own MonoGame projects. _Available from version 1.2.4._

Note that versions 1.2.0 through 1.2.3 had template ID collisions that made the blank template unavailable; the demo game template (`mg2dstartkitvb`) remains accessible across all these versions. _Additionally, v1.2.3 marks the final gameplay polish for the v1.x series (see [Version History](#version-history) for details)._

> **New to MonoGame?** Check out [BEGINNER_GUIDE.md](MonoGameStartKitVB/BEGINNER_GUIDE.md) for a comprehensive guide to transitioning from `vbPixelGameEngine` to MonoGame.

## What's New in v1.2.5+

> **Note**: Version 1.2.5.2 was the feature-complete 1.x version, but unexpectedly included the unfinished 2.0.0 template project. It has been deprecated and should not be used - please upgrade to 1.2.5.3.

**v1.2.5.3 Code Quality Enhancements** 🎮

1. Removed unused hidden variable `JOYSTICK_DEAD_ZONE` from `Actor.vb`
2. Implemented the `{ get; private set; }` pattern for `PauseButtonWidth` and `PauseButtonHeight` in `Renderer.vb`
3. Made `Renderer.Joystick` property shared, with this logic applied to `Player.vb`'s `Update()` method: 
  - `If Joystick Is Nothing Then Joystick = Renderer.Joystick`

These improvements have been verified across multiple platforms (DesktopGL, WindowsDX, and Android) and do not impact the demo game's functionality.

✅ **Final User Experience (UX) Polish in 1.2.5** - The definitive finishing touches for _Seed-Scape: Forest Planting Quest_:
- **Joystick Dead Zone**: Integrated `VirtualJoystick` class into player input handling with a 25% dead zone, eliminating accidental movements from slight touches
- **Joystick State Tracking**: Added `IsActive` property for improved input state management
- **Refactored Input Handling**: Now uses `VirtualJoystick.Update()` for consistent input processing across touch and mouse
- **Android Touch Input Fix**: Corrected joystick center coordinate calculation for Android devices, ensuring accurate touch input detection

---

## Project Status

### Core Architecture
- ✅ **Clean Architecture Pattern**: Proper separation of concerns with dedicated managers
- ✅ **Event-Driven Design**: Comprehensive event system for game state management
- ✅ **True OOP Implementation**: Polymorphic actor system with base `Actor` class
- ✅ **Memory Management**: Complete `IDisposable` pattern implementation

### Game Systems
- ✅ **GameManager**: Centralized game state management with collision detection
- ✅ **Renderer**: Advanced 2D rendering with sprite sheets and animations
- ✅ **SoundManager**: Fully functional audio playback support
- ✅ **VirtualJoystick**: Cross-platform input handling (touch, mouse, keyboard)

### Entity System
- ✅ **Actor Framework**: Base class with `Player`, `Enemy`, and `Seed` subclasses
- ✅ **Enemy Movement**: Random patrolling behavior with direction changes
- ✅ **Enemy Respawn Mechanics**: Fully implemented and tested
- ✅ **Progression System**: Multiple seed types (Acorn, Berry, Nut) and enemy types (Beetle, Caterpillar) working seamlessly
- ✅ **Forest Growth**: Sapling-to-tree transformation when seeds are collected

### Platform Support
- ✅ **WindowsDX**: Primary platform with full feature support
- ✅ **Android**: Touch input fully functional and tested
- ⚠️ **iOS**: Compatibility in progress (untested)
- ✅ **DesktopGL**: Now functional, though high DPI mode is unavailable (game window may exceed screen height)

---

## Project Architecture

```
MonoGameStartKitVB/
├── MonoGameStartKitVB.Core/          # Shared VB.NET game logic
│   ├── Actor.vb                      # Entity framework (Player, Enemy, Seed)
│   ├── Essentials.vb                 # Constants, enums, events, utilities
│   ├── GameMain.vb                   # Main game class
│   ├── GameManager.vb                # Game state and logic
│   ├── Renderer.vb                   # Graphics rendering
│   ├── SoundManager.vb               # Audio management
│   ├── SpriteSheet.vb                # Sprite and animation system
│   └── VirtualJoystick.vb            # Input handling
├── MonoGameStartKitVB.WindowsDX/     # Windows desktop launcher
├── MonoGameStartKitVB.Android/       # Android mobile launcher
├── MonoGameStartKitVB.iOS/           # iOS mobile launcher
└── MonoGameStartKitVB.DesktopGL/     # Cross-platform OpenGL launcher
```

### Key Components

| Component | Responsibility | Status |
|-----------|----------------|--------|
| **GameMain** | Initializes game systems, coordinates Update/Draw cycles | ✅ Complete |
| **GameManager** | Manages game state, collision detection, level progression | ✅ Working |
| **Renderer** | Handles all rendering, animations, and HUD display | ✅ Working |
| **SoundManager** | Plays background music and sound effects | ✅ Working |
| **Actor** | Base class for all game entities with inheritance | ✅ Working |

---

## Getting Started

### Prerequisites
- Visual Studio 2026, Visual Studio Code, or another .NET-compatible IDE
- [.NET SDK 10.0](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- MonoGame 3.8+ installation (see [Getting Started](https://docs.monogame.net/articles/getting_started/index.html) in the MonoGame documentation)
- Essential tools and extensions for VB.NET development

### Using the NuGet Template

Install the template via NuGet:

```bash
dotnet new install PacDessert1436.MonoGame.StartKitVB
```

Create a new project with the **demo game**:

```bash
dotnet new mg2dstartkitvb -n YourGameName
```

Create a new project with the **blank template**:

```bash
dotnet new mgblank2dstartkitvb -n YourGameName
```

### Building and Running from Source Code

1. **Clone or download** the project repository
2. **Open** the solution in your preferred .NET IDE
3. **Restore NuGet packages** if prompted
4. **Set the startup project** based on your target platform:
  - **WindowsDX**: For Windows desktop development (recommended)
  - **Android**: For mobile development and testing
  - **iOS**: For Apple mobile development (requires macOS)
  - **DesktopGL**: Cross-platform testing (high DPI mode unavailable)
5. **Build and run** the project

---

## Game Features (Seed-Scape: Forest Planting Quest)

### Core Gameplay
- **Player Movement**: Smooth 4-directional movement with responsive controls
- **Seed Collection**: Gather seeds to grow trees and earn points
- **Enemy Movement**: Patrolling insects with dynamic direction changes
- **Pesticide Power-Up**: Temporarily makes enemies vulnerable
- **Forest Growth**: Saplings transform into mature trees as seeds are collected
- **Level Progression**: Increasing difficulty with new enemy types as you advance

### Input Support
- **Keyboard**: WASD or arrow keys for movement
- **Touch**: Optimized virtual joystick for mobile devices (fully tested and functional)
- **Mouse**: Click-based controls for menus and virtual joystick interaction

### Visual Features
- **Sprite Animations**: Smooth, fluid animations for all game entities
- **Dynamic HUD**: Real-time display of score, lives, level, and high score
- **Multiple Game States**: Title screen, in-game, paused, game over, and level cleared screens

---

## Platform Support Status

### ✅ WindowsDX (DirectX)
- **Status**: Fully Supported
- **Features**: All game features work correctly
- **Performance**: Optimal for Windows desktop systems
- **Recommendation**: Primary development platform

### ✅ Android
- **Status**: Fully Supported
- **Features**: Touch input with virtual joystick (tested and working seamlessly)
- **Deployment**: Ready for testing on Android devices
- **Notes**: Touch controls have been thoroughly tested and function properly

### ⚠️ iOS
- **Status**: Planned
- **Features**: Basic compatibility implemented
- **Notes**: Requires macOS for development; untested on physical devices

### ✅ DesktopGL (OpenGL)
- **Status**: Functional
- **Features**: All assets load correctly, and the game is fully playable
- **Notes**: High DPI mode is unavailable, which may cause the game window to exceed screen height on some displays

---

## Roadmap

### Completed Features in 1.2.x
- ✅ Enhanced sound effects and music system
- ✅ **High score saving** with JSON serialization
- ✅ DesktopGL build fix (FFMPEG pipeline resolution)

### Known Issues in 1.2.x
- DesktopGL high DPI mode unavailable (game window may exceed screen height)
- iOS compatibility untested on physical devices

### Version 2.0.0: _Mending Garden_ (Upcoming - Development Paused)
> **Important Note**: Development will be paused starting July 2026 as I focus on preparing for the Postgraduate Entrance Exam. **This pause will continue until the end of 2026, when the exam is complete.** Updates will resume afterward, though plans for this new game template may evolve based on my focus at that time. Thank you for your understanding!

_Mending Garden_ (version 2.0.0) is a farming simulation game deeply rooted in the theme of Plant Protection, set on a charming Chinese countryside farm:
- 🌱 **Crop Planting**: Cultivate regionally significant crops like corn, cotton, and wheat
- 🍃 **Seasonal Cycle**: Follow the natural rhythm of Spring planting → Summer growth → Autumn harvest
- ⏰ **Timed Care**: Tend to crops at specific intervals to maintain their health and promote growth
- 🐛 **Pest Defense**: Protect your harvest from insects using fast-paced "Duck Hunt" style gameplay
- 🏆 **Harvest System**: Collect mature crops and earn rewards for successful farming
- 🎨 **Authentic Visuals**: Hand-drawn art style inspired by Chinese rural landscapes and farming culture

---

## Technical Notes

### Event Scheduling System
The game uses the latest version of [ModuleEventRaiser.Generator](https://github.com/Pac-Dessert1436/ModuleEventRaiser.Generator) to schedule events, providing a robust event-driven architecture:
- `GameStateChanged` - Fired when the game state transitions
- `PlayerScoreChanged` - Fired when the player collects items
- `SeedCollected` - Fired when a seed is collected
- `EnemyKilled` - Fired when an enemy is defeated
- `LevelCleared` - Fired when all seeds in a level are collected

### Content Pipeline
All assets are processed through the MonoGame Content Pipeline:
- Sprites: PNG format, loaded via the SpriteSheet class
- Audio: WAV format for sound effects
- Fonts: SpriteFont format for text rendering

---

## Version History

### Version 1.2.5.3 (Latest)
- **Code Cleanup**: Removed unused hidden variable `JOYSTICK_DEAD_ZONE` from `Actor.vb`
- **Encapsulation Improvements**: Implemented the `{ get; private set; }` pattern for `PauseButtonWidth` and `PauseButtonHeight` in `Renderer.vb`
- **Input Handling Refinement**: Made `Renderer.Joystick` property shared, with this logic applied to `Player.vb`'s `Update()` method: `If Joystick Is Nothing Then Joystick = Renderer.Joystick`
- **Cross-Platform Verification**: All improvements verified across DesktopGL, WindowsDX, and Android platforms without impacting game functionality

### Version 1.2.5.2 (Deprecated)
⚠️ This version unexpectedly **included unfinished v2.0.0 template project** and has been deprecated. Please use v1.2.5.3 instead.

### Version 1.2.5.1 (Hotfix)
- **Joystick Null Reference Fix**: Added a defensive check in `GameMain.Update()` to ensure the joystick is always properly connected to the player after game restarts, preventing null reference exceptions when pausing, returning to the menu, and starting a new game

**Note for v1.2.5 users**: If you are using version 1.2.5, apply this hotfix by adding the following code at the top of the `Update` method in `GameMain.vb`:
```vb
With _gameManager
    If .Player IsNot Nothing AndAlso .Player.Joystick Is Nothing Then
        .Player.Joystick = _renderer.Joystick
    End If
End With
```

### Version 1.2.5 (Final UX Polish)
- **Joystick Dead Zone**: Integrated `VirtualJoystick` class into player input handling with a 25% dead zone
- **Refactored Input Handling**: The Player class now uses `VirtualJoystick.Update()` for consistent input processing
- **Joystick Property Added**: Added a `Joystick` property to the Player class for proper initialization and state management
- **Android Touch Input Fix**: Corrected joystick center coordinate calculation to account for screen offset and scaling, resolving the issue where joystick touch detection was misaligned with its drawn position
- **Definitive v1.x Release**: This marks the final polish for _Seed-Scape: Forest Planting Quest_, with v2.0.0 (_Mending Garden_) planned for post-2026

### Version 1.2.4
- **Template ID Collision Fix**: Changed the blank template identity from `PacDessert1436.MonoGame.StartKitVB` to `PacDessert1436.MonoGame.BlankStartKitVB`
- ⚠️ **Important**: Versions 1.2.0 through 1.2.3 had template ID collisions; **v1.2.4+ is recommended for production use**

### Version 1.2.3 (Definitive Gameplay Fixes)
- **Sound Playback Fix**: Resolved an edge case where `life_gained.wav` played at incorrect times
- **Animation Timing Adjustments**: Fine-tuned the death sequence duration and "Season Cleared" caption display
- **Code Cleanup**: Removed the unused `DrawFrameWithOrigin` method from `SpriteSheet.vb`
- **Visual Polish**: Updated enemy blink color to purple for improved visibility
- **Roadmap Update**: _Mending Garden_ (v2.0.0) development planned for post-2026

### Version 1.2.2 (Gameplay & Stability Improvements)
- **Maze Generation Fix**: Enemies now spawn correctly without overlapping seeds or pesticides
  - Smart tile placement with proper distance constraints
  - Dynamic enemy count scaling with level (5-12 enemies, clamped)
- **Level & Score Limits**: Levels capped at 99, score clamped between 0-999999 to prevent overflow
- **Complete XML Documentation**: Full `<summary>` comments added to `Actor.vb`
  - `Actor` base class with properties and methods documented
  - `Player` class with input handling and game mechanics documented
  - `Enemy` class with AI behavior and respawning mechanics documented
  - `Seed` class with collectible item documentation

### Version 1.2.1
- Improved `GameMain.vb` in the blank template, with proper viewport handling for all platforms
- Upgraded the `ModuleEventRaiser.Generator` library to the latest version

### Version 1.2.0 (Production-Ready)
- **Complete Copyright Safety**: All assets now properly licensed
- Main character `player_sheet.png` adapted from CC0-licensed [Forest Boy](https://opengameart.org/content/forest-boy-platformer-animated-character-24x24)
- All original graphics (seeds, trees, beetles, caterpillars) created in Aseprite
- All audio assets replaced with CC0-licensed alternatives from Open Game Art
- Font updated to [Fusion Pixel Font](https://github.com/TakWolf/fusion-pixel-font) by TakWolf (12px Monospace)
- Blank template now available for starting new projects (`mgblank2dstartkitvb`)
- High score saving with JSON serialization
- Bonus life sound effect (`life_gained.wav`)
- Fixed button interaction issues between WindowsDX and Android platforms
- Pause button now works consistently across platforms
- ⚠️ **Versions prior to 1.2.0 have been unlisted** due to copyright concerns

### Version 1.1.3
- Improved touch detection accuracy for mobile devices
- Better coordinate system alignment between rendering and input handling

### Version 1.1.1 to 1.1.2
- Fully-functional game template ready for production use
- Complete Android touch input support with virtual joystick
- Clean architecture pattern implementation
- Event-driven design with comprehensive event system
- Polymorphic actor system with Player, Enemy, and Seed classes
- Sprite sheet animations for all game entities
- WindowsDX and Android platforms fully supported
- ⚠️ DesktopGL build issues (FFMPEG pipeline - known limitation)

### Version 1.1.0
- Clean architecture pattern implementation
- Event-driven design with comprehensive event system
- Polymorphic actor system
- Sprite sheet animations
- Android touch controls tested and working
- ❌ DesktopGL build issues (FFMPEG pipeline)

### Version 1.0.3
- IDisposable implementation for sound manager and renderer
- Backward-compatible solution file format
- ❌ DesktopGL build issues (FFMPEG pipeline)

### Version 1.0.2
- Object-oriented architecture refactoring
- Improved code structure and maintainability
- Event-driven design implementation
- ❌ DesktopGL build issues (FFMPEG pipeline)

### Version 1.0.1
- Sound effects and sprite assets added
- DesktopGL support (later broken by FFMPEG issues)

---

## Asset Credits

### Graphics
- `player_sheet.png` - Adapted from [Forest Boy: Platformer Animated Character](https://opengameart.org/content/forest-boy-platformer-animated-character-24x24) (CC0, public domain)
- All original sprites (seeds, trees, beetles, caterpillars, UI elements) - Created in Aseprite, published to Open Game Art with CC0 & OGA-BY licenses  
  - See: [Assets of _Seed-Scape: Forest Planting Quest_](https://opengameart.org/content/seed-scape-forest-planting-quest)

### Fonts
- `GameFont.spritefont` - 12px Monospace variant of [Fusion Pixel Font](https://github.com/TakWolf/fusion-pixel-font) by TakWolf (SIL Open Font License 1.1)

### Music & Sound Effects
All audio assets are **CC0-licensed** (public domain), sourced from [Open Game Art](https://opengameart.org/):

| Asset | Description | Source |
|-------|-------------|--------|
| `BGM/main_theme.mp3` | _Dark Forest Waltz_ | [10-Track Modern Chiptune Demo](https://opengameart.org/content/10-track-modern-chiptune-demo) by IndieDevs |
| `level_cleared.wav` | _New Thing Get!_ | [congusbongus](https://opengameart.org/content/new-thing-get) |
| `game_start.wav` | _Difference_ (first 4 seconds) | [Chasersgaming](https://opengameart.org/content/difference) |
| `game_over.wav` | _Shutdown Sound #1_ | [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio |
| `enemy_killed.wav` | _Impact Sound #13_ | [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio |
| `enemy_respawn.wav` | _Neutral Sound #11_ | [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio |
| `seed_packet.wav` | _Coin Sound (Double #1)_ | [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio |
| `life_gained.wav` | _Powerup Sound #12_ | [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio |
| `life_lost.wav` | _Human Death Scream #12_ (0.5x speed) | [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio |
| `pesticide.wav` | _Coin Sound (Cluster #3)_ | [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio |
| `at_next_level.wav` | _Fanfare Sound #3_ (0.25x speed) | [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio |

## License