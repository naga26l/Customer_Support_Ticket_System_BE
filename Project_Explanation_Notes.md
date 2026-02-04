# Project Explanation Notes

## 1. Project Structure
The solution `CustomerSupportTicketSystem` contains two main projects:
1. **TicketSystem.Api**: This is the Backend. It uses ASP.NET Core Web API.
2. **TicketSystem.Desktop**: This is the Frontend. It uses WPF (Windows Presentation Foundation).

## 2. Backend (TicketSystem.Api)
- **Controllers**: 
    - `AuthController`: Handles Login and Registration.
    - `TicketsController`: Handles all ticket operations (Create, List, Details, Assign, Updates).
- **Data Access**: 
    - `DatabaseHelper`: Manages MySQL connection using ADO.NET.
    - `UserRepository` & `TicketRepository`: Contain the raw SQL queries to interact with your Aiven Cloud Database.
- **Database Initialization**: 
    - `DbInitializer`: Automatically checks if tables exist and creates them if they don't when the API starts.

## 3. Frontend (TicketSystem.Desktop)
- **MVVM Pattern**: Model-View-ViewModel. This separates the UI (View) from the Logic (ViewModel).
- **Services**: 
    - `ApiService`: Validates the connection to the Backend API using `HttpClient`.
- **Views**:
    - `LoginWindow`: Entry point over `MainWindow`.
    - `TicketListWindow`: Shows the grid of tickets. Adapts to Admin/User role.
    - `CreateTicketWindow`: Simple form to add tickets.
    - `TicketDetailsWindow`: Shows details and Admin actions (Assign/Update Status).

## 4. How it works
1. **Startup**: 
    - `App.xaml` starts `LoginWindow`.
    - `LoginWindow` asks for credentials.
    - `LoginViewModel` sends credentials to API (`POST /api/auth/login`).
2. **Ticket Flow**:
    - On success, `TicketListWindow` opens.
    - It calls `GET /api/tickets`.
    - The API queries the Database and returns a list.
    - The Desktop App displays this list.
3. **Actions**:
    - When you "Create Ticket", it sends a `POST`.
    - When an Admin "Assigns" or "Updates Status", it sends `POST` requests to specific endpoints.

## 5. Database
- Your data is stored in Aiven Cloud MySQL.
- The `schema.sql` (and `DbInitializer`) defines 4 tables: `Users`, `Tickets`, `TicketStatusHistory`, `TicketComments`.

## 6. Running It
1. Keep the Terminal open where `TicketSystem.Api` is running (`dotnet run`).
2. Open a new terminal for `TicketSystem.Desktop` and run `dotnet run`.
