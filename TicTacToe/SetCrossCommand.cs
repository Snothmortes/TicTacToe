using System.Drawing;
using System.Windows.Forms;

namespace Game
{
    public class SetCrossCommand : ICommand
    {
        private PictureBox _picBox;
        public SetCrossCommand(PictureBox picBox) => _picBox = picBox;
        public void ExecuteAction() => _picBox.Image = Image.FromFile(@"..\..\cross.bmp");
        public void RedoAction() => ExecuteAction();
        public void UndoAction()
        {
            if (_picBox.Image != null)
                _picBox.Image = null;
        }

    }
}
