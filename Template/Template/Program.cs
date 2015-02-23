using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Template
{
    class Program
    {
        private static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Hero Player = ObjectManager.Player;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (!Methods.CheckChampion())
            { return; }

            if (Settings.SHOW_NOTIFICATION)
            {
                Methods.ShowNotification(Settings.ASSEMBLY_NAME + Methods.SPACE + Settings.VERSION + " by " + Settings.AUTHOR_NAME + " loaded!");
            }

            Drawings.Initialize();
            Menu.Initialize();
            Spells.Initialize();
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo.Run();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear.Run();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit.Run();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass.Run();
                    break;
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
