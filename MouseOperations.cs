namespace MouseOps
{
    using System;
    using System.Runtime.InteropServices;

    public partial class MouseOperations
    {
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [LibraryImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetCursorPos(int x, int y);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool GetCursorPos(out MousePoint lpMousePoint);

        [LibraryImport("user32.dll")]
        private static partial void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public static void MouseEvent(MouseEventFlags value)
        {
            GetCursorPos(out MousePoint position);
            mouse_event((int)value, position.X, position.Y, 0, 0);
        }

        public static MousePoint GetMousePosition()
        {
            GetCursorPos(out MousePoint position);
            return position;
        }

        public struct MousePoint(int x, int y)
        {
            public int X = x;
            public int Y = y;
        }
    }
}