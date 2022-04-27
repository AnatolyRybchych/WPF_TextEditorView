using System;
using System.Collections.Generic;
using System.Drawing;
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

        public TextEditor()
        {
            InitializeComponent();
            Intrinsic.Paint += Intrinsic_Paint;

            graphics = Graphics.FromHwnd(Intrinsic.Handle);
            renderer = new SimpleTextEditorRenderer(graphics.GetHdc(), Intrinsic.Width, Intrinsic.Height);
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
