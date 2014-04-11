using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace TimeBasedRanks
{
    public class rankInfo
    {
        public readonly int rankCost;
        public readonly string nextGroup;

        public rankInfo(string nextGroup, int rankCost)
        {
            this.nextGroup = nextGroup;
            this.rankCost = rankCost;
        }
    }

    public class TRConfig
    {
        public int IncrementTimeInterval = 1;
        public float PointDivisor = 1;
        public int CheckAfkStatusInterval = 1;
        public int SavePlayerStatsInterval = 600;
        public bool CreateNonExistantGroups = false;
        public string StartGroup = "default";
        public bool AutoStartUsers = true;
        public readonly Dictionary<string, rankInfo> Groups = new Dictionary<string, rankInfo>();


        public void Write(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static TRConfig Read(string path)
        {
            if (!File.Exists(path))
                return new TRConfig();
            return JsonConvert.DeserializeObject<TRConfig>(File.ReadAllText(path));
        }
    }
}
