using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_TextEditorView
{
    internal class Range
    {
        public uint Index { get; set; }
        public int Moving { get; set; }

        public Range(uint index, int moving)
        {
            Index = index;
            Moving = moving;
        }
    }
}
