using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace TehMalzahar
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName == "Malzahar")
            {
                new Malzahar();
                Game.PrintChat("");
            }
            else Game.PrintChat("Failed to load Malzahar for the selected champion: " + ObjectManager.Player.ChampionName);
        }
    }
}
