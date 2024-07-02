namespace MouseOps
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public partial class MouseOperations
    {
        // Mouse hook related declarations
        private static IntPtr hookID = IntPtr.Zero;
        private static LowLevelMouseProc mouseProc = HookCallback;

        [LibraryImport("user32.dll", SetLastError = true)]
        private static partial IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool UnhookWindowsHookEx(IntPtr hhk);

        [LibraryImport("user32.dll", SetLastError = true)]
        private static partial IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        private static partial IntPtr GetModuleHandle(string lpModuleName);


        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_MOUSE_LL = 14;
        private const int WM_XBUTTONDOWN = 0x020B;
        private const int XBUTTON1 = 0x0001;
        private const int XBUTTON2 = 0x0002;

        private static bool isClickingEnabled = false;

        public static void StartMouseHook()
        {
            hookID = SetHook(mouseProc);
        }

        public static void StopMouseHook()
        {
            UnhookWindowsHookEx(hookID);
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using ProcessModule curModule = Process.GetCurrentProcess()!.MainModule!;
            return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == WM_XBUTTONDOWN)
            {
                int mouseData = Marshal.ReadInt32(lParam + 4);
                if ((mouseData & XBUTTON1) != 0)
                {
                    isClickingEnabled = !isClickingEnabled;
                    Console.WriteLine("Toggled auto-clicking to " + (isClickingEnabled ? "enabled" : "disabled"));
                }
            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        public static bool IsClickingEnabled => isClickingEnabled;
    }
}
