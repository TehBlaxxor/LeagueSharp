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
        
        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        public static void Game_OnGameLoad(EventArgs args)
        {
            if (!Methods.CheckChampion())
            { return; }

            if (Settings.SHOW_NOTIFICATION)
            {
                Methods.ShowNotification(Settings.ASSEMBLY_NAME + Methods.SPACE + Settings.VERSION + " by " + Settings.AUTHOR_NAME + " loaded!");
            }

            Menu.Initialize();
            Spells.Initialize();
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            switch (Menu.Orbwalker.ActiveMode)
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

            if (Menu.root.Item("harassToggler").GetValue<KeyBind>().Active)
            {
                Harass.Run();
            }

            KillSteal.TryToKS();
            TryToSurvive();
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            Drawings.Initialize();
        }

        public static void TryToSurvive()
        {
            if (!Menu.GetBool("surviveW"))
            {
                return;
            }
            var target2 = TargetSelector.GetTarget(700f, TargetSelector.DamageType.Magical);

            if (Player.HealthPercentage() <= Menu.GetSlider("surviveWHP") && Spells.W.IsReady() && target2.IsValidTarget(700f))
            {
                Spells.W.Cast();
            }
        }
    }
}
