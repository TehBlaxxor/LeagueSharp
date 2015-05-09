using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace The_Masterpiece.ChampionAssets.BaseChampionInstance
{
    public static class Spells
    {
        public static Spell Q, W, E, R;
        public static List<Spell> SpellList = new List<Spell>();
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static void Load()
        {
            Q = new Spell(SpellSlot.Q, 0);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 0);
            R = new Spell(SpellSlot.R, 0);

            Game.OnUpdate += Game_OnUpdate;

            SpellList.Add(Q); SpellList.Add(W); SpellList.Add(E); SpellList.Add(R);

            
        }

        public static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsChannelingImportantSpell() || Player.IsCastingInterruptableSpell())
            {
                
            }
        }

        public enum TargetType
        {
            LowestHealth,
            MaxHealth,
            BestDamage,
            MostHealth,
            Tankiest,
            Closest
        }
        public static Obj_AI_Hero GetBestTarget(TargetType type, float range)
        {
            var List = HeroManager.Enemies.Where(x => !x.IsDead && x.Distance(Player.Position) <= range);
            if (type == TargetType.LowestHealth)
            {
                return List.OrderBy(x => x.Health).FirstOrDefault();
            }
            if (type == TargetType.MaxHealth)
            {
                return List.OrderBy(x => x.MaxHealth).FirstOrDefault();
            }
            if (type == TargetType.BestDamage)
            {
                return List.OrderBy(x => x.TotalMagicalDamage + x.TotalAttackDamage ).LastOrDefault();
            }
            if (type == TargetType.Closest)
            {
                return List.OrderBy(x => x.Distance(Player.Position)).FirstOrDefault();
            }
            if (type == TargetType.Tankiest)
            {
                return List.OrderBy(x => x.Armor + x.FlatMagicReduction).FirstOrDefault();
            }
            if (type == TargetType.MostHealth)
            {
                return List.OrderBy(x => x.Health).LastOrDefault();
            }
            else return TargetSelector.GetTarget(range, TargetSelector.DamageType.Physical);
        }

        public static float GetMaxRange(this Obj_AI_Hero player)
        {
            if (player != ObjectManager.Player)
                return 0f;
            return Math.Max(Math.Max(Q.Range, W.Range), Math.Max(E.Range, R.Range));
        }
        public static bool CanBeKilled(this Obj_AI_Hero player, bool q = false, bool w = false, bool e = false, bool r = false)
        {
            if (player.IsAlly)
                return false;
            return (Q.GetDamage(player) + W.GetDamage(player) + E.GetDamage(player) + R.GetDamage(player) > player.Health + 25);
        }
        public static bool HasFullCombo(this Obj_AI_Hero player)
        {
            if (player != ObjectManager.Player)
                return false;
            return Q.IsReady() && W.IsReady() && E.IsReady() && R.IsReady();
        }
        public static float GetFullDamage(this Obj_AI_Hero me, Obj_AI_Hero tg, bool inrange = false)
        {
            if (me != ObjectManager.Player)
                return 0f;

            double damage = 0d;

            if (inrange)
            {
                if (Q.IsInRange(tg) && Q.IsReady())
                    damage += Q.GetDamage(tg);
                if (W.IsInRange(tg) && W.IsReady())
                    damage += W.GetDamage(tg);
                if (E.IsInRange(tg) && E.IsReady())
                    damage += E.GetDamage(tg);
                if (R.IsInRange(tg) && R.IsReady())
                    damage += R.GetDamage(tg);
            }
            else
            {
                if (Q.IsReady())
                    damage += Q.GetDamage(tg);
                if (W.IsReady())
                    damage += W.GetDamage(tg);
                if (E.IsReady())
                    damage += E.GetDamage(tg); 
                if (R.IsReady())
                    damage += R.GetDamage(tg);
            }

            return (float)damage;
        }
        
    }
}
