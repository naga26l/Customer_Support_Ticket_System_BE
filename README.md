# Customer Support Ticket System

A comprehensive Customer Support Ticket System featuring a robust ASP.NET Web API backend and cross-platform desktop frontend options (WinForms & WPF). This system allows users to create and track support tickets, while providing administrators with tools to manage, assign, and resolve them.

##  Project Overview

The system is designed with a clear separation of concerns, utilizing an API-first approach. All communication between the desktop clients and the database happens through the Web API, ensuring better security and scalability.

### Key Features
- **Role-Based Access**: Specialized views and actions for both `Users` and `Admins`.
- **Ticket Management**: Create, view, assign, and update ticket statuses.
- **Audit Trail**: Tracking of status changes and history.
- **Cross-Frontend**: Choose between a modern WPF interface or a classic WinForms desktop application.
- **Cloud Database**: Integrated with Aiven Cloud MySQL for reliable data persistence.

##  Tech Stack

### Backend
- **Framework**: ASP.NET Core Web API (C#)
- **Data Access**: ADO.NET with `MySqlConnector`
- **Database**: MySQL (hosted on Aiven Cloud)
- **Features**: RESTful endpoints, Role-based logic, Automatic schema initialization.

### Frontend
- **Desktop (WPF)**: Windows Presentation Foundation using the **MVVM Pattern** for clean logic separation.
- **Desktop (WinForms)**: Traditional Windows Forms for lightweight interaction.
- **Communication**: `HttpClient` for asynchronous API consumption.

## Steps to Run Locally

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.
- MySQL server (if not using the pre-configured cloud database).

### 1. Setup the Backend
1. Navigate to the API directory:
   ```bash
   cd TicketSystem.Api
   ```
2. Configure the database connection in `appsettings.json`. Replace `YOUR_PASSWORD_HERE` with your actual MySQL password.
3. Run the API:
   ```bash
   dotnet run
   ```
   *The DB schema will automatically initialize on the first run.*

### 2. Setup the Frontend
You can run either the WPF or WinForms client.

#### WPF Client (Recommended)
1. Open a new terminal.
2. Navigate to the WPF directory:
   ```bash
   cd TicketSystem.WPF
   ```
3. Start the application:
   ```bash
   dotnet run
   ```

#### WinForms Client
1. Open a new terminal.
2. Navigate to the Desktop directory:
   ```bash
   cd TicketSystem.Desktop
   ```
3. Start the application:
   ```bash
   dotnet run
   ```

## Design Decisions & Assumptions

- **API-Direct Only**: One of the core requirements was ensuring the desktop applications have **zero** direct database access. All data flows exclusively through the Web API.
- **ADO.NET over ORM**: To keep the project lightweight and demonstrate raw SQL proficiency, ADO.NET was chosen over Entity Framework.
- **MVVM in WPF**: The WPF application strictly follows the Model-View-ViewModel pattern to ensure testability and maintainability.
- **Automatic Schema Creation**: The `DbInitializer` in the API project was implemented to streamline the onboarding process, automatically creating necessary tables if they don't exist.
- **Security Check**: Sensitive credentials have been removed from the source code. Users must provide their own database passwords in `appsettings.json`.

##  License
This project is open-source and available under the MIT License.