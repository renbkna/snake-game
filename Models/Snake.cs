using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SnakeGame.Models
{
  /// <summary>
  /// Represents the snake in the game
  /// </summary>
  public class Snake
  {
    /// <summary>
    /// List of body segments
    /// </summary>
    private readonly List<SnakeSegment> _body = new();

    /// <summary>
    /// Current direction of the snake
    /// </summary>
    public Direction Direction { get; set; }

    /// <summary>
    /// Color of the snake head
    /// </summary>
    public Color HeadColor { get; set; } = Color.LimeGreen;

    /// <summary>
    /// Color of the snake body
    /// </summary>
    public Color BodyColor { get; set; } = Color.Green;

    /// <summary>
    /// Gets the head position of the snake
    /// </summary>
    public Point HeadPosition => _body.First().Position;

    /// <summary>
    /// Gets the length of the snake
    /// </summary>
    public int Length => _body.Count;

    /// <summary>
    /// Initializes a new snake at the given position
    /// </summary>
    /// <param name="startX">Starting X position</param>
    /// <param name="startY">Starting Y position</param>
    public Snake(int startX, int startY)
    {
      // Initialize snake with length of 3
      _body.Add(new SnakeSegment(new Point(startX, startY), HeadColor));
      _body.Add(new SnakeSegment(new Point(startX - 1, startY), BodyColor));
      _body.Add(new SnakeSegment(new Point(startX - 2, startY), BodyColor));
      Direction = Direction.Right;
    }

    /// <summary>
    /// Moves the snake in the current direction
    /// </summary>
    /// <param name="maxWidth">Grid width</param>
    /// <param name="maxHeight">Grid height</param>
    /// <returns>True if moved successfully, false if collided with self</returns>
    public bool Move(int maxWidth, int maxHeight)
    {
      Point head = _body.First().Position;
      Point newHead = CalculateNewHeadPosition(head, maxWidth, maxHeight);

      // FIXED: Check for collision with any part of the body except the very last segment
      // This is because the tail will move and won't be there when the head arrives
      // We start from index 1 (skipping head) and check all parts except the very last one
      bool collidesWithSelf = false;
      for (int i = 1; i < _body.Count - 1; i++)
      {
        if (_body[i].Position.X == newHead.X && _body[i].Position.Y == newHead.Y)
        {
          collidesWithSelf = true;
          break;
        }
      }

      if (collidesWithSelf)
      {
        return false;
      }

      // Add new head
      _body.Insert(0, new SnakeSegment(newHead, HeadColor));

      // Update colors (previous head becomes body color)
      if (_body.Count > 1)
      {
        _body[1].Color = BodyColor;
      }

      return true;
    }

    /// <summary>
    /// Calculates the position of the new head
    /// </summary>
    private Point CalculateNewHeadPosition(Point currentHead, int maxWidth, int maxHeight)
    {
      Point newHead = currentHead;

      switch (Direction)
      {
        case Direction.Right:
          newHead.X += 1;
          break;
        case Direction.Down:
          newHead.Y += 1;
          break;
        case Direction.Left:
          newHead.X -= 1;
          break;
        case Direction.Up:
          newHead.Y -= 1;
          break;
      }

      // Wrap around the boundaries
      if (newHead.X < 0)
        newHead.X = maxWidth - 1;
      else if (newHead.X >= maxWidth)
        newHead.X = 0;

      if (newHead.Y < 0)
        newHead.Y = maxHeight - 1;
      else if (newHead.Y >= maxHeight)
        newHead.Y = 0;

      return newHead;
    }

    /// <summary>
    /// Grows the snake (keeps the tail instead of removing it)
    /// </summary>
    public void Grow()
    {
      // No action needed as we're keeping the tail, which happens
      // automatically in the move method if we don't remove it
    }

    /// <summary>
    /// Shrinks the snake by removing the tail
    /// </summary>
    public void ShrinkTail()
    {
      if (_body.Count > 0)
      {
        _body.RemoveAt(_body.Count - 1);
      }
    }

    /// <summary>
    /// Checks if the snake contains a given position
    /// </summary>
    public bool Contains(Point position)
    {
      // Check each segment to see if it's at the given position
      // Use coordinate comparison (X,Y values) not reference equality
      return _body.Any(segment =>
          segment.Position.X == position.X &&
          segment.Position.Y == position.Y);
    }

    /// <summary>
    /// Renders the snake on the game surface
    /// </summary>
    public void Render(Graphics g, int gridSize)
    {
      foreach (var segment in _body)
      {
        segment.Render(g, gridSize);
      }
    }

    /// <summary>
    /// Changes the direction of the snake if valid
    /// </summary>
    public void ChangeDirection(Direction newDirection)
    {
      // Prevent 180-degree turns
      if ((Direction == Direction.Right && newDirection == Direction.Left) ||
          (Direction == Direction.Left && newDirection == Direction.Right) ||
          (Direction == Direction.Up && newDirection == Direction.Down) ||
          (Direction == Direction.Down && newDirection == Direction.Up))
      {
        return;
      }

      Direction = newDirection;
    }
  }

  /// <summary>
  /// Represents a segment of the snake's body
  /// </summary>
  public class SnakeSegment : GameObject
  {
    public SnakeSegment(Point position, Color color) : base(position, color)
    {
    }

    public override void Render(Graphics g, int gridSize)
    {
      using var brush = new SolidBrush(Color);

      // Calculate exact grid position
      int x = Position.X * gridSize;
      int y = Position.Y * gridSize;

      // Create rectangle with a small margin to make segments visually distinct
      int margin = 1;
      var rect = new Rectangle(
          x + margin,
          y + margin,
          gridSize - (margin * 2),
          gridSize - (margin * 2));

      // Fill the segment
      g.FillRectangle(brush, rect);

      // Simple highlight for 3D effect
      var highlightSize = gridSize / 5;
      using var highlightBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
      g.FillEllipse(
          highlightBrush,
          x + margin + 1,
          y + margin + 1,
          highlightSize,
          highlightSize);
    }
  }
}
