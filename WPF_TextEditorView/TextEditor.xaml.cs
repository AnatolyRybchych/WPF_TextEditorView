﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using static WPF_TextEditorView.WinApi;

namespace WPF_TextEditorView
{
    public partial class TextEditor : UserControl
    {
        TextEditorRenderer renderer;
        Graphics graphics;
        StringBuilder text;

        public TextEditor()
        {
            text = new StringBuilder();
            InitializeComponent();
            Intrinsic.Paint += Intrinsic_Paint;
            Intrinsic.Resize += Intrinsic_Resize;
            Intrinsic.MouseDown += Intrinsic_MouseDown;

            graphics = Graphics.FromHwnd(Intrinsic.Handle);
            renderer = new SimpleTextEditorRenderer(graphics.GetHdc(), Intrinsic.Width, Intrinsic.Height);

            renderer.SetFont("TimesNewRoman", 0, 48, 400);
            renderer.TextAppend(new TextPasting(0, @"sadsafsafsdfasfdfsdfigfdipgogojig" + "\t" + @"ofsdppohpo
fdkgfdgpfdhfghfdgfhgfhdfhfghdghgfdhdghhdfiopppppppppppppp
ppppppppppppppppppppppppppppp[weifdsfd9ggi9-4r9g9
er9ure90ureq09=re09=u9ruw9req90qrew90rgt=fog0dfogahgpfhpgohpgf
fggreherrehhrerhereh"));

            renderer.SetSelections(new Range[] { new Range(2, 10) });
            renderer.SetCaretes(new uint[] { 5, 0, 100, (uint)renderer.Text.Length, (uint)renderer.Text.Length });

            Intrinsic.MouseWheel += Intrinsic_MouseWheel;

            //MessageBox.Show(string.Join("\n", new InstalledFontCollection().Families.Select(family => family.Name)));
        }

        private void Intrinsic_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            MessageBox.Show($"char:'{renderer.Text[(int)renderer.GetCharIndexFromTextRenderRectPoint(e.X - renderer.TextOffsetLeft, e.Y - renderer.TextOffsetTop)]}'");
        }

        private void Intrinsic_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            
            renderer.VerticalScrollPixels -= e.Delta > 0 ? renderer.FontHeight : -renderer.FontHeight;
            //renderer.HorisontalScrollPixels -= e.Delta > 0 ? renderer.FontHeight : -renderer.FontHeight;
            renderer.ForseRender();
        }

        private void Intrinsic_Resize(object sender, EventArgs e)
        {
            renderer.Resize(Intrinsic.Width, Intrinsic.Height);
        }

        private void Intrinsic_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            renderer.ForseRender(e.ClipRectangle);
        }

        ~TextEditor()
        {
            graphics.Dispose();
        }
    }
}
