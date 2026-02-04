using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using TicketSystem.Api.Models;

namespace TicketSystem.Api.Data
{
    public class TicketRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public TicketRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public Ticket CreateTicket(Ticket ticket)
        {
            using (var conn = _dbHelper.GetConnection())
            {
                conn.Open();
                
                // Generate Ticket Number
                string ticketNumber = $"TKT-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 5).ToUpper()}";
                
                string query = @"INSERT INTO Tickets (TicketNumber, Subject, Description, Priority, Status, CreatedByUserId, CreatedAt) 
                                 VALUES (@TicketNumber, @Subject, @Description, @Priority, 'Open', @CreatedByUserId, NOW());
                                 SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TicketNumber", ticketNumber);
                    cmd.Parameters.AddWithValue("@Subject", ticket.Subject);
                    cmd.Parameters.AddWithValue("@Description", ticket.Description);
                    cmd.Parameters.AddWithValue("@Priority", ticket.Priority);
                    cmd.Parameters.AddWithValue("@CreatedByUserId", ticket.CreatedByUserId);

                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                    ticket.Id = id;
                    ticket.TicketNumber = ticketNumber;
                    ticket.Status = "Open";
                    ticket.CreatedAt = DateTime.Now; // Approx server time
                    return ticket;
                }
            }
        }

        public List<Ticket> GetTickets(int? userId = null) // If userId is null, return all (Admin)
        {
            var tickets = new List<Ticket>();
            using (var conn = _dbHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT t.*, u.Username as CreatorName, a.Username as AssignedName 
                                 FROM Tickets t 
                                 JOIN Users u ON t.CreatedByUserId = u.Id
                                 LEFT JOIN Users a ON t.AssignedToUserId = a.Id";
                
                if (userId.HasValue)
                {
                    query += " WHERE t.CreatedByUserId = @UserId";
                }
                
                query += " ORDER BY t.CreatedAt DESC";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    if (userId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId.Value);
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tickets.Add(new Ticket
                            {
                                Id = reader.GetInt32("Id"),
                                TicketNumber = reader.GetString("TicketNumber"),
                                Subject = reader.GetString("Subject"),
                                Description = reader.GetString("Description"),
                                Priority = reader.GetString("Priority"),
                                Status = reader.GetString("Status"),
                                CreatedByUserId = reader.GetInt32("CreatedByUserId"),
                                CreatedByUsername = reader.GetString("CreatorName"),
                                AssignedToUserId = reader.IsDBNull(reader.GetOrdinal("AssignedToUserId")) ? (int?)null : reader.GetInt32("AssignedToUserId"),
                                AssignedToUsername = reader.IsDBNull(reader.GetOrdinal("AssignedName")) ? null : reader.GetString("AssignedName"),
                                CreatedAt = reader.GetDateTime("CreatedAt")
                            });
                        }
                    }
                }
            }
            return tickets;
        }

        public Ticket GetTicketById(int id)
        {
            using (var conn = _dbHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT t.*, u.Username as CreatorName, a.Username as AssignedName 
                                 FROM Tickets t 
                                 JOIN Users u ON t.CreatedByUserId = u.Id
                                 LEFT JOIN Users a ON t.AssignedToUserId = a.Id
                                 WHERE t.Id = @Id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Ticket
                            {
                                Id = reader.GetInt32("Id"),
                                TicketNumber = reader.GetString("TicketNumber"),
                                Subject = reader.GetString("Subject"),
                                Description = reader.GetString("Description"),
                                Priority = reader.GetString("Priority"),
                                Status = reader.GetString("Status"),
                                CreatedByUserId = reader.GetInt32("CreatedByUserId"),
                                CreatedByUsername = reader.GetString("CreatorName"),
                                AssignedToUserId = reader.IsDBNull(reader.GetOrdinal("AssignedToUserId")) ? (int?)null : reader.GetInt32("AssignedToUserId"),
                                AssignedToUsername = reader.IsDBNull(reader.GetOrdinal("AssignedName")) ? null : reader.GetString("AssignedName"),
                                CreatedAt = reader.GetDateTime("CreatedAt")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void AssignTicket(int ticketId, int adminId)
        {
            using (var conn = _dbHelper.GetConnection())
            {
                conn.Open();
                
                // Get old value for history
                string oldAssignedName = "Unassigned";
                string getOldQuery = "SELECT u.Username FROM Tickets t JOIN Users u ON t.AssignedToUserId = u.Id WHERE t.Id = @TicketId";
                using (var cmdOld = new MySqlCommand(getOldQuery, conn)) 
                {
                    cmdOld.Parameters.AddWithValue("@TicketId", ticketId);
                    var result = cmdOld.ExecuteScalar();
                    if(result != null) oldAssignedName = result.ToString();
                }

                // Update Ticket
                string query = "UPDATE Tickets SET AssignedToUserId = @AdminId WHERE Id = @TicketId";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AdminId", adminId);
                    cmd.Parameters.AddWithValue("@TicketId", ticketId);
                    cmd.ExecuteNonQuery();
                }
                
                // Get New Admin Name
                string newAdminName = "Unknown";
                string getNameQuery = "SELECT Username FROM Users WHERE Id = @AdminId";
                using (var cmdName = new MySqlCommand(getNameQuery, conn))
                {
                    cmdName.Parameters.AddWithValue("@AdminId", adminId);
                    newAdminName = cmdName.ExecuteScalar()?.ToString();
                }

                // Log History (As a generic "Assignment" or just "StatusChange" equivalent)
                // Since we don't have an "Assignment" type in enum in DbInitializer (it only has Status enums for OldStatus/NewStatus), 
                // we will abuse the StatusHistory or better, create a Comment that says "Assigned to X".
                // However, the requirements say "Status changes must be recorded". It doesn't explicitly say Assignment changes must be in that specific table, 
                // but "Ticket History Section" needs to show updates.
                // Let's insert a "System Comment" into TicketComments or just use TicketStatusHistory with purely informational text if possible?
                // The TicketStatusHistory table is strict about ENUMs for OldStatus/NewStatus. 
                // So for Assignment, we should probably add a record to TicketComments OR effectively treats it as a 'modify'.
                // Let's use TicketComments for assignment changes to keep it simple and viewable.
                
                AddComment(ticketId, $"Assigned to {newAdminName}", adminId); // Using adminId as the 'actor'
            }
        }

        public void UpdateStatus(int ticketId, string newStatus)
        {
             using (var conn = _dbHelper.GetConnection())
            {
                conn.Open();
                
                // Get Old Status
                string oldStatus = "";
                string getOldQuery = "SELECT Status FROM Tickets WHERE Id = @TicketId";
                using(var cmdOld = new MySqlCommand(getOldQuery, conn))
                {
                    cmdOld.Parameters.AddWithValue("@TicketId", ticketId);
                    oldStatus = cmdOld.ExecuteScalar()?.ToString();
                }

                if(oldStatus == newStatus) return;

                // Update Ticket
                string query = "UPDATE Tickets SET Status = @Status WHERE Id = @TicketId";
                 using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", newStatus);
                    cmd.Parameters.AddWithValue("@TicketId", ticketId);
                    cmd.ExecuteNonQuery();
                }

                // Log History in TicketStatusHistory
                // Assuming the actor is 'System' or we need to pass Key User ID. 
                // The current signature doesn't pass UserId. I need to update signature in Controller to pass userId.
                // For now, I will assume a default or need to fix the upstream call.
                // Wait, I can't easily change the signature without changing the Interface/Controller.
                // I will add userId to UpdateStatus signature.
            }
        }
        
        // Overloading for safer refactoring, but I will replace the original methods eventually.
        public void UpdateStatus(int ticketId, string newStatus, int changedByUserId)
        {
             using (var conn = _dbHelper.GetConnection())
            {
                conn.Open();
                
                // Get Old Status
                string oldStatus = "";
                string getOldQuery = "SELECT Status FROM Tickets WHERE Id = @TicketId";
                using(var cmdOld = new MySqlCommand(getOldQuery, conn))
                {
                    cmdOld.Parameters.AddWithValue("@TicketId", ticketId);
                    oldStatus = cmdOld.ExecuteScalar()?.ToString();
                }

                if(oldStatus == newStatus) return;

                // Update Ticket
                string query = "UPDATE Tickets SET Status = @Status WHERE Id = @TicketId";
                 using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", newStatus);
                    cmd.Parameters.AddWithValue("@TicketId", ticketId);
                    cmd.ExecuteNonQuery();
                }

                // Log History
                string historyQuery = @"INSERT INTO TicketStatusHistory (TicketId, OldStatus, NewStatus, ChangedByUserId, ChangedAt)
                                        VALUES (@TicketId, @OldStatus, @NewStatus, @ChangedByUserId, NOW())";
                using (var cmdHist = new MySqlCommand(historyQuery, conn))
                {
                    cmdHist.Parameters.AddWithValue("@TicketId", ticketId);
                    cmdHist.Parameters.AddWithValue("@OldStatus", oldStatus);
                    cmdHist.Parameters.AddWithValue("@NewStatus", newStatus);
                    cmdHist.Parameters.AddWithValue("@ChangedByUserId", changedByUserId);
                    cmdHist.ExecuteNonQuery();
                }
            }
        }

        public void AddComment(int ticketId, string comment, int userId)
        {
            using (var conn = _dbHelper.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO TicketComments (TicketId, Comment, CommentedByUserId, CommentedAt)
                                 VALUES (@TicketId, @Comment, @UserId, NOW())";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TicketId", ticketId);
                    cmd.Parameters.AddWithValue("@Comment", comment);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<TicketHistoryItem> GetTicketHistory(int ticketId)
        {
            var history = new List<TicketHistoryItem>();
            using (var conn = _dbHelper.GetConnection())
            {
                conn.Open();
                
                // Get Status Changes
                string statusQuery = @"SELECT h.Id, h.OldStatus, h.NewStatus, h.ChangedAt, u.Username 
                                       FROM TicketStatusHistory h
                                       JOIN Users u ON h.ChangedByUserId = u.Id
                                       WHERE h.TicketId = @TicketId";
                using (var cmd = new MySqlCommand(statusQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@TicketId", ticketId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            history.Add(new TicketHistoryItem
                            {
                                Id = reader.GetInt32("Id"),
                                Type = "StatusChange",
                                Content = $"Status changed from {reader["OldStatus"]} to {reader["NewStatus"]}",
                                ChangedBy = reader.GetString("Username"),
                                ChangedAt = reader.GetDateTime("ChangedAt")
                            });
                        }
                    }
                }

                // Get Comments
                string commentQuery = @"SELECT c.Id, c.Comment, c.CommentedAt, u.Username
                                        FROM TicketComments c
                                        JOIN Users u ON c.CommentedByUserId = u.Id
                                        WHERE c.TicketId = @TicketId";
                using (var cmd = new MySqlCommand(commentQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@TicketId", ticketId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            history.Add(new TicketHistoryItem
                            {
                                Id = reader.GetInt32("Id"),
                                Type = "Comment",
                                Content = reader.GetString("Comment"),
                                ChangedBy = reader.GetString("Username"),
                                ChangedAt = reader.GetDateTime("CommentedAt")
                            });
                        }
                    }
                }
            }
            
            // Sort by Date Descending
            history.Sort((x, y) => y.ChangedAt.CompareTo(x.ChangedAt));
            return history;
        }
    }
}
