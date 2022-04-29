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
        private IntPtr backBufferHdc;

        private bool requiredBufferRedraw;

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

        protected override void OnFontChanged(string faceName, int width, int heigth, int weight)
        {
            if (weight % 100 != 0 || weight < 0 || weight > 900) throw new Exception("Incompatible font weight it should be (0 <= weight => 900) && (weight % 100 == 0)");

            if(font != IntPtr.Zero)
                DeleteObject(font);

            font = CreateFontW(heigth, width, 0, 0, weight, 0, 0, 0, 0, 0, 0, CLEARTYPE_QUALITY, 0, faceName);
            SelectObject(backBufferHdc, font);
            SetBkMode(backBufferHdc, BkMode.TRANSPARENT);

            requiredBufferRedraw = true;
        }

        protected override void OnSetingCaretes(uint[] indices)
        {
            requiredBufferRedraw = true;
        }

        protected override void OnSettingSelections(Range[] selections)
        {
            requiredBufferRedraw = true;
        }

        protected override void OnTextAppend(TextPasting snippet)
        {
            requiredBufferRedraw = true;
        }

        protected override void OnTextRemove(Range range)
        {
            requiredBufferRedraw = true;
        }

        private void RedrawBuffer()
        {
            SelectObject(hdc, font);

            string text = TextSource.ToString();
            int l = TextSource.Length;

            List<Rectangle> selectionRects = new List<Rectangle>();

            foreach (var selection in Selections)
            {
                int selectionEndIndex = (int)selection.Index + selection.Moving;
                if (selection.Index > l || selectionEndIndex < 0 || selectionEndIndex + 1 > l) continue;

                int min = Math.Min(selectionEndIndex, (int)selection.Index);
                int max = Math.Max(selectionEndIndex, (int)selection.Index);
                Range startRange = GetLineRange(min);
                Range endRange = GetLineRange(max);
                

                Rectangle r = new Rectangle();
                string beforeSelection = text.Substring(min - startRange.Moving, startRange.Moving);
                r.X = TextOffsetLeft + GetTextPixelSize(text.Substring(min - startRange.Moving, startRange.Moving)).Width;
                r.Y = TextOffsetTop + (int)startRange.Index * FontHeight;
                r.Height = Math.Min(FontHeight - r.Y + TextOffsetTop, TextRenderHeight);
                if (endRange.Index == startRange.Index)
                    r.Width = Math.Max(GetTextPixelSize(text.Substring(min, max)).Width - TextOffsetRight, 0);
                else
                {
                    r.Width = Math.Max(BufferWidth - r.X - TextOffsetRight, 0);
                    selectionRects.Add(
                        new Rectangle(
                            TextOffsetLeft, 
                            TextOffsetTop + (int)((startRange.Index + 1) * FontHeight), 
                            Math.Min(BufferWidth, TextRenderWidth), 
                            Math.Min((int)(endRange.Index - startRange.Index - 1) * FontHeight, TextRenderHeight - (int)((startRange.Index + 1) * FontHeight))
                        ));

                    selectionRects.Add(
                        new Rectangle(
                            TextOffsetLeft, 
                            TextOffsetTop + (int)endRange.Index * FontHeight, 
                            Math.Min(GetTextPixelSize(text.Substring(max - endRange.Moving, endRange.Moving)).Width, TextRenderWidth),
                            Math.Min(FontHeight, TextRenderHeight - (int)endRange.Index * FontHeight)
                        ));
                }
                selectionRects.Add(r);
            }

            using (Graphics g = Graphics.FromHdc(backBufferHdc))
            {
                g.FillRectangle(Brushes.DarkGray, TextOffsetLeft, TextOffsetTop, BufferWidth - TextOffsetRight - TextOffsetLeft, BufferHeight - TextOffsetBottom - TextOffsetTop);
                foreach (var r in selectionRects)
                    g.FillRectangle(Brushes.Orange, r);

                foreach (var carete in Caretes)
                {
                    if (carete > l) continue;
                    Range careteLineRange = GetLineRange((int)carete);
                    int careteTextX = GetTextPixelSize(text.Substring((int)carete - careteLineRange.Moving, careteLineRange.Moving)).Width;
                    int careteTextY = (int)careteLineRange.Index * FontHeight;

                    if (careteTextX > TextRenderWidth || careteTextY > TextRenderHeight) continue;

                    g.FillRectangle(
                        Brushes.Red, 
                        TextOffsetLeft + careteTextX, 
                        TextOffsetTop + careteTextY,
                        Math.Min(2, TextRenderWidth), 
                        Math.Min(FontHeight, TextRenderHeight - careteTextY));
                }
            }

            RECT rect = new RECT(TextOffsetLeft, TextOffsetTop, BufferWidth - TextOffsetRight, BufferHeight - TextOffsetBottom);
            DrawTextW(backBufferHdc, TextSource.ToString(), -1, ref rect, DrawTextFormat.DT_TOP);
        }



        protected override void Render(Rectangle square)
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

        public override Size GetTextPixelSize(string text)
        {
            SelectObject(hdc, font);
            Size size;
            GetTextExtentPoint32W(hdc, text, text.Length, out size);
            return size;
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
