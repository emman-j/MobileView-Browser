using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileView.Classes
{
    public class FormManager
    {
        private readonly Form _form;
        public string BorderUsed { get; private set; }

        public FormManager(Form form)
        {
            _form = form;
            _form.Resize += Form_Resize;
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            if (_form.WindowState != FormWindowState.Normal) return;

            int newWidth;
            int newHeight;

            if (BorderUsed == "top" || BorderUsed == "bottom")
            {
                double aspectRatio = 16.0 / 9.0;
                newHeight = _form.Height;
                newWidth = (int)(newHeight / aspectRatio);

                if (_form.Width != newWidth)
                {
                    _form.Width = newWidth;
                }
                return;
            }

            double defaultAspectRatio = 9.0 / 16.0;
            newWidth = _form.Width;
            newHeight = (int)(newWidth / defaultAspectRatio);

            if (_form.Height != newHeight)
            {
                _form.Height = newHeight;
            }
        }
        public void HandleWndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            if (m.Msg == WM_NCHITTEST)
            {
                switch ((int)m.Result)
                {
                    case 10: BorderUsed = "left"; break;
                    case 11: BorderUsed = "right"; break;
                    case 12: BorderUsed = "top"; break;
                    case 15: BorderUsed = "bottom"; break;
                    case 13: BorderUsed = "topleft"; break;
                    case 14: BorderUsed = "topright"; break;
                    case 16: BorderUsed = "botleft"; break;
                    case 17: BorderUsed = "botright"; break;
                }
            }
        }
        public void PreserveCurrentFormLocationAndSize(Form currentForm)
        {
            if (currentForm == null) { return; }
            _form.Size = currentForm.Size;
            var state = currentForm.WindowState;
            var location = currentForm.Location;

            _form.WindowState = state;
            _form.StartPosition = FormStartPosition.Manual;

            var centerX = location.X + (currentForm.Width - _form.Width) / 2;
            var centerY = location.Y + (currentForm.Height - _form.Height) / 2;
            _form.Location = new Point(centerX, centerY);
        }
    }
}
