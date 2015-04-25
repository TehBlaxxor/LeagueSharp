using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace The_Masterpiece
{
    class Program
    {
        internal static GlobalEnums.RunningMode Permission = GlobalEnums.RunningMode.USER;
        private static Obj_AI_Hero p = ObjectManager.Player;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            GlobalMethods.LoadChampion();

            Game.OnInput += ChatCommands.Game_OnInput;
        }
    }
}
