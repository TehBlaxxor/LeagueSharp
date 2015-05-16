using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;

namespace Jinx
{
    public static class Spells
    {
        public static Spell Q, W, E, R;
        public static List<Spell> SpellList = new List<Spell>();
        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static float QLastCast;
        public static float WLastCast;
        public static float ELastCast;
        public static float RLastCast;

        public static float QMana;
        public static float WMana;
        public static float EMana;
        public static float RMana;


        public static void Initiate()
        {
            Q = new Spell(SpellSlot.Q); W = new Spell(SpellSlot.W, 1500f); E = new Spell(SpellSlot.E, 900f); R = new Spell(SpellSlot.R, 7000f);
            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine); E.SetSkillshot(0.7f, 120f, 1750f, false, SkillshotType.SkillshotCircle); R.SetSkillshot(0.6f, 140f, 1700f, false, SkillshotType.SkillshotLine);
            SpellList.Add(Q); SpellList.Add(W); SpellList.Add(E); SpellList.Add(R);

            Messages.OnSpellInitializationSequence(1);

            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Game.OnUpdate += Game_OnUpdate;

            Messages.OnSpellInitializationSequence(2);
         }

        static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            { QMana = 0; WMana = 0; EMana = 0; RMana = 0; }
            else
            {
                QMana = 10;
                WMana = W.Instance.ManaCost;
                EMana = E.Instance.ManaCost;
                RMana = R.Instance.ManaCost;
            }
        }

        public static bool IsMoving(this Obj_AI_Base unit)
        {
            return unit.Path.Count() >= 2;
        }

        static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!sender.Owner.IsMe)
                return;

            var slot = args.Slot;

            if (slot == SpellSlot.Q)
                QLastCast = Game.Time;
            if (slot == SpellSlot.W)
                WLastCast = Game.Time;
            if (slot == SpellSlot.E)
                ELastCast = Game.Time;
            if (slot == SpellSlot.R)
                RLastCast = Game.Time;

        }

        public static PredictionOutput PredOut(this Spell qwer, Obj_AI_Base unit)
        {
            if (qwer == E || qwer.Slot == SpellSlot.E)
            {
                return Prediction.GetPrediction(new PredictionInput 
                { Unit = unit,
                    Delay = 0.71f,
                    Radius = 120f,
                    Speed = 1750f,
                    Range = 900f,
                    Type = SkillshotType.SkillshotLine });
            }
            return qwer.GetPrediction(unit);
        }

    }
}
