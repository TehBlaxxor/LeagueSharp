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
    class LaneClear
    {
        public static void Run()
        {
            if (ObjectManager.Player.HealthPercentage() <= Menu.GetSlider("modHP"))
            {
                return;
            }

            foreach (var minion in MinionManager.GetMinions(610f, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth))
            {
                if (Spells.Q.GetDamage(minion) > minion.Health + 15 && Menu.GetBool("laneClearQ") && Spells.Q.IsReady())
                {
                    Spells.Q.Cast(minion);
                }
            }

            if (Methods.GetMinionCountInRange(Spells.W.Range) >= Menu.GetSlider("modW2") && Spells.W.IsReady() && Menu.GetBool("laneClearW"))
            {
                Spells.W.Cast();
            }
            if (Methods.GetMinionCountInRange(Spells.E.Range) >= Menu.GetSlider("modE2") && Spells.E.IsReady() && Menu.GetBool("laneClearE"))
            {
                Spells.E.Cast();
            }
        }
    }
}
