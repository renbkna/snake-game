using System;
using System.Drawing;
using System.Windows.Forms;
using SnakeGame.Models;

namespace SnakeGame
{
  /// <summary>
  /// Main game engine that manages game logic
  /// </summary>
  public class GameEngine
  {
    // Game objects
    private Snake? _snake;
    private Food? _food;

    // Game state
    private GameState _gameState = GameState.Menu;
    private int _score = 0;
    private int _highScore = 0;
    private int _level = 1;

    // Grid properties
    private readonly int _gridSize = 25;
    private int _gridWidth;
    private int _gridHeight;

    // Game settings
    private int _baseSpeed = 100;
    private bool _gridEnabled = true;
    private Color _backgroundColor = Color.FromArgb(15, 15, 40);
    private Color _gridColor = Color.FromArgb(30, 30, 50);
    private Color _snakeColor = Color.Green;
    private int _difficulty = 2; // 1-Easy, 2-Medium, 3-Hard

    // Events
    public event EventHandler<int>? ScoreChanged;
    public event EventHandler? GameOver;

    /// <summary>
    /// Gets the current score
    /// </summary>
    public int Score => _score;

    /// <summary>
    /// Gets the high score
    /// </summary>
    public int HighScore => _highScore;

    /// <summary>
    /// Gets the current level
    /// </summary>
    public int Level => _level;

    /// <summary>
    /// Gets the current game state
    /// </summary>
    public GameState GameState => _gameState;

    /// <summary>
    /// Gets the current speed of the game based on level and difficulty
    /// </summary>
    public int Speed
    {
      get
      {
        int difficultyModifier = _difficulty switch
        {
          1 => 20, // Easy - slower
          3 => -20, // Hard - faster
          _ => 0 // Medium - normal
        };

        return _baseSpeed - (_level * 5) + difficultyModifier;
      }
    }

    /// <summary>
    /// Gets or sets whether the grid is enabled
    /// </summary>
    public bool GridEnabled
    {
      get => _gridEnabled;
      set => _gridEnabled = value;
    }

    /// <summary>
    /// Gets or sets the snake color
    /// </summary>
    public Color SnakeColor
    {
      get => _snakeColor;
      set
      {
        _snakeColor = value;
        if (_snake != null)
        {
          _snake.BodyColor = value;
          _snake.HeadColor = GetLighterColor(value);
        }
      }
    }

    /// <summary>
    /// Gets or sets the difficulty level
    /// </summary>
    public int Difficulty
    {
      get => _difficulty;
      set => _difficulty = Math.Clamp(value, 1, 3);
    }

    /// <summary>
    /// Initializes a new game engine
    /// </summary>
    /// <param name="gameWidth">Game area width</param>
    /// <param name="gameHeight">Game area height</param>
    public GameEngine(int gameWidth, int gameHeight)
    {
      ResizeGameArea(gameWidth, gameHeight);
    }

    /// <summary>
    /// Resizes the game area
    /// </summary>
    public void ResizeGameArea(int width, int height)
    {
      _gridWidth = width / _gridSize;
      _gridHeight = height / _gridSize;
    }

    /// <summary>
    /// Initializes a new game
    /// </summary>
    public void StartNewGame()
    {
      _score = 0;
      _level = 1;
      _snake = new Snake(_gridWidth / 2, _gridHeight / 2)
      {
        BodyColor = _snakeColor,
        HeadColor = GetLighterColor(_snakeColor)
      };
      _food = Food.GenerateRandom(_gridWidth, _gridHeight, _snake);
      _gameState = GameState.Playing;

      // Notify score change
      ScoreChanged?.Invoke(this, _score);
    }

    /// <summary>
    /// Creates a lighter version of a color for the snake head
    /// </summary>
    private Color GetLighterColor(Color color)
    {
      return Color.FromArgb(
        color.A,
        Math.Min(255, color.R + 50),
        Math.Min(255, color.G + 50),
        Math.Min(255, color.B + 50)
      );
    }

    /// <summary>
    /// Updates the game state
    /// </summary>
    public void Update()
    {
      if (_gameState != GameState.Playing || _snake == null || _food == null)
        return;

      // Store previous head position before moving
      Point previousHead = _snake.HeadPosition;

      // Move the snake
      bool moved = _snake.Move(_gridWidth, _gridHeight);

      // Check for collision with self
      if (!moved)
      {
        HandleGameOver();
        return;
      }

      // Get current head position after movement
      Point currentHead = _snake.HeadPosition;

      // Check if food is eaten with simple exact position matching
      bool collision = (currentHead.X == _food.Position.X && currentHead.Y == _food.Position.Y);

      if (collision)
      {
        // Add score
        _score += _food.Value;
        ScoreChanged?.Invoke(this, _score);

        // Update high score if needed
        if (_score > _highScore)
          _highScore = _score;

        // Generate new food
        _food = Food.GenerateRandom(_gridWidth, _gridHeight, _snake);

        // Snake grows (we already added a new head in the Move method, so we just don't remove the tail)

        // Check for level up (every 50 points)
        if (_score % 50 == 0)
        {
          _level = (_score / 50) + 1;
        }
      }
      else
      {
        // Remove the tail
        _snake.ShrinkTail();
      }
    }

    /// <summary>
    /// Handles game over state
    /// </summary>
    private void HandleGameOver()
    {
      _gameState = GameState.GameOver;
      GameOver?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Changes the snake direction based on key input
    /// </summary>
    public void HandleKeyInput(Keys key)
    {
      switch (_gameState)
      {
        case GameState.Menu:
          if (key == Keys.Enter || key == Keys.Space)
          {
            StartNewGame();
          }
          break;

        case GameState.Playing:
          if (_snake == null) return;

          switch (key)
          {
            case Keys.Up:
              _snake.ChangeDirection(Direction.Up);
              break;
            case Keys.Down:
              _snake.ChangeDirection(Direction.Down);
              break;
            case Keys.Left:
              _snake.ChangeDirection(Direction.Left);
              break;
            case Keys.Right:
              _snake.ChangeDirection(Direction.Right);
              break;
            case Keys.P:
            case Keys.Escape:
              PauseGame();
              break;
          }
          break;

        case GameState.Paused:
          if (key == Keys.P || key == Keys.Escape || key == Keys.Space)
          {
            ResumeGame();
          }
          break;

        case GameState.GameOver:
          if (key == Keys.Enter || key == Keys.Space)
          {
            StartNewGame();
          }
          break;
      }
    }

    /// <summary>
    /// Pauses the game
    /// </summary>
    public void PauseGame()
    {
      if (_gameState == GameState.Playing)
      {
        _gameState = GameState.Paused;
      }
    }

    /// <summary>
    /// Resumes the game
    /// </summary>
    public void ResumeGame()
    {
      if (_gameState == GameState.Paused)
      {
        _gameState = GameState.Playing;
      }
    }

    /// <summary>
    /// Renders the game on the provided graphics surface
    /// </summary>
    public void Render(Graphics g)
    {
      // Fill background
      g.Clear(_backgroundColor);

      // Draw grid if enabled
      if (_gridEnabled)
      {
        using Pen gridPen = new Pen(_gridColor);

        // Draw vertical lines
        for (int x = 0; x <= _gridWidth; x++)
        {
          g.DrawLine(gridPen, x * _gridSize, 0, x * _gridSize, _gridHeight * _gridSize);
        }

        // Draw horizontal lines
        for (int y = 0; y <= _gridHeight; y++)
        {
          g.DrawLine(gridPen, 0, y * _gridSize, _gridWidth * _gridSize, y * _gridSize);
        }
      }

      // Draw game objects based on current state
      switch (_gameState)
      {
        case GameState.Playing:
        case GameState.Paused:
          // Draw snake and food
          _snake?.Render(g, _gridSize);
          _food?.Render(g, _gridSize);

          // Draw score
          DrawGameInfo(g);

          // If paused, draw pause overlay
          if (_gameState == GameState.Paused)
          {
            DrawPausedOverlay(g);
          }
          break;

        case GameState.Menu:
          DrawMenuScreen(g);
          break;

        case GameState.GameOver:
          // Still show the game state but with overlay
          _snake?.Render(g, _gridSize);
          _food?.Render(g, _gridSize);
          DrawGameInfo(g);
          DrawGameOverOverlay(g);
          break;
      }
    }

    /// <summary>
    /// Draws the game information (score, level)
    /// </summary>
    private void DrawGameInfo(Graphics g)
    {
      // Score text
      string scoreText = $"Score: {_score}";
      string highScoreText = $"High Score: {_highScore}";
      string levelText = $"Level: {_level}";

      using Font gameFont = new Font("Segoe UI", 12, FontStyle.Bold);
      using Brush textBrush = new SolidBrush(Color.White);
      using Brush shadowBrush = new SolidBrush(Color.Black);

      // Add shadow effect
      g.DrawString(scoreText, gameFont, shadowBrush, 12, 12);
      g.DrawString(scoreText, gameFont, textBrush, 10, 10);

      g.DrawString(highScoreText, gameFont, shadowBrush, 12, 42);
      g.DrawString(highScoreText, gameFont, textBrush, 10, 40);

      g.DrawString(levelText, gameFont, shadowBrush, 12, 72);
      g.DrawString(levelText, gameFont, textBrush, 10, 70);
    }

    /// <summary>
    /// Draws the menu screen
    /// </summary>
    private void DrawMenuScreen(Graphics g)
    {
      string title = "SNAKE GAME";
      string startText = "Press ENTER to Start";
      string controlsText = "Use Arrow Keys to Move";

      using Font titleFont = new Font("Arial", 30, FontStyle.Bold);
      using Font instructionsFont = new Font("Arial", 16, FontStyle.Regular);

      // Draw title with shadow
      var titleSize = g.MeasureString(title, titleFont);
      var titleX = (_gridWidth * _gridSize - titleSize.Width) / 2;
      var titleY = (_gridHeight * _gridSize - titleSize.Height) / 2 - 50;

      using (Brush shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
      {
        g.DrawString(title, titleFont, shadowBrush, titleX + 3, titleY + 3);
      }

      using (Brush titleBrush = new SolidBrush(Color.LimeGreen))
      {
        g.DrawString(title, titleFont, titleBrush, titleX, titleY);
      }

      // Draw instructions
      using (Brush textBrush = new SolidBrush(Color.White))
      {
        var startSize = g.MeasureString(startText, instructionsFont);
        var startX = (_gridWidth * _gridSize - startSize.Width) / 2;
        g.DrawString(startText, instructionsFont, textBrush, startX, titleY + 80);

        var controlsSize = g.MeasureString(controlsText, instructionsFont);
        var controlsX = (_gridWidth * _gridSize - controlsSize.Width) / 2;
        g.DrawString(controlsText, instructionsFont, textBrush, controlsX, titleY + 120);
      }

      // Draw a snake silhouette in the background
      DrawSnakeSilhouette(g);
    }

    /// <summary>
    /// Draws a snake silhouette on the menu screen
    /// </summary>
    private void DrawSnakeSilhouette(Graphics g)
    {
      using var snakeBrush = new SolidBrush(Color.FromArgb(40, 0, 255, 0));

      // Create a path for the snake
      int segments = 20;
      Point[] points = new Point[segments];

      int centerX = _gridWidth * _gridSize / 2;
      int centerY = _gridHeight * _gridSize / 2;

      for (int i = 0; i < segments; i++)
      {
        double angle = (i * 0.5) % (2 * Math.PI);
        int radius = 150 - (i * 5);

        points[i] = new Point(
            (int)(centerX + Math.Cos(angle) * radius),
            (int)(centerY + Math.Sin(angle) * radius)
        );
      }

      // Draw segments
      for (int i = 0; i < segments; i++)
      {
        int size = 30 - (i / 2);
        g.FillEllipse(snakeBrush,
            points[i].X - size / 2,
            points[i].Y - size / 2,
            size, size);
      }
    }

    /// <summary>
    /// Draws the paused game overlay
    /// </summary>
    private void DrawPausedOverlay(Graphics g)
    {
      // Semi-transparent overlay
      using (Brush overlayBrush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
      {
        g.FillRectangle(overlayBrush, 0, 0, _gridWidth * _gridSize, _gridHeight * _gridSize);
      }

      // Paused text
      string pausedText = "PAUSED";
      string resumeText = "Press P or ESC to Resume";

      using Font pausedFont = new Font("Arial", 30, FontStyle.Bold);
      using Font resumeFont = new Font("Arial", 16, FontStyle.Regular);
      using Brush textBrush = new SolidBrush(Color.White);

      var pausedSize = g.MeasureString(pausedText, pausedFont);
      var pausedX = (_gridWidth * _gridSize - pausedSize.Width) / 2;
      var pausedY = (_gridHeight * _gridSize - pausedSize.Height) / 2 - 30;

      g.DrawString(pausedText, pausedFont, textBrush, pausedX, pausedY);

      var resumeSize = g.MeasureString(resumeText, resumeFont);
      var resumeX = (_gridWidth * _gridSize - resumeSize.Width) / 2;

      g.DrawString(resumeText, resumeFont, textBrush, resumeX, pausedY + 60);
    }

    /// <summary>
    /// Draws the game over overlay
    /// </summary>
    private void DrawGameOverOverlay(Graphics g)
    {
      // Semi-transparent overlay
      using (Brush overlayBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
      {
        g.FillRectangle(overlayBrush, 0, 0, _gridWidth * _gridSize, _gridHeight * _gridSize);
      }

      // Game over text
      string gameOverText = "GAME OVER";
      string scoreText = $"Your Score: {_score}";
      string newGameText = "Press ENTER to Play Again";

      using Font gameOverFont = new Font("Arial", 30, FontStyle.Bold);
      using Font scoreFont = new Font("Arial", 20, FontStyle.Regular);
      using Font newGameFont = new Font("Arial", 16, FontStyle.Regular);

      using Brush titleBrush = new SolidBrush(Color.Red);
      using Brush textBrush = new SolidBrush(Color.White);

      var gameOverSize = g.MeasureString(gameOverText, gameOverFont);
      var gameOverX = (_gridWidth * _gridSize - gameOverSize.Width) / 2;
      var gameOverY = (_gridHeight * _gridSize - gameOverSize.Height) / 2 - 60;

      // Draw with shadow effect
      g.DrawString(gameOverText, gameOverFont, new SolidBrush(Color.FromArgb(150, 0, 0, 0)),
          gameOverX + 3, gameOverY + 3);
      g.DrawString(gameOverText, gameOverFont, titleBrush, gameOverX, gameOverY);

      var scoreSize = g.MeasureString(scoreText, scoreFont);
      var scoreX = (_gridWidth * _gridSize - scoreSize.Width) / 2;

      g.DrawString(scoreText, scoreFont, textBrush, scoreX, gameOverY + 60);

      var newGameSize = g.MeasureString(newGameText, newGameFont);
      var newGameX = (_gridWidth * _gridSize - newGameSize.Width) / 2;

      g.DrawString(newGameText, newGameFont, textBrush, newGameX, gameOverY + 110);
    }
  }
}
