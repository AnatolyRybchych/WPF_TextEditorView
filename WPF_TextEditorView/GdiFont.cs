using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static WPF_TextEditorView.WinApi;

namespace WPF_TextEditorView
{
    public class GdiFont
    {
        public IntPtr Font { get; private set; }

        public GdiFont(string face, int height, int weight = 400, int width = 0)
        {
            Font = CreateFontW(height, width, 0, 0, weight, 0, 0, 0, 0, 0, 0, CLEARTYPE_QUALITY, 0, face);
        }

        public void Select(IntPtr hdc)
        {
            SelectObject(hdc, Font);
        }

        public static System.Drawing.Size GetTextSize(IntPtr hdc, string text)
        {
            System.Drawing.Size res;
            GetTextExtentPoint32W(hdc, text, text.Length, out res);
            return res;
        }

        public static ABC GetABC(IntPtr hdc, char ch)
        {
            ABC res;
            GetCharABCWidthsW(hdc, ch, ch, out res);
            return res;
        }

        ~GdiFont()
        {
            DeleteObject(Font);
        }
    }
}
