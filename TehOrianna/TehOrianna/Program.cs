using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace TehOrianna
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            try
            {
                if (ObjectManager.Player.BaseSkinName.ToLowerInvariant() == "orianna")
                    new Orianna();
                else
                    NotificationManager.ShowNotification("TehOrianna failed to load!", NotificationManager.NotificationColor);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
                NotificationManager.ShowNotification("EXC@" + e, color.Red);
            }
        }
    }
}
