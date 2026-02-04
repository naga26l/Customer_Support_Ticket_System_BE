using System.Windows;
using TicketSystem.Desktop.Models;
using TicketSystem.Desktop.ViewModels;

namespace TicketSystem.Desktop.Views
{
    public partial class TicketDetailsWindow : Window
    {
        public TicketDetailsWindow(Ticket ticket, User user)
        {
            InitializeComponent();
            DataContext = new TicketDetailsViewModel(ticket, user);
        }
    }
}
