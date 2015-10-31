using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MithrilCog
{
    public class CogKeyboard
    {
        private KeyPress keyPress;
        private bool numLock;
        private bool capsLock;
        private bool scrollLock;

        internal CogKeyboard(GameWindow gameWindow)
        {
            gameWindow.KeyDown += gameWindow_KeyDown;
        }

        private void gameWindow_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == OpenTK.Input.Key.NumLock) numLock = !numLock;
            else if (e.Key == OpenTK.Input.Key.CapsLock) capsLock = !capsLock;
            else if (e.Key == OpenTK.Input.Key.ScrollLock) scrollLock = !scrollLock;
            KeyPress newKeyPress = new KeyPress(e.Key, e.Alt, e.Shift, e.Control, e.IsRepeat, numLock, capsLock, scrollLock);
            if (keyPress != newKeyPress) keyPress = newKeyPress;
        }

        /// <summary>
        /// Checks to see if a key was pressed.
        /// </summary>
        /// <returns>Key Press, null if nothing was pressed.</returns>
        public KeyPress GetKeyPress()
        {
            KeyPress kp = keyPress;
            keyPress = null;
            return kp;
        }
    }
}
