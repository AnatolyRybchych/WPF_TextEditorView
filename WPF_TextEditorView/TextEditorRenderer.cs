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
        //  -----------------------------------------------------------
        //  |                    TextoffsetTop                        |
        //  |T   ------------------------------------------------- T  |
        //  |e   |Text Text Text text                 ^          | e  |
        //  |x   |text and text                       |          | x  |
        //  |t  L|                            TextRenderHeight   | t R|
        //  |O  e|                                    |          | O i|
        //  |f  f|                                    |          | f g|
        //  |f  t| <----------TextRenderWidth---------|--------->| f h|
        //  |s   |                                    |          | s t|
        //  |e   |                                    |          | e  |
        //  |t   |-----------------------------------------------| t  |
        //  |                 TextOffsetBottom                        |
        //  ------------------TextRendererCanvas-----------------------

        private int textOffsetTop;
        private int textOffsetLeft;
        private StringBuilder textContent;
        private int textOffsetRight;
        private int textOffsetBottom;

        protected StringBuilder TextSource => textContent;
        public uint[] Caretes { get; private set; }
        public Range[] Selections { get; private set; }
        public string Text => textContent.ToString();
        public event Action TextOffsetsChanged;
        public int TextRenderWidth => Math.Max(BufferWidth - TextOffsetRight - TextOffsetLeft, 0);
        public int TextRenderHeight => Math.Max(BufferHeight - TextOffsetTop - TextOffsetBottom, 0);

        public string FontFace { get; private set; }
        public int FontHeight { get; private set; }
        public int FontWidth { get; private set; }
        public int FontWeight { get; private set; }

        public int TextOffsetLeft
        {
            get => textOffsetLeft;
            protected set
            {
                textOffsetLeft = value;
                TextOffsetsChanged?.Invoke();
            }
        }

        public int TextOffsetTop
        {
            get => textOffsetTop;
            set
            {
                textOffsetTop = value;
                TextOffsetsChanged?.Invoke();
            }
        }


        public int TextOffsetRight
        {
            get => textOffsetRight;
            set
            {
                textOffsetRight = value;
                TextOffsetsChanged?.Invoke();
            }
        }


        public int TextOffsetBottom
        {
            get => textOffsetBottom;
            set
            {
                textOffsetBottom = value;
                TextOffsetsChanged?.Invoke();
            }
        }


        protected abstract void OnTextRemove(Range range);
        protected abstract void OnTextAppend(TextPasting snippet);
        protected abstract void OnSetingCaretes(uint[] indices);
        protected abstract void OnSettingSelections(Range[] selections);
        protected abstract void OnFontChanged(string faceName, int width, int heigth, int weight);
        public abstract Size GetTextPixelSize(string text);


        public void TextRemove(Range range)
        {
            TextSource.Remove((int)range.Index, range.Moving);
            OnTextRemove(range);
        }

        public void TextAppend(TextPasting snippet)
        {
            TextSource.Insert((int)snippet.Index, snippet.Text);
            OnTextAppend(snippet);
        }

        public void SetCaretes(uint[] indices)
        {
            Caretes = indices;
            OnSetingCaretes(indices);
        }

        public void SetSelections(Range[] selections)
        {
            Selections = selections;
            OnSettingSelections(selections);
        }

        public void ChangeFont(string faceName, int width, int heigth, int weight)
        {
            FontFace = faceName;
            FontHeight = heigth;
            FontWidth = width;
            FontWeight = weight;

            OnFontChanged(faceName, width, heigth, width);
        }

        public TextEditorRenderer(IntPtr hdc, int bufferWidth, int bufferHeight) : base(hdc, bufferWidth, bufferHeight)
        {
            if (hdc == IntPtr.Zero) throw new ArgumentNullException("hdc");
            if (bufferWidth <= 0 || bufferHeight <= 0) throw new ArgumentException("buffer shold have size value > 0");

            FontFace = null;
            FontHeight = 0;
            FontWidth = 0;
            FontWeight = 0;

            textContent = new StringBuilder();
            textOffsetLeft = 0;
            textOffsetTop = 0;
            textOffsetRight = 0;
            textOffsetBottom = 0;
        }
    }
}
