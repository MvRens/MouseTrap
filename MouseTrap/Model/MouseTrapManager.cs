using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using MouseTrap.Hooks;

namespace MouseTrap.Model
{
    public class ForegroundWindowChangedEventArgs
    {
        public string Title { get; }
        public string Executable { get; }
        
        
        public ForegroundWindowChangedEventArgs(string title, string executable)
        {
            Title = title;
            Executable = executable;
        }
    }


    public delegate void ForegroundWindowChangedEventHandler(object sender, ForegroundWindowChangedEventArgs args);


    public class CursorRestrictionChangedEventArgs
    {
        public CursorArea Area;


        public CursorRestrictionChangedEventArgs(CursorArea area)
        {
            Area = area;
        }
    }


    public delegate void CursorRestrictionChangedEventHandler(object sender, CursorRestrictionChangedEventArgs args);


    public class EnabledChangedEventArgs
    {
        public bool Enabled { get; }

        
        public EnabledChangedEventArgs(bool enabled)
        {
            Enabled = enabled;
        }
    }

    public delegate void EnabledChangedEventHandler(object sender, EnabledChangedEventArgs args);



    /// <summary>
    /// Monitors the active window and limits the cursor accordingly.
    /// </summary>
    public class MouseTrapManager : IDisposable
    {
        public event ForegroundWindowChangedEventHandler OnForegroundWindowChanged;
        public event CursorRestrictionChangedEventHandler OnCursorRestrictionChanged;
        public event EnabledChangedEventHandler OnEnabledChanged;


        public bool Enabled
        {
            get => enabled;
            set
            {
                if (value == enabled)
                    return;

                if (value)
                    InstallMouseHook();
                else
                    UninstallMouseHook();

                enabled = value;
                OnEnabledChanged?.Invoke(this, new EnabledChangedEventArgs(value));
            }
        }
        

        private bool started;
        private bool enabled;
        private Timer windowMonitorTimer;
        private MouseLowLevelWindowsHook mouseHook;

        private string lastWindowText;
        private string lastExecutable;
        private IntPtr lastMonitor;
        private CursorArea area;

        private static readonly TimeSpan WindowMonitorPollingInterval = TimeSpan.FromMilliseconds(500);

        
        public void Dispose()
        {
            UninstallMouseHook();

            windowMonitorTimer?.Dispose();
            windowMonitorTimer = null;

            GC.SuppressFinalize(this);
        }
        

        public void Start()
        {
            if (started)
                return;

            // It would be nice to be able to use a CBT windows hook to monitor the active window.
            // Unfortunately that would require a native DLL as it needs to be injected into every process.
            // Doable but more effort and riskier, I'll live with a simple timer for now as it does not need to be instant.
            windowMonitorTimer = new Timer(_ =>
            {
                PollActiveWindow();
            }, null, WindowMonitorPollingInterval, WindowMonitorPollingInterval);

            started = true;
        }



        private void InstallMouseHook()
        {
            mouseHook = new MouseLowLevelWindowsHook();
            mouseHook.OnMouseMove += (_, args) =>
            {
                if (area == null || !enabled)
                    return;

                if (args.X < area.MinX)
                    args.X = area.MinX;
                else if (args.X >= area.MaxX)
                    args.X = area.MaxX - 1;

                if (args.Y < area.MinY)
                    args.Y = area.MinY;
                else if (args.Y >= area.MaxY)
                    args.Y = area.MaxY - 1;
            };

            mouseHook.Hook();
        }


        private void UninstallMouseHook()
        {
            mouseHook?.Dispose();
            mouseHook = null;
        }
        
        
        private void RestrictCursor(CursorArea newArea)
        {
            if (newArea.Equals(area))
                return;

            area = newArea;
            OnCursorRestrictionChanged?.Invoke(this, new CursorRestrictionChangedEventArgs(newArea));
        }


        private void PollActiveWindow()
        {
            // Get information about the current foreground window
            var foregroundWindow = WindowsAPI.GetForegroundWindow();
            
            var windowText = new StringBuilder(255);
            if (WindowsAPI.GetWindowText(foregroundWindow, windowText, windowText.Capacity) == 0)
                return;

            if (WindowsAPI.GetWindowThreadProcessId(foregroundWindow, out var processId) == 0)
                return;

            var process = Process.GetProcessById((int)processId);
            if (process.MainModule == null)
                return;
            
            var executable = Path.GetFileName(process.MainModule.FileName);

            if (windowText.ToString() != lastWindowText || executable != lastExecutable)
            {
                lastWindowText = windowText.ToString();
                lastExecutable = executable;
                
                OnForegroundWindowChanged?.Invoke(this, new ForegroundWindowChangedEventArgs(lastWindowText, lastExecutable));
            }


            // Check the coordinates of the monitor it is on
            var monitor = WindowsAPI.MonitorFromWindow(foregroundWindow, WindowsAPI.MONITOR_DEFAULTTONULL);
            if (monitor == lastMonitor)
                return;

            var monitorInfo = new WindowsAPI.MonitorInfoEx();
            monitorInfo.Init();
            
            if (!WindowsAPI.GetMonitorInfo(monitor, ref monitorInfo))
                return;
            
            lastMonitor = monitor;
            RestrictCursor(new CursorArea(monitorInfo.Monitor));
        }
    }
}
