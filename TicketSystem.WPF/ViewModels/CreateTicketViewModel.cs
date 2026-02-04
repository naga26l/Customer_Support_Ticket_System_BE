using System.Windows;
using System.Windows.Input;
using TicketSystem.Desktop.Models;
using TicketSystem.Desktop.Services;
using TicketSystem.Desktop.Utilities;

namespace TicketSystem.Desktop.ViewModels
{
    public class CreateTicketViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private readonly User _currentUser;
        
        private string _subject;
        private string _description;
        private string _priority = "Low";

        public CreateTicketViewModel(User user)
        {
            _currentUser = user;
            _apiService = new ApiService();
            SubmitCommand = new RelayCommand(ExecuteSubmit);
        }

        public string Subject
        {
            get => _subject;
            set { _subject = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public string Priority
        {
            get => _priority;
            set { _priority = value; OnPropertyChanged(); }
        }

        public ICommand SubmitCommand { get; }

        private async void ExecuteSubmit(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Subject) || string.IsNullOrWhiteSpace(Description))
            {
                MessageBox.Show("Subject and Description are required");
                return;
            }

            var request = new
            {
                Subject = Subject,
                Description = Description,
                Priority = Priority,
                UserId = _currentUser.Id
            };

            var result = await _apiService.CreateTicketAsync(request);
            if (result != null)
            {
                MessageBox.Show("Ticket Created Successfully!");
                // Close the window
                if (parameter is Window window)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            else
            {
                MessageBox.Show("Failed to create ticket.");
            }
        }
    }
}
