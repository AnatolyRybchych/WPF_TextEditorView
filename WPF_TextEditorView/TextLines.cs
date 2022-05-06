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

        public string GetLinesSafe(int index, int count = int.MaxValue)
        {
            int currIndex = -1;
            LinkedListNode<string> node = Lines.First;
            while (++currIndex != index)
                if ((node = node.Next) == null) return "";

            string result = "";
            int lastIndex = currIndex + count;

            while (currIndex++ < lastIndex)
            {
                result += node.Value;
                if ((node = node.Next) == null) return result;
            }
            return result;
        }

        private LinkedListNode<string> GetNodeByCharIndex(uint index, out int line, out int lineStartIndex)
        {
            lineStartIndex = 0;
            line = 0;
            LinkedListNode<string> curr = Lines.First;

            do
            {
                lineStartIndex += curr.Value.Length;
                line++;
                if (lineStartIndex > index)
                {
                    line--;
                    lineStartIndex -= curr.Value.Length;
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
        private void AddTextIntoLine(LinkedListNode<string> into, int index, string text)
        {
            into.Value = into.Value.Substring(0, (int)index) + text + into.Value.Substring((int)index);
        }

        //require line without newline char
        private void AddLineIntoLine(LinkedListNode<string> into, int index, string text)
        {
            Lines.AddAfter(into, into.Value.Substring((int)index));
            into.Value = into.Value.Substring(0, (int)index) + text + "\n";
        }

        public void RemoveRangeFromNode(LinkedListNode<string> node,  Range range, ref int changedLines)
        {
            if (range.Moving <= 0) return;

            if (range.Moving + range.Index >= node.Value.Length)
            {
                changedLines++;
                RemoveRangeFromNode(node.Next, new Range(0, range.Moving - node.Value.Length), ref changedLines);
                if (range.Index == 0)
                    node.Value = node.Value.Remove((int)range.Index) + node.Next.Value;
                
                Lines.Remove(node.Next);
            }
            else
            {
                changedLines++;
                node.Value = node.Value.Remove((int)range.Index, range.Moving);
            }

        }

        public void RemoveText(Range range, out ChangeTextInfo removeInfo){

            int line, lineStartIndex;

            LinkedListNode<string> node = GetNodeByCharIndex(range.Index, out line, out lineStartIndex);

            removeInfo = new ChangeTextInfo();
            removeInfo.FirstChangedLine = node;
            removeInfo.FirstChangedLineIndex = line;
            removeInfo.FirstChangedLineCharIndex = lineStartIndex;

            int changedLinesCount = 0;

            RemoveRangeFromNode(node, new Range(range.Index - (uint)lineStartIndex, range.Moving), ref changedLinesCount);
            removeInfo.ChangedLinesCount = changedLinesCount;
        }

        public void AppendText(TextPasting snippet, out ChangeTextInfo appendInfo)
        {
            string[] newLines = snippet.Text.Split('\n');
            string first = newLines[0];
            string last = newLines[newLines.Length - 1];

            int line, lineStartIndex;

            LinkedListNode<string> node = GetNodeByCharIndex(snippet.Index, out line, out lineStartIndex);

            appendInfo = new ChangeTextInfo();
            appendInfo.ChangedLinesCount = 1;
            appendInfo.FirstChangedLine = node;
            appendInfo.FirstChangedLineIndex = line;
            appendInfo.FirstChangedLineCharIndex = lineStartIndex;


            if (newLines.Length > 1)
            {
                for (int i = 0; i < newLines.Length - 1; i++)
                {
                    AddLineIntoLine(node, (int)snippet.Index - lineStartIndex, newLines[i]);
                    node = node.Next;
                    appendInfo.ChangedLinesCount++;
                }

                if (node.Next == null)
                {
                    AddLine(node, last);
                    appendInfo.ChangedLinesCount++;
                }
                else
                {
                    AddTextIntoLine(node.Next, 0, last);
                    appendInfo.ChangedLinesCount++;
                }
            }
            else
            {
                AddTextIntoLine(node, (int)snippet.Index - lineStartIndex, first);
            }
        }


        public struct ChangeTextInfo
        {
            public LinkedListNode<string> FirstChangedLine { get; set; }
            public int FirstChangedLineIndex { get; set; }
            public int FirstChangedLineCharIndex { get; set; }
            public int ChangedLinesCount { get; set; }

            public ChangeTextInfo(LinkedListNode<string> firstChangedLine, int firstChangedLineIndex, int firstChangedLineIndexCharIndex, int changedLinesCount)
            {
                FirstChangedLine = firstChangedLine;
                FirstChangedLineIndex = firstChangedLineIndex;
                FirstChangedLineCharIndex = firstChangedLineIndexCharIndex;
                ChangedLinesCount = changedLinesCount;
            }
        }
    }
}
