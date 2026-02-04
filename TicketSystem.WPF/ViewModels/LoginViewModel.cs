using System.Windows;
using System.Windows.Input;
using TicketSystem.Desktop.Services;
using TicketSystem.Desktop.Utilities;

namespace TicketSystem.Desktop.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private string _username;
        /* Note: Password binding in WPF is tricky due to security (PasswordBox). 
           We will handle password via parameter or a helper in the View. */
        
        public LoginViewModel()
        {
            _apiService = new ApiService();
            LoginCommand = new RelayCommand(ExecuteLogin);
        }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        private async void ExecuteLogin(object parameter)
        {
            var passwordBox = parameter as System.Windows.Controls.PasswordBox;
            var password = passwordBox?.Password;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter username and password");
                return;
            }

            var user = await _apiService.LoginAsync(Username, password);
            if (user != null)
            {
                // Navigate to Ticket List
                var ticketListWindow = new Views.TicketListWindow(user);
                ticketListWindow.Show();
                
                // Close current window (Login)
                if (parameter is DependencyObject obj)
                {
                    Window.GetWindow(obj)?.Close();
                }
            }
            else
            {
                MessageBox.Show("Invalid Credentials");
            }
        }
    }
}
