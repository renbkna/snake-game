using System;
using System.Windows.Forms;

namespace SnakeGame
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable visual styles
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Create and run the main form
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                // Show any unhandled exceptions
                MessageBox.Show($"An error occurred: {ex.Message}\n\n{ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
