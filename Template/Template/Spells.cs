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
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        public static List<Spell> SpellList = new List<Spell>();

        public static void Initialize()
        {
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 0);
            R = new Spell(SpellSlot.R, 0);

            Q.SetSkillshot(250f, 70, 1800, true, SkillshotType.SkillshotLine);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Console.WriteLine("Spells intialized successfully!");
        }

    }
}
