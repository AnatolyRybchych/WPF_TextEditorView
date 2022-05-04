using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_TextEditorView
{
    public class TextEditorRendererGDIWordByWord : TextEditorRendererGDI
    {
        private LinkedList<uint> lineLenghts;
        private TextLines lines;

        public TextEditorRendererGDIWordByWord(IntPtr hdc, int bufferWidth, int bufferHeight) : base(hdc, bufferWidth, bufferHeight)
        {
            lineLenghts = new LinkedList<uint>();
            lineLenghts.AddLast(0);
            RequireBufferRedraw();
            lines = new TextLines();

        }

        public override uint GetCharIndexFromTextRenderRectPoint(int x, int y)
        {
            throw new NotImplementedException();
        }

        public override uint GetTextHeightPixels()
        {
            throw new NotImplementedException();
        }

        public override Size GetTextSizePixels(string text)
        {
            Size sz;
            WinApi.GetTextExtentPoint32W(BackBufferHdc, text, text.Length, out sz);
            return sz;
        }

        public override uint GetTextWidthPixels()
        {
            throw new NotImplementedException();
        }

        protected override void OnFontChanged()
        {
            base.OnFontChanged();
            RequireBufferRedraw();
        }

        protected override void OnBufferChangedDc(IntPtr newHdc)
        {
            RequireBufferRedraw();
        }

        protected override void OnSetingCaretes()
        {
        }

        protected override void OnSettingHorisontalScrollPixels()
        {
        }

        protected override void OnSettingSelections()
        {
        }

        protected override void OnSettingVerticalScrollPixels()
        {
        }

        protected override void OnTextAppend(TextPasting snippet)
        {
            TextLines.AppendTextInfo appendInfo;
            lines.AppendText(snippet, out appendInfo);

            DrawBg(new Rectangle(0, 0, BufferWidth, BufferHeight));
            Font.DrawText(
                lines.GetLines(appendInfo.FirstEditedLine, (int)appendInfo.EditedLinesCount),
                new RECT(TextOffsetLeft, TextOffsetTop + FontHeight * (int)appendInfo.FirstEditedLineIndex, BufferWidth - TextOffsetRight, BufferWidth - TextOffsetBottom),
                DrawTextFormat.DT_LEFT);
        }

        protected void DrawBg(Rectangle rect)
        {
            using (Graphics g = Graphics.FromHdc(BackBufferHdc))
                g.FillRectangle(Brushes.Orange, rect);
        }

        protected override void OnTextRemove(Range range, string removedText)
        {
        }

        protected override void RedrawBuffer()
        {
            DrawBg(new Rectangle(0, 0, BufferWidth, BufferHeight));

            Font.DrawText(
                Text,
                new RECT(TextOffsetLeft, TextOffsetTop, BufferWidth - TextOffsetRight, BufferWidth - TextOffsetBottom),
                DrawTextFormat.DT_LEFT);
        }
    }
}
