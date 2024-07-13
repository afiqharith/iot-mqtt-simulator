using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModelInterface
{
    public static class SystemHelper
    {
        public static void AppendRichTextBox(RichTextBox textbox, string text, Color color)
        {
            if (textbox.InvokeRequired)
            {
                Action safeThread = delegate { SystemHelper.AppendRichTextBox(textbox, text, color); };
                textbox.Invoke(safeThread);
            }
            else
            {
                textbox.SelectionStart = textbox.TextLength;
                textbox.SelectionLength = 0;

                textbox.SelectionColor = color;
                textbox.AppendText(DateTime.Now.ToString("HH:mm:ss,fff") + String.Format(" {0}", text) + Environment.NewLine);
                textbox.SelectionColor = textbox.ForeColor;
                textbox.ScrollToCaret();
            }
        }
    }
}
