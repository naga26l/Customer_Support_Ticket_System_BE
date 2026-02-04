using System.Windows;
using TicketSystem.Desktop.Models;
using TicketSystem.Desktop.ViewModels;

namespace TicketSystem.Desktop.Views
{
    public partial class CreateTicketWindow : Window
    {
        public CreateTicketWindow(User user)
        {
            InitializeComponent();
            DataContext = new CreateTicketViewModel(user);
        }
    }
}
