# PacDessert1436.MonoGame.StartKitVB - Devour-Man: Eat Seeds & Plant a Forest!

## Overview

**Devour-Man** is a minimal viable game template built with **VB.NET** for [MonoGame](https://www.monogame.net/), inspired by Pac-Man. Players collect seeds to grow a forest and dodge patrolling insects.

This project offers a working starting point for making MonoGame games in VB.NET - with a clean structure, event-driven states, and super-simple geometric placeholder art - ideal for rapid prototyping, learning, and extending. _The template is an "early spring planting" - a community-starter seed for learning and exploring MonoGame with VB.NET._

> **Note:** This is a **minimal viable** template, currently supporting only the **MonoGame DesktopGL** platform. It is unfinished and intended for education or prototyping - not for shipping production games as-is!

## Features

- **Pac-Man-like Gameplay**: Collect seeds for points, grow a forest, avoid hazards
- **Simple Controls**: Arrow keys or WASD for movement, Enter to start/retry
- **Basic Score System**: Tracks score, seeds, and trees planted
- **Game States**: Title, running game, and game over/restart support
- **Procedural Level**: Seeds and enemies spawned randomly every game
- **Minimalist Graphics**: Circles/squares - easy to replace/skin with your own sprites later!
- **Clean Source**: Code organized for easy extension. Modern event/state management for beginners.

## Platform Support

- **Tested and designed for:** MonoGame DesktopGL (Windows/Linux/Mac)
  - Mobile and UWP are **not** supported out-of-the-box (host layers are C#; core gameplay is VB.NET).
  - For mobile or other platform adaptations, **additional work** is required!

## Installation (as a .NET Template)

1. **Install from NuGet:**
    ```bash
    dotnet new install PacDessert1436.MonoGame.StartKitVB::1.0.0
    ```
    Or, if you have the `.nupkg` locally:
    ```bash
    dotnet new install ./PacDessert1436.MonoGame.StartKitVB.1.0.0.nupkg
    ```

2. **Create a new project:**
    ```bash
    dotnet new mgstartkit-vb -n MyDevourManGame
    cd MyDevourManGame
    ```

3. **Build & Run:**
    ```bash
    dotnet build
    dotnet run
    ```

> _If you run into path or .NET SDK issues, see the MonoGame and .NET CLI [documentation](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-new) or check dependencies listed on the [MonoGame website](https://www.monogame.net/downloads/)._

## How to Play

1. **Press ENTER** at the title screen
2. **Move**: Arrow keys or WASD (desktop keyboard)
3. **Collect Seeds** (green): For every 10, plant a tree!
4. **Avoid Insects** (red): Touching them ends your run
5. **Earn Points**: Seeds and trees both increase your score
6. **Restart**: Press ENTER after Game Over

## Project Structure

- `GameMain.vb` - Game loop & main logic
- `Actor.vb` - Entity base classes (Player, Enemy, Seed, Tree)
- `Essentials.vb` - Game constants, config, and state management
- `Content/Fonts/GameFont.spritefont` - Only required asset (UI font)
- `LICENSE` - BSD 3-Clause License and usage terms

## Customization & Extending

You can easily:
- **Change Colors/Sprites:** Swap geometric drawing for your own images, update sprite fonts, etc.
- **Tweak Difficulty:** Adjust constants in `Essentials.vb` (player speed, enemy speed, size, etc.)
- **Add Features:** More enemy types, powerups, sound, high-score system - start building your arcade!
- **Port to Other Platforms:** Advanced users can rework the host/deployment layer for mobile if desired (requires C# interop).

## Contributing

Contributions are welcome! Open issues or PRs to:
- Improve art/assets or add new sprites/UI
- Generalize the template for other platforms
- Clean up/documentation help
- Bug fixes and suggestions

## License

This project is licensed under the **BSD 3-Clause License**. See the [LICENSE](LICENSE) file for details.