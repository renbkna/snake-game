using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class MainForm : Form
    {
        // Game variables
        private System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        private List<Point> snake = new List<Point>();
        private Point food = Point.Empty;
        private int gridSize = 20;
        private int direction = 0; // 0: Right, 1: Down, 2: Left, 3: Up
        private bool gameOver = false;
        private Random rand = new Random();
        private int score = 0;

        public MainForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.BackColor = Color.Black;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyDown += new KeyEventHandler(OnKeyDown);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            InitializeGame();

            gameTimer.Interval = 100; // Milliseconds
            gameTimer.Tick += new EventHandler(GameTick);
            gameTimer.Start();
        }

        private void InitializeGame()
        {
            snake.Clear();
            // Start with a snake of length 3
            snake.Add(new Point(5, 5));
            snake.Add(new Point(4, 5));
            snake.Add(new Point(3, 5));
            direction = 0;
            score = 0;
            gameOver = false;
            GenerateFood();
        }

        private void GenerateFood()
        {
            while (true)
            {
                int x = rand.Next(0, this.ClientSize.Width / gridSize);
                int y = rand.Next(0, this.ClientSize.Height / gridSize);
                Point newFood = new Point(x, y);
                if (!snake.Contains(newFood))
                {
                    food = newFood;
                    break;
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (direction != 1) direction = 3;
                    break;
                case Keys.Down:
                    if (direction != 3) direction = 1;
                    break;
                case Keys.Left:
                    if (direction != 0) direction = 2;
                    break;
                case Keys.Right:
                    if (direction != 2) direction = 0;
                    break;
            }
        }

        private void GameTick(object sender, EventArgs e)
        {
            if (gameOver)
            {
                gameTimer.Stop();
                DialogResult result = MessageBox.Show($"Game Over! Your score: {score}\nDo you want to play again?", "Game Over", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    InitializeGame();
                    gameTimer.Start();
                }
                else
                {
                    this.Close();
                }
                return;
            }

            // Calculate new head position
            Point head = snake[0];
            Point newHead = head;

            switch (direction)
            {
                case 0: // Right
                    newHead.X += 1;
                    break;
                case 1: // Down
                    newHead.Y += 1;
                    break;
                case 2: // Left
                    newHead.X -= 1;
                    break;
                case 3: // Up
                    newHead.Y -= 1;
                    break;
            }

            // Teleporting borders (wrap around)
            int maxX = this.ClientSize.Width / gridSize;
            int maxY = this.ClientSize.Height / gridSize;

            if (newHead.X < 0)
                newHead.X = maxX - 1;
            else if (newHead.X >= maxX)
                newHead.X = 0;

            if (newHead.Y < 0)
                newHead.Y = maxY - 1;
            else if (newHead.Y >= maxY)
                newHead.Y = 0;

            // Check collision with self
            if (snake.Contains(newHead))
            {
                gameOver = true;
                return;
            }

            // Add new head
            snake.Insert(0, newHead);

            // Check if food is eaten
            if (newHead.Equals(food))
            {
                score += 10;
                GenerateFood();
                // Optionally, increase speed
                if (gameTimer.Interval > 50)
                    gameTimer.Interval -= 2; // Increase speed slightly
            }
            else
            {
                // Remove tail
                snake.RemoveAt(snake.Count - 1);
            }

            this.Invalidate(); // Trigger repaint
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Draw food
            if (!food.IsEmpty)
            {
                Brush foodBrush = Brushes.Red;
                g.FillEllipse(foodBrush, food.X * gridSize, food.Y * gridSize, gridSize, gridSize);
            }

            // Draw snake
            Brush snakeBrush = Brushes.Green;
            foreach (Point p in snake)
            {
                g.FillRectangle(snakeBrush, p.X * gridSize, p.Y * gridSize, gridSize, gridSize);
            }

            // Draw score
            string scoreText = $"Score: {score}";
            Font scoreFont = new Font("Arial", 14);
            Brush scoreBrush = Brushes.White;
            g.DrawString(scoreText, scoreFont, scoreBrush, new PointF(5, 5));
        }
    }
}
