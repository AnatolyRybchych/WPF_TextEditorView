using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_TextEditorView
{
    public class TextPasting
    {
        public uint Index { get; private set; }
        public string Text { get; private set; }

        public TextPasting(uint index, string text)
        {
            Index = index;
            Text = text;
        }
    }
}
