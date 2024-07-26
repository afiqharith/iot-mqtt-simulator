﻿using System;
using System.Collections.Generic;
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

        public static CancellationTokenSource CancelTokenSource
        {
            get;
            private set;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CancelTokenSource = new CancellationTokenSource();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new ListenerWindow());
            Application.Run(WndHandle);
        }
    }
}
