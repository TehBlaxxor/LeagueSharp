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
    class KillSteal
    {
        public static void TryToKS()
        {
            if (!Menu.GetBool("ks"))
            {
                return;
            }


            foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(tg => tg.IsValidTarget(610f)))
            {
                if (Spells.Q.GetDamage(target) > target.Health + target.HPRegenRate && Spells.Q.IsReady() && target.IsValidTarget(Spells.Q.Range) && !target.HasBuff("UndyingRage") && Menu.GetBool("ksQ"))
                {
                    Spells.Q.Cast(target);
                }
                else if (Spells.E.GetDamage(target) > target.Health + target.HPRegenRate && Spells.E.IsReady() && target.IsValidTarget(Spells.E.Range) && !target.HasBuff("UndyingRage") && Menu.GetBool("ksE"))
                {
                    Spells.E.Cast();
                }
                else if (Spells.GetIgniteDamage(target) > target.Health + target.HPRegenRate && target.IsValidTarget(600f) && !target.HasBuff("UndyingRage") && Menu.GetBool("ksIgnite"))
                {
                    ObjectManager.Player.Spellbook.CastSpell(Spells.summonerdot, target);
                }
            }
        
        }
    }
}
