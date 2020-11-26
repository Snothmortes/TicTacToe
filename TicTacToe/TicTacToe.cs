using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

// TODO: Implement AI
// TODO: History
// TODO: Saving application instances and games
// Todo: Player randomizer: ie. dynamic who-goes-first

namespace Game
{
    public partial class TicTacToe : Form
    {
        // Fields
        private int[] board;
        private int player = 1;
        private readonly SolidBrush brush = new SolidBrush(Color.Black);
        private readonly Pen pen = new Pen(Color.Black, 3);
        private readonly CommandManager cm = new CommandManager();

        // Constructor
        public TicTacToe()
        {
            InitializeComponent();
            Init();
        }

        // Logic
        private void EvaluateMove()
        {
            if (Winner())
            {
                if (MessageBox.Show($"Player {player} wins!") == DialogResult.OK)
                    ResetGame();
            }
            else if (Draw())
            {
                if (MessageBox.Show("Draw.") == DialogResult.OK)
                    ResetGame();
            }
        }
        private void PlayerTurn(PictureBox picBox) => Execute(cm, new SetCrossCommand(picBox), picBox);
        private void AITurn()
        {
            if (Array.TrueForAll(board, a => a != 0))
                return;

            var square = new Random();
            int i = 0;
            while (true)
            {
                i = square.Next(0, 8);
                if (board[i] == 0)
                    break;
            }

            foreach (PictureBox item in panel1.Controls)
            {
                var j = i + 1;
                if (item.Name == "pictureBox" + j)
                    Execute(cm, new SetCircleCommand(item), item);
            }
        }
        private void NextTurn()
        {
            EvaluateMove();
            player = player == 1 && !(Winner() || StartOfGame()) ? 2 : 1;
        }
        private bool Winner()
        {
            int[][] perms = new int[8][];

            perms[0] = new int[] { 0, 1, 2 };
            perms[1] = new int[] { 3, 4, 5 };
            perms[2] = new int[] { 6, 7, 8 };
            perms[3] = new int[] { 0, 3, 6 };
            perms[4] = new int[] { 1, 4, 7 };
            perms[5] = new int[] { 2, 5, 8 };
            perms[6] = new int[] { 0, 4, 8 };
            perms[7] = new int[] { 2, 4, 6 };

            foreach (var x in perms)
                if (board[x[0]] == board[x[1]] && board[x[1]] == board[x[2]] && board[x[0]] != 0)
                    return true;

            return false;
        }
        private bool Draw() => Array.TrueForAll(board, a => a > 0);
        private bool StartOfGame() => Array.TrueForAll(board, a => a == 0);
        private void ResetGame()
        {
            board = new int[9];
            Reset();
            player = 1;
        }
        private void Execute(CommandManager commandManager, ICommand command, PictureBox picBox)
        {
            commandManager.SetCommand(command);
            commandManager.Invoke();
            board[picBox.Name.Last() - '1'] = player;
        }

        // Interface
        private void Init()
        {
            board = new int[9];
            foreach (Control control in panel1.Controls)
            {
                OnLoadCompleted(control);
                OnMouseClick(control);
            }
            foreach (Control control in this.Controls)
                if (control.GetType() == typeof(Button))
                    OnButtonClick(control);
            this.undoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            this.redoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y;

        }
        private void RefreshButtons()
        {
            undoButton.Enabled = cm.undoEnabled;
            redoButton.Enabled = cm.redoEnabled;
            undoToolStripMenuItem.Enabled = cm.undoEnabled;
            redoToolStripMenuItem.Enabled = cm.redoEnabled;
        }
        private void OnMouseClick(Control control)
        {
            control.MouseDown += (sender, e) =>
            {
                PictureBox picBox = (PictureBox)sender;
                if (picBox.Image == null)
                {
                    PlayerTurn(picBox);
                    AITurn();
                }
            };
        }

        private void OnButtonClick(Control control)
        {
            control.Click += (sender, e) =>
            {
                Button btn = (Button)sender;
                switch (btn.Tag)
                {
                    case "undo":
                        Undo();
                        break;
                    case "redo":
                        Redo();
                        break;
                    case "reset":
                        Reset();
                        break;
                }
            };
        }
        private void TicTacToe_KeyDown(object sender, KeyEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.Z:
                        Undo();
                        break;
                    case Keys.Y:
                        Redo();
                        break;
                }
            }
        }

        private void Undo() => cm.UndoCommand();
        private void Redo() => cm.RedoCommand();
        private void Reset() => cm.Reset();

        private void undoToolStripMenuItem_Click(object sender, EventArgs e) => Undo();
        private void redoToolStripMenuItem_Click(object sender, EventArgs e) => Redo();
        private void resetToolStripMenuItem_Click(object sender, EventArgs e) => Reset();

        private void OnLoadCompleted(Control control)
        {
            var pb = control as PictureBox;
            pb.ImageChanged += (sender, e) =>
            {
                NextTurn();
                RefreshButtons();
                Debug.WriteLine(player);
            };
        }
    }
}
