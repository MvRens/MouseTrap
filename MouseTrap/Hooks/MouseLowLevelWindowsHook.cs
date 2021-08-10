using System;
using System.Runtime.InteropServices;

namespace MouseTrap.Hooks
{
    public class MouseMoveEventArgs
    {
        private int x;
        public int X
        {
            get => x;
            set
            {
                if (value == x)
                    return;

                x = value;
                Modified = true;
            }
        }

        private int y;
        public int Y
        {
            get => y;
            set
            {
                if (value == y)
                    return;

                y = value;
                Modified = true;
            }
        }
        
        public bool Modified { get; private set; }

        public MouseMoveEventArgs(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public delegate void MouseMoveEventHandler(object sender, MouseMoveEventArgs args);
    
    
    /// <summary>
    /// Implements the low-level mouse hook.
    /// </summary>
    /// <remarks>
    /// This is not a complete implementation, only the WM_MOUSEMOVE message is passed along.
    /// </remarks>
    public class MouseLowLevelWindowsHook : BaseWindowsHook
    {
        public event MouseMoveEventHandler OnMouseMove; 


        public MouseLowLevelWindowsHook() : base(WindowsAPI.WH_MOUSE_LL)
        {
        }


        protected override int HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode != 0 || wParam.ToInt32() != WindowsAPI.WM_MOUSEMOVE)
                return CallNextHook(nCode, wParam, lParam);

            var msllHookStruct = (WindowsAPI.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(WindowsAPI.MSLLHOOKSTRUCT));
            var args = new MouseMoveEventArgs(msllHookStruct.pt.x, msllHookStruct.pt.y);
            
            OnMouseMove?.Invoke(this, args);

            if (!args.Modified) 
                return CallNextHook(nCode, wParam, lParam);
            
            WindowsAPI.SetCursorPos(args.X, args.Y);
            return 1;
        }
    }
}
