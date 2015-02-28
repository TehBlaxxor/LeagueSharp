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
    class Harass
    {
        public static void Run()
        {
            if (ObjectManager.Player.HealthPercentage() <= Menu.GetSlider("modHP"))
            {
                return;
            }
            var target = TargetSelector.GetTarget(Settings.TARGETSELECTOR_RANGE, Settings.DAMAGE_TYPE);

            if (Menu.GetBool("harassQ") && Spells.Q.IsReady() && target.IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.Cast(target);
            }
            if (Menu.GetBool("harassE") && Spells.E.IsReady() && target.IsValidTarget(Spells.E.Range))
            {
                Spells.E.Cast();
            }

        }
    }
}
