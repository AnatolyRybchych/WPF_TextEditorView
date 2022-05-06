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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using static WPF_TextEditorView.WinApi;

namespace WPF_TextEditorView
{
    public partial class TextEditor : System.Windows.Controls.UserControl
    {
        TextEditorRenderer renderer;
        TextEditorManager manager;
        Graphics graphics;

        public TextEditor()
        {
            InitializeComponent();
            Intrinsic.Paint += Intrinsic_Paint;
            Intrinsic.Resize += Intrinsic_Resize;

            Intrinsic.PreviewKeyDown += Intrinsic_PreviewKeyDown;
            Intrinsic.KeyUp += Intrinsic_KeyUp;
            Intrinsic.MouseDown += Intrinsic_MouseDown;
            Intrinsic.MouseUp += Intrinsic_MouseUp;
            Intrinsic.MouseMove += Intrinsic_MouseMove;
            Intrinsic.MouseWheel += Intrinsic_MouseWheel;
            Intrinsic.Char += Intrinsic_Char;

            graphics = Graphics.FromHwnd(Intrinsic.Handle);
            renderer = new TextEditorRendererGDIWordByWord(graphics.GetHdc(), Intrinsic.Width, Intrinsic.Height);

            manager = new SimpleTextEditorManager();
            manager.SetRenderer(renderer);
        }

        private void Intrinsic_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            manager.MouseMove(e.X, e.Y);
        }

        private void Intrinsic_Char(char ch)
        {
            manager.CharInput(ch);
        }

        private void Intrinsic_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            manager.KeyDown((int)e.KeyCode, e.Shift, e.Alt);
        }

        private void Intrinsic_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            manager.Scroll(0, e.Delta > 0 ? 1 : -1);
        }

        private void Intrinsic_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            manager.MouseUp(e.X, e.Y, e.Button);
        }

        private void Intrinsic_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Intrinsic.Focus();
            manager.MouseDown(e.X, e.Y, e.Button);
        }

        private void Intrinsic_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            manager.KeyUp((int)e.KeyCode, e.Shift, e.Alt);
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
