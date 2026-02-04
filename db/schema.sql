CREATE DATABASE IF NOT EXISTS TicketSystemDb;
USE TicketSystemDb;

-- Users Table
CREATE TABLE IF NOT EXISTS Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Role ENUM('User', 'Admin') NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Tickets Table
CREATE TABLE IF NOT EXISTS Tickets (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    TicketNumber VARCHAR(20) NOT NULL UNIQUE, -- Generated, e.g., TKT-20231027-001
    Subject VARCHAR(100) NOT NULL,
    Description TEXT NOT NULL,
    Priority ENUM('Low', 'Medium', 'High') NOT NULL,
    Status ENUM('Open', 'In Progress', 'Closed') DEFAULT 'Open',
    CreatedByUserId INT NOT NULL,
    AssignedToUserId INT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id),
    FOREIGN KEY (AssignedToUserId) REFERENCES Users(Id)
);

-- Ticket Status History
CREATE TABLE IF NOT EXISTS TicketStatusHistory (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    TicketId INT NOT NULL,
    OldStatus ENUM('Open', 'In Progress', 'Closed') NULL,
    NewStatus ENUM('Open', 'In Progress', 'Closed') NOT NULL,
    ChangedByUserId INT NOT NULL,
    ChangedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (TicketId) REFERENCES Tickets(Id),
    FOREIGN KEY (ChangedByUserId) REFERENCES Users(Id)
);

-- Ticket Comments
CREATE TABLE IF NOT EXISTS TicketComments (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    TicketId INT NOT NULL,
    Comment TEXT NOT NULL,
    CommentedByUserId INT NOT NULL,
    CommentedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (TicketId) REFERENCES Tickets(Id),
    FOREIGN KEY (CommentedByUserId) REFERENCES Users(Id)
);
