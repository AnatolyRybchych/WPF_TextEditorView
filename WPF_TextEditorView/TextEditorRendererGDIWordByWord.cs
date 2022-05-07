using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPF_TextEditorView
{
    public class ColorDictionary : Dictionary<string, uint> { }

    public class TextEditorRendererGDIWordByWord : TextEditorRendererGDI
    {
        private TextLines lines;

        private string wordSeparators = " \n()[]{}&^<>/\\~`*%;\"\'-+=,.";
        public string WordSeparators { get { return wordSeparators; } set { wordSeparators = value; RequireBufferRedraw(); } }
        public virtual string[] SeparateLineByWords(string text) => text.KeepSplit(WordSeparators, true);

        private Dictionary<string, uint> wordColors;
        public Dictionary<string, uint> WordColors
        {
            get => wordColors;
            set
            {
                wordColors = value;
                RequireBufferRedraw();
            }
        }

        private uint baseWordColor;
        public uint BaseWordColor
        {
            get => baseWordColor;
            set
            {

                baseWordColor = value;
                RequireBufferRedraw();
            }
        }

        public override void Init(IntPtr hdc, int bufferWidth, int bufferHeight)
        {
            base.Init(hdc, bufferWidth, bufferHeight);
            RequireBufferRedraw();
        }

        public TextEditorRendererGDIWordByWord()
        {
            lines = new TextLines();
        }

        public override int GetCharIndexFromTextRenderRectPoint(int x, int y)
        {
            int currCharOffset = 0;
            int line = (y + VerticalScrollPixels) / FontHeight;

            LinkedListNode<string> lineNode = lines.Lines.First;
            for (int i = 0; i < line; i++)
            {
                currCharOffset += lineNode.Value.Length;
                if (lineNode.Next == null) break;
                lineNode = lineNode.Next;
            }

            string lineStr = lineNode.Value;
            int charOffset = x + HorisontalScrollPixels;
            int currLineCharOffset = 0;
            int charIndex = 0;
            while(currLineCharOffset < charOffset)
            {
                if (lineStr.Length - 2 <= charIndex) return currCharOffset + charIndex;
                currLineCharOffset += GetTextWidthPixels(lineStr[charIndex].ToString());
                charIndex++;
            }

            return currCharOffset + charIndex - 1;
        }

        public override int GetTextWidthPixels(string text)
        {
            Size sz;
            WinApi.GetTextExtentPoint32W(BackBufferHdc, text, text.Length, out sz);
            return sz.Width;
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
            RequireBufferRedraw();
        }

        protected override void OnSettingHorisontalScrollPixels()
        {
            RequireBufferRedraw();
        }

        protected override void OnSettingSelections()
        {
            RequireBufferRedraw();
        }

        protected override void OnSettingVerticalScrollPixels()
        {
            RequireBufferRedraw();
        }


        protected override void OnTextAppend(TextPasting snippet)
        {
            TextLines.ChangeTextInfo changeInfo;
            lines.AppendText(snippet, out changeInfo);
            RequireBufferRedraw();
        }

        protected virtual void DrawCarete(string lineText, int line, int index)
        {
            using (Graphics g = Graphics.FromHdc(BackBufferHdc))
                g.FillRectangle(Brushes.Black,
                    new Rectangle(
                        GetTextWidthPixels(lineText.Substring(0, index)) + HorisontalScrollPixels,
                        (line - VerticalScrollPixels / FontHeight) * FontHeight - VerticalScrollPixels % FontHeight,
                        2, FontHeight)
                    );
        }

        
        protected virtual void DrawSelectionRect(Rectangle rect)
        {
            using (Graphics g = Graphics.FromHdc(BackBufferHdc))
                g.FillRectangle(Brushes.Red, rect);
        }

        protected virtual void DrawSelection(LinkedListNode<string> startLine, int startLineIndex, int start, int count)
        {
            if (count <= 0 || startLine == null) return;

            string line = startLine.Value;
            Rectangle rect = new Rectangle(
                TextOffsetLeft,
                TextOffsetTop + startLineIndex * FontHeight - VerticalScrollPixels,
                TextRenderWidth,
                FontHeight
            );

            if (start != 0) rect.X = GetTextWidthPixels(line.Substring(0, start));

            if(line.Length > start + count)
            {
                rect.Width = GetTextWidthPixels(line.Substring(start, count));
            }
            else
            {
                rect.Width = GetTextWidthPixels(line.Substring(start, line.Length - start - 1));
                DrawSelection(startLine.Next, startLineIndex + 1, 0, count + start - line.Length);
            }
            DrawSelectionRect(rect);
        }

        private void DrawSelections()
        {
            LinkedListNode<string> lineNode = lines.Lines.First;

            int currLineStart = 0;
            int currLine = 0;

            foreach (var selection in Selections.OrderBy(selection => selection.Index))
            {
                while (lineNode != null)
                {
                    if (currLineStart + lineNode.Value.Length > selection.Index)
                    {
                        DrawSelection(lineNode, currLine, (int)selection.Index - currLineStart, selection.Moving);
                        break;
                    }
                    currLineStart += lineNode.Value.Length;
                    lineNode = lineNode.Next;
                    currLine++;
                }
            }
        }

        private void DrawCaretes()
        {
            foreach (var carete in Caretes)
            {
                int line, lineStartIndex;
                LinkedListNode<string> lineNode =  lines.GetNodeByCharIndex(carete, out line, out lineStartIndex);
                DrawCarete(lineNode.Value, line, (int)carete -  lineStartIndex);
            }
        }


        static Random r = new Random();
        uint[] colors = new uint[100].Select(coor => (uint)r.Next(0, (int)(uint.MaxValue >> 8))).ToArray();

        protected virtual void DrawWord(string word, RECT rect, int wordWidth,  int wordInLine, int line)
        {
            if(WordColors != null && WordColors.ContainsKey(word))
                WinApi.SetTextColor(BackBufferHdc, WordColors[word]);
            else
                WinApi.SetTextColor(BackBufferHdc, BaseWordColor);
             Font.DrawText(word, rect, DrawTextFormat.DT_LEFT);
        }

        private void DrawText()
        {
            RECT textRect = new RECT(TextOffsetLeft - HorisontalScrollPixels, TextOffsetTop - VerticalScrollPixels % FontHeight, TextRenderWidth - TextOffsetRight, TextRenderHeight - TextOffsetBottom);

            LinkedListNode<string> lineNode = lines.Lines.First;
            for (int i = 0; i < VerticalScrollPixels / FontHeight; i++)
                if ((lineNode = lineNode.Next) == null) return;

            int lastLine = (VerticalScrollPixels + BufferHeight) / FontHeight + 1;
            for (int line = VerticalScrollPixels / FontHeight; line < lastLine; line++)
            {
                if (lineNode == null) return;
                int wordInLine = 0;
                foreach (var word in SeparateLineByWords(lineNode.Value))
                {
                    int wordWidth = GetTextWidthPixels(word);
                    wordInLine++;
                    DrawWord(word, textRect, wordWidth, wordInLine, line);
                    textRect.Left += wordWidth;
                }
                textRect.Top += FontHeight;
                textRect.Left = TextOffsetLeft;
                lineNode = lineNode.Next;
            }
        }

        protected virtual void DrawBg()
        {
            using (Graphics g = Graphics.FromHdc(BackBufferHdc))
                g.FillRectangle(Brushes.Orange, new Rectangle(0, 0, BufferWidth, BufferHeight));
        }

        protected override void OnTextRemove(Range range, string removedText)
        {
            TextLines.ChangeTextInfo changeInfo;
            lines.RemoveText(range, out changeInfo);
            RequireBufferRedraw();
        }

        protected override void RedrawBuffer()
        {
            DrawBg();
            DrawSelections();
            DrawText();
            DrawCaretes();
        }
    }
}
