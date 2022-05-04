using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_TextEditorView
{
    public static class StringExt
    {
        public static string[] KeepSplit(this string self, char delimiter)
        {
            LinkedList<string> res = new LinkedList<string>();
            string curr = "";

            foreach (var ch in self)
            {
                curr += ch;
                if(ch == delimiter)
                {
                    res.AddLast(curr);
                    curr = "";
                }
            }
            if (curr.LastOrDefault() != delimiter)
                res.AddLast(curr);
            return res.ToArray();
        }

        public static string[] KeepSplit(this string self, string delimiters)
        {
            LinkedList<string> res = new LinkedList<string>();
            string curr = "";

            foreach (var ch in self)
            {
                curr += ch;
                if (delimiters.Contains(ch))
                {
                    res.AddLast(curr);
                    curr = "";
                }
            }
            if (delimiters.Contains(curr.LastOrDefault()))
                res.AddLast(curr);
            return res.ToArray();
        }
    }
}
