using System;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

using TShockAPI;
using TShockAPI.DB;

namespace TimeBasedRanks
{
    public class Database
    {
        private readonly IDbConnection _db;

        public Database(IDbConnection db)
        {
            _db = db;

            var SQLCreator = new SqlTableCreator(db,
                                             db.GetSqlType() == SqlType.Sqlite
                                             ? (IQueryBuilder)new SqliteQueryCreator()
                                             : new MysqlQueryCreator());

            var table = new SqlTable("TimeBasedRanking",
                new SqlColumn("ID", MySqlDbType.Int32) { Primary = true, AutoIncrement = true },
                new SqlColumn("Name", MySqlDbType.VarChar, 50) { Unique = true },
                new SqlColumn("Time", MySqlDbType.Int32),
                new SqlColumn("FirstLogin", MySqlDbType.Text),
                new SqlColumn("LastLogin", MySqlDbType.Text),
                new SqlColumn("Experience", MySqlDbType.Int32)
                );
            SQLCreator.EnsureExists(table);
        }

        /// <summary>
        /// Inserts a player into the database. Is only called for players that do not exist already.
        /// </summary>
        /// <param name="player">Player to insert</param>
        public bool insertPlayer(TrPlayer player)
        {
            return _db.Query("INSERT INTO TimeBasedRanking (Name, Time, FirstLogin, LastLogin, Experience)"
                + " VALUES (@0, @1, @2, @3, @4)", player.name, player.time, player.firstLogin, 
                player.lastLogin, player.points) != 0;
        }

        /// <summary>
        /// Removes a player from the database using a name search
        /// </summary>
        /// <param name="player">player to remove</param>
        /// <returns></returns>
        public bool deletePlayer(string player)
        {
            return _db.Query("DELETE FROM TimeBasedRanking WHERE Name = @0", player) != 0;
        }

        /// <summary>
        /// Updates a player's saved statistics
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool savePlayer(TrPlayer player)
        {
            player.lastLogin = DateTime.UtcNow.ToString("G");
            return _db.Query("UPDATE TimeBasedRanking SET Time = @0, LastLogin = @1," + 
                " Experience = @2 WHERE Name = @3",
                player.time, player.lastLogin, player.points, player.name) != 0;
        }

        public void saveAllPlayers()
        {
            foreach (var player in TBR.Tools.Players.Where(player => player.index != -1 && 
                TShock.Players[player.index] != null && TShock.Players[player.index].IsLoggedIn))
                savePlayer(player);
        }

        /// <summary>
        /// Syncs all player's stats into the server on initialization
        /// </summary>
        private void initialSyncPlayers()
        {
            using (var reader = _db.QueryReader("SELECT * FROM TimeBasedRanking"))
            {
                while (reader.Read())
                {
                    var name = reader.Get<string>("Name");
                    var time = reader.Get<int>("Time");
                    var firstLogin = reader.Get<string>("FirstLogin");
                    var lastLogin = reader.Get<string>("LastLogin");
                    var points = reader.Get<int>("Experience");
                    TBR.Tools.Players.Add(new TrPlayer(name, time, firstLogin, lastLogin, points));
                }
            }
        }

        /// <summary>
        /// Creates the database table and columns if they don't exist. Calls initialiSyncPlayers()
        /// </summary>
        public void Initialize()
        {
            initialSyncPlayers();
        }
    }
}
