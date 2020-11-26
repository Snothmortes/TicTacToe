using System.Drawing;
using System.Windows.Forms;

namespace Game
{
    class SetCircleCommand : ICommand
    {
        private PictureBox _picBox;
        public SetCircleCommand(PictureBox picBox) => _picBox = picBox;
        public void ExecuteAction()
        {
            _picBox.Image = Image.FromFile(@"..\..\circle.bmp");
        }

        public void RedoAction() => ExecuteAction();
        public void UndoAction()
        {
            if (_picBox.Image != null)
                _picBox.Image = null;
        }
    }
}
