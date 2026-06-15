# MonoGame 2D StartKit VB.NET

A **fully-functional** multi-platform game template built with **VB.NET** for [MonoGame](https://www.monogame.net/), featuring the game **Seed-Scape: Forest Planting Quest**.

> **v1.1.3 Latest Update**: Fixed button interaction issue between WindowsDX and Android platforms. The pause button now works consistently across both platforms.

---

## Overview

- **Project Name:** PacDessert1436.MonoGame.StartKitVB
- **Template Short Name**: `mg2dstartkitvb`
- **Game Name:** Seed-Scape: Forest Planting Quest

_PacDessert1436.MonoGame.StartKitVB_ is a **production-ready** game template demonstrating modern MonoGame development practices using VB.NET. The included game, **Devour-Man**, is a Pac-Man inspired arcade game where players collect seeds to grow a forest while dodging patrolling insects.

> **New to MonoGame?** Check out [BEGINNER_GUIDE.md](BEGINNER_GUIDE.md) for a comprehensive guide transitioning from `vbPixelGameEngine` to MonoGame.

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
- ❌ **DesktopGL**: Build issues due to FFMPEG pipeline (see note below)

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

### Building and Running

1. **Clone or download** the project
2. **Open** the solution in Visual Studio
3. **Restore NuGet packages** if prompted
4. **Set startup project** based on target platform:
   - **WindowsDX**: For Windows desktop development (recommended)
   - **Android**: For mobile development and testing
   - **iOS**: For Apple mobile development (requires macOS)
   - **DesktopGL**: Cross-platform testing (currently has build issues)
5. **Build and run** the project

---

## Game Features (Devour-Man)

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

### ❌ DesktopGL (OpenGL)
- **Status**: Build Issues
- **Features**: Framework in place but cannot build
- **Notes**: FFMPEG pipeline issues prevent building. This platform is currently non-functional.

---

## Roadmap

### Planned Features
- ✅ Enhanced sound effects and music system
- [ ] DesktopGL build fix (FFMPEG pipeline)
- [ ] High score saving on future versions

### Known Issues
- DesktopGL cannot build due to FFMPEG pipeline issues
- iOS untested on actual devices

---

## Technical Notes

### Event Scheduling System
The game utilizes [ModuleEventRaiser.Generator](https://github.com/Pac-Dessert1436/ModuleEventRaiser.Generator) to schedule events, providing a comprehensive event-driven architecture:
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

### Version 1.1.3 (Latest; Production-Ready)
- ✅ Fixed button interaction issue between WindowsDX and Android platforms
- ✅ Pause button now works consistently across platforms
- ✅ Improved touch detection accuracy for mobile devices
- ✅ Better coordinate system alignment between rendering and input handling

### Version 1.1.1 and 1.1.2 (Production-Ready)
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

## Music/Sound Credits

Every music/sound asset is **CC0-licensed**, selected from [Open Game Art](https://opengameart.org/). Many sound effects are carefully chosen from [512 Sound Effects (8-Bit Style)](https://opengameart.org/content/512-sound-effects-8-bit-style) by SubspaceAudio, together with a few sound effects from the original MonoGame StartKit (C# Version).

- `BGM/main_theme.mp3` - _Dark Forest Waltz_, from [10-Track Modern Chiptune Demo](https://opengameart.org/content/10-track-modern-chiptune-demo) by IndieDevs.
- `level_cleared.wav` - _[New Thing Get!](https://opengameart.org/content/new-thing-get)_ composed by congusbongus.
- `game_start.wav` - _[Difference](https://opengameart.org/content/difference)_ composed by Chasersgaming; only the first 4 seconds are chosen.
- `game_over.wav` - from 512 Sound Effects (8-Bit Style) by SubspaceAudio.
- `enemy_killed.wav` - from 512 Sound Effects (8-Bit Style) by SubspaceAudio.
- `enemy_respawn.wav` - from 512 Sound Effects (8-Bit Style) by SubspaceAudio.
- `seed_packet.wav` - from the original MonoGame StartKit (C# Version).
- `life_lost.wav` - from the original MonoGame StartKit (C# Version).
- `pesticide.wav` - from 512 Sound Effects (8-Bit Style) by SubspaceAudio.
- `at_next_level.wav` - from the original MonoGame StartKit (C# Version).

## License

This project is licensed under the BSD-3-Clause License.

---

**Happy coding!** Build amazing games with MonoGame and VB.NET! 🎮

> For beginners transitioning from vbPixelGameEngine, see [BEGINNER_GUIDE.md](BEGINNER_GUIDE.md).