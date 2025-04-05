using System;
using System.IO;
using System.Media;

namespace SnakeGame
{
  /// <summary>
  /// Manages game sounds
  /// </summary>
  public class SoundManager
  {
    private static SoundManager? _instance;

    /// <summary>
    /// Gets the singleton instance
    /// </summary>
    public static SoundManager Instance => _instance ??= new SoundManager();

    /// <summary>
    /// Flag indicating if sound is enabled
    /// </summary>
    public bool SoundEnabled { get; set; } = true;

    // Sound players
    private SoundPlayer? _eatSound;
    private SoundPlayer? _gameOverSound;

    private SoundManager()
    {
      // Initialize sounds
      try
      {
        // These would normally be loaded from resources
        string soundsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");

        if (Directory.Exists(soundsDirectory))
        {
          string eatSoundPath = Path.Combine(soundsDirectory, "eat.wav");
          string gameOverSoundPath = Path.Combine(soundsDirectory, "gameover.wav");

          if (File.Exists(eatSoundPath))
          {
            _eatSound = new SoundPlayer(eatSoundPath);
            _eatSound.LoadAsync();
          }

          if (File.Exists(gameOverSoundPath))
          {
            _gameOverSound = new SoundPlayer(gameOverSoundPath);
            _gameOverSound.LoadAsync();
          }
        }
      }
      catch
      {
        // Ignore sound loading errors
      }
    }

    /// <summary>
    /// Play the food eaten sound
    /// </summary>
    public void PlayEatSound()
    {
      PlaySound(_eatSound);
    }

    /// <summary>
    /// Play the game over sound
    /// </summary>
    public void PlayGameOverSound()
    {
      PlaySound(_gameOverSound);
    }

    /// <summary>
    /// Helper method to play a sound if enabled
    /// </summary>
    private void PlaySound(SoundPlayer? player)
    {
      if (SoundEnabled && player != null)
      {
        try
        {
          player.Play();
        }
        catch
        {
          // Ignore sound playing errors
        }
      }
    }
  }
}
