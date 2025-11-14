using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MobileView.Winforms.Utilities
{
    // could've and should've just made a usercontrol but i dont really like the fact that the added usercontrols
    // disappear when the project is cleaned especially considering how often i cleaned before rebuilding this project during testing.
    public class TitleBar
    {
        private Form _ParentForm;
        private Panel _Panel;
        private Label _Label;
        private Button _CloseButton;
        private Button _MinimizeButton;
        private Button _MaximizeButton;
        private bool isMaximized = false;

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")] public extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")] public extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        public TitleBar(Form parentForm, Panel panel, Label formLabel = null, Button closeButton = null, Button minimizeButton = null, Button maximizeButton = null)
        {
            _ParentForm = parentForm;
            _Panel = panel;
            _Label = formLabel;
            _CloseButton = closeButton;
            _MinimizeButton = minimizeButton;
            _MaximizeButton = maximizeButton;

            if (_Panel == null) return;
            _Panel.MouseDown += Panel_MouseDown;
            if (_Label != null) _Label.MouseDown += Panel_MouseDown;
            if (_CloseButton != null) _CloseButton.Click += CloseButton_Click;
            if (_MinimizeButton != null) _MinimizeButton.Click += MinimizeButton_Click;
            if (_MaximizeButton != null) _MaximizeButton.Click += MaximizeButton_Click;
        }
        private void Panel_MouseDown(object? sender, MouseEventArgs e)
        {
            if (this._ParentForm != null)
            {
                ReleaseCapture();
                SendMessage(this._ParentForm.Handle, 0x112, 0xf012, 0);
            }
        }
        private void CloseButton_Click(object? sender, EventArgs e) => _ParentForm.Close();
        private void MinimizeButton_Click(object? sender, EventArgs e) => MinimizeWindow();
        private void MaximizeButton_Click(object? sender, EventArgs e) => MaximizeWindow();
        public void AttachPanelMouseDownEvent(Panel externalPanel)
        {
            if (externalPanel == null)
                return;

            externalPanel.MouseDown += Panel_MouseDown;
        }
        public void DetachPanelMouseDownEvent(Panel externalPanel)
        {
            if (externalPanel == null)
                return;

            externalPanel.MouseDown -= Panel_MouseDown;
        }
        public void MaximizeWindow()
        {
            if (_ParentForm == null)
                return;

            _ParentForm.WindowState = isMaximized ? FormWindowState.Normal : FormWindowState.Maximized;
            isMaximized = !isMaximized;
        }
        public void MinimizeWindow()
        {
            if (this._ParentForm == null)
                return;
            this._ParentForm.WindowState = FormWindowState.Minimized;
        }
    }
}
