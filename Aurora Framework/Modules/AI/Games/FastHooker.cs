using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


public sealed class FastHooker : IDisposable
{
    private static class WinAPI
    {
        public static class Kernel32
        {
            [DllImport("kernel32")]
            public static extern IntPtr LoadLibrary(string lpFileName);
        }

        public static class User32
        {
            // KBDLLHOOKSTRUCT
            // https://msdn.microsoft.com/ru-ru/library/windows/desktop/ms644967(v=vs.85).aspx
            public struct KeyboardHookStruct
            {
                public uint VKCode;
                public uint ScanCode;
                public uint Flags;
                public uint Time;
                public IntPtr dwExtraInfo;
            }

            // MSLLHOOKSTRUCT
            // https://msdn.microsoft.com/ru-ru/library/windows/desktop/ms644970(v=vs.85).aspx
            public struct MouseHookStruct
            {
                public int X;
                public int Y;
                public uint MouseData;
                public uint Flags;
                public uint Time;
                public IntPtr dwExtraInfo;
            }

            // Константы WH_*
            public enum WindowsHook : int
            {
                KeyboardLowLevel = 13,
                MouseLowLevel = 14,
            }

            // Константы WM_*
            public enum WindowsMessage : int
            {
                KeyDown = 0x100,
                KeyUp = 0x101,

                SysKeyDown = 0x104,
                SysKeyUp = 0x105,

                MouseMove = 0x200,
                LeftButtonDown = 0x201,
                LeftButtonUp = 0x202,

                RightButtonDown = 0x204,
                RightButtonUp = 0x205,

                MiddleButtonDown = 0x207,
                MiddleButtonUp = 0x208,
            }

            public delegate int KeyboardHookProc(int code,
                WindowsMessage wParam, ref KeyboardHookStruct lParam);

            public delegate int MouseHookProc(int code,
                WindowsMessage wParam, ref MouseHookStruct lParam);

            [DllImport("user32")]
            public static extern int CallNextHookEx(IntPtr hHk, int nCode,
                WindowsMessage wParam, ref KeyboardHookStruct lParam);

            [DllImport("user32")]
            public static extern int CallNextHookEx(IntPtr hHk, int nCode,
                WindowsMessage wParam, ref MouseHookStruct lParam);

            [DllImport("user32")]
            public static extern IntPtr SetWindowsHookEx(WindowsHook idHook,
                KeyboardHookProc lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32")]
            public static extern IntPtr SetWindowsHookEx(WindowsHook idHook,
                MouseHookProc lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32")]
            public static extern bool UnhookWindowsHookEx(IntPtr hHk);
        }
    }


    // Дескрипторы хуков
    private readonly IntPtr _keyboardHookHandle;

    // Хуки
    private readonly WinAPI.User32.KeyboardHookProc _keyboardCallback;

    public event KeyEventHandler KeyDown = (s, e) => { };
    public event KeyEventHandler KeyUp = (s, e) => { };

    public FastHooker()
    {
        // Создадим колбэки и сохраним их в полях класса, чтобы их не собрал GC
        _keyboardCallback = new WinAPI.User32.KeyboardHookProc((int code,
            WinAPI.User32.WindowsMessage wParam, ref WinAPI.User32.KeyboardHookStruct lParam) =>
        {
                // Если code < 0, мы не должны обрабатывать это сообщение системы
                if (code >= 0)
            {
                var key = (Keys)lParam.VKCode;
                var eventArgs = new KeyEventArgs(key);

                    // В зависимости от типа пришедшего сообщения вызовем то или иное событие
                    switch (wParam)
                {
                    case WinAPI.User32.WindowsMessage.KeyDown:
                    case WinAPI.User32.WindowsMessage.SysKeyDown:
                        KeyDown(this, eventArgs);
                        break;

                    case WinAPI.User32.WindowsMessage.KeyUp:
                    case WinAPI.User32.WindowsMessage.SysKeyUp:
                        KeyUp(this, eventArgs);
                        break;
                }

                    // Если событие помечено приложением как обработанное,
                    // прервём дальнейшее распространение сообщения
                    if (eventArgs.Handled)
                    return 1;
            }

                // Вызовем следующий обработчик
                return WinAPI.User32.CallNextHookEx(_keyboardHookHandle, code, wParam, ref lParam);
        });

        // В SetWindowsHookEx следует передать дескриптор библиотеки user32.dll
        // Библиотека user32 всё равно всегда загружена в приложениях .NET,
        // хранить и освобождать дескриптор или что-либо ещё с ним делать нет необходимости
        IntPtr user32Handle = WinAPI.Kernel32.LoadLibrary("user32");

        // Установим хуки
        _keyboardHookHandle = WinAPI.User32.SetWindowsHookEx(
            WinAPI.User32.WindowsHook.KeyboardLowLevel, _keyboardCallback, user32Handle, 0);
    }

    #region IDisposable implementation

    private bool _isDisposed = false;

    public bool IsDisposed { get { return _isDisposed; } }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;

        // Удалим хуки
        WinAPI.User32.UnhookWindowsHookEx(_keyboardHookHandle);
    }

    ~FastHooker()
    {
        Dispose();
    }

    #endregion
}

