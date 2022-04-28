using System;
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

            graphics = Graphics.FromHwnd(Intrinsic.Handle);
            renderer = new SimpleTextEditorRenderer(graphics.GetHdc(), Intrinsic.Width, Intrinsic.Height, text);

            text.Append(
                @"sadsafsafsdfasfdfsdfigfdipgogojigofsdppohpo
fdkgfdgpfdhfghfdgfhgfhdfhfghdghgfdhdghhdfiopppppppppppppp
ppppppppppppppppppppppppppppp[weifdsfd9ggi9-4r9g9
er9ure90ureq09=re09=u9ruw9req90qrew90rgt=fog0dfogahgpfhpgohpgf
fggreherrehhrerhereh"
            );
            renderer.OnFontChanged("TimesNewRoman", 0, 48, 400);
            renderer.OnTextAppend(new TextPasting[] { new TextPasting(0, text.ToString()) });

            renderer.OnSettingSelections(new Range[] { new Range(20, 120) });
            renderer.OnSetingCaretes(new uint[] { 5, 0, 100, (uint)text.Length });

            //MessageBox.Show(string.Join("\n", new InstalledFontCollection().Families.Select(family => family.Name)));
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
