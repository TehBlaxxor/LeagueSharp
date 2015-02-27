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
    class LastHit
    {
        public static float CountKillableMinions(Spell spell, float range)
        {
            int killable = 0;
            foreach (var minion in MinionManager.GetMinions(range))
            {
                if (spell.GetDamage(minion) > minion.Health + 15)
                {
                    killable = killable + 1;
                }
            }
            return killable;
        }

        public static void Run()
        {
            if (ObjectManager.Player.HealthPercentage() <= Menu.GetSlider("modHP"))
            {
                return;
            }

            if (CountKillableMinions(Spells.E, Spells.E.Range) >= Menu.GetSlider("modE3") && Spells.E.IsReady() && Menu.GetBool("lastHitE"))
            {
                Spells.E.Cast();
            }

            foreach (var minio in MinionManager.GetMinions(610f))
            {
                if (Spells.Q.GetDamage(minio) > minio.Health + 15 && Menu.GetBool("lastHitQ"))
                {
                    Spells.Q.Cast(minio);
                }
            }

        }
    }
}
