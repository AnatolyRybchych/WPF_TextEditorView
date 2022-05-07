using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WPF_TextEditorView
{
    internal static class WinApi
    {
        public const int ANTIALIASED_QUALITY = 0x4;
        public const int CLEARTYPE_QUALITY = 5;
        public const UInt32 SRCCOPY = 0x00CC0020;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int DrawTextW(IntPtr hdc, string lpchText, int cchText, ref RECT lprc, DrawTextFormat format);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateFontW(int cHeight, int cWidth, int cEscapement, int cOrientation, int cWeight, UInt32 bItalic, UInt32 bUnderline,
                            UInt32 bStrikeOut, UInt32 iCharSet, UInt32 iOutPrecision, UInt32 iClipPrecision, UInt32 iQuality,
                            UInt32 iPitchAndFamily,string pszFaceName);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCharABCWidthsW(IntPtr hdc, uint wFirst, uint wLast,out ABC lpABC);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr obj);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr obj);

        [DllImport("gdi32.dll")]
        public static extern int SetBkMode(IntPtr hdc, BkMode mode);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(IntPtr dst, int dstX, int dstY, int dstCx, int dstCy,
                                         IntPtr src, int srcX, int secY, UInt32 rop);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTextExtentPoint32W(IntPtr hdc, string str, int strLen, out Size size);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int DrawTextExW(IntPtr hdc, string lpchText, int cchText, ref RECT lprc, DrawTextFormat format, ref DrawTextParams @params);

        [DllImport("gdi32.dll")]
        public static extern uint SetTextColor(IntPtr hdc, uint color);
    }


    public struct DrawTextParams
    {
        public uint CbSize { get; private set; }
        public int ITabLength { get; private set; }
        public int ILeftMargin { get; private set; }
        public int IRightMargin { get; private set; }
        public uint UiLengthDrawn { get; private set; }

        public DrawTextParams(int tabLength, int leftMargin, int rightMargin, uint uiLengthDrawn)
        {
            CbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf<DrawTextParams>();
            ITabLength = tabLength;
            ILeftMargin = leftMargin;
            IRightMargin = rightMargin;
            UiLengthDrawn = uiLengthDrawn;
        }
    }

    public struct RECT
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        public int X
        {
            get => Left;
            set => Left = value;
        }

        public int Y
        {
            get => Top;
            set => Top = value;
        }

        public int Width
        {
            get => Right - Left;
            set => Right = value + Left;
        }

        public int Height
        {
            get => Bottom - Top;
            set => Bottom = value + Top;
        }

        public RECT(Rectangle rect) : this(rect.Left, rect.Top, rect.Right, rect.Bottom)
        {}

        public RECT(int width, int height) : this(0, 0, width, height)
        { }

        public RECT(int left, int top, int rigth, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = rigth;
            this.Bottom = bottom;
        }
    }

    public struct ABC
    {
        public int A { get; set; }
        public uint B { get; set; }
        public int C { get; set; }

        public ABC(int a, uint b, int c)
        {
            A = a;
            B = b;
            C = c;
        }
    }

    public enum BkMode : int
    {
        OPAQUE = 0,
        TRANSPARENT = 1,
    }

    public enum DrawTextFormat : uint
    {
        DT_TOP = 0x00000000,
        DT_LEFT = 0x00000000,
        DT_CENTER = 0x00000001,
        DT_RIGHT = 0x00000002,
        DT_VCENTER = 0x00000004,
        DT_BOTTOM = 0x00000008,
        DT_WORDBREAK = 0x00000010,
        DT_SINGLELINE = 0x00000020,
        DT_EXPANDTABS = 0x00000040,
        DT_TABSTOP = 0x00000080,
        DT_NOCLIP = 0x00000100,
        DT_EXTERNALLEADING = 0x00000200,
        DT_CALCRECT = 0x00000400,
        DT_NOPREFIX = 0x00000800,
        DT_INTERNAL = 0x00001000,
    }
}
