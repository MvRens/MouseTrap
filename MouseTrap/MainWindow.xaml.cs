using MouseTrap.Model;
using MouseTrap.Settings;
using MouseTrap.ViewModel;

namespace MouseTrap
{
    // ReSharper disable once UnusedMember.Global - used by WPF
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var userSettings = new UserSettings();
            userSettings.Read();
            
            var manager = new MouseTrapManager();
            var viewModel = new MainWindowViewModel(manager, userSettings);

            
            Dispatcher.ShutdownStarted += (_, _) =>
            {
                manager?.Dispose();
                manager = null;
            };

            if (userSettings.EnableAtStartup)
                manager.Enabled = true;

            manager.Start();
            DataContext = viewModel;
        }
    }
}
