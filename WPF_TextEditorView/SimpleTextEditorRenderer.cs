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
        private int fontWidth;
        private int fontHeight;
        private int fontWeight;
        private uint[] caretes;
        private Range[] selections;

        private Bitmap backBuffer;
        private IntPtr backBufferHdc;

        private bool requiredBufferRedraw;

        public SimpleTextEditorRenderer(IntPtr hdc, int bufferWidth, int bufferHeight, StringBuilder observableText) : base(hdc, bufferWidth, bufferHeight, observableText)
        {
            font = IntPtr.Zero;
            fontWidth = 0;
            fontHeight = 0;
            fontWeight = 0;
            caretes = new uint[0];
            selections = new Range[0];

            backBuffer = new Bitmap(BufferWidth, BufferHeight);
            using(Graphics g = Graphics.FromImage(backBuffer))
                backBufferHdc = g.GetHdc();
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

        public override void OnFontChanged(string faceName, int width, int heigth, int weight)
        {
            fontWidth = width;
            fontHeight = heigth;
            fontWeight = weight;

            if (weight % 100 != 0 || weight < 0 || weight > 900) throw new Exception("Incompatible font weight it should be (0 <= weight => 900) && (weight % 100 == 0)");

            if(font != IntPtr.Zero)
                DeleteObject(font);

            font = CreateFontW(heigth, width, 0, 0, weight, 0, 0, 0, 0, 0, 0, CLEARTYPE_QUALITY, 0, faceName);
            SelectObject(backBufferHdc, font);
            SetBkMode(backBufferHdc, BkMode.TRANSPARENT);

            requiredBufferRedraw = true;
        }

        public override void OnSetingCaretes(uint[] indices)
        {
            caretes = indices;
            requiredBufferRedraw = true;
        }

        public override void OnSettingSelections(Range[] selections)
        {
            this.selections = selections;
            requiredBufferRedraw = true;
        }

        public override void OnTextAppend(TextPasting[] pastingSnippets)
        {
        }

        public override void OnTextRemove(Range[] ranges)
        {
        }

        private void RedrawBuffer()
        {
            SelectObject(hdc, font);

            string text = observableText.ToString();
            int l = observableText.Length;

            Size size;

            List<Rectangle> selectionRects = new List<Rectangle>();

            foreach (var selection in selections)
            {
                int selectionEndIndex = (int)selection.Index + selection.Moving;
                if (selection.Index > l || selectionEndIndex < 0 || selectionEndIndex + 1 > l) continue;

                int min = Math.Min(selectionEndIndex, (int)selection.Index);
                int max = Math.Max(selectionEndIndex, (int)selection.Index);
                Range startRange = GetLineRange(min);
                Range endRange = GetLineRange(max);
                

                Rectangle r = new Rectangle();
                string beforeSelection = text.Substring(min - startRange.Moving, startRange.Moving);
                GetTextExtentPoint32W(hdc, text.Substring(min - startRange.Moving, startRange.Moving), beforeSelection.Length, out size);
                r.X = size.Width;
                r.Y = (int)startRange.Index * fontHeight;
                r.Height = fontHeight;
                if (endRange.Index == startRange.Index)
                {
                    GetTextExtentPoint32W(hdc, text.Substring(min, max), max - min, out size);
                    r.Width = size.Width;
                }
                else
                {
                    r.Width = BufferWidth - size.Width;
                    GetTextExtentPoint32W(hdc, text.Substring(min - startRange.Moving, startRange.Moving), beforeSelection.Length, out size);
                    string s = text.Substring(max - endRange.Moving, endRange.Moving);
                    selectionRects.Add(new Rectangle(0, (int)((startRange.Index + 1) * fontHeight), BufferWidth, (int)(endRange.Index - startRange.Index - 1) * fontHeight));
                    GetTextExtentPoint32W(hdc, s, s.Length, out size);
                    selectionRects.Add(new Rectangle(0, (int)endRange.Index * fontHeight, size.Width, fontHeight));
                }
                selectionRects.Add(r);
            }

            using (Graphics g = Graphics.FromHdc(backBufferHdc))
            {
                g.FillRectangle(Brushes.DarkGray, 0, 0, BufferWidth, BufferHeight);
                foreach (var r in selectionRects)
                    g.FillRectangle(Brushes.Orange, r);

                foreach (var carete in caretes)
                {
                    Range careteLineRange = GetLineRange((int)carete);
                    string textBeforeCarete = text.Substring((int)carete - careteLineRange.Moving, careteLineRange.Moving);
                    GetTextExtentPoint32W(hdc, textBeforeCarete, textBeforeCarete.Length, out size);
                    g.FillRectangle(Brushes.Red, size.Width, careteLineRange.Index * fontHeight, 2, fontHeight);
                }
            }

            Rectangle rect = new Rectangle(0, 0, BufferWidth, BufferHeight);
            DrawTextW(backBufferHdc, observableText.ToString(), -1, ref rect, DrawTextFormat.DT_TOP);
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
            foreach (var ch in observableText.ToString())
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

        ~SimpleTextEditorRenderer()
        {
            if(font != IntPtr.Zero)
                DeleteObject(font);

            DeleteDC(backBufferHdc);
            backBuffer.Dispose();
        }
    }
}
