using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SnakeGame.Models
{
  /// <summary>
  /// Represents food items that the snake can eat
  /// </summary>
  public class Food : GameObject
  {
    /// <summary>
    /// Points value of this food
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Random number generator
    /// </summary>
    private static readonly Random Random = new();

    /// <summary>
    /// Creates a new food item
    /// </summary>
    /// <param name="position">Position on the grid</param>
    /// <param name="color">Color of the food</param>
    /// <param name="value">Point value</param>
    public Food(Point position, Color color, int value = 10) : base(position, color)
    {
      Value = value;
    }

    /// <summary>
    /// Generates a random food position
    /// </summary>
    /// <param name="gridWidth">Width of the grid</param>
    /// <param name="gridHeight">Height of the grid</param>
    /// <param name="snake">Snake to avoid overlapping with</param>
    /// <returns>New food instance</returns>
    public static Food GenerateRandom(int gridWidth, int gridHeight, Snake snake)
    {
      Point position;

      // Further constrain valid positions to ensure food is fully visible
      // Keep a 1-cell safety margin from all edges
      int minX = 1;
      int minY = 1;
      int maxX = Math.Max(1, gridWidth - 2);  // At least 1
      int maxY = Math.Max(1, gridHeight - 2); // At least 1

      // If grid is too small, fall back to using the whole grid
      if (maxX <= minX || maxY <= minY)
      {
        minX = 0;
        minY = 0;
        maxX = Math.Max(0, gridWidth - 1);
        maxY = Math.Max(0, gridHeight - 1);
      }

      // Safety counter to prevent infinite loops
      int attempts = 0;
      const int maxAttempts = 100;

      // Safely generate food positions
      do
      {
        int x = Random.Next(minX, maxX + 1);
        int y = Random.Next(minY, maxY + 1);
        position = new Point(x, y);

        attempts++;

        // If we tried too many times, just use any valid position
        if (attempts > maxAttempts)
        {
          // Last resort - find any position on the grid not occupied by snake
          for (int testX = 0; testX < gridWidth; testX++)
          {
            for (int testY = 0; testY < gridHeight; testY++)
            {
              position = new Point(testX, testY);
              if (!snake.Contains(position))
              {
                // Found a non-snake spot, use it
                break;
              }
            }
          }
          break;
        }
      } while (snake.Contains(position));

      // Random type of food (different colors and values)
      Color color;
      int value;
      int foodType = Random.Next(0, 10);

      if (foodType == 0)  // Special food (10% chance)
      {
        color = Color.Gold;
        value = 30;
      }
      else
      {
        color = Color.Red;
        value = 10;
      }

      return new Food(position, color, value);
    }

    /// <summary>
    /// Renders the food on the game grid
    /// </summary>
    public override void Render(Graphics g, int gridSize)
    {
      // Calculate exact grid cell location
      int x = Position.X * gridSize;
      int y = Position.Y * gridSize;

      // Draw a simple circle filling most of the grid cell without any rotation or complex effects
      using (var brush = new SolidBrush(Color))
      {
        // Size it to fit within the grid with a small margin
        int padding = 2;
        var rect = new Rectangle(
            x + padding,
            y + padding,
            gridSize - (padding * 2),
            gridSize - (padding * 2));

        // Fill the main circle
        g.FillEllipse(brush, rect);

        // Add a small highlight for 3D effect
        using var highlightBrush = new SolidBrush(Color.FromArgb(150, 255, 255, 255));
        g.FillEllipse(
            highlightBrush,
            x + gridSize / 4,
            y + gridSize / 4,
            gridSize / 5,
            gridSize / 5);
      }
    }
  }
}
