using System.Linq;
using System.Timers;
using TShockAPI;

namespace TimeBasedRanks
{
    public class TbrTimers
    {
        private Timer aTimer;
        private Timer uTimer;
        private Timer bTimer;

        public TbrTimers()
        {
            bTimer = new Timer(TBR.config.SavePlayerStatsInterval * 1000);
            uTimer = new Timer(TBR.config.IncrementTimeInterval * 1000);
            aTimer = new Timer(TBR.config.CheckAfkStatusInterval * 1000);
        }

        public void Start()
        {
            aTimer.Enabled = true;
            aTimer.Elapsed += afkTimer;

            uTimer.Enabled = true;
            uTimer.Elapsed += updateTimer;

            bTimer.Enabled = true;
            bTimer.Elapsed += backupTimer;
        }

        private void afkTimer(object sender, ElapsedEventArgs args)
        {
            /* TODO Add complex afk timer so IcyPhoenix's dumbass player's can't bypass it in some unheard of weird way */
        }

        private void updateTimer(object sender, ElapsedEventArgs args)
        {
            foreach (var player in TBR.Tools.Players.Where(player => player.online))
            {
                player.time += TBR.config.IncrementTimeInterval;

                if (player.index == -1) 
                    continue;

                player.points += (int)(TBR.config.IncrementTimeInterval / TBR.config.PointDivisor);

                if (player.points < TBR.config.Groups[player.group].rankCost) 
                    continue;

                if (player.getNextGroupName == player.group)
                    continue;

                player.points = 0;

                TShock.Users.SetUserGroup(
                    TShock.Users.GetUserByName(player.name), player.getNextGroupName);

                TShock.Players[player.index].SendWarningMessage("You have ranked up!");
                TShock.Players[player.index].SendWarningMessage("Your current rank position: "
                                                                + player.getGroupPosition + " (" + player.@group + ")");
                TShock.Players[player.index].SendWarningMessage("Your next rank: " + player.getNextGroupName);
                TShock.Players[player.index].SendWarningMessage("Next rank in: " + player.getNextRankTime);
            }
        }

        private void backupTimer(object sender, ElapsedEventArgs args)
        {
            TBR.dbManager.saveAllPlayers();
        }
    }
}
