using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Template
{
    class Methods
    {

        public static string SPACE = " ";

        public static int GetMinionCountInRange(float range)
        {
            var minions = MinionManager.GetMinions(range);
            return minions.Count;
        }

        public static bool CheckChampion()
        {
            if (Program.Player.BaseSkinName == Settings.CHAMPION_NAME)
            { return true; }
            else
            { return false; }
        }

        public static Notification ShowCustomNotification(string message, Color color, int duration = -1, bool dispose = true)
        {
            var notif = new Notification(message).SetTextColor(color);
            Notifications.AddNotification(notif);

            if (dispose)
            {
                Utility.DelayAction.Add(duration, () => notif.Dispose());
            }

            return notif;
        }

        public static Notification ShowNotification(string message)
        {
            var notif = new Notification(message).SetTextColor(Settings.NOTIFICATION_COLOR);
            Notifications.AddNotification(notif);

            if (Settings.NOTIFICATION_DISPOSE)
            {
                Utility.DelayAction.Add(Settings.NOTIFICATION_DURATION, () => notif.Dispose());
            }

            return notif;
        }
    }
}
