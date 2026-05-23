# Gameplay of Devour-Man (MonoGame StartKit for VB.NET)

## Game Mechanics

1. Player controls Devour-Man using the joystick at the bottom of the screen (on mobile devices), or uses the arrow keys to move (on desktop devices, but joystick will be animated based on keyboard input). The players starts at the **bottom** of the maze as this property shows:
```vb
Public ReadOnly Property PlayerStartingPoint As New Point(MAZE_WIDTH \ 2, MAZE_HEIGHT - 2)
```
2. Enemies randomly move around the maze but never turns around 180 degrees. When the player collects a pesticide, the enemies' color will change to light blue.
3. Pesticides allow players to kill enemies through contact, but only for a short period of time. An enemy, when killed, will reappear in another cell after 5 seconds.
4. The new cell where the enemy reappears still cannot be too close to the player (ensure: manhattan distance > 5). When it reappears, it turns back to normal color and won't hurt the player within 2 seconds.
5. Player starts with 3 lives, and loses a life when it collides with an enemy with normal color. Death animation is played when a life is lost. Losing all lives ends the game.
6. A sapling will grow into a tree when the player picks up a packet of seeds. **The goal for each level is to pick up all the seed packets and grow all the trees in the play area.**

## Level Mechanics

| Level | Seed Type | Enemy Type | Tree Type |
|:---:|:---:|:---:|:---:|
| 1 | Acorn | Beetle | Pine Tree |
| 2 | Berry | Caterpillar | Fruit Tree |
| 3 | Nut | Beetle | Oak Tree |
| 4 | Acorn | Caterpillar | Pine Tree |
| 5 | Berry | Beetle | Fruit Tree |
| 6 | Nut | Caterpillar | Oak Tree |

Level 7 goes back to the mechanics of Level 1, Level 8 uses the mechanics of Level 2, and so forth.

## Scoring System

Player earns points by: 
- collecting seed packets (10 points each)
- killing enemies (50 points each)
- collecting pesticides (15 points each)

## Frame Distribution for Each Sprite Sheet

```yml
- player_sheet.png:
    - [1, 2]: moving left
    - [3, 4]: moving right
    - [5, 6]: moving up
    - [7, 8]: moving down
    - [9, 16]: death animation
- enemy_sheet.png:
    - [1, 2]: bettle moving left
    - [3, 4]: bettle moving right
    - [5, 6]: bettle moving up
    - [7, 8]: bettle moving down
    - [9, 10]: caterpillar moving left
    - [11, 12]: caterpillar moving right
    - [13, 14]: caterpillar moving up
    - [15, 16]: caterpillar moving down
- object_sheet.png:
    - [1]: fence
    - [2]: acorn seed packet
    - [3]: berry seed packet
    - [4]: nut seed packet
    - [5]: pine tree
    - [6]: fruit tree
    - [7]: oak tree
    - [8]: sapling
    - [9]: pesticide
- icon_sheet.png:
    - [1]: life icon
    - [2]: acorn seed icon
    - [3]: berry seed icon
    - [4]: nut seed icon
```

## Expected UI Layout
1UP: (player score)  HI: (high score)
(life icons) SEASON xx (seed icon)


(play area rendered with RenderTarget2D)


    (joystick at the bottom)

### Notes on the Layout

1. `xx` stands for level number, like `SEASON 1`, which serves as a homage to the arcade game Libble Rabble. 
2. The pause button is located at the **bottom-left** of the screen.
3. In the title screen, there will be a title card inside the play area. The "Start" and "Exit" buttons (using `general_button.png` as the button texture) are located at the bottom of the title screen.
4. On game start, there will be a "GET READY!" message displayed on the screen within 5 seconds, with `game_start.wav` played at this time. When a player enters the next level, the message behaves the same but with `at_next_level.wav` played.
5. Icons are stored in `icon_sheet.png`, all 12x12 pixels for each icon. The first icon is a life icon, and the rest are seed icons (acorn, berry & nut).
6. All the buttons turns light grey the moment they are pressed, returning to the original color when released. The joystick never changes its color.