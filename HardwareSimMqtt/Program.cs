using HardwareSimMqtt.HardwareHub;
using HardwareSimMqtt.UIComponent;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HardwareSimMqtt
{
    internal static class Program
    {
        private static ListenerWindow _wnd;
        public static ListenerWindow WndHandle
        {
            get
            {
                if (_wnd == null)
                {
                    _wnd = new ListenerWindow();
                }
                return _wnd;
            }
        }

        private static Form _wnd2;
        public static Form WndHandle2
        {
            get
            {
                if (_wnd2 == null)
                {
                    _wnd2 = new Form();
                    _wnd2.Size = new Size(574, 563);
                    _wnd2.MinimumSize = new Size(_wnd2.Size.Width, _wnd2.Size.Height);
                    _wnd2.Text = "Main";
                    _wnd2.StartPosition = FormStartPosition.CenterScreen;
                    _wnd2.AutoSizeMode = AutoSizeMode.GrowOnly;
                    _wnd2.Dock = DockStyle.Fill;
                }
                return _wnd2;
            }
        }

        private static CancellationTokenSource _cancellationTokenSource;
        public static CancellationTokenSource CancelTokenSource
        {
            get
            {
                if (_cancellationTokenSource == null)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                }
                return _cancellationTokenSource;
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //MainSystem aa = new MainSystem();
            //Application.Run(WndHandle2);
            Application.Run(WndHandle);

        }
    }
}
