using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AutoClicker
{
    public partial class Form1 : Form
    {
        bool isAutoClicking = false;

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr hookId = IntPtr.Zero;
        private LowLevelKeyboardProc keyboardProcDelegate;

        public Form1()
        {
            InitializeComponent();
            keyboardProcDelegate = HookCallback;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr IParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)0x0100 || wParam == (IntPtr)0x0101))
            {
                if (Marshal.ReadInt32(IParam) == 0x046 && wParam == (IntPtr)0x0100)
                {
                    if (true)
                    {
                        if (!isAutoClicking)
                        {
                            StartAutoClick();
                        }

                        else if (isAutoClicking)
                        {
                            StopAutoClick();
                        }

                    }
                }
            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, IParam);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            hookId = SetHook(keyboardProcDelegate);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            UnhookWindowsHookEx(hookId);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(13, proc, GetModuleHandle(curModule.ModuleName), 0);
            }

        }

        private void AutoClicker_Tick(object sender, System.EventArgs e)
        {
             mouse_event(0x002, 0, 0, 0, 0);
             mouse_event(0x004, 0, 0, 0, 0);
        }

        private void StartAutoClick()
        {
            if (!isAutoClicking)
            {
                AutoClicker.Interval = (int)(1000.0 / cpsBar.Value);
                AutoClicker.Start();
                isAutoClicking = true;
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            }

        }

        private void StopAutoClick()
        {
            if (isAutoClicking)
            {
                AutoClicker.Stop();
                isAutoClicking= false;
                btnStart.Enabled= true;
                btnStop.Enabled= false;    
            }

        }

        private void btnStart_Click(object sender, System.EventArgs e)
        {
            StartAutoClick();
        }

        private void btnStop_Click(object sender, System.EventArgs e)
        {
            StopAutoClick();
        }

        private void cpsBar_Scroll(object sender, ScrollEventArgs e)
        {
            cpsText.Text = cpsBar.Value.ToString();
        }

        private void siticoneCheckBox1_CheckedChanged(object sender, System.EventArgs e)
        {
            if (siticoneCheckBox1.Checked == true)
            {
                this.ShowInTaskbar = true;
            }

            else
            {
                this.ShowInTaskbar = false;
            }
        }
    }
}
