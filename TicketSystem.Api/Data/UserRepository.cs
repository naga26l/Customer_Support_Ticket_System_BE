using System.Data;
using MySql.Data.MySqlClient;
using TicketSystem.Api.Models;

namespace TicketSystem.Api.Data
{
    public class UserRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public UserRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public User GetUserByUsername(string username)
        {
            using (var conn = _dbHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id, Username, PasswordHash, Role FROM Users WHERE Username = @Username";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32("Id"),
                                Username = reader.GetString("Username"),
                                PasswordHash = reader.GetString("PasswordHash"),
                                Role = reader.GetString("Role")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public User CreateUser(string username, string password, string role)
        {
             using (var conn = _dbHelper.GetConnection())
            {
                conn.Open();
                // Check if exists -- simple check, can be improved
                 string checkQuery = "SELECT Count(*) FROM Users WHERE Username = @Username";
                 using (var checkCmd = new MySqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Username", username);
                    long count = (long)checkCmd.ExecuteScalar();
                    if(count > 0) return null;
                }

                string query = "INSERT INTO Users (Username, PasswordHash, Role) VALUES (@Username, @Password, @Role); SELECT LAST_INSERT_ID();";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Role", role);
                    
                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                    return new User { Id = id, Username = username, Role = role };
                }
            }
        }

        public System.Collections.Generic.List<User> GetAdmins()
        {
            var admins = new System.Collections.Generic.List<User>();
            using (var conn = _dbHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT Id, Username, Role FROM Users WHERE Role = 'Admin'";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            admins.Add(new User
                            {
                                Id = reader.GetInt32("Id"),
                                Username = reader.GetString("Username"),
                                Role = reader.GetString("Role")
                            });
                        }
                    }
                }
            }
            return admins;
        }
    }
}
