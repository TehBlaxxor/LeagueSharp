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
    class Spells
    {
        public static Spell Q, W, E, R;

        public static List<Spell> SpellList = new List<Spell>();

        public static SpellSlot summonerdot;

        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 150);
            E = new Spell(SpellSlot.E, 610);
            R = new Spell(SpellSlot.R, 700);

            R.SetSkillshot(0.3f, 170, 700, false, SkillshotType.SkillshotCircle);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            summonerdot = ObjectManager.Player.GetSpellSlot("summonerdot");

            Console.WriteLine("Spells intialized successfully!");
        }

        public static float GetIgniteDamage(Obj_AI_Hero target)
        {
            if (summonerdot == SpellSlot.Unknown || ObjectManager.Player.Spellbook.CanUseSpell(summonerdot) != SpellState.Ready)
            {  return 0f; }
            return (float)ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

    }
}
