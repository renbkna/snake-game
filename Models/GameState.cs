namespace SnakeGame.Models
{
  /// <summary>
  /// Represents the possible states of the snake game
  /// </summary>
  public enum GameState
  {
    /// <summary>
    /// Game is in menu/start screen
    /// </summary>
    Menu,

    /// <summary>
    /// Game is actively running
    /// </summary>
    Playing,

    /// <summary>
    /// Game is paused
    /// </summary>
    Paused,

    /// <summary>
    /// Game is over
    /// </summary>
    GameOver
  }
}
