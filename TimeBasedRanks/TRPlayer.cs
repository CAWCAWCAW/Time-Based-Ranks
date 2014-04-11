using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;

namespace TimeBasedRanks
{
    public class TrPlayer
    {
        public int index;
        public bool online;
        public string name;
        public string firstLogin;
        public string lastLogin;
        public string group { get { return TShock.Players[index].Group.Name; } }

        public rankInfo rankInfo { get { return getRankInfo; } }

        public int time;
        public int points;

        public TrPlayer(string name, int time, string first, string last, int points)
        {
            this.time = time;
            this.name = name;
            firstLogin = first;
            lastLogin = last;
            this.points = points;
        }

        public string getTotalRegisteredTime
        {
            get
            {
                var sb = new StringBuilder();

                DateTime reg;
                DateTime.TryParse(firstLogin, out reg);

                var ts = DateTime.UtcNow - reg;

                var months = ts.Days / 30;

                if (months > 0)
                    sb.Append(string.Format("{0} month{1}~", months, appendString(months)));
                if (ts.Days > 0)
                    sb.Append(string.Format("{0} day{1}~", ts.Days, appendString(ts.Days)));
                if (ts.Hours > 0)
                    sb.Append(string.Format("{0} hour{1}~", ts.Hours, appendString(ts.Hours)));
                if (ts.Minutes > 0)
                    sb.Append(string.Format("{0} minute{1}~", ts.Minutes, appendString(ts.Minutes)));
                if (ts.Seconds > 0)
                    sb.Append(string.Format("{0} second{1}", ts.Seconds, appendString(ts.Seconds)));


                return string.Join(", ", sb.ToString().Split('~'));
            }
        }

        public string getTimePlayed
        {
            get
            {
                var sb = new StringBuilder();

                var ts = new TimeSpan(0, 0, 0, time);

                var months = ts.Days / 30;

                if (months > 0)
                    sb.Append(string.Format("{0} month{1}~", months, appendString(months)));
                if (ts.Days > 0)
                    sb.Append(string.Format("{0} day{1}~", ts.Days, appendString(ts.Days)));
                if (ts.Hours > 0)
                    sb.Append(string.Format("{0} hour{1}~", ts.Hours, appendString(ts.Hours)));
                if (ts.Minutes > 0)
                    sb.Append(string.Format("{0} minute{1}~", ts.Minutes, appendString(ts.Minutes)));
                if (ts.Seconds > 0)
                    sb.Append(string.Format("{0} second{1}", ts.Seconds, appendString(ts.Seconds)));


                return string.Join(", ", sb.ToString().Split('~'));
            }
        }

        public string getLastOnline
        {
            get
            {
                var sb = new StringBuilder();
                DateTime last;
                DateTime.TryParse(lastLogin, out last);

                var ts = DateTime.UtcNow - last;

                var months = ts.Days / 30;

                if (months > 0)
                    sb.Append(string.Format("{0} month{1}~", months, appendString(months)));
                if (ts.Days > 0)
                    sb.Append(string.Format("{0} day{1}~", ts.Days, appendString(ts.Days)));
                if (ts.Hours > 0)
                    sb.Append(string.Format("{0} hour{1}~", ts.Hours, appendString(ts.Hours)));
                if (ts.Minutes > 0)
                    sb.Append(string.Format("{0} minute{1}~", ts.Minutes, appendString(ts.Minutes)));
                if (ts.Seconds > 0)
                    sb.Append(string.Format("{0} second{1}", ts.Seconds, appendString(ts.Seconds)));

                return string.Join(", ", sb.ToString().Split('~'));
            }
        }

        public string getNextRankTime
        {
            get
            {
                if (!configContainsGroup)
                    return "group is not a part of the ranking system";

                if (getNextGroupName == "max rank achieved")
                    return "max rank achieved";


                var reqPoints = getNextRankInfo.rankCost;
                var ts = new TimeSpan(0, 0, 0, reqPoints - points);

                var sb = new StringBuilder();

                var months = ts.Days/30;

                if (months > 0)
                    sb.Append(string.Format("{0} month{1}~", months, appendString(months)));
                if (ts.Days > 0)
                    sb.Append(string.Format("{0} day{1}~", ts.Days, appendString(ts.Days)));
                if (ts.Hours > 0)
                    sb.Append(string.Format("{0} hour{1}~", ts.Hours, appendString(ts.Hours)));
                if (ts.Minutes > 0)
                    sb.Append(string.Format("{0} minute{1}~", ts.Minutes, appendString(ts.Minutes)));
                if (ts.Seconds > 0)
                    sb.Append(string.Format("{0} second{1}", ts.Seconds, appendString(ts.Seconds)));

                return string.Join(", ", sb.ToString().Split('~'));
            }
        }

        public string getGroupPosition
        {
            get
            {
                if (!configContainsGroup)
                    return "0 / 0";

                return TBR.config.Groups.Keys.ToList().IndexOf(group) + " / " + TBR.config.Groups.Keys.Count;
            }
        }

        /// <summary>
        /// Returns a string value of the next group the player will move into
        /// </summary>
        public string getNextGroupName
        {
            get
            {
                if (rankInfo.nextGroup == group)
                    return "max rank achieved";
                return !configContainsGroup ? "group is not part of the ranking system" :
                    getRankInfo.nextGroup;
            }
        }
        public rankInfo getNextRankInfo
        {
            get
            {
                return configContainsGroup
                    ? (rankInfo.nextGroup == group
                        ? new rankInfo("max rank", 0)
                        : TBR.config.Groups.First(g => g.Key == rankInfo.nextGroup).Value)
                    : new rankInfo("none", 0);
            }
        }

        public rankInfo getRankInfo
        {
            get
            {
                return configContainsGroup
                    ? (group == TBR.config.StartGroup
                        ? new rankInfo(TBR.config.Groups.Keys.ToList()[0], 0)
                        : TBR.config.Groups.First(g => g.Key == group).Value)
                    : new rankInfo("none", 0);
            }
        }

        /// <summary>
        /// Appends a suffix to a string if the number is > 1 or 0
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private string appendString(int number)
        {
            return number > 1 || number == 0 ? "s" : "";
        }

        /// <summary>
        /// Returns a boolean value indicating whether the config file contains the player's group
        /// </summary>
        private bool configContainsGroup
        {
            get
            {
                return TBR.config.Groups.Keys.Contains(group);
            }
        }
    }
}
