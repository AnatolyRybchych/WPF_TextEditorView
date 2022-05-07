using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WPF_TextEditorView.WinApi;

namespace WPF_TextEditorView
{
    public abstract class BufferedTextEditorRenderer : TextEditorRenderer
    {
        private Bitmap backBuffer;
        protected IntPtr BackBufferHdc { get; private set; }
        private bool requiredBufferRedraw;

        protected abstract void RedrawBuffer();
        protected abstract void OnBufferChangedDc(IntPtr newHdc);

        public override void Init(IntPtr hdc, int bufferWidth, int bufferHeight)
        {
            base.Init(hdc, bufferWidth, bufferHeight);
            backBuffer = new Bitmap(BufferWidth, BufferHeight);
            using (Graphics g = Graphics.FromImage(backBuffer))
                BackBufferHdc = g.GetHdc();

            OnBufferChangedDc(BackBufferHdc);
        }

        public void RequireBufferRedraw()
        {
            requiredBufferRedraw = true;
        }

        public override void Resize(int width, int heigth)
        {
            if (width < 1 || heigth < 1) return;
            base.Resize(width, heigth);

            DeleteDC(BackBufferHdc);
            backBuffer.Dispose();

            backBuffer = new Bitmap(width, heigth);
            using (Graphics g = Graphics.FromImage(backBuffer))
                BackBufferHdc = g.GetHdc();

            OnBufferChangedDc(BackBufferHdc);
        }

        protected override void Render(Rectangle square, IntPtr hdc)
        {
            if (requiredBufferRedraw)
            {
                RedrawBuffer();
                requiredBufferRedraw = false;
            }

            BitBlt(hdc, square.Left, square.Top, square.Right - square.Left, square.Bottom - square.Top, BackBufferHdc, square.Left, square.Top, SRCCOPY);
        }
    }
}
