# Devour-Man - MonoGame StartKit (VB.NET)

A multi-platform game template built with **VB.NET** for [MonoGame](https://www.monogame.net/), inspired by Pac-Man.

For version notes of this project, please refer to the [Features](#features) section.

## Overview

_Devour-Man_ is a relatively simple game template that demonstrates modern MonoGame development practices using VB.NET. Players collect seeds to grow a forest while dodging patrolling insects. This project showcases a true object-oriented architecture with cross-platform support.

> **Important Cross-Platform Note**: While this project is designed to be cross-platform, it has been **primarily validated on UWP (Universal Windows Platform)**. Other platforms have limited support and may require additional configuration or have known limitations.

This template serves as an excellent foundation for developers looking to build 2D games with MonoGame across multiple platforms, using Visual Basic .NET for the core game logic. However, it remains in development and is not a feature-complete game template. **Please stay tuned for version 1.1.0 for full feature implementation.**

## Project Architecture

This project follows a clean architecture pattern with:

- **Shared Core**: VB.NET game logic (`MonoGameStartKitVB.Core`)
- **Platform Launchers**: Platform-specific projects (WindowsDX, Android, iOS, DesktopGL)
- **True OOP Design**: Proper `IDisposable` implementation, event-driven architecture
- **Content Pipeline**: Centralized asset management

## Getting Started

### Prerequisites
- Visual Studio 2026 or later
- [.NET 10.0](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- MonoGame 3.8+ installation
- VB.NET development tools

### Critical: The Solution File Format

The solution file has been changed from `slnx` into the `sln` format, perfect for older versions of Visual Studio. If you would like the latest `slnx` format, simply change the `sln` file to `slnx`, and replace everything in the solution file with the following:

```xml
<Solution>
  <Project Path="MonoGameStartKitVB.Core\MonoGameStartKitVB.Core.vbproj"/>
  <Project Path="MonoGameStartKitVB.WindowsDX\MonoGameStartKitVB.WindowsDX.csproj"/>
  <Project Path="MonoGameStartKitVB.DesktopGL\MonoGameStartKitVB.DesktopGL.csproj"/>
  <Project Path="MonoGameStartKitVB.Android\MonoGameStartKitVB.Android.csproj"/>
  <Project Path="MonoGameStartKitVB.iOS\MonoGameStartKitVB.iOS.csproj"/>
</Solution>
```

## Features

### Version 1.0.3 Updates
- ✅ **IDisposable Implementation**: Proper disposal pattern for sound manager and renderer
- ✅ **Backward-Compatible Solution**: Updated solution file format for broader Visual Studio compatibility

### Version 1.0.2 Updates
- ✅ **Object-Oriented Architecture**: Refactored to true object-oriented patterns for better code organization, like `GameManager`, `Renderer`, `SoundManager`, etc.
- ✅ **Improved Code Structure**: Enhanced maintainability and extensibility
- ✅ **Event-Driven Design**: Better separation of concerns with event handling

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

This template supports multiple platforms with a shared VB.NET core and platform-specific launchers:

### ✅ WindowsDX (DirectX)
- **Status**: Fully Supported
- **Features**: All game features work perfectly
- **Performance**: Optimal for Windows desktop
- **Recommended**: Primary development platform

### ⚠️ Android
- **Status**: Untested
- **Deployment**: May require additional setup

### ⚠️ iOS
- **Status**: Untested
- **Deployment**: May require additional setup

### ⚠️ DesktopGL (OpenGL)
- **Status**: Limited Support
- **Known Limitations**:
  - Background music synchronization issues
  - Font file loading inconsistencies
  - Recommended for testing only

### Building and Running

1. **Clone or download** the project
2. **Open** the solution in Visual Studio
3. **Restore NuGet packages** if prompted
4. **Set startup project** based on target platform:
   - **WindowsDX**: For Windows desktop development (recommended)
   - **Android**: For mobile development and testing
   - **iOS**: For Apple mobile development (requires macOS)
   - **DesktopGL**: For cross-platform testing (with limitations)
5. **Build and run** the project

### Recommended Development Workflow

1. **Primary Development**: Use **WindowsDX** for full feature testing and debugging
2. **Mobile Testing**: Test with **Android** for mobile functionality
3. **Cross-Platform Validation**: Use **DesktopGL** for compatibility testing
4. **Asset Management**: Ensure all assets are properly configured through the Content Pipeline

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