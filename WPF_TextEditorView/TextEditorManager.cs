using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_TextEditorView
{

    //layer between Renderer and control input system
    //for managing renderer text, selections, move caretes and scrolling
    public abstract class TextEditorManager
    {
        protected TextEditorRenderer Renderer { get; private set; }

        public void SetRenderer(TextEditorRenderer renderer)
        {
            Renderer = renderer;
            OnRendererChanged();
        }

        public void CharInput(char ch)
        {
            if (Renderer == null) return;
            OnCharInput(ch);
        }

        public void TextInput(string text)
        {
            if (Renderer == null) return;
            OnTextInput(text);
        }

        public void MouseDown(int x, int y, int mouseButton)
        {
            if (Renderer == null) return;
            OnMouseDown(x, y, mouseButton);
        }

        public void MouseUp(int x, int y, int mouseButton)
        {
            if (Renderer == null) return;
            OnMouseUp(x, y, mouseButton);
        }

        public void KeyDown(int keyCode, bool shift, bool alt)
        {
            if (Renderer == null) return;
            OnKeyDown(keyCode, shift, alt);
        }

        public void KeyUp(int keyCode, bool shift, bool alt)
        {
            if (Renderer == null) return;
            OnKeyUp(keyCode, shift, alt);
        }

        public void Scroll(int xWheelTriggers, int yWheelTriggers)
        {
            if (Renderer == null) return;
            OnScroll(xWheelTriggers, yWheelTriggers);
        }

        //should provide privious renderer data to new renderer
        protected abstract void OnRendererChanged();
        protected abstract void OnCharInput(char ch);
        protected abstract void OnTextInput(string text);
        protected abstract void OnMouseDown(int x, int y, int mouseButton);
        protected abstract void OnMouseUp(int x, int y, int mouseButton);
        protected abstract void OnKeyDown(int keyCode, bool shift, bool alt);
        protected abstract void OnKeyUp(int keyCode, bool shift, bool alt);
        protected abstract void OnScroll(int xWheelTriggers, int yWheelTriggers);
    }
}
