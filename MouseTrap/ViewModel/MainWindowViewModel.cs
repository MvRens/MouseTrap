using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MouseTrap.Model;
using MouseTrap.Settings;

namespace MouseTrap.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly UserSettings userSettings;
        private string lastActiveWindow;
        private string lastActiveExecutable;
        private string cursorArea;
        private bool enabled;
        

        public string LastActiveWindow
        {
            get => lastActiveWindow;
            set => SetProperty(ref lastActiveWindow, value);
        }
        
        
        public string LastActiveExecutable
        {
            get => lastActiveExecutable;
            set => SetProperty(ref lastActiveExecutable, value);
        }


        public string CursorArea
        {
            get => cursorArea;
            set => SetProperty(ref cursorArea, value);
        }


        public bool Enabled
        {
            get => enabled;
            set => SetProperty(ref enabled, value);
        }


        public bool EnableAtStartup
        {
            get => userSettings?.EnableAtStartup ?? false;
            set => userSettings.EnableAtStartup = value;
        }


        public ICommand EnableCommand { get; }
        public ICommand DisableCommand { get; }



        public MainWindowViewModel(MouseTrapManager manager, UserSettings userSettings)
        {
            this.userSettings = userSettings;
            
            if (manager == null)
                return;

            EnableCommand = new RelayCommand(() => manager.Enabled = true);
            DisableCommand = new RelayCommand(() => manager.Enabled = false);


            enabled = manager.Enabled;
            
            manager.OnEnabledChanged += (_, args) =>
            {
                Enabled = args.Enabled;
            };

            manager.OnForegroundWindowChanged += (_, args) =>
            {
                LastActiveWindow = args.Title;
                LastActiveExecutable = args.Executable;
            };

            manager.OnCursorRestrictionChanged += (_, args) =>
            {
                CursorArea = $"{args.Area.MinX}, {args.Area.MinY} - {args.Area.MaxX}, {args.Area.MaxY}";
            };
        }
    }
    
    
    
    public class MainWindowViewModelDesignTime : MainWindowViewModel
    {
        public MainWindowViewModelDesignTime() : base(null, null)
        {
            LastActiveWindow = "Visual Studio";
            LastActiveExecutable = "devenv.exe";
            CursorArea = "0, 0 - 1920, 1080";
        }
    }
}
