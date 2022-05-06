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
        private bool isLbuttonDown = false;
        private bool selection = false;
        int selectionStart = 0;

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

        protected override void OnMouseDown(int x, int y, MouseButtons mouseButton)
        {
            if (mouseButton == MouseButtons.Left)
            {
                isLbuttonDown = true;
                selectionStart = Renderer.GetCharIndexFromTextRenderRectPoint(x, y);
            }
        }

        protected override void OnMouseMove(int x, int y)
        {
            if(isLbuttonDown)
            {
                Range selectionRange = new Range((uint)selectionStart, Renderer.GetCharIndexFromTextRenderRectPoint(x, y) - selectionStart + 1);
                if (selectionRange.Moving < 0) selectionRange = new Range((uint)(selectionRange.Index + selectionRange.Moving), -selectionRange.Moving);
                Renderer.SetSelections(new Range[] { selectionRange });
                Renderer.ForseRender();
                selection = true;
            }
        }

        protected override void OnMouseUp(int x, int y, MouseButtons mouseButton)
        {
            if (isLbuttonDown)
            {
                if (selection)
                {
                    Range selectionRange = new Range((uint)selectionStart, Renderer.GetCharIndexFromTextRenderRectPoint(x, y) - selectionStart + 1);
                    if (selectionRange.Moving < 0) selectionRange = new Range((uint)(selectionRange.Index + selectionRange.Moving), -selectionRange.Moving);
                    Renderer.SetSelections(new Range[] { selectionRange });
                }
                else
                    Renderer.SetSelections(new Range[0]);
                Renderer.ForseRender();
                isLbuttonDown = false;
            }
            selection = false;
        }

        protected override void OnRendererChanged()
        {
            Renderer.SetCaretes(new uint[] { 0 });
            Renderer.SetFont("Monoid-Regular", 0, 48, 400);
            Renderer.ForseRender();
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
