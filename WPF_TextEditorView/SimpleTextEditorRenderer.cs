using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_TextEditorView
{
    internal class SimpleTextEditorRenderer : TextEditorRenderer
    {
        public SimpleTextEditorRenderer(IntPtr hdc, int bufferWidth, int bufferHeight) : base(hdc, bufferWidth, bufferHeight)
        {

        }

        protected override void Render(Rectangle square)
        {
            using (Graphics g = Graphics.FromHdc(hdc))
                g.FillRectangle(Brushes.Red, 0, 0, 100, 100);
        }
    }
}
