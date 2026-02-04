# Customer Support Ticket System

A Desktop Application (WPF) with an ASP.NET Core Web API Backend and MySQL Database.

## Technology Stack
- **Frontend**: C# WinForms (.NET 8/9)
- **Backend**: ASP.NET Core Web API
- **Database**: MySQL (Aiven Cloud)
- **Communication**: HTTP / JSON

## Setup & Run

### Prerequisites
- .NET SDK (8.0 or later)
- Internet connection (for Aiven Cloud MySQL)

### Steps
1. **Start the Backend**:
   ```bash
   cd TicketSystem.Api
   dotnet run
   ```
   *Note: This must be running for the Desktop App to work.*

2. **Start the Frontend**:
   Open a new terminal.
   ```bash
   cd TicketSystem.Desktop
   dotnet run
   ```

3. **Login**:
   - Make sure you have users in your database.
   - For testing, you can insert users manually via SQL or use the provided `AuthController` logic.

## Logic Overview
- **Users** create tickets.
- **Admins** view all tickets, assign them, and update status.
- **Validation** checks for required fields.
- **History** tracks status changes (Database side logic supported).

## Source Code
- `TicketSystem.Api`: Backend
- `TicketSystem.Desktop`: Frontend
- `db`: Database Schema Scripts