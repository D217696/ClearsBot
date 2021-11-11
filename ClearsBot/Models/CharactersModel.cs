using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ClearsBot.Models
{
    public class CharactersModel
    {
        readonly string ConnectionString = "Server=Localhost;Database=ClearsBot;Uid=root;Pwd=;";
        public List<DbCharacter> GetAllCharacters()
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
            {
                return connection.Query<DbCharacter>("SELECT * FROM characters").ToList();
            }
        }

        public DbCharacter GetCharacterById(long characterId)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
            {
                return connection.Query<DbCharacter>("SELECT * FROM characters WHERE character_id = @character_id", new { Character_id = characterId }).FirstOrDefault();
            }
        }

        public List<DbCharacter> GetCharactersByMembershipId(long membershipId)
        {
            using (IDbConnection connection = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
            {
                return connection.Query<DbCharacter>("SELECT * FROM characters WHERE membership_id = @membership_id", new { Membership_id = membershipId }).ToList();
            }
        }
    }
}
