using System.Windows;
using System.Windows.Input;
using TicketSystem.Desktop.Models;
using TicketSystem.Desktop.Services;
using TicketSystem.Desktop.Utilities;

namespace TicketSystem.Desktop.ViewModels
{
    public class TicketDetailsViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private readonly User _currentUser;
        private Ticket _ticket;
        private string _assignToUserId; // Simple Text Input for now. Ideal would be a dropdown of admins.
        private string _newStatus;

        public TicketDetailsViewModel(Ticket ticket, User user)
        {
            _ticket = ticket;
            _currentUser = user;
            _apiService = new ApiService();
            
            AssignTicketCommand = new RelayCommand(ExecuteAssignTicket);
            UpdateStatusCommand = new RelayCommand(ExecuteUpdateStatus);
        }

        public Ticket Ticket
        {
            get => _ticket;
            set { _ticket = value; OnPropertyChanged(); }
        }

        public Visibility AdminVisibility => _currentUser.Role == "Admin" ? Visibility.Visible : Visibility.Collapsed;

        public string AssignToUserId
        {
            get => _assignToUserId;
            set { _assignToUserId = value; OnPropertyChanged(); }
        }

        public string NewStatus
        {
            get => _newStatus;
            set { _newStatus = value; OnPropertyChanged(); }
        }

        public ICommand AssignTicketCommand { get; }
        public ICommand UpdateStatusCommand { get; }

        private async void ExecuteAssignTicket(object obj)
        {
            if (int.TryParse(AssignToUserId, out int adminId))
            {
                var success = await _apiService.AssignTicketAsync(Ticket.Id, adminId);
                if (success)
                {
                    MessageBox.Show("Ticket Assigned!");
                    // Refresh not implemented in detail view, close/reopen needed or event bus
                }
                else
                {
                    MessageBox.Show("Assignment failed.");
                }
            }
            else
            {
                MessageBox.Show("Invalid User ID");
            }
        }

        private async void ExecuteUpdateStatus(object obj)
        {
            if (string.IsNullOrEmpty(NewStatus)) return;
            
            var success = await _apiService.UpdateStatusAsync(Ticket.Id, NewStatus);
            if (success)
            {
                MessageBox.Show("Status Updated!");
                Ticket.Status = NewStatus;
                OnPropertyChanged(nameof(Ticket));
            }
             else
            {
                MessageBox.Show("Update failed.");
            }
        }
    }
}
