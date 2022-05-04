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
        private IntPtr hdc;

        public IntPtr Font { get; private set; }
        public BkMode BkMode { get; private set; }

        public GdiFont(IntPtr hdc, string face, int height, int weight = 400, int width = 0)
        {
            this.hdc = hdc;
            Font = CreateFontW(height, width, 0, 0, weight, 0, 0, 0, 0, 0, 0, CLEARTYPE_QUALITY, 0, face);
            Select();
            SetBkMode(BkMode.TRANSPARENT);
            BkMode = BkMode.TRANSPARENT;
        }

        public virtual void SetHdc(IntPtr hdc)
        {
            this.hdc = hdc;
            Select();
            SetBkMode(BkMode);
        }

        public void SetBkMode(BkMode mode)
        {
            WinApi.SetBkMode(hdc, mode);
        }

        public void Select()
        {
            SelectObject(hdc, Font);
        }

        public System.Drawing.Size GetTextSize(string text)
        {
            System.Drawing.Size res;
            GetTextExtentPoint32W(hdc, text, text.Length, out res);
            return res;
        }

        public void DrawText(string text, RECT rect, DrawTextFormat format)
        {
            DrawTextW(hdc, text, text.Length, ref rect, format);
        }

        public void DrawText(string text, RECT rect, DrawTextFormat format, DrawTextParams @params)
        {
            DrawTextExW(hdc, text, text.Length, ref rect, format, ref @params);
        }

        public ABC GetABC(char ch)
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
