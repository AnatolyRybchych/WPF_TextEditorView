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
            Font = new GdiFont(BackBufferHdc, "Times New Roman", 32, 400, 0);
        }

        public override void Resize(int width, int heigth)
        {
            base.Resize(width, heigth);
            Font.SetHdc(BackBufferHdc);
        }

        protected override void OnFontChanged()
        {
            Font = new GdiFont(BackBufferHdc, FontFace, FontHeight, FontWeight, FontWidth);
        }
    }
}
