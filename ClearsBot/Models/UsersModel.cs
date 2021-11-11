using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace ClearsBot.Models
{
    public class UsersModel
    {
        readonly string ConnectionString = "Server=Localhost;Database=ClearsBot;Uid=root;Pwd=;";
        public List<DbUser> GetAllUsers()
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
            {
                return connection.Query<DbUser>("SELECT * FROM users").ToList();
            }
        }

        public DbUser GetUserById(int id)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
            {
                return connection.Query<DbUser>("SELECT * FROM users WHERE user_id = @id;", new { Id = id }).FirstOrDefault();
            }
        }
    }
}
