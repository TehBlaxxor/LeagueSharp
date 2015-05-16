using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace The_Masterpiece
{
    public static class GlobalFunctions
    {
        //These are rly easy to use and makes code much better! like for some a lot used calculations or smth!

        public static float GetDamageTo(this Obj_AI_Hero from, Obj_AI_Base to)
        {
            return (float)from.GetComboDamage(to, new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R});
        }
    }
}
