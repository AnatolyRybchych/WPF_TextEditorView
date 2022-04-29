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

        private int textOffsetLeft;
        public int TextOffsetLeft 
        {
            get => textOffsetLeft;
            protected set
            {
                textOffsetLeft = value;
                TextOffsetsChanged?.Invoke();
            }
        }

        private int textOffsetTop;
        public int TextOffsetTop
        {
            get => textOffsetTop;
            set
            {
                textOffsetTop = value;
                TextOffsetsChanged?.Invoke();
            }
        }

        private int textOffsetRight;
        public int TextOffsetRight
        {
            get => textOffsetRight;
            set
            {
                textOffsetRight = value;
                TextOffsetsChanged?.Invoke();
            }
        }

        private int textOffsetBottom;
        public int TextOffsetBottom
        {
            get => textOffsetBottom;
            set
            {
                textOffsetBottom = value;
                TextOffsetsChanged?.Invoke();
            }
        }

        public event Action TextOffsetsChanged;

        public int TextRenderWidth => Math.Max(BufferWidth - TextOffsetRight - TextOffsetLeft, 0);
        public int TextRenderHeight => Math.Max(BufferHeight - TextOffsetTop - TextOffsetBottom, 0);

        protected StringBuilder observableText;

        protected TextEditorRenderer(IntPtr hdc, int bufferWidth, int bufferHeight, StringBuilder observableText) : base(hdc, bufferWidth, bufferHeight)
        {
            if (observableText == null) throw new ArgumentNullException("observableText");
            if (hdc == IntPtr.Zero) throw new ArgumentNullException("hdc");
            if (observableText.Length != 0) throw new ArgumentException("observableText should be empty on init");
            if (bufferWidth <= 0 || bufferHeight <= 0) throw new ArgumentException("buffer shold have size value > 0");

            textOffsetLeft = 0;
            textOffsetTop = 0;
            textOffsetRight = 0;
            textOffsetBottom = 0;

            this.observableText = observableText;
        }
    }
}
