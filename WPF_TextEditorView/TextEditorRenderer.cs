using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_TextEditorView
{
    internal abstract class TextEditorRenderer : Renderer
    {
        //ranges represented by characters indices before removing
        public abstract void OnTextRemove(Range[] ranges);
        //indices represented by characters indices before appending
        public abstract void OnTextAppend(TextPasting[] pastingSnippets);
        public abstract void OnSetingCaretes(uint[] indices);
        public abstract void OnSettingSelections(Range[] selections);
        public abstract void OnFontChanged(string faceName, int width, int heigth, int weight);

        public abstract Size GetTextSize(string text);
        public abstract int TextX { get; }

        protected StringBuilder observableText;

        protected TextEditorRenderer(IntPtr hdc, int bufferWidth, int bufferHeight, StringBuilder observableText) : base(hdc, bufferWidth, bufferHeight)
        {
            this.observableText = observableText;
        }
    }
}
