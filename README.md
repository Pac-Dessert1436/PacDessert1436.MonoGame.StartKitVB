# MonoGame 2D StartKit VB.NET

A **fully-functional** multi-platform game template built with **VB.NET** for [MonoGame](https://www.monogame.net/), featuring the game **Seed-Scape: Forest Planting Quest**.

> **v1.2.1 Latest Update**: **Blank Template Enhanced**! Improved structure and cross-platform compatibility:
> - Refined `GameMain.vb` with proper viewport handling for Android
> - Consistent screen resolution handling across all platforms
> - Applied the latest version of `ModuleEventRaiser.Generator`
>
> **v1.2.0 Important Update**: ✅ **Complete Copyright Safety Achieved**! All assets are now fully licensed and attribution-ready:
> - Main character sprite adapted from CC0-licensed art
> - All original graphics created in Aseprite
> - All audio assets CC0-licensed from Open Game Art
> - Font properly licensed for commercial use
> - **Blank template now available** for starting your own projects

---

## 🚨 Important Notice

**Versions prior to 1.2.0 have been unlisted and should not be used** due to potential copyright concerns with included assets. Always use version 1.2.0 or later for production projects.

This template is designed as a **VB.NET alternative** to the original C# MonoGame StartKit (`mg2dstartkit`), which enables VB.NET developers to leverage MonoGame's powerful game development capabilities, especially the ones transitioning from `vbPixelGameEngine` to MonoGame.

---

## Overview

- **Project Name:** PacDessert1436.MonoGame.StartKitVB
- **Template Short Name**: `mg2dstartkitvb`
- **Game Name:** Seed-Scape: Forest Planting Quest

_PacDessert1436.MonoGame.StartKitVB_ is a **production-ready** game template demonstrating modern MonoGame development practices using VB.NET. The package includes **two templates**:

1. **Complete Demo Game**: _**Seed-Scape: Forest Planting Quest**_ - An arcade game where players collect seeds to grow a forest while dodging patrolling insects.
2. **Blank Template (`mgblank2dstartkitvb`)**: A clean starting point for your own MonoGame projects.

> **New to MonoGame?** Check out [BEGINNER_GUIDE.md](MonoGameStartKitVB/BEGINNER_GUIDE.md) for a comprehensive guide transitioning from `vbPixelGameEngine` to MonoGame.

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
- ✅ **Progression System**: Multiple seed types (Acorn, Berry, Nut) and enemy types (Beetle, Caterpillar) working perfectly
- ✅ **Forest Growth**: Sapling-to-tree transformation when seeds are collected

### Platform Support
- ✅ **WindowsDX**: Primary platform with full feature support
- ✅ **Android**: Touch input fully functional and tested
- ⚠️ **iOS**: Compatibility in progress (untested)
- ✅ **DesktopGL**: Now functional, but high DPI mode unavailable (game window may exceed screen height)

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
└── MonoGameStartKitVB.DesktopGL/     # Cross-platform OpenGL launcher (build issues)
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
- Visual Studio 2026 or later
- [.NET 10.0](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- MonoGame 3.8+ installation
- VB.NET development tools

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

### Building and Running (Source Code)

1. **Clone or download** the project
2. **Open** the solution in Visual Studio
3. **Restore NuGet packages** if prompted
4. **Set startup project** based on target platform:
   - **WindowsDX**: For Windows desktop development (recommended)
   - **Android**: For mobile development and testing
   - **iOS**: For Apple mobile development (requires macOS)
   - **DesktopGL**: Cross-platform testing (high DPI mode unavailable)
5. **Build and run** the project

---

## Game Features (Seed-Scape: Forest Planting Quest)

### Core Gameplay
- **Player Movement**: 4-directional movement with smooth controls
- **Seed Collection**: Collect seeds to grow trees and earn points
- **Enemy Movement**: Patrolling insects with dynamic direction changes
- **Pesticide Power-Up**: Temporarily makes enemies vulnerable
- **Forest Growth**: Saplings transform into trees as seeds are collected
- **Level Progression**: Increasing difficulty with new enemy types

### Input Support
- **Keyboard**: WASD or arrow keys for movement
- **Touch**: Virtual joystick optimized for mobile devices (tested and working)
- **Mouse**: Click-based controls for menus and virtual joystick

### Visual Features
- **Sprite Animations**: Smooth character animations for all entities
- **Dynamic HUD**: Score, lives, level, and high score display
- **Multiple Game States**: Title screen, playing, paused, game over, level cleared

---

## Platform Support Status

### ✅ WindowsDX (DirectX)
- **Status**: Fully Supported
- **Features**: All game features work correctly
- **Performance**: Optimal for Windows desktop
- **Recommended**: Primary development platform

### ✅ Android
- **Status**: Fully Supported
- **Features**: Touch input with virtual joystick (tested and working smoothly)
- **Deployment**: Ready for testing on Android devices
- **Notes**: Touch controls have been tested and work properly

### ⚠️ iOS
- **Status**: Planned
- **Features**: Basic compatibility implemented
- **Notes**: Requires macOS for development, untested on actual devices

### ✅ DesktopGL (OpenGL)
- **Status**: Functional
- **Features**: All assets handled correctly, game fully playable
- **Notes**: High DPI mode is not available, which may cause the game window height to exceed the device screen height.

---

## Roadmap

### Completed Features on 1.2.1
- ✅ Enhanced sound effects and music system
- ✅ **High score saving** with JSON serialization
- ✅ DesktopGL build fix (FFMPEG pipeline)

### Known Issues on 1.2.1
- DesktopGL high DPI mode unavailable (game window may exceed screen height)
- iOS untested on actual devices

### Version 2.0.0: _Mending Garden_ (Upcoming - Development Paused)
> **Important Note**: Development will be paused starting July this year, since I focus on retaking the Postgraduate Entrance Exam. **This pause will continue until the end of 2026, when the exam is complete.** Updates will resume afterward, though plans for this new game template may evolve based on my focus at that time. Thank you for your understanding!

_Mending Garden_ (version 2.0.0) is a farming simulation game deeply connected to the theme of Plant Protection, set on a delightful Chinese countryside farm:
- 🌱 **Crop Planting**: Cultivate regionally significant crops like corn, cotton, and wheat
- 🍃 **Seasonal Cycle**: Follow the natural rhythm of Spring planting → Summer growth → Autumn harvest
- ⏰ **Timed Care**: Tend crops at specific intervals to maintain their health and growth
- 🐛 **Pest Defense**: Protect your harvest from insects using fast-paced "Duck Hunt" style gameplay
- 🏆 **Harvest System**: Collect mature crops and earn rewards for successful farming
- 🎨 **Authentic Visuals**: Hand-drawn art style inspired by Chinese rural landscapes and farming culture

---

## Technical Notes

### Event Scheduling System
The game utilizes the latest version of [ModuleEventRaiser.Generator](https://github.com/Pac-Dessert1436/ModuleEventRaiser.Generator) to schedule events, providing a comprehensive event-driven architecture:
- `GameStateChanged` - Fired when game state transitions
- `PlayerScoreChanged` - Fired when player collects items
- `SeedCollected` - Fired when a seed is collected
- `EnemyKilled` - Fired when an enemy is defeated
- `LevelCleared` - Fired when all seeds are collected

### Content Pipeline
All assets are processed through the MonoGame Content Pipeline:
- Sprites: PNG format, loaded via SpriteSheet class
- Audio: WAV for sound effects
- Fonts: SpriteFont format for text rendering

---

## Version History

### Version 1.2.1 (Latest; Improved)
- ✅ Improved `GameMain.vb` in blank template, with proper viewport handling for all platforms
- ✅ Better screen resolution handling across all platforms
- ✅ Upgraded the `ModuleEventRaiser.Generator` library to the latest version

### Version 1.2.0 (Production-Ready)
- ✅ **Complete Copyright Safety**: All assets now properly licensed
- ✅ Main character `player_sheet.png` adapted from CC0-licensed [Forest Boy](https://opengameart.org/content/forest-boy-platformer-animated-character-24x24)
- ✅ All original graphics (seeds, trees, beetles, caterpillars) created in Aseprite
- ✅ All audio assets replaced with CC0-licensed alternatives from Open Game Art
- ✅ Font updated to [Fusion Pixel Font](https://github.com/TakWolf/fusion-pixel-font) by TakWolf (12px Monospace)
- ✅ **Blank template now available** for starting new projects (`mgblank2dstartkitvb`)
- ✅ **High score saving** with JSON serialization
- ✅ **Bonus life sound effect** (`life_gained.wav`)
- ✅ Fixed button interaction issue between WindowsDX and Android platforms
- ✅ Pause button now works consistently across platforms
- ⚠️ **Versions prior to 1.2.0 have been unlisted** due to copyright concerns

### Version 1.1.3
- ✅ Improved touch detection accuracy for mobile devices
- ✅ Better coordinate system alignment between rendering and input handling

### Version 1.1.1 to 1.1.2
- ✅ Fully-functional game template ready for production use
- ✅ Complete Android touch input support with virtual joystick
- ✅ Clean architecture pattern implementation
- ✅ Event-driven design with comprehensive event system
- ✅ Polymorphic actor system with Player, Enemy, and Seed classes
- ✅ Sprite sheet animations for all game entities
- ✅ WindowsDX and Android platforms fully supported
- ⚠️ DesktopGL build issues (FFMPEG pipeline - known limitation)

### Version 1.1.0
- ✅ Clean architecture pattern implementation
- ✅ Event-driven design with comprehensive event system
- ✅ Polymorphic actor system
- ✅ Sprite sheet animations
- ✅ Android touch controls tested and working
- ❌ DesktopGL build issues (FFMPEG pipeline)

### Version 1.0.3
- ✅ IDisposable implementation for sound manager and renderer
- ✅ Backward-compatible solution file format
- ❌ DesktopGL build issues (FFMPEG pipeline)

### Version 1.0.2
- ✅ Object-oriented architecture refactoring
- ✅ Improved code structure and maintainability
- ✅ Event-driven design implementation
- ❌ DesktopGL build issues (FFMPEG pipeline)

### Version 1.0.1
- ✅ Sound effects and sprite assets
- ✅ DesktopGL support; **later broken by FFMPEG issues**

---

## Asset Credits

### Graphics
- `player_sheet.png` - Adapted from [Forest Boy: Platformer Animated Character](https://opengameart.org/content/forest-boy-platformer-animated-character-24x24) (CC0, public domain)
- All other sprites (seeds, trees, beetles, caterpillars, UI elements) - **Original artwork created in Aseprite**

### Fonts
- `GameFont.spritefont` - 12px Monospace variant of [Fusion Pixel Font](https://github.com/TakWolf/fusion-pixel-font) by TakWolf (SIL Open Font License 1.1)

### Music & Sound Effects (All CC0-Licensed)
All audio assets are **CC0-licensed** (public domain), selected from [Open Game Art](https://opengameart.org/):

- `BGM/main_theme.mp3` - _Dark Forest Waltz_, from [10-Track Modern Chiptune Demo](https://opengameart.org/content/10-track-modern-chiptune-demo) by IndieDevs.
- `level_cleared.wav` - _[New Thing Get!](https://opengameart.org/content/new-thing-get)_ by congusbongus.
- `game_start.wav` - _[Difference](https://opengameart.org/content/difference)_ by Chasersgaming (first 4 seconds).
- `game_over.wav` - _Shutdown Sound #1_, from [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio.
- `enemy_killed.wav` - _Impact Sound #13_, from [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio.
- `enemy_respawn.wav` - _Neutral Sound #11_, from [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio.
- `seed_packet.wav` - _Coin Sound (Double #1)_, from [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio.
- `life_gained.wav` - _Powerup Sound #12_, from [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio.
- `life_lost.wav` - _Human Death Scream #12_ (0.5x speed), from [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio.
- `pesticide.wav` - _Coin Sound (Cluster #3)_, from [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio.
- `at_next_level.wav` - _Fanfare Sound #3_ (0.25x speed), from [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio.

## License

This project is licensed under the BSD-3-Clause License. See the [LICENSE](LICENSE) file for details.

---

**Happy coding!** Build amazing games with MonoGame and VB.NET! 🎮

> For beginners transitioning from vbPixelGameEngine, see [BEGINNER_GUIDE.md](BEGINNER_GUIDE.md).