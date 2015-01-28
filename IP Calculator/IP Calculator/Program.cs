using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace IP_Calculator
{
    class Program
    {
        public static Menu Config;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            Game.OnGameUpdate += OnUpdate;
            
        }

        public static void Game_OnGameLoad(EventArgs args)
        {
            Config = new Menu("IP Calculator", "IP Calculator", true);
            Config.AddItem(new MenuItem("IPwin", "IP rewarded for win is "));
            Config.AddItem(new MenuItem("IPloss", "IP rewarded for loss is "));
            Config.AddItem(new MenuItem("test", "IP rewarded for loss is "));
            Config.AddToMainMenu();
            
        }

        public static void OnUpdate(EventArgs args)
        {
            double mins = Game.Time / 60 - 1;
            double winip = 18 + 2.312 * mins;
            double loseip = 16 + 1.405 * mins;
            Config.Item("IPwin").DisplayName = "IP rewarded for win is " + winip;
            Config.Item("IPloss").DisplayName = "IP rewarded for loss is " + loseip;
            Config.Item("test").DisplayName = "" + mins;
        }
    
    }
}
