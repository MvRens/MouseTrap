using System;
using System.ComponentModel;
using System.Diagnostics;

namespace MouseTrap.Hooks
{
    /// <summary>
    /// Provides a base implementation for windows hoooks. Note that only global hooks are supported,
    /// since you can not inject this application into other processes.
    /// </summary>
    public abstract class BaseWindowsHook : IDisposable
    {
        private readonly int idHook;
        private IntPtr hookHandle;
        private readonly WindowsAPI.HOOKPROC hookProc;
        
        
        protected BaseWindowsHook(int idHook)
        {
            this.idHook = idHook;
            hookProc = HookCallback;
        }


        public void Dispose()
        {
            Unhook();
            GC.SuppressFinalize(this);
        }


        public void Hook()
        {
            if (hookHandle != IntPtr.Zero)
                return;

            using var currentProcess = Process.GetCurrentProcess();
            using var currentModule = currentProcess.MainModule;
            
            if (currentModule == null)
                return;
                
            var moduleHandle = WindowsAPI.GetModuleHandle(currentModule.ModuleName);
            if (moduleHandle == IntPtr.Zero)
                throw new Win32Exception();

            // Pass hookProc instead of HookCallback directly, otherwise the garbage collector will
            // unload it almost immediately resulting in an error
            hookHandle = WindowsAPI.SetWindowsHookEx(idHook, hookProc, moduleHandle, 0);

            if (hookHandle == IntPtr.Zero)
                throw new Win32Exception();
        }


        public void Unhook()
        {
            if (hookHandle == IntPtr.Zero)
                return;

            var result = WindowsAPI.UnhookWindowsHookEx(hookHandle);
            hookHandle = IntPtr.Zero;

            if (!result)
                throw new Win32Exception();
        }


        protected abstract int HookCallback(int nCode, IntPtr wParam, IntPtr lParam);

        protected int CallNextHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            return WindowsAPI.CallNextHookEx(hookHandle, nCode, wParam, lParam);
        }
    }
}
