using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace The_Masterpiece.Handlers
{
    internal class SSpell
    {
        public string SummonerNames { get; set; }
        public float Range { get; set; }
        public SpellSlot SSpellSlot
        {
            get
            {
                if (ObjectManager.Player.GetSpellSlot(SummonerNames) != SpellSlot.Unknown)
                    return ObjectManager.Player.GetSpellSlot(SummonerNames);
                return SpellSlot.Unknown;
            }
        }
    }

    static class SpellHandler
    {
        #region Summoner Spells

        public static SSpell Ghost = new SSpell
        {
            SummonerNames = "SummonerHaste"
        };

        public static SSpell Clarity = new SSpell
        {
            SummonerNames = "SummonerMana",
            Range = 600f
        };

        public static SSpell Heal = new SSpell
        {
            SummonerNames = "SummonerHeal",
            Range = 850f
        };

        public static SSpell Barrier = new SSpell
        {
            SummonerNames = "SummonerBarrier"
        };

        public static SSpell Exhaust = new SSpell
        {
            SummonerNames = "SummonerExhaust",
            Range = 650f
        };

        public static SSpell Cleanse = new SSpell
        {
            SummonerNames = "SummonerBoost"
        };

        public static SSpell Flash = new SSpell
        {
            SummonerNames = "SummonerFlash",
            Range = 425f
        };

        public static SSpell Ignite = new SSpell
        {
            SummonerNames = "SummonerDot",
            Range = 600f
        };
        #endregion

        #region Application Programming Interface [API]
        public static bool IsReady(this SSpell summoner)
        {
            return (summoner.SSpellSlot != SpellSlot.Unknown 
                && ObjectManager.Player.Spellbook.GetSpell(summoner.SSpellSlot).IsReady());
        }

        public static bool Exists(this SSpell summoner)
        {
            return summoner.SSpellSlot != SpellSlot.Unknown;
        }

        public static void Cast(this SSpell summoner)
        {
            ObjectManager.Player.Spellbook.CastSpell(summoner.SSpellSlot);
        }

        public static void Cast(this SSpell summoner, Obj_AI_Hero target)
        {
            ObjectManager.Player.Spellbook.CastSpell(summoner.SSpellSlot, target);
        }

        public static void Cast(this SSpell summoner, Vector3 position)
        {
            ObjectManager.Player.Spellbook.CastSpell(summoner.SSpellSlot, position);
        }
        #endregion
    }
}
