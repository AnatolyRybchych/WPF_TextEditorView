using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WPF_TextEditorView
{
    internal class SimpleTextEditorManager : TextEditorManager
    {
        private bool isShiftDown = false;
        public SimpleTextEditorManager()
        {
        }

        protected override void OnCharInput(char ch)
        {
            if (Renderer.Caretes.Length <= 0) return;

            Renderer.TextAppend(new TextPasting(Renderer.Caretes.First(), $"{ch}"));
            Renderer.Caretes[0]++;
            Renderer.SetCaretes();
            Renderer.ForseRender();
        }

        protected override void OnKeyDown(int keyCode, bool shift, bool alt)
        {
            switch ((Keys)keyCode)
            {
                case Keys.Back:
                    if (Renderer.Caretes.Length <= 0) break;
                    if (Renderer.Caretes.First() <= 0) break;

                    Renderer.TextRemove(new Range(--Renderer.Caretes[0], 1));
                    Renderer.SetCaretes();
                    Renderer.ForseRender();
                    break;
                case Keys.Left:
                    if (Renderer.Caretes.Length <= 0) break;
                    if (Renderer.Caretes.First() <= 0) break;

                    Renderer.Caretes[0]--;
                    Renderer.SetCaretes();
                    Renderer.ForseRender();
                    break;
                case Keys.Right:
                    if (Renderer.Caretes.Length <= 0) break;
                    if (Renderer.Caretes.First() > Renderer.Text.Length - 1) break;

                    Renderer.Caretes[0]++;
                    Renderer.SetCaretes();
                    Renderer.ForseRender();
                    break;
                case Keys.Shift:
                    isShiftDown = true;
                    break;
            }
        }

        protected override void OnKeyUp(int keyCode, bool shift, bool alt)
        {
            switch ((Keys)keyCode)
            {
                case Keys.Shift:
                    isShiftDown = false;
                    break;
            }
        }

        protected override void OnMouseDown(int x, int y, int mouseButton)
        {
        }

        protected override void OnMouseUp(int x, int y, int mouseButton)
        {
        }

        protected override void OnRendererChanged()
        {
            Renderer.SetCaretes(new uint[] { 0 });
            Renderer.SetFont("Times New Roman", 0, 48, 400);
        }

        protected override void OnScroll(int xWheelTriggers, int yWheelTriggers)
        {
            Renderer.VerticalScrollPixels -= yWheelTriggers * Renderer.FontHeight / 2;
            Renderer.ForseRender();
        }

        protected override void OnTextInput(string text)
        {
        }
    }
}
