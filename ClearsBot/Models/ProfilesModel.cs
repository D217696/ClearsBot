using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using System.Linq;

namespace ClearsBot.Models
{
    public class ProfilesModel
    {
        readonly string ConnectionString = "Server=Localhost;Database=ClearsBot;Uid=root;Pwd=;";
        public List<DbProfile> GetAllProfiles()
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
            {
                return connection.Query<DbProfile>("SELECT * FROM profiles").ToList();
            }
        }

        public DbProfile GetProfileByMembershipId(long membershipId)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString)) 
            {
                return connection.Query<DbProfile>("SELECT * FROM profiles WHERE membership_id = @membership_id", new { MembershipId = membershipId }).FirstOrDefault();
            }
        }

        public List<DbProfile> GetProfilesByUserId(int userId)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
            {
                return connection.Query<DbProfile>("SELECT p.* FROM profiles AS p INNER JOIN users AS u ON p.user_id = u.user_id WHERE u.user_id = @user_id", new { User_id = userId }).ToList();
            }
        }

        public List<DbProfile> GetProfilesByDiscordId(long discordId)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
            {
                return connection.Query<DbProfile>("SELECT p.* FROM profiles AS p INNER JOIN users AS u ON p.user_id = u.user_id WHERE u.discord_id = @discord_id", new { Discord_id = discordId }).ToList();
            }
        }
    }
}
