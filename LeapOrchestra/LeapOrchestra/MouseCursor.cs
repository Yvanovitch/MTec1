using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ConsoleLeapMouse
{
    class MouseCursor
    {
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        public static void MoveCursor(int x, int y)
        {
            SetCursorPos(x, y);
        }

    }
}
