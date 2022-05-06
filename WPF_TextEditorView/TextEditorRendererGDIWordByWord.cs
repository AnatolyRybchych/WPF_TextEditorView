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
            RequireBufferRedraw();
        }

        protected override void OnSettingSelections()
        {
        }

        protected override void OnSettingVerticalScrollPixels()
        {
            RequireBufferRedraw();
        }

        protected override void OnTextAppend(TextPasting snippet)
        {
            TextLines.ChangeTextInfo changeInfo;
            lines.AppendText(snippet, out changeInfo);

            int overtTextCy = FontHeight * changeInfo.FirstChangedLineIndex;
            int textCy = FontHeight * changeInfo.ChangedLinesCount;

            WinApi.BitBlt(BackBufferHdc, TextOffsetLeft, TextOffsetTop, TextRenderWidth, overtTextCy, BackBufferHdc, TextOffsetLeft, TextOffsetTop, WinApi.SRCCOPY);

            DrawBg(new Rectangle(TextOffsetLeft, TextOffsetTop + overtTextCy, TextRenderWidth, textCy));
            DrawText(lines.GetLinesSafe(changeInfo.FirstChangedLineIndex, changeInfo.ChangedLinesCount),
                new RECT(-HorisontalScrollPixels, -(VerticalScrollPixels % FontHeight) + overtTextCy, 0, 0));
        }

        protected virtual void DrawText(string text, RECT margin = new RECT())
        {
            Font.DrawText(
                text,
                new RECT(TextOffsetLeft + margin.Left, TextOffsetTop + margin.Top , BufferWidth - TextOffsetRight - margin.Right, BufferWidth - TextOffsetBottom - margin.Bottom),
                DrawTextFormat.DT_LEFT);
        }

        protected virtual void DrawBg(Rectangle rect)
        {
            using (Graphics g = Graphics.FromHdc(BackBufferHdc))
                g.FillRectangle(Brushes.Orange, rect);
        }

        protected override void OnTextRemove(Range range, string removedText)
        {
            TextLines.ChangeTextInfo changeInfo;
            lines.RemoveText(range, out changeInfo);
            RequireBufferRedraw();
        }

        protected override void RedrawBuffer()
        {
            DrawBg(new Rectangle(0, 0, BufferWidth, BufferHeight));
            DrawText(lines.GetLinesSafe(VerticalScrollPixels / FontHeight, BufferHeight / FontHeight + 2),
                new RECT(-HorisontalScrollPixels, -VerticalScrollPixels % FontHeight, 0, 0));
        }
    }
}
