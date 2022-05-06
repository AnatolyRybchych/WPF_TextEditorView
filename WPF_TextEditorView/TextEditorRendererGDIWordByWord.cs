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
        private bool requireCaretesRedraw;

        public TextEditorRendererGDIWordByWord(IntPtr hdc, int bufferWidth, int bufferHeight) : base(hdc, bufferWidth, bufferHeight)
        {
            lineLenghts = new LinkedList<uint>();
            lineLenghts.AddLast(0);
            RequireBufferRedraw();
            lines = new TextLines();
            requireCaretesRedraw = true;

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
            requireCaretesRedraw = true;
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

        private void RenderOffsetTextUnderLine(int line, int offsetLines)
        {
            if(offsetLines == 0) return;

            int y = TextOffsetTop + line * FontHeight + FontHeight;
            int newY = y + offsetLines * FontHeight;

            WinApi.BitBlt(
                BackBufferHdc, 
                TextOffsetLeft,
                newY, 
                TextRenderWidth, 
                BufferHeight - newY + FontHeight, 
                BackBufferHdc, 
                TextOffsetLeft,
                y, 
                WinApi.SRCCOPY
            );
        }

        private void RedrawLines(int first, int count)
        {
            int overTextCy = FontHeight * first;
            
            int textCy = FontHeight * count;            
            
            DrawBg(new Rectangle(TextOffsetLeft, TextOffsetTop + overTextCy, TextRenderWidth, textCy));
            DrawText(lines.GetLinesSafe(first, count),
                new RECT(-HorisontalScrollPixels, -(VerticalScrollPixels % FontHeight) + overTextCy, 0, 0));
        }

        private void RedrawLines(int first)
        {
            int count = (VerticalScrollPixels + BufferHeight) / FontHeight - first + 1;
            int overTextCy = FontHeight * first;
            int textCy = FontHeight * count;

            DrawBg(new Rectangle(TextOffsetLeft, TextOffsetTop + overTextCy, TextRenderWidth, textCy));
            DrawText(lines.GetLinesSafe(first, count),
                new RECT(-HorisontalScrollPixels, -(VerticalScrollPixels % FontHeight) + overTextCy, 0, 0));
        }

        protected override void OnTextAppend(TextPasting snippet)
        {
            TextLines.ChangeTextInfo changeInfo;
            lines.AppendText(snippet, out changeInfo);
            RenderOffsetTextUnderLine(changeInfo.FirstChangedLineIndex, changeInfo.ChangedLinesCount - 1);
            RedrawLines(changeInfo.FirstChangedLineIndex, changeInfo.ChangedLinesCount + 1);
        }

        protected virtual void DrawCarete(string lineText, int line, int index)
        {
            using (Graphics g = Graphics.FromHdc(BackBufferHdc))
                g.FillRectangle(Brushes.Black,
                    new Rectangle(
                        GetTextSizePixels(lineText.Substring(0, index)).Width + HorisontalScrollPixels,
                        (line - VerticalScrollPixels / FontHeight) * FontHeight + VerticalScrollPixels % FontHeight,
                        2, FontHeight)
                    );
        }

        protected virtual void DrawCaretes()
        {
            foreach (var carete in Caretes)
            {
                int line, lineStartIndex;
                LinkedListNode<string> lineNode =  lines.GetNodeByCharIndex(carete, out line, out lineStartIndex);
                DrawCarete(lineNode.Value, line, (int)carete -  lineStartIndex);
            }
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
            RedrawLines(changeInfo.FirstChangedLineIndex);
        }

        protected override void Render(Rectangle square, IntPtr hdc)
        {
            if (requireCaretesRedraw) DrawCaretes();
            base.Render(square, hdc);
        }

        protected override void RedrawBuffer()
        {
            DrawBg(new Rectangle(0, 0, BufferWidth, BufferHeight));
            DrawText(lines.GetLinesSafe(VerticalScrollPixels / FontHeight, BufferHeight / FontHeight + 2),
                new RECT(-HorisontalScrollPixels, -VerticalScrollPixels % FontHeight, 0, 0));
        }
    }
}
