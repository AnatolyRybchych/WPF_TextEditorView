using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WPF_TextEditorView
{
    public class TextEditorIntrinsic: Panel
    {
        public delegate void CharHandler(char ch);
        public event CharHandler Char;

        private const int WM_UNICHAR = 0x0109;
        private const int WM_CHAR = 0x0102;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if(m.Msg == WM_UNICHAR || m.Msg == WM_CHAR)
            {
                char ch = (char)m.WParam;
                if(char.IsControl(ch) == false)
                    Char?.Invoke(ch);
                else if (ch == '\r')
                    Char?.Invoke('\n');
            }
        }
    }
}
