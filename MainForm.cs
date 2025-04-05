using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SnakeGame.Models;

namespace SnakeGame
{
    public partial class MainForm : Form
    {
        private GameEngine _gameEngine = null!;
        private System.Windows.Forms.Timer _gameTimer = null!;
        private MenuStrip _menuStrip = null!;
        private long _lastKeyPressTicks = 0;

        public MainForm()
        {
            InitializeComponent();

            // Set form properties
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.Text = "Snake Game";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(850, 700);
            this.ClientSize = new Size(830, 650);
            this.KeyPreview = true;

            // Create menu strip first (before initializing its contents)
            _menuStrip = new MenuStrip
            {
                BackColor = Color.FromArgb(30, 30, 60),
                ForeColor = Color.White,
                Renderer = new CustomMenuRenderer()
            };
            this.MainMenuStrip = _menuStrip;
            this.Controls.Add(_menuStrip);

            // Initialize game engine before menu options that depend on it
            _gameEngine = new GameEngine(this.ClientSize.Width, this.ClientSize.Height - _menuStrip.Height);
            _gameEngine.ScoreChanged += GameEngine_ScoreChanged;
            _gameEngine.GameOver += GameEngine_GameOver;

            // Now initialize the menu with game engine available
            InitializeMenu();

            // Set up event handlers
            this.KeyDown += MainForm_KeyDown;
            this.Paint += MainForm_Paint;
            this.Resize += MainForm_Resize;

            // Set up timer
            _gameTimer = new System.Windows.Forms.Timer();
            _gameTimer.Tick += GameTimer_Tick;
            _gameTimer.Interval = _gameEngine.Speed;
            _gameTimer.Start();
        }

        private void InitializeMenu()
        {
            // Game menu
            ToolStripMenuItem gameMenu = new ToolStripMenuItem("Game");

            ToolStripMenuItem newGameItem = new ToolStripMenuItem("New Game");
            newGameItem.Click += (s, e) => _gameEngine.StartNewGame();

            ToolStripMenuItem pauseGameItem = new ToolStripMenuItem("Pause Game");
            pauseGameItem.Click += (s, e) =>
            {
                if (_gameEngine.GameState == GameState.Playing)
                    _gameEngine.PauseGame();
                else if (_gameEngine.GameState == GameState.Paused)
                    _gameEngine.ResumeGame();
            };

            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => this.Close();

            gameMenu.DropDownItems.Add(newGameItem);
            gameMenu.DropDownItems.Add(pauseGameItem);
            gameMenu.DropDownItems.Add(new ToolStripSeparator());
            gameMenu.DropDownItems.Add(exitItem);

            // Options menu
            ToolStripMenuItem optionsMenu = new ToolStripMenuItem("Options");

            ToolStripMenuItem settingsItem = new ToolStripMenuItem("Settings");
            settingsItem.Click += (s, e) => ShowSettings();

            ToolStripMenuItem toggleGridItem = new ToolStripMenuItem("Show Grid");
            toggleGridItem.Checked = _gameEngine.GridEnabled;
            toggleGridItem.Click += (s, e) =>
            {
                toggleGridItem.Checked = !toggleGridItem.Checked;
                _gameEngine.GridEnabled = toggleGridItem.Checked;
                this.Invalidate();
            };

            ToolStripMenuItem toggleSoundItem = new ToolStripMenuItem("Enable Sound");
            toggleSoundItem.Checked = SoundManager.Instance.SoundEnabled;
            toggleSoundItem.Click += (s, e) =>
            {
                toggleSoundItem.Checked = !toggleSoundItem.Checked;
                SoundManager.Instance.SoundEnabled = toggleSoundItem.Checked;
            };

            optionsMenu.DropDownItems.Add(settingsItem);
            optionsMenu.DropDownItems.Add(new ToolStripSeparator());
            optionsMenu.DropDownItems.Add(toggleGridItem);
            optionsMenu.DropDownItems.Add(toggleSoundItem);

            // Help menu
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("Help");

            ToolStripMenuItem aboutItem = new ToolStripMenuItem("About");
            aboutItem.Click += (s, e) => MessageBox.Show(
                "Snake Game\nVersion 1.0\n\nControls:\nArrow keys - Move snake\nP - Pause game\nEsc - Pause game",
                "About Snake Game",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            helpMenu.DropDownItems.Add(aboutItem);

            // Add menus to strip
            _menuStrip.Items.Add(gameMenu);
            _menuStrip.Items.Add(optionsMenu);
            _menuStrip.Items.Add(helpMenu);
        }

        private void ShowSettings()
        {
            // Pause game while settings are open
            bool wasPlaying = _gameEngine.GameState == GameState.Playing;
            if (wasPlaying)
                _gameEngine.PauseGame();

            // Show settings dialog
            using SettingsForm settingsForm = new SettingsForm(
                _gameEngine.GridEnabled,
                SoundManager.Instance.SoundEnabled,
                _gameEngine.Difficulty,
                _gameEngine.SnakeColor);

            if (settingsForm.ShowDialog(this) == DialogResult.OK)
            {
                // Apply settings
                _gameEngine.GridEnabled = settingsForm.ShowGrid;
                SoundManager.Instance.SoundEnabled = settingsForm.SoundEnabled;
                _gameEngine.Difficulty = settingsForm.Difficulty;
                _gameEngine.SnakeColor = settingsForm.SnakeColor;

                // Update menu items to reflect new settings
                foreach (ToolStripMenuItem menuItem in _menuStrip.Items)
                {
                    if (menuItem.Text == "Options")
                    {
                        foreach (ToolStripItem item in menuItem.DropDownItems)
                        {
                            if (item is ToolStripMenuItem mi)
                            {
                                if (mi.Text == "Show Grid")
                                    mi.Checked = settingsForm.ShowGrid;
                                else if (mi.Text == "Enable Sound")
                                    mi.Checked = settingsForm.SoundEnabled;
                            }
                        }
                    }
                }
            }

            // Resume game if it was playing
            if (wasPlaying)
                _gameEngine.ResumeGame();

            // Force redraw
            this.Invalidate();
        }

        private void MainForm_Resize(object? sender, EventArgs e)
        {
            // Update game engine when form is resized
            _gameEngine.ResizeGameArea(this.ClientSize.Width, this.ClientSize.Height - _menuStrip.Height);
            this.Invalidate();
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            // Update the game state
            _gameEngine.Update();

            // Update timer interval based on level
            _gameTimer.Interval = _gameEngine.Speed;

            // Redraw the form
            this.Invalidate();
        }

        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            // If game is not running, handle normally
            if (_gameEngine.GameState != GameState.Playing)
            {
                _gameEngine.HandleKeyInput(e.KeyCode);
                this.Invalidate();
                return;
            }

            // Get current tick count to implement debouncing
            long currentTicks = DateTime.Now.Ticks;

            // Add debounce for direction changes to avoid rapidly changing directions
            // and accidentally hitting yourself
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
                e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                // Store the last key pressed time in a static variable
                if (currentTicks - _lastKeyPressTicks < TimeSpan.TicksPerMillisecond * 100)
                {
                    // Ignore rapid direction changes (less than 100ms apart)
                    return;
                }

                // Update last key press time
                _lastKeyPressTicks = currentTicks;
            }

            // Pass key press to game engine
            _gameEngine.HandleKeyInput(e.KeyCode);

            // Force immediate redraw
            this.Invalidate();
        }

        private void MainForm_Paint(object? sender, PaintEventArgs e)
        {
            // Get graphics context with smoothing enabled
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Adjust drawing area to account for menu
            e.Graphics.TranslateTransform(0, _menuStrip.Height);

            // Let the game engine render everything
            _gameEngine.Render(e.Graphics);
        }

        private void GameEngine_ScoreChanged(object? sender, int score)
        {
            // Play sound when score changes (food eaten)
            SoundManager.Instance.PlayEatSound();
        }

        private void GameEngine_GameOver(object? sender, EventArgs e)
        {
            // Play sound on game over
            SoundManager.Instance.PlayGameOverSound();
        }
    }

    /// <summary>
    /// Custom renderer for menu strip to match game theme
    /// </summary>
    public class CustomMenuRenderer : ToolStripProfessionalRenderer
    {
        public CustomMenuRenderer() : base(new CustomMenuColors())
        {
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Selected)
            {
                base.OnRenderMenuItemBackground(e);
                return;
            }

            Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(60, 60, 100)), rect);
        }
    }

    /// <summary>
    /// Custom colors for menu strip
    /// </summary>
    public class CustomMenuColors : ProfessionalColorTable
    {
        public override Color MenuItemSelected => Color.FromArgb(60, 60, 100);
        public override Color MenuItemSelectedGradientBegin => Color.FromArgb(60, 60, 100);
        public override Color MenuItemSelectedGradientEnd => Color.FromArgb(60, 60, 100);
        public override Color MenuItemPressedGradientBegin => Color.FromArgb(60, 60, 100);
        public override Color MenuItemPressedGradientEnd => Color.FromArgb(60, 60, 100);
        public override Color MenuBorder => Color.FromArgb(40, 40, 80);
        public override Color MenuItemBorder => Color.FromArgb(40, 40, 80);
        public override Color ToolStripDropDownBackground => Color.FromArgb(30, 30, 60);
        public override Color ImageMarginGradientBegin => Color.FromArgb(30, 30, 60);
        public override Color ImageMarginGradientMiddle => Color.FromArgb(30, 30, 60);
        public override Color ImageMarginGradientEnd => Color.FromArgb(30, 30, 60);
    }
}
