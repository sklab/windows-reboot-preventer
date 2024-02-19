using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace WindowsRebootPreventer
{
    public partial class WindowsRebootPreventer : Form
    {
        delegate void DelegateProcess();

        private const string COUNTDOWN_MESSAGE = "Start reboot prevention after {0} second(s).";

        private const int COUNTDONW_SECONDS = 10;

        private bool _skipCountdown = false;
        private int _duration = 0;

        public WindowsRebootPreventer()
        {
            InitializeComponent();
        }

        private void WindowsRebootPreventer_Load(object sender, EventArgs e)
        {
            Initialize();
        }

        private void WindowsRebootPreventer_Activated(object sender, EventArgs e)
        {
            _duration = COUNTDONW_SECONDS;
            timer1.Tick += Countdown;
            timer1.Start();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            _skipCountdown = true;

            StartRebootPrevention();

            this.Visible = false;
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = true;
            this.ShowInTaskbar = true;
        }

        private void Initialize()
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("Exit", null, (sender, e) => Application.Exit());

            notifyIcon1.Icon = Properties.Resources.icon;
            notifyIcon1.ContextMenuStrip = menu;
            notifyIcon1.Text = Application.ProductName;
            notifyIcon1.Visible = true;

            lblMessage.Text = string.Format(COUNTDOWN_MESSAGE, COUNTDONW_SECONDS);
        }

        private void ShowSaveFileDialog()
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = Application.ProductName;
                saveFileDialog.ShowDialog();
            }
        }

        private void StartRebootPrevention()
        {
            // Get position bottom right corner.
            int x = Screen.PrimaryScreen.WorkingArea.Width - 10;
            int y = Screen.PrimaryScreen.WorkingArea.Height - 10;

            Task.Run(() =>
            {
                var proc = new DelegateProcess(ShowSaveFileDialog);
                this.Invoke(proc);
            });

            Task.Run(() =>
            {
                Thread.Sleep(1000);
                // Move for Bottom right corner.
                Win32Util.SetWindowPosition(Application.ProductName, x, y);
            });
        }

        private void Countdown(object sender, EventArgs e)
        {
            if (_skipCountdown)
            {
                timer1.Stop();
                return;
            }

            if (_duration == 0)
            {
                timer1.Stop();
                BtnStart.Enabled = false;
                StartRebootPrevention();
                this.Visible = false;
            }
            else if (_duration > 0)
            {
                _duration--;
                lblMessage.Text = string.Format(COUNTDOWN_MESSAGE, _duration);
                this.Refresh();
            }
        }
    }
}
