using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_TextEditorView
{
    public abstract class Renderer
    {
        protected IntPtr hdc;
        public int BufferWidth { get; private set; }
        public int BufferHeight { get; private set; }

        public Renderer(IntPtr hdc, int bufferWidth, int bufferHeight)
        {
            this.BufferWidth = bufferWidth;
            this.BufferHeight = bufferHeight;
            this.hdc = hdc;
        }

        public void ForseRender(Rectangle square)
        {
            Render(square, hdc);
        }

        public void ForseRender()
        {
            ForseRender(new Rectangle(0, 0, BufferWidth, BufferHeight));
        }

        protected abstract void Render(Rectangle square, IntPtr hdc);

        public virtual void Resize(int width, int heigth)
        {
            this.BufferWidth = width;
            this.BufferHeight = heigth;
        }
    }
}
