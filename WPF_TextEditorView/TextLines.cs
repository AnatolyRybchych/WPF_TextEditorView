using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_TextEditorView
{
    internal class TextLines
    {
        public LinkedList<string> Lines { get; private set; }

        public TextLines()
        {
            Lines = new LinkedList<string>();
            Lines.AddLast("\n");
        }

        public string GetLines(int index, int count)
        {
            string result;
            LinkedListNode<string> node = Lines.First;

            for (int i = 0; i < index; i++)
                node = node.Next;

            result = node.Value;
            for (int i = 1; i < count; i++)
                result += (node = node.Next).Value;
            return result;
        }

        public string GetLines(LinkedListNode<string> node, int count)
        {
            string result = node.Value;
            for (int i = 1; i < count; i++)
                result += (node = node.Next).Value;
            return result;
        }

        private LinkedListNode<string> GetNodeByCharIndex(uint index, out uint line, out uint lineStartIndex)
        {
            lineStartIndex = 0;
            line = 0;
            LinkedListNode<string> curr = Lines.First;

            do
            {
                lineStartIndex += (uint)curr.Value.Length;
                line++;
                if (lineStartIndex > index)
                {
                    line--;
                    lineStartIndex -= (uint)curr.Value.Length;
                    return curr;
                }
            } while ((curr = curr.Next) != null);
            return Lines.Last;
        }

        //require line without newline char
        private void AddLine(LinkedListNode<string> after, string line)
        {
            Lines.AddAfter(after, line + "\n");
        }

        //require line without newline char
        private void AddTextIntoLine(LinkedListNode<string> into, uint index, string text)
        {
            into.Value = into.Value.Substring(0, (int)index) + text + into.Value.Substring((int)index);
        }

        public void AppendText(TextPasting snippet, out AppendTextInfo appendInfo)
        {
            string[] newLines = snippet.Text.Split('\n');
            string first = newLines[0];
            string last = newLines[newLines.Length - 1];

            uint line, lineStartIndex;

            LinkedListNode<string> node = GetNodeByCharIndex(snippet.Index, out line, out lineStartIndex);

            appendInfo = new AppendTextInfo();
            appendInfo.EditedLinesCount = 1;
            appendInfo.FirstEditedLine = node;
            appendInfo.FirstEditedLineIndex = line;
            appendInfo.FirstEditedLineIndexCharIndex = lineStartIndex;

            if (string.IsNullOrEmpty(first) == false)
                AddTextIntoLine(node, snippet.Index - lineStartIndex, first);

            if (newLines.Length > 1)
            {
                for (int i = 1; i < newLines.Length - 1; i++)
                {
                    node = Lines.AddAfter(node, newLines[i]);
                    appendInfo.EditedLinesCount++;
                }

                if (node.Next == null)
                {
                    AddLine(node, last);
                    appendInfo.EditedLinesCount++;
                }
                else
                {
                    AddTextIntoLine(node.Next, 0, last);
                    appendInfo.EditedLinesCount++;
                }
            }
        }

        public struct AppendTextInfo
        {
            public LinkedListNode<string> FirstEditedLine { get; set; }
            public uint FirstEditedLineIndex { get; set; }
            public uint FirstEditedLineIndexCharIndex { get; set; }
            public uint EditedLinesCount { get; set; }

            public AppendTextInfo(LinkedListNode<string> firstEditedLine, uint firstEditedLineIndex, uint firstEditedLineIndexCharIndex, uint editedLinesCount)
            {
                FirstEditedLine = firstEditedLine;
                FirstEditedLineIndex = firstEditedLineIndex;
                FirstEditedLineIndexCharIndex = firstEditedLineIndexCharIndex;
                EditedLinesCount = editedLinesCount;
            }
        }
    }
}
