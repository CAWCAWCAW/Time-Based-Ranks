using System.Collections.Generic;
using System.Linq;
using TShockAPI;

namespace TimeBasedRanks
{
    public class Tools
    {
        public List<TrPlayer> Players = new List<TrPlayer>();

        /// <summary>
        ///     Creates TShock groups from groups defined in the configuration file, if they do not exist
        /// </summary>
        public static void CreateGroups()
        {
            var addedGroups = new List<string>();
            if (TBR.config.Groups.Count > 0)
                foreach (string group in
                    TBR.config.Groups.Keys.Where(group => !TShock.Groups.GroupExists(group)))
                {
                    TShock.Groups.AddGroup(group, string.Empty);
                    addedGroups.Add(group);
                }

            if (addedGroups.Count > 0)
                Log.ConsoleInfo("[TimeRanks]: Auto-created {0} groups as defined in config: {1}",
                    addedGroups.Count, string.Join(", ", addedGroups));
        }

        /// <summary>
        ///     Uses an exact name match to return a TRPlayer
        /// </summary>
        /// <param name="name">Name to search</param>
        /// <returns>null or TRPlayer</returns>
        public TrPlayer GetPlayerByName(string name)
        {
            return Players.FirstOrDefault(player => player.name == name) != null
                ? Players.FirstOrDefault(player => player.name == name)
                : null;
        }

        /// <summary>
        ///     Returns a list of players by loose name matching
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<TrPlayer> GetPlayerListByName(string name)
        {
            name = name.ToLower();
            var retList = new List<TrPlayer>();

            foreach (TrPlayer player in Players)
            {
                if (player.name.ToLower() == name)
                    return new List<TrPlayer> {player};
                if (player.name.ToLower().Contains(name))
                    retList.Add(player);
            }

            return retList;
        }
    }
}