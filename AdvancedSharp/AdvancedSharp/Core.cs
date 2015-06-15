using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AdvancedSharp
{
    class Core
    {
        public static void LoadChampion(String Name)
        {
            switch (Name.ToLowerInvariant())
            {
                case "cassiopeia":
                    new Instance.Cassiopeia();
                    break;
                default:
                    new Instance.Orbwalker();
                    break;
            }
        }

        public static void LoadWelcomeMessage(bool orbwalker = false)
        {
            if (orbwalker)
                Notifications.AddNotification("Adv#: Orbwalker", 500);
            else Notifications.AddNotification("Adv#: " + ObjectManager.Player.ChampionName, 500);
        }
    }
}
