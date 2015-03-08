using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace TehKatarina
{
    class IgniteHandler
    {
        public static float IgniteRange = 600f;

        public static void Run()
        {
            Program.IgniteSlot = Program.Player.GetSpellSlot("summonerdot");
        }

        public static float GetIgniteDamage(Obj_AI_Hero hero)
        {
            if (Program.IgniteSlot != SpellSlot.Unknown)
            {
                return (float)Program.Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            else
            {
                return 0f;
            }

        }
    }
}
