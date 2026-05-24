# MonoGame 2D StartKit VB.NET (`PacDessert1436.MonoGame.StartKitVB`)

A multi-platform game template built with **VB.NET** for [MonoGame](https://www.monogame.net/), featuring the game **Devour-Man** - inspired by Pac-Man.

> **Important Note:** _This project is still in active development - currently a few steps away from feature completeness. Additionaly, the template's short name has already been changed since version 1.1.0._

---

## Overview

- **Project Name:** PacDessert1436.MonoGame.StartKitVB
- **Template Short Name**: `mg2dstartkitvb` (formerly `mgstartkit-vb`)
- **Game Name:** Devour-Man

_PacDessert1436.MonoGame.StartKitVB_ is an **actively developed** game template demonstrating modern MonoGame development practices using VB.NET. The included game, **Devour-Man**, is a Pac-Man inspired arcade game where players collect seeds to grow a forest while dodging patrolling insects.

---

## Current Development Status

### Core Architecture
- ✅ **Clean Architecture Pattern**: Proper separation of concerns with dedicated managers
- ✅ **Event-Driven Design**: Comprehensive event system for game state management
- ✅ **True OOP Implementation**: Polymorphic actor system with base `Actor` class 
- ✅ **Memory Management**: Complete `IDisposable` pattern implementation

### Game Systems
- ✅ **GameManager**: Centralized game state management with collision detection
- ✅ **Renderer**: Advanced 2D rendering with sprite sheets and animations
- ⚠️ **SoundManager**: Basic audio playback support (needs expansion)
- ✅ **VirtualJoystick**: Cross-platform input handling (touch, mouse, keyboard)

### Entity System
- ✅ **Actor Framework**: Base class with `Player`, `Enemy`, and `Seed` subclasses
- ✅ **Enemy Movement**: Random patrolling behavior with direction changes
- ⚠️ **Enemy Respawn Mechanics**: Currently not fully implemented and needs refinement
- ⚠️ **Progression System**: Multiple seed types (Acorn, Berry, Nut) and enemy types (Beetle, Caterpillar); _Refinement required for the next version_
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
| **SoundManager** | Plays background music and sound effects | ⚠️ Basic |
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
- [ ] Enhanced sound effects and music system
- [ ] DesktopGL build fix (FFMPEG pipeline)
- [ ] High score saving and level progression system

### Known Issues
- DesktopGL cannot build due to FFMPEG pipeline issues
- iOS untested on actual devices

---

## Technical Notes

### Event System
The game uses a comprehensive event-driven architecture:
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

### Version 1.1.0 (Current - Development)
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

## License

This project is licensed under the BSD-3-Clause License.

---

**Active Development Notice**: This project is still under active development. Features may be incomplete or subject to change.

**Happy coding!** Build amazing games with MonoGame and VB.NET!
