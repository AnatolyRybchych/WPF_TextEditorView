using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_TextEditorView
{
    public abstract class TextEditorRendererGDI : BufferedTextEditorRenderer
    {
        public GdiFont Font { get; private set; }

        protected TextEditorRendererGDI(IntPtr hdc, int bufferWidth, int bufferHeight) : base(hdc, bufferWidth, bufferHeight)
        {
            Font = new GdiFont("Times New Roman", 32);
        }

        protected override void OnFontChanged()
        {
            Font = new GdiFont(FontFace, FontHeight, FontWeight, FontWeight);
        }

        public override Size GetTextSizePixels(string text)
        {
            Font.Select(BackBufferHdc);
            return GdiFont.GetTextSize(BackBufferHdc, text);
        }
    }
}
