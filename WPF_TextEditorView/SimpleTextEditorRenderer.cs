using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static WPF_TextEditorView.WinApi;

namespace WPF_TextEditorView
{
    internal class SimpleTextEditorRenderer : TextEditorRenderer
    {
        private IntPtr font;
        private int fontWidth;
        private int fontHeight;
        private int fontWeight;
        private uint[] caretes;
        private Range[] selections;

        public SimpleTextEditorRenderer(IntPtr hdc, int bufferWidth, int bufferHeight, StringBuilder observableText) : base(hdc, bufferWidth, bufferHeight, observableText)
        {
            font = IntPtr.Zero;
            fontWidth = 0;
            fontHeight = 0;
            fontWeight = 0;
            caretes = new uint[0];
            selections = new Range[0];
        }

        public override void OnFontChanged(string faceName, int width, int heigth, int weight)
        {
            fontWidth = width;
            fontHeight = heigth;
            fontWeight = weight;

            if (weight % 100 != 0 || weight < 0 || weight > 900) throw new Exception("Incompatible font weight it should be (0 <= weight => 900) && (weight % 100 == 0)");

            if(font != IntPtr.Zero)
                DeleteObject(font);

            font = CreateFontW(heigth, width, 0, 0, weight, 0, 0, 0, 0, 0, 0, CLEARTYPE_QUALITY, 0, faceName);
        }

        public override void OnSetingCaretes(uint[] indices)
        {
            caretes = indices;
        }

        public override void OnSettingSelections(Range[] selections)
        {
            this.selections = selections;
        }

        public override void OnTextAppend(TextPasting[] pastingSnippets)
        {
        }

        public override void OnTextRemove(Range[] ranges)
        {
        }

        protected override void Render(Rectangle square)
        {
            int l = observableText.Length;

            SelectObject(hdc, font);
            Rectangle rect = new Rectangle(0, 0, BufferWidth, BufferHeight);
            DrawTextW(hdc, observableText.ToString(), -1, ref rect, DrawTextFormat.DT_TOP);
        }

        ~SimpleTextEditorRenderer()
        {
            if(font != IntPtr.Zero)
                DeleteObject(font);
        }
    }
}
