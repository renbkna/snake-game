# Snake Game Codebase Guide

This document provides an overview of the snake game codebase to help you understand the structure and purpose of each component.

## Core Files

### Program.cs

- Entry point for the application
- Sets up the Windows Forms application and initializes the main form

### MainForm.cs

- Main game window that hosts the game renderer
- Handles user input (keyboard controls)
- Contains the game loop using a Timer
- Responsible for calling the game engine's update and render methods

### MainForm.Designer.cs

- Auto-generated code for the form's UI components
- Contains the setup code for UI elements created in the Windows Forms designer

### GameEngine.cs

- Core game logic controller
- Manages game state (menu, playing, paused, game over)
- Updates game objects (snake, food)
- Handles collision detection
- Manages scoring and level progression
- Contains rendering logic for drawing the game and UI

### SoundManager.cs

- Handles audio playback for game events
- Loads and plays sound effects for actions like eating food, game over, etc.

### SettingsForm.cs

- Settings dialog for configuring game options
- Allows customization of snake color, difficulty, grid visibility, etc.

## Models Directory

### GameObject.cs

- Base class for all game objects
- Defines common properties like position and color
- Contains a virtual Render method that derived classes override

### Snake.cs

- Implements the snake character
- Manages the snake's body segments as a collection
- Handles movement, growth, and direction changes
- Contains collision detection with itself
- Includes rendering logic for the snake segments

### Food.cs

- Implements the food objects that the snake eats
- Contains logic for random generation of food
- Prevents food from spawning on the snake
- Includes visual effects for the food items

### Direction.cs

- Enum that defines possible movement directions (Up, Down, Left, Right)
- Used to track and control the snake's movement

### GameState.cs

- Enum that defines the different states of the game
- States include Menu, Playing, Paused, and GameOver
- Used by the game engine to control game flow

## Project Files

### SnakeGame.csproj

- Project configuration file for the C# project
- Defines project settings, dependencies, and build configurations

## Other Directories

### Sounds/

- Contains audio files for game sound effects

### bin/

- Contains compiled binaries generated when the game is built

### obj/

- Contains intermediate build files

## Game Flow Overview

1. The game starts from `Program.cs` which initializes `MainForm`
2. `MainForm` creates a `GameEngine` instance and sets up a timer for the game loop
3. On each timer tick, the form calls the engine's `Update()` and `Render()` methods
4. The engine manages the game state and updates the snake and food positions
5. When the player presses arrow keys, `MainForm` passes the input to the engine
6. The engine moves the snake, checks for collisions, and updates the score
7. The game continues until the snake collides with itself (game over)

## Key Gameplay Mechanics

1. **Snake Movement**: The snake moves in one of four directions and wraps around the screen edges
2. **Growth**: The snake grows when it eats food, adding a new segment to its body
3. **Collision Detection**: The game checks for collisions between the snake's head and:
   - Its own body (causing game over)
   - Food items (adding score and length)
4. **Levels**: The game speed increases as the player reaches higher levels
5. **Food Generation**: New food spawns randomly on the grid, never on the snake
