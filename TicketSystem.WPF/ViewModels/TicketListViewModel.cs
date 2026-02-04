using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TicketSystem.Desktop.Models;
using TicketSystem.Desktop.Services;
using TicketSystem.Desktop.Utilities;

namespace TicketSystem.Desktop.ViewModels
{
    public class TicketListViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private readonly User _currentUser;
        private ObservableCollection<Ticket> _tickets;

        public TicketListViewModel(User user)
        {
            _currentUser = user;
            _apiService = new ApiService();
            Tickets = new ObservableCollection<Ticket>();
            LoadTickets();
            RefreshCommand = new RelayCommand(_ => LoadTickets());
            CreateTicketCommand = new RelayCommand(OpenCreateTicket);
            ViewDetailsCommand = new RelayCommand(OpenTicketDetails);
        }

        public ObservableCollection<Ticket> Tickets
        {
            get => _tickets;
            set { _tickets = value; OnPropertyChanged(); }
        }

        public string WelcomeMessage => $"Welcome, {_currentUser.Username} ({_currentUser.Role})";
        
        // Visibility for Admin features could be handled here
        public Visibility AdminVisibility => _currentUser.Role == "Admin" ? Visibility.Visible : Visibility.Collapsed;

        public ICommand RefreshCommand { get; }
        public ICommand CreateTicketCommand { get; }
        public ICommand ViewDetailsCommand { get; }

        private async void LoadTickets()
        {
            int? userId = _currentUser.Role == "Admin" ? (int?)null : _currentUser.Id;
            var ticketList = await _apiService.GetTicketsAsync(userId);
            Tickets = new ObservableCollection<Ticket>(ticketList);
        }

        private void OpenCreateTicket(object obj)
        {
            var createTicketWindow = new Views.CreateTicketWindow(_currentUser);
            if (createTicketWindow.ShowDialog() == true)
            {
                LoadTickets();
            }
        }
        
        private void OpenTicketDetails(object obj)
        {
            if (obj is Ticket ticket)
            {
                // Refresh ticket details before showing? For now just pass the object
                var detailsWindow = new Views.TicketDetailsWindow(ticket, _currentUser);
                detailsWindow.ShowDialog();
                LoadTickets(); // Refresh list after potential updates
            }
        }
    }
}
