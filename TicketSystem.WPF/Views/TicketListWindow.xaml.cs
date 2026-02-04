using System.Windows;
using TicketSystem.Desktop.Models;
using TicketSystem.Desktop.ViewModels;

namespace TicketSystem.Desktop.Views
{
    public partial class TicketListWindow : Window
    {
        public TicketListWindow(User user)
        {
            InitializeComponent();
            DataContext = new TicketListViewModel(user);
        }
    }
}
