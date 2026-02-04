using MySql.Data.MySqlClient;

namespace TicketSystem.Api.Data
{
    public class DbInitializer
    {
        private readonly DatabaseHelper _dbHelper;

        public DbInitializer(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public void Initialize()
        {
            using (var conn = _dbHelper.GetConnection())
            {
                conn.Open();
                
                // Users
                string userTable = @"CREATE TABLE IF NOT EXISTS Users (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Username VARCHAR(50) NOT NULL UNIQUE,
                    PasswordHash VARCHAR(255) NOT NULL,
                    Role ENUM('User', 'Admin') NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                );";
                ExecuteCommand(conn, userTable);

                // Tickets
                string ticketTable = @"CREATE TABLE IF NOT EXISTS Tickets (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    TicketNumber VARCHAR(50) NOT NULL UNIQUE, 
                    Subject VARCHAR(100) NOT NULL,
                    Description TEXT NOT NULL,
                    Priority ENUM('Low', 'Medium', 'High') NOT NULL,
                    Status ENUM('Open', 'In Progress', 'Closed') DEFAULT 'Open',
                    CreatedByUserId INT NOT NULL,
                    AssignedToUserId INT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id),
                    FOREIGN KEY (AssignedToUserId) REFERENCES Users(Id)
                );";
                ExecuteCommand(conn, ticketTable);

                // TicketStatusHistory
                string historyTable = @"CREATE TABLE IF NOT EXISTS TicketStatusHistory (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    TicketId INT NOT NULL,
                    OldStatus ENUM('Open', 'In Progress', 'Closed') NULL,
                    NewStatus ENUM('Open', 'In Progress', 'Closed') NOT NULL,
                    ChangedByUserId INT NOT NULL,
                    ChangedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (TicketId) REFERENCES Tickets(Id),
                    FOREIGN KEY (ChangedByUserId) REFERENCES Users(Id)
                );";
                ExecuteCommand(conn, historyTable);
                
                 // TicketComments
                string commentsTable = @"CREATE TABLE IF NOT EXISTS TicketComments (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    TicketId INT NOT NULL,
                    Comment TEXT NOT NULL,
                    CommentedByUserId INT NOT NULL,
                    CommentedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (TicketId) REFERENCES Tickets(Id),
                    FOREIGN KEY (CommentedByUserId) REFERENCES Users(Id)
                );";
                ExecuteCommand(conn, commentsTable);
            }
        }

        private void ExecuteCommand(MySqlConnection conn, string query)
        {
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}
