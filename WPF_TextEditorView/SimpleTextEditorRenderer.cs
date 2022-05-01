using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WPF_TextEditorView.WinApi;

namespace WPF_TextEditorView
{
    internal class SimpleTextEditorRenderer : TextEditorRenderer
    {
        private IntPtr font;

        private Bitmap backBuffer;
        protected IntPtr backBufferHdc;

        private bool requiredBufferRedraw;

        private int textDrawingOffsetY => VerticalScrollPixels % FontHeight;
        private int textDrawingLinesOffset => VerticalScrollPixels / FontHeight; 
        private string textToDraw => string.Join("\n", TextSource.ToString().Split('\n').Where((str, line) => line >= textDrawingLinesOffset));

        public SimpleTextEditorRenderer(IntPtr hdc, int bufferWidth, int bufferHeight) : base(hdc, bufferWidth, bufferHeight)
        {
            font = IntPtr.Zero;

            backBuffer = new Bitmap(BufferWidth, BufferHeight);
            using(Graphics g = Graphics.FromImage(backBuffer))
                backBufferHdc = g.GetHdc();

            TextOffsetLeft = 100;
            TextOffsetTop = 20;
            TextOffsetBottom = 20;
            TextOffsetRight = 20;
        }

        public override void Resize(int width, int heigth)
        {
            if (width < 1 || heigth < 1) return;
            base.Resize(width, heigth);

            DeleteDC(backBufferHdc);
            backBuffer.Dispose();

            backBuffer = new Bitmap(width, heigth);
            using (Graphics g = Graphics.FromImage(backBuffer))
                backBufferHdc = g.GetHdc();

            SelectObject(backBufferHdc, font);
            SetBkMode(backBufferHdc, BkMode.TRANSPARENT);

            requiredBufferRedraw = true;
        }

        protected override void OnFontChanged()
        {
            if(font != IntPtr.Zero)
                DeleteObject(font);

            font = CreateFontW(FontHeight, FontWidth, 0, 0, FontWeight, 0, 0, 0, 0, 0, 0, CLEARTYPE_QUALITY, 0, FontFace);
            SelectObject(backBufferHdc, font);
            SetBkMode(backBufferHdc, BkMode.TRANSPARENT);

            requiredBufferRedraw = true;
        }

        protected override void OnSetingCaretes()
        {
            requiredBufferRedraw = true;
        }

        protected override void OnSettingSelections()
        {
            requiredBufferRedraw = true;
        }

        protected override void OnTextAppend(TextPasting snippet)
        {
            requiredBufferRedraw = true;
        }

        protected override void OnTextRemove(Range range, string removedText)
        {
            requiredBufferRedraw = true;
        }

        private void RedrawBuffer()
        {
            string text = textToDraw;
            string src = TextSource.ToString();

            int l = src.Length;

            List<Rectangle> selectionRects = new List<Rectangle>();

            foreach (var selection in Selections)
            {
                int textDrawingRectOffsetY = FontHeight * textDrawingLinesOffset;
                int selectionEndIndex = (int)selection.Index + selection.Moving;
                if (selection.Index > l || selectionEndIndex < 0 || selectionEndIndex + 1 > l) continue;

                int min = Math.Min(selectionEndIndex, (int)selection.Index);
                int max = Math.Max(selectionEndIndex, (int)selection.Index);
                Range startRange = GetLineRange(min);
                Range endRange = GetLineRange(max);
                

                Rectangle r = new Rectangle();
                string beforeSelection = src.Substring(min - startRange.Moving, startRange.Moving);
                r.X = Math.Max(TextOffsetLeft + GetTextSizePixels(src.Substring(min - startRange.Moving, startRange.Moving)).Width - HorisontalScrollPixels, TextOffsetLeft);
                r.Y = Math.Min(TextOffsetTop + (int)startRange.Index * FontHeight - textDrawingRectOffsetY, TextOffsetTop);
                r.Height = Math.Min(FontHeight - ((int)startRange.Index * textDrawingLinesOffset), TextRenderHeight - (int)startRange.Index * textDrawingLinesOffset);
                if (endRange.Index == startRange.Index)
                    r.Width = Math.Max(GetTextSizePixels(src.Substring(min, max)).Width - TextOffsetRight - HorisontalScrollPixels, 0) ;
                else
                {
                    r.Width = Math.Max(BufferWidth - r.X - TextOffsetRight, 0);
                    selectionRects.Add(
                        new Rectangle(
                            TextOffsetLeft,
                            TextOffsetTop + (int)((startRange.Index + 1) * FontHeight) - textDrawingRectOffsetY,
                            Math.Min(BufferWidth, TextRenderWidth), 
                            Math.Min((int)(endRange.Index - startRange.Index - 1) * FontHeight, TextRenderHeight - (int)((startRange.Index + 1) * FontHeight) )
                        ));

                    int lastSelectedLineWidth = GetTextSizePixels(src.Substring(max - endRange.Moving, endRange.Moving)).Width;

                    selectionRects.Add(
                        new Rectangle(
                            TextOffsetLeft, 
                            TextOffsetTop + (int)endRange.Index * FontHeight - textDrawingRectOffsetY,
                            
                            Math.Min(lastSelectedLineWidth - HorisontalScrollPixels, TextRenderWidth),
                            Math.Min(FontHeight, TextRenderHeight - (int)endRange.Index * FontHeight)
                        ));
                }
                selectionRects.Add(r);
            }

            using (Graphics g = Graphics.FromHdc(backBufferHdc))
            {
                g.FillRectangle(Brushes.DarkGray, new Rectangle(TextOffsetLeft, TextOffsetTop, TextRenderWidth, TextRenderHeight));

                foreach (var r in selectionRects)
                    g.FillRectangle(Brushes.Orange, r);

                foreach (var carete in Caretes)
                {

                    if (carete > TextSource.Length) continue;
                    Range careteLineRange = GetLineRange((int)carete);

                    if (careteLineRange.Index < textDrawingLinesOffset) continue;

                    int careteTextX = GetTextSizePixels(Text.Substring((int)carete - careteLineRange.Moving, careteLineRange.Moving)).Width - HorisontalScrollPixels;
                    int careteTextY = (int)careteLineRange.Index * FontHeight - FontHeight * textDrawingLinesOffset - textDrawingOffsetY;

                    if (careteTextX > TextRenderWidth || careteTextY > TextRenderHeight) continue;

                    g.FillRectangle(
                        Brushes.Red, 
                        TextOffsetLeft + careteTextX, 
                        TextOffsetTop + careteTextY,
                        2, 
                        FontHeight);
                }
            }

            RECT rect = new RECT(TextOffsetLeft, TextOffsetTop - textDrawingOffsetY, BufferWidth - TextOffsetRight, BufferHeight - TextOffsetBottom);
            DrawTextParams @params = new DrawTextParams(0, -HorisontalScrollPixels, VerticalScrollPixels, 0);
            DrawTextExW(backBufferHdc, text, -1, ref rect, DrawTextFormat.DT_TOP, ref @params);
        }



        protected override void Render(Rectangle square, IntPtr hdc)
        {
            if (requiredBufferRedraw)
            {
                RedrawBuffer();
                requiredBufferRedraw = false;
            }

            BitBlt(hdc, square.Left, square.Top, square.Right - square.Left, square.Bottom - square.Top, backBufferHdc, square.Left, square.Top, SRCCOPY);
        }

        private Range GetLineRange(int index)
        {
            int line = 0;
            int i = -1;
            int in_line = 0;
            bool newLine = false;
            foreach (var ch in TextSource.ToString())
            {
                if (ch == '\n') newLine = true;
                if (++i == index) return new Range((uint)line, in_line);
                if(newLine)
                {
                    in_line = -1;
                    line++;
                    newLine = false;
                }
                in_line++;
            }
            return new Range((uint)line, in_line);
        }

        public override Size GetTextSizePixels(string text)
        {
            Size size;
            GetTextExtentPoint32W(backBufferHdc, text, text.Length, out size);
            return size;
        }

        protected override void OnSettingHorisontalScrollPixels()
        {
            requiredBufferRedraw = true;
        }

        protected override void OnSettingVerticalScrollPixels()
        {
            requiredBufferRedraw = true;
        }

        public override uint GetCharIndexFromTextRenderRectPoint(int x, int y)
        {
            string[] lines = Text.Split('\n').Select(l => l + "\n").ToArray();

            int line = Math.Min(y / FontHeight, lines.Length - 1);
            int charLineX = 0;

            for(int i = 0; i < lines[line].Length; i++)
            {
                charLineX += GetTextSizePixels($"{lines[line][i]}").Width;
                if (charLineX > x) return (uint)lines.Where((l, id) => id < line).Select(l => l.Length).Sum() + (uint)i;
            }

            return (uint)TextSource.Length - 1;
        }

        public override uint GetTextWidthPixels()
        {
            Size size;
            GetTextExtentPoint32W(backBufferHdc, Text, Text.Length, out size);
            return (uint)size.Width;
        }

        public override uint GetTextHeightPixels()
        {
            return (uint)(Text.Split('\n').Length * FontHeight);
        }

        ~SimpleTextEditorRenderer()
        {
            if(font != IntPtr.Zero)
                DeleteObject(font);

            DeleteDC(backBufferHdc);
            backBuffer.Dispose();
        }
    }
}
