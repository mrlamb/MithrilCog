using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MithrilCog
{
    public class KeyPress
    {
        public Key Key { get; private set; }
        public bool Alt { get; private set; }
        public bool Shift { get; private set; }
        public bool Control { get; private set; }
        public bool Repeating { get; private set; }
        public bool NumLock { get; private set; }
        public bool CapsLock { get; private set; }
        public bool ScrollLock { get; private set; }
        public char? Char
        {
            get
            {
                switch (Key)
                {
                    case Key.A: return Shift ^ CapsLock ? 'A' : 'a';
                    case Key.B: return Shift ^ CapsLock ? 'B' : 'b';
                    case Key.C: return Shift ^ CapsLock ? 'C' : 'c';
                    case Key.D: return Shift ^ CapsLock ? 'D' : 'd';
                    case Key.E: return Shift ^ CapsLock ? 'E' : 'e';
                    case Key.F: return Shift ^ CapsLock ? 'F' : 'f';
                    case Key.G: return Shift ^ CapsLock ? 'G' : 'g';
                    case Key.H: return Shift ^ CapsLock ? 'H' : 'h';
                    case Key.I: return Shift ^ CapsLock ? 'I' : 'i';
                    case Key.J: return Shift ^ CapsLock ? 'J' : 'j';
                    case Key.K: return Shift ^ CapsLock ? 'K' : 'k';
                    case Key.L: return Shift ^ CapsLock ? 'L' : 'l';
                    case Key.M: return Shift ^ CapsLock ? 'M' : 'm';
                    case Key.N: return Shift ^ CapsLock ? 'N' : 'n';
                    case Key.O: return Shift ^ CapsLock ? 'O' : 'o';
                    case Key.P: return Shift ^ CapsLock ? 'P' : 'p';
                    case Key.Q: return Shift ^ CapsLock ? 'Q' : 'q';
                    case Key.R: return Shift ^ CapsLock ? 'R' : 'r';
                    case Key.S: return Shift ^ CapsLock ? 'S' : 's';
                    case Key.T: return Shift ^ CapsLock ? 'T' : 't';
                    case Key.U: return Shift ^ CapsLock ? 'U' : 'u';
                    case Key.V: return Shift ^ CapsLock ? 'V' : 'v';
                    case Key.W: return Shift ^ CapsLock ? 'W' : 'w';
                    case Key.X: return Shift ^ CapsLock ? 'X' : 'x';
                    case Key.Y: return Shift ^ CapsLock ? 'Y' : 'y';
                    case Key.Z: return Shift ^ CapsLock ? 'Z' : 'z';
                    case Key.BackSlash: return Shift ? '|' : '\\';
                    case Key.BracketLeft: return Shift ? '{' : '[';
                    case Key.BracketRight: return Shift ? '}' : ']';
                    case Key.Comma: return Shift ? '<' : ',';
                    case Key.Grave: return Shift ? '~' : '`';
                    case Key.Keypad0: return NumLock ? '0' : (char?)null;
                    case Key.Keypad1: return NumLock ? '1' : (char?)null;
                    case Key.Keypad2: return NumLock ? '1' : (char?)null;
                    case Key.Keypad3: return NumLock ? '1' : (char?)null;
                    case Key.Keypad4: return NumLock ? '1' : (char?)null;
                    case Key.Keypad5: return NumLock ? '5' : (char?)null;
                    case Key.Keypad6: return NumLock ? '6' : (char?)null;
                    case Key.Keypad7: return NumLock ? '7' : (char?)null;
                    case Key.Keypad8: return NumLock ? '8' : (char?)null;
                    case Key.Keypad9: return NumLock ? '9' : (char?)null;
                    case Key.KeypadPlus: return '+';
                    case Key.KeypadDecimal: return NumLock ? '.' : (char?)null;
                    case Key.KeypadDivide: return '/';
                    case Key.KeypadMinus: return '-';
                    case Key.KeypadMultiply: return '*';
                    case Key.Number0: return Shift ? ')' : '0';
                    case Key.Number1: return Shift ? '!' : '1';
                    case Key.Number2: return Shift ? '@' : '2';
                    case Key.Number3: return Shift ? '#' : '3';
                    case Key.Number4: return Shift ? '$' : '4';
                    case Key.Number5: return Shift ? '%' : '5';
                    case Key.Number6: return Shift ? '^' : '6';
                    case Key.Number7: return Shift ? '&' : '7';
                    case Key.Number8: return Shift ? '*' : '8';
                    case Key.Number9: return Shift ? '(' : '9';
                    case Key.Period: return Shift ? '>' : '.';
                    case Key.Plus: return Shift ? '+' : '=';
                    case Key.Minus: return Shift ? '_' : '-';
                    case Key.Quote: return Shift ? '"' : '\'';
                    case Key.Semicolon: return Shift ? ':' : ';';
                    case Key.Slash: return Shift ? '?' : '/';
                    case Key.Space: return Shift ? ' ' : ' ';
                    default: return null;
                }
            }
        }

        public KeyPress(Key key, bool alt, bool shift, bool control, bool repeating, bool numLock, bool capsLock, bool scrollLock)
        {
            Key = key;
            Alt = alt;
            Shift = shift;
            Control = control;
            Repeating = repeating;
            NumLock = numLock;
            CapsLock = capsLock;
            ScrollLock = scrollLock;
        }

        public static bool operator ==(KeyPress A, KeyPress B)
        {
            if (object.ReferenceEquals(A, B)) return true;
            if (((object)A == null) || ((object)B == null))
            {
                return false;
            }
            else return (A.Key == B.Key && A.Alt == B.Alt && A.Shift == B.Shift && A.Control == B.Control && A.Repeating == B.Repeating);
        }

        public static bool operator !=(KeyPress A, KeyPress B)
        {
            return !(A == B);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            else return (this == (obj as KeyPress));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
