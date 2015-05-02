using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;



namespace The_Masterpiece.Handlers
{
    //Credits to LiveToRise.
    class SpellHandler2
    {

        private static readonly List<SpellHandler2> SpellList = new List<SpellHandler2>();

        public static Obj_AI_Hero Target;

        public static Menu Spells;

        public const string ChampionName = "";

        private static Obj_AI_Hero Player;
        public static Spell Ignite = null;
        public static Spell Ghost = null;
        public static Spell Flash = null;
        public static Spell Clarity = null;
        public static Spell Heal = null;
        public static Spell Barrier = null;
        public static Spell Cleanse = null;
        public static Spell Exhaust = null;

        private static void ItemLists()
        {
            Player = ObjectManager.Player;


            Spells = new Menu("Spells" + ChampionName, ChampionName, true);

            if (Player.GetSpellSlot("SummonerDot") != SpellSlot.Unknown)
            {
                Ignite = new Spell(Player.GetSpellSlot("SummonerDot"));
                Ignite.Range = Ignite.Instance.SData.CastRange;
                Spells.SubMenu("Spells").AddItem(new MenuItem("AutoI", "Auto Ignite").SetValue(true));
                
            }
            if (Player.GetSpellSlot("SummonerHaste") != SpellSlot.Unknown)
            {
                Ghost = new Spell(Player.GetSpellSlot("SummonerHaste"));
            }
            if (Player.GetSpellSlot("SummonerMana") != SpellSlot.Unknown)
            {
                Clarity = new Spell(Player.GetSpellSlot("SummonerHaste"));
                Clarity.Range = 600f;
            }
            if (Player.GetSpellSlot("SummonerHeal") != SpellSlot.Unknown)
            {
                Heal = new Spell(Player.GetSpellSlot("SummonerHeal"));
                Heal.Range = 850f;
            }
            if (Player.GetSpellSlot("SummonerBarrier") != SpellSlot.Unknown)
            {
                Barrier = new Spell(Player.GetSpellSlot("SummonerBarrier"));
            }
            if (Player.GetSpellSlot("SummonerExhaust") != SpellSlot.Unknown)
            {
                Exhaust = new Spell(Player.GetSpellSlot("SummonerExhaust"));
                Exhaust.Range = 650f;
            }
            if (Player.GetSpellSlot("SummonerBoost") != SpellSlot.Unknown)
            {
                Cleanse = new Spell(Player.GetSpellSlot("SummonerBoost"));
            }
            if (Player.GetSpellSlot("SummonerFlash") != SpellSlot.Unknown)
            {
                Flash = new Spell(Player.GetSpellSlot("SummonerFlash"));
                Flash.Range = 425f;
            }

     //    public static bool Exists(this Spell summoner)
     //   {
    //        return summoner. != SpellSlot.Unknown;
    //    }

    //    }





    }
}








