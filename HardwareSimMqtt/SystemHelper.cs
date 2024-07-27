using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HardwareSimMqtt
{
    public static class SystemHelper
    {
        public static void AppendRichTextBox(RichTextBox textbox, string text, Color color)
        {
            SafeInvoke(textbox, () =>
            {
                textbox.SelectionStart = textbox.TextLength;
                textbox.SelectionLength = 0;

                textbox.SelectionColor = color;
                textbox.AppendText(DateTime.Now.ToString("HH:mm:ss,fff") + String.Format(" {0}", text) + Environment.NewLine);
                textbox.SelectionColor = textbox.ForeColor;
                textbox.ScrollToCaret();
            });
        }

        public static void SafeInvoke(Control control, Action action)
        {
            if (control == null || control.IsDisposed || control.Disposing)
            {
                return;
            }

            if (control.InvokeRequired)
            {
                try
                {
                    control.Invoke(new Action(() => SafeInvoke(control, action)));
                }
                catch (InvalidAsynchronousStateException)
                {

                }
            }
            else
            {
                action();
            }
        }
    }
}
