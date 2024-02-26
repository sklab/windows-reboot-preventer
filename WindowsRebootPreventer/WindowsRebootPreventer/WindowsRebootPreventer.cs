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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace WindowsRebootPreventer
{
    public partial class WindowsRebootPreventer : Form
    {
        delegate void DelegateProcess();

        private const string COUNTDOWN_MESSAGE = "Start reboot prevention after {0} second(s).";

        private const string ACTTIVE_HOURS_REGKEY = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WindowsUpdate\UX\Settings";
        private const string ACTTIVE_HOURS_START_VALUE_NAME = "ActiveHoursStart";
        private const string ACTTIVE_HOURS_END_VALUE_NAME = "ActiveHoursEnd";

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
            timerCountdown.Tick += Countdown;
            timerCountdown.Start();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            _skipCountdown = true;

            BtnStart.Enabled = false;

            //StartRebootPreventionByDialog();
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

            // Check for current user can access registry.
            if (!Win32Util.CanAccessRegistry())
            {
                lblMessage.Text = "Error: You don't have administrator role. \r\nPlease execute by [Run as Administrator]. ";
                BtnStart.Enabled = false;
                return;
            }

            // Get App.config setting
            string waitTime = System.Configuration.ConfigurationManager.AppSettings.Get("WAIT_TIME");
            string intervalForUpdate = System.Configuration.ConfigurationManager.AppSettings.Get("INTERVAL_FOR_UPDATE");

            // Validation for config
            if (!IsValidRange(waitTime, 0, 3600)) {
                lblMessage.Text = "Error: Invlid setting from [WAIT_TIME] in App.config";
                BtnStart.Enabled = false;
                return;
            }
            if (!IsValidRange(intervalForUpdate, 1, 1440))
            {
                lblMessage.Text = "Error: Invlid setting from [INTERVAL_FOR_UPDATE] in App.config";
                BtnStart.Enabled = false;
                return;
            }

            _duration = int.Parse(waitTime);
            timerInterval.Interval = int.Parse(intervalForUpdate) * 60 * 1000;

            lblMessage.Text = string.Format(COUNTDOWN_MESSAGE, _duration);
        }


        private void Countdown(object sender, EventArgs e)
        {
            if (_skipCountdown)
            {
                timerCountdown.Stop();
                return;
            }

            if (_duration == 0)
            {
                timerCountdown.Stop();
                BtnStart.PerformClick();
            }
            else if (_duration > 0)
            {
                _duration--;
                lblMessage.Text = string.Format(COUNTDOWN_MESSAGE, _duration);
                this.Refresh();
            }
        }

        private bool IsValidRange(string value, int min, int max)
        {
            int num;
            bool isNumber = int.TryParse(value, out num);
            if (!isNumber) return false;

            if (min <= num && num <= max) return true;

            return false;
        }

        private void StartRebootPrevention()
        {
            UpdateActiveHours(null, null);

            timerInterval.Tick += UpdateActiveHours;
            timerInterval.Start();
        }


        private void UpdateActiveHours(object sender, EventArgs e)
        {
            var start = DateTime.Now.Hour;
            var end = DateTime.Now.AddHours(12.0).Hour;
            Win32Util.SetRegistryValue(ACTTIVE_HOURS_REGKEY, ACTTIVE_HOURS_START_VALUE_NAME, start);
            Win32Util.SetRegistryValue(ACTTIVE_HOURS_REGKEY, ACTTIVE_HOURS_END_VALUE_NAME, end);
#if DEBUG
            var updatedStart = Win32Util.GetRegistryValue(ACTTIVE_HOURS_REGKEY, ACTTIVE_HOURS_START_VALUE_NAME);
            var updatedEnd = Win32Util.GetRegistryValue(ACTTIVE_HOURS_REGKEY, ACTTIVE_HOURS_END_VALUE_NAME);
            Console.WriteLine(DateTime.Now.ToString() 
                + " updatedStart=" + updatedStart.ToString() 
                + " updatedEnd=" + updatedEnd.ToString());
#endif
        }

        #region Obsolute code

        [Obsolete]
        private void ShowSaveFileDialog()
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = Application.ProductName;
                saveFileDialog.ShowDialog();
            }
        }

        [Obsolete]
        private void StartRebootPreventionByDialog()
        {
            // Get position bottom right corner.
            int x = Screen.PrimaryScreen.WorkingArea.Width - 110;
            int y = Screen.PrimaryScreen.WorkingArea.Height - 110;

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

        #endregion

    }
}
