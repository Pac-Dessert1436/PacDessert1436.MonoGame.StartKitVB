# Devour-Man - MonoGame StartKit (VB.NET)

**Version 1.0.1** - A minimal viable game template built with **VB.NET** for [MonoGame](https://www.monogame.net/), inspired by Pac-Man.

## Overview

_Devour-Man_ is a simple yet functional game template that demonstrates core MonoGame concepts in VB.NET. Players collect seeds to grow a forest while dodging patrolling insects.

This project serves as an excellent starting point for developers looking to build 2D games with MonoGame, using Visual Basic .NET for the core game logic.

> **Note**: This game template is still a **minimal viable product** (MVP). It is not a complete game with all features implemented - stay tuned for version 1.1.0!

## Getting Started

### Prerequisites
- Visual Studio 2026 or later
- [.NET 10.0](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- MonoGame 3.8+ installation
- VB.NET development tools

## Features

### Version 1.0.1 Updates
- ✅ **Sound Effects**: Added immersive audio feedback for game actions
- ✅ **Sprite Assets**: Implemented visual elements for characters and environment
- ✅ **UWP Support**: Fully functional on Universal Windows Platform
- ✅ **DesktopGL Support**: Basic functionality on Desktop OpenGL (with limitations)

### Core Gameplay
- Player character movement and controls
- Seed collection mechanics
- Enemy AI with patrolling behavior
- Forest growth progression system
- Basic collision detection

## Platform Support

### ✅ Universal Windows Platform (UWP)
- **Status**: Fully Supported
- **Features**: All game features work perfectly
- **Audio**: Background music and sound effects function correctly
- **Assets**: All sprite and font assets load properly

### ⚠️ DesktopGL (OpenGL)
- **Status**: Limited Support
- **Known Limitations**:
  - Background music files cannot be synchronized
  - Font files have loading/synchronization issues

### Building and Running

1. **Clone or download** the project
2. **Open** the solution in Visual Studio
3. **Restore NuGet packages** if prompted
4. **Set target platform**:
   - For full functionality: Choose **UWP**
   - For Desktop testing: Choose **DesktopGL** (with limitations)
5. **Build and run** the project

### Recommended Development Workflow

1. **Primary Development**: Use UWP target for full feature testing
2. **Cross-Platform Testing**: Test with DesktopGL to identify platform-specific issues
3. **Asset Management**: Ensure audio and font files are properly configured for both platforms

## Technical Notes

### Asset Synchronization Issues

The DesktopGL platform has known limitations with certain asset types:

- **Audio Files**: Background music synchronization issues
- **Font Files**: Loading and rendering inconsistencies
- **Workarounds**: Consider using alternative asset formats or platform-specific content pipelines

### Content Pipeline

Ensure all assets are properly processed through the MonoGame Content Pipeline:
- Sprites should be in supported formats (PNG recommended)
- Audio files should use compatible formats
- Fonts require proper Content Pipeline processing

## Future Enhancements

Planned improvements for upcoming versions:
- Resolve DesktopGL asset synchronization issues
- Add more enemy types and behaviors
- Implement power-up system
- Add level progression
- Improve visual effects and animations

## Contributing

This project welcomes contributions! Areas that need attention:
- DesktopGL platform compatibility fixes
- Additional game features
- Performance optimizations
- Bug fixes and testing

## License

This project is licensed under the BSD-3-Clause License. See the [LICENSE](LICENSE) file for details.

---

**Happy coding!** Build amazing games with MonoGame and VB.NET!