using System.Drawing;

namespace SnakeGame.Models
{
  /// <summary>
  /// Base class for objects in the game
  /// </summary>
  public abstract class GameObject
  {
    /// <summary>
    /// Position on the game grid
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    /// Color of the game object
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// Creates a new game object
    /// </summary>
    /// <param name="position">Initial position</param>
    /// <param name="color">Object color</param>
    protected GameObject(Point position, Color color)
    {
      Position = position;
      Color = color;
    }

    /// <summary>
    /// Renders the game object
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="gridSize">Size of grid cells</param>
    public abstract void Render(Graphics g, int gridSize);
  }
}
