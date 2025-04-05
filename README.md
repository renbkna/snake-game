# Snake Game

A modern, feature-rich implementation of the classic Snake game built with C# and Windows Forms.

![Snake Game Screenshot](screenshot.png)

## Features

- Smooth animation with anti-aliasing graphics
- Multiple difficulty levels
- Customizable snake color
- Special food items that give bonus points
- Level progression with increasing speed
- Game state system (menu, playing, paused, game over)
- High score tracking
- Sound effects
- Menu system with settings
- Grid display toggle
- Responsive design that adapts to window size

## Controls

- **Arrow Keys**: Control the snake's direction
- **P** or **ESC**: Pause/Resume the game
- **Enter** or **Space**: Start game (from menu or after game over)

## Menu Options

### Game

- **New Game**: Start a new game
- **Pause Game**: Pause/Resume the current game
- **Exit**: Quit the application

### Options

- **Settings**: Open settings dialog to customize the game
- **Show Grid**: Toggle grid visibility
- **Enable Sound**: Toggle sound effects

### Help

- **About**: Display game information

## Settings

The settings dialog allows you to customize:

1. **Grid visibility**: Show or hide the grid lines
2. **Sound**: Enable or disable sound effects
3. **Difficulty**: Choose between Easy, Medium, or Hard
4. **Snake Color**: Customize the color of the snake

## Gameplay

- The snake moves around the screen and must eat food to grow
- Each food item eaten earns points and makes the snake longer
- Special gold food appears occasionally for bonus points
- The game speed increases as you progress through levels
- If the snake collides with itself, the game is over
- The snake can pass through the edges of the screen and appear on the opposite side

## Building and Running

1. Ensure you have .NET 8.0 SDK or later installed
2. Open the solution in Visual Studio or your preferred IDE
3. Build the solution
4. Run the application

## Project Structure

- **Models/**: Contains game object models (Snake, Food, etc.)
- **MainForm.cs**: Main game window and user interface
- **GameEngine.cs**: Core game logic and rendering
- **SoundManager.cs**: Handles game audio
- **SettingsForm.cs**: Settings dialog for game customization

## Development

This project is structured following object-oriented principles with clean separation of concerns:

- **Game Objects**: Snake and food are modeled as separate classes
- **Game Engine**: Handles game state, updates, and rendering
- **UI Layer**: Manages user interaction and display

## License

MIT License - Feel free to use, modify, and distribute this code.
