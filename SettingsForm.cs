using System;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeGame
{
  /// <summary>
  /// Form for game settings
  /// </summary>
  public partial class SettingsForm : Form
  {
    // Settings properties
    public bool ShowGrid { get; private set; }
    public bool SoundEnabled { get; private set; }
    public int Difficulty { get; private set; } // 1-Easy, 2-Medium, 3-Hard
    public Color SnakeColor { get; private set; }

    // UI controls
    private CheckBox _showGridCheckBox = new();
    private CheckBox _soundEnabledCheckBox = new();
    private RadioButton _easyRadioButton = new();
    private RadioButton _mediumRadioButton = new();
    private RadioButton _hardRadioButton = new();
    private GroupBox _difficultyGroupBox = new();
    private Button _snakeColorButton = new();
    private Button _okButton = new();
    private Button _cancelButton = new();
    private ColorDialog _colorDialog = new();

    /// <summary>
    /// Initializes a new settings form
    /// </summary>
    public SettingsForm(bool showGrid, bool soundEnabled, int difficulty, Color snakeColor)
    {
      InitializeComponent();

      // Set default values
      ShowGrid = showGrid;
      SoundEnabled = soundEnabled;
      Difficulty = difficulty;
      SnakeColor = snakeColor;

      // Initialize UI with current values
      _showGridCheckBox.Checked = ShowGrid;
      _soundEnabledCheckBox.Checked = SoundEnabled;

      // Set difficulty selection
      switch (Difficulty)
      {
        case 1:
          _easyRadioButton.Checked = true;
          break;
        case 2:
          _mediumRadioButton.Checked = true;
          break;
        case 3:
          _hardRadioButton.Checked = true;
          break;
        default:
          _mediumRadioButton.Checked = true;
          break;
      }

      // Set snake color button
      _snakeColorButton.BackColor = SnakeColor;
    }

    /// <summary>
    /// Initializes the form components
    /// </summary>
    private void InitializeComponent()
    {
      // Main form settings
      this.Text = "Game Settings";
      this.Size = new Size(350, 350);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.BackColor = Color.FromArgb(30, 30, 70);
      this.ForeColor = Color.White;
      this.Font = new Font("Segoe UI", 10);

      // Create controls
      _showGridCheckBox = new CheckBox
      {
        Text = "Show Grid",
        Location = new Point(30, 30),
        Size = new Size(200, 25),
        Checked = true,
        ForeColor = Color.White
      };

      _soundEnabledCheckBox = new CheckBox
      {
        Text = "Enable Sound",
        Location = new Point(30, 60),
        Size = new Size(200, 25),
        Checked = true,
        ForeColor = Color.White
      };

      _difficultyGroupBox = new GroupBox
      {
        Text = "Difficulty",
        Location = new Point(30, 100),
        Size = new Size(280, 120),
        ForeColor = Color.White
      };

      _easyRadioButton = new RadioButton
      {
        Text = "Easy",
        Location = new Point(20, 30),
        Size = new Size(200, 25),
        ForeColor = Color.White
      };

      _mediumRadioButton = new RadioButton
      {
        Text = "Medium",
        Location = new Point(20, 55),
        Size = new Size(200, 25),
        Checked = true,
        ForeColor = Color.White
      };

      _hardRadioButton = new RadioButton
      {
        Text = "Hard",
        Location = new Point(20, 80),
        Size = new Size(200, 25),
        ForeColor = Color.White
      };

      _snakeColorButton = new Button
      {
        Text = "Snake Color",
        Location = new Point(30, 230),
        Size = new Size(120, 30),
        BackColor = Color.Green,
        ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat
      };

      _okButton = new Button
      {
        Text = "OK",
        DialogResult = DialogResult.OK,
        Location = new Point(130, 270),
        Size = new Size(80, 30),
        BackColor = Color.FromArgb(70, 130, 70),
        ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat
      };

      _cancelButton = new Button
      {
        Text = "Cancel",
        DialogResult = DialogResult.Cancel,
        Location = new Point(230, 270),
        Size = new Size(80, 30),
        BackColor = Color.FromArgb(130, 70, 70),
        ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat
      };

      _colorDialog = new ColorDialog
      {
        Color = Color.Green,
        FullOpen = true
      };

      // Add difficulty radio buttons to group box
      _difficultyGroupBox.Controls.Add(_easyRadioButton);
      _difficultyGroupBox.Controls.Add(_mediumRadioButton);
      _difficultyGroupBox.Controls.Add(_hardRadioButton);

      // Add all controls to form
      this.Controls.Add(_showGridCheckBox);
      this.Controls.Add(_soundEnabledCheckBox);
      this.Controls.Add(_difficultyGroupBox);
      this.Controls.Add(_snakeColorButton);
      this.Controls.Add(_okButton);
      this.Controls.Add(_cancelButton);

      // Wire up events
      _snakeColorButton.Click += SnakeColorButton_Click;
      _okButton.Click += OkButton_Click;

      // Set accept and cancel buttons
      this.AcceptButton = _okButton;
      this.CancelButton = _cancelButton;
    }

    /// <summary>
    /// Handles the snake color button click
    /// </summary>
    private void SnakeColorButton_Click(object? sender, EventArgs e)
    {
      _colorDialog.Color = SnakeColor;
      if (_colorDialog.ShowDialog() == DialogResult.OK)
      {
        SnakeColor = _colorDialog.Color;
        _snakeColorButton.BackColor = SnakeColor;
      }
    }

    /// <summary>
    /// Handles the OK button click
    /// </summary>
    private void OkButton_Click(object? sender, EventArgs e)
    {
      // Save settings
      ShowGrid = _showGridCheckBox.Checked;
      SoundEnabled = _soundEnabledCheckBox.Checked;

      // Get difficulty
      if (_easyRadioButton.Checked)
        Difficulty = 1;
      else if (_mediumRadioButton.Checked)
        Difficulty = 2;
      else if (_hardRadioButton.Checked)
        Difficulty = 3;
    }
  }
}
