using ClearsBot.Objects;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClearsBot.Objects.Database;

namespace ClearsBot.Modules
{
    public class Database
    {
        private readonly MySqlConnection mySqlConnection;
        public Database()
        {
            mySqlConnection = new MySqlConnection("datasource=localhost;port=3306;username=root;password=;database=clearsbot;");
        }

        public async void HandlePgcrs(IEnumerable<GetPostGameCarnageReport> reports)
        {
            mySqlConnection.Open();

            List<Task> insertPgcrTasks = new List<Task>();
            List<Task> insertEntriesTasks = new List<Task>();

            foreach (var report in reports)
            {
                insertPgcrTasks.Add(Task.Run(() => InsertPgcrsIntoDb(report)));
                insertEntriesTasks.Add(Task.Run(() => InsertEntriesIntoUserPgcr(report.Response.Entries, report.Response.ActivityDetails.InstanceId)));
            }

            await Task.WhenAll(insertPgcrTasks);
            await Task.WhenAll(insertEntriesTasks);

            mySqlConnection.Close();
        }

        public void InsertPgcrsIntoDb(GetPostGameCarnageReport report)
        {
            string sql = "INSERT INTO pgcrs (instance_id, raid_hash, time, datetime, starting_phase_index, kills, player_count, json) VALUES (@instance_id, @raid_hash, @time, @datetime, @starting_phase_index, @kills, @player_count, @json);";
            MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);

            try
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@instance_id", MySqlDbType.Int64);
                cmd.Parameters.Add("@raid_hash", MySqlDbType.UInt64);
                cmd.Parameters.Add("@time", MySqlDbType.Time);
                cmd.Parameters.Add("@datetime", MySqlDbType.DateTime);
                cmd.Parameters.Add("@starting_phase_index", MySqlDbType.Int16);
                cmd.Parameters.Add("@kills", MySqlDbType.Int16);
                cmd.Parameters.Add("@player_count", MySqlDbType.Int16);
                cmd.Parameters.Add("@json", MySqlDbType.JSON);

                cmd.Parameters["@instance_id"].Value = report.Response.ActivityDetails.InstanceId;
                cmd.Parameters["@raid_hash"].Value = report.Response.ActivityDetails.ReferenceId;
                cmd.Parameters["@time"].Value = TimeSpan.FromSeconds(report.Response.Entries.First().Values["activityDurationSeconds"].Basic.Value);
                cmd.Parameters["@datetime"].Value = report.Response.Period;
                cmd.Parameters["@starting_phase_index"].Value = report.Response.StartingPhaseIndex;
                cmd.Parameters["@kills"].Value = report.Response.Entries.Sum(x => x.Values["kills"].Basic.Value);
                cmd.Parameters["@player_count"].Value = report.Response.Entries.Where(x => x.Values["completed"].Basic.Value == 1 && x.Values["completionReason"].Basic.DisplayValue == "Objective Completed" && x.Values["kills"].Basic.Value > 0 && x.Values["assists"].Basic.Value > 0).Count();
                cmd.Parameters["@json"].Value = JsonConvert.SerializeObject(report);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void InsertEntriesIntoUserPgcr(IEnumerable<DestinyPostGameCarnageReportEntry> entries, long instanceId)
        {
            string sql = "INSERT INTO users_pgcrs (instance_id, membership_id, character_id) VALUES (@instance_id, @membership_id, @character_id);";
            MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);

            foreach (var entry in entries)
            {
                try
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@instance_id", MySqlDbType.Int64);
                    cmd.Parameters.Add("@membership_id", MySqlDbType.Int64);
                    cmd.Parameters.Add("@character_id", MySqlDbType.Int64);

                    cmd.Parameters["@instance_id"].Value = instanceId;
                    cmd.Parameters["@membership_id"].Value = entry.Player.DestinyUserInfo.MembershipId;
                    cmd.Parameters["@character_id"].Value = entry.CharacterId;

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public IEnumerable<DatabasePostGameCarnageReport> GetPostGameCarnageReportsByMembershipId(long membershipId)
        {
            string sql = "SELECT * FROM pgcrs p INNER JOIN users_pgcrs up ON up.instance_id = p.instance_id WHERE membership_id = @membership_id;";
            MySqlCommand cmd = new MySqlCommand(sql, mySqlConnection);

            List<DatabasePostGameCarnageReport> enums = new List<DatabasePostGameCarnageReport>();
            try
            {
                cmd.Parameters.AddWithValue("@membership_id", membershipId);
                mySqlConnection.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        enums.Add(new DatabasePostGameCarnageReport()
                        {
                            InstanceId = reader.GetInt64(0),
                            RaidHash = reader.GetUInt64(1),
                            Time = reader.GetTimeSpan(2),
                            Period = reader.GetDateTime(3),
                            StartingPhaseIndex = reader.GetInt32(4),
                            Kills = reader.GetDouble(5),
                            PlayerCount = reader.GetInt32(6),
                            GetPostGameCarnageReport = JsonConvert.DeserializeObject<GetPostGameCarnageReport>(reader.GetString(7))
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            mySqlConnection.Open();

            return enums;
        }
    }
}
