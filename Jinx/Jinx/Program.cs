using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Jinx
{
    public class Program
    {
        public static Spell Q = Spells.Q, W = Spells.W, E = Spells.E, R = Spells.R;
        public static Menu Z = MenuUtils.Z;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static SpellSlot Ignite { get { return Player.GetSpellSlot("SummonerDot"); } }

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Jinx")
            {
                Messages.OnWrongChampion();
                return;
            }
            Utils.ClearConsole();

            MenuUtils.Create();
            Spells.Initiate();

            Utility.HpBarDamageIndicator.Enabled = false;


            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

            Messages.OnLoad();
        }

        

        public enum Modes
        {
            Combo,
            LaneClear,
            Harass,
            None
        }

        static void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsValidTarget(E.Range) 
                && args.DangerLevel == Interrupter2.DangerLevel.High 
                && E.IsReady()
                && E.GetPrediction(sender).Hitchance == HitChance.High)
                E.Cast(sender.Position);
        }

        public static bool FishbonesActive()
        {
            return Player.AttackRange > 565f;
        }

        public static float CalcTotalRange()
        {
            return 565f + Player.Spellbook.GetSpell(SpellSlot.Q).Level * 25 + 50;
        }

        static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            //Fishbones && Pow-Pow Switch
            var tg = (Obj_AI_Base)args.Target;
            if (tg.IsValidTarget())
            {
                if (GetMode() == Modes.LaneClear)
                {
                    var closest = MinionManager.GetMinions(CalcTotalRange()).OrderBy(x => x.Distance(Player.Position)).FirstOrDefault();
                    if (MinionManager.GetMinions(tg.Position, 150f).Count >= Z.GetValue<Slider>("rocket.lane").Value
                        && !FishbonesActive()
                        && Z.GetValue<bool>("spells.q")
                        && Z.GetValue<bool>("rocket.lane.2"))
                    {
                        Q.Cast();
                    }
                    else if (MinionManager.GetMinions(tg.Position, 150f).Count < Z.GetValue<Slider>("rocket.lane").Value
                        && FishbonesActive()
                        && tg.IsValidTarget(565f)
                        && Z.GetValue<bool>("spells.q")
                        && Z.GetValue<bool>("rocket.lane.2"))
                    {
                        Q.Cast();
                    }
                    else if (!FishbonesActive()
                        && closest.IsValidTarget()
                        && Z.GetValue<bool>("spells.q")
                        && closest.Distance(Player.Position) > 565f
                        && Z.GetValue<bool>("rocket.lane.1"))
                    {
                        Q.Cast();
                    }
                }
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            var p = Player.Position;

            if (Z.GetValue<bool>("draw.no"))
            {
                return;
            }

            if (Z.GetValue<bool>("draw.q"))
            {
                if (FishbonesActive())
                {
                    Render.Circle.DrawCircle(p, CalcTotalRange(), Q.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);
                }

                else if (!FishbonesActive())
                {
                    Render.Circle.DrawCircle(p, 565f, Q.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);
                }
            }

            if (Z.GetValue<bool>("draw.tg") && target != null)
            {
                Render.Circle.DrawCircle(target.Position, 75f, System.Drawing.Color.Purple);
            }

            if (Z.GetValue<bool>("draw.w"))
            {
                Render.Circle.DrawCircle(p, W.Range, W.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);
            }

            if (Z.GetValue<bool>("draw.e"))
            {
                Render.Circle.DrawCircle(p, E.Range, E.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);
            }

            if (Z.GetValue<bool>("draw.r"))
            {
                Render.Circle.DrawCircle(p, R.Range, R.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);
            }
            if (Z.GetValue<bool>("draw.pred.w") && W.IsInRange(target))
            {
                new Render.Line(Player.Position.To2D(), W.PredOut(target).CastPosition.To2D(), 1, new Color { R = 205, G = 255, B = 205, A = 255 })
                 {
                     VisibleCondition = delegate
                     {
                         return Z.GetValue<bool>("draw.pred.w") && W.IsInRange(target) && !Z.GetValue<bool>("draw.no");
                     }
                 };
            }
            if (Z.GetValue<bool>("draw.pred.e"))
            {
                Render.Circle.DrawCircle(E.GetPrediction(target).CastPosition, 75f, System.Drawing.Color.Aqua);
            }
        }

        public static Modes GetMode()
        {
            var orb = MenuUtils.Orbwalker.ActiveMode;
            switch (orb)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    return Modes.Combo;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    return Modes.LaneClear;
                case Orbwalking.OrbwalkingMode.Mixed:
                    return Modes.Harass;
                default: return Modes.None;
            }
        }

        static void Game_OnUpdate(EventArgs args)
        {
            var orbwalkingMode = MenuUtils.Orbwalker.ActiveMode;

            if (Z.GetValue<bool>("killsecure"))
                KillSecure();

            if (Z.GetValue<KeyBind>("spells.r.manual").Active)
                AssistedUlt();

            switch (GetMode())
            {
                case Modes.Combo:
                    Combo();
                    break;
                case Modes.Harass:
                    Harass();
                    break;
            }
        }

        public static Obj_AI_Hero GetMostDamageTarget()
        {
            var enemies = HeroManager.Enemies.Where(x => R.IsInRange(x) && !x.IsDead);
            var realtg = TargetSelector.GetTarget(7000f, TargetSelector.DamageType.Physical);
            foreach (var fella in enemies)
            {
                if (R.GetDamage(fella) > R.GetDamage(realtg))
                    realtg = fella;
            }
            return (Obj_AI_Hero)realtg;
        }

        public static void AssistedUlt()
        {
            var mode = Z.GetValue<StringList>("spells.r.mode").SelectedIndex + 1;
            var baseTg = HeroManager.Enemies.Where(z => R.IsInRange(z) && !z.IsDead);
            switch (mode)
            {
                case 1:
                    R.Cast(baseTg.OrderBy(x => x.Health).FirstOrDefault());
                    break;
                case 2:
                    R.Cast(baseTg.OrderBy(x => x.Distance(Player.Position)).FirstOrDefault());
                    break;
                case 3:
                    if (GetMostDamageTarget() != null)
                        R.Cast(GetMostDamageTarget());
                    break;
                case 4:
                    foreach (var nerd in HeroManager.Enemies.Where(x => R.IsInRange(x) && !x.IsDead))
                        R.CastIfWillHit(nerd, 2);
                    break;
            }
        }

        public static void KillSecure()
        {
            foreach (var p in HeroManager.Enemies.Where(x => !x.IsDead && x.Distance(Player.Position) < 7000f))
            {
                if (Ignite.IsReady() 
                    &&  p.Health + 50 < Player.GetSummonerSpellDamage(p, Damage.SummonerSpell.Ignite)
                    && p.Distance(Player.Position) <= 600f)
                    Player.Spellbook.CastSpell(Ignite, p);
                if (p.Health + 10 < Player.GetAutoAttackDamage(p)
                    && Player.Distance(p.Position) <= Player.AttackRange)
                    Player.IssueOrder(GameObjectOrder.AttackUnit, p);
                if (W.IsReady() && p.Health + 10 < W.GetDamage(p)
                    && W.IsInRange(p))
                    W.CastIfHitchanceEquals(p, HitChance.High);
                if (R.IsReady() && !W.IsInRange(p) && Z.GetValue<bool>("spells.r") && p.Health + 20 < R.GetDamage(p))
                {
                    if (Z.GetValue<bool>("hc.r"))
                        R.CastIfHitchanceEquals(p, HitChance.High);
                    else R.Cast(p);
                }
            }
                
        }

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

            if (!FishbonesActive() && target.Distance(Player.Position) > 565f
                && target.Distance(Player.Position) < CalcTotalRange() && Z.GetValue<bool>("spells.q"))
                Q.Cast();
            else if (FishbonesActive() && Z.GetValue<bool>("spells.q") && target.Distance(Player.Position) < 565f
                && target.Health + 10 > Player.GetAutoAttackDamage(target) && target.CountEnemiesInRange(150f) <= 1)
                Q.Cast();
            else if (!FishbonesActive() && Z.GetValue<bool>("spells.q") && target.Distance(Player.Position) < 565f
                && target.CountEnemiesInRange(150f) >= 2)
                Q.Cast();

            if (target.IsMoving() && target.IsFacing(Player) 
                && E.IsReady() 
                && E.IsInRange(target)
                && Z.GetValue<bool>("spells.e")
                && Z.GetValue<bool>("spells.e.def"))
            {
                if (Z.GetValue<bool>("hc.e"))
                    E.CastIfHitchanceEquals(target, HitChance.High);
                else E.Cast(target);
            }
            else if (Game.Time + E.Delay + 1 / 2 <= target.Buffs
                .OrderByDescending(x => x.EndTime - Game.Time)
                .Where(x => x.Type == BuffType.Slow)
                .Select(x => x.EndTime)
                .FirstOrDefault() 
                && Z.GetValue<bool>("spells.e") 
                && E.IsReady()
                && E.IsInRange(target))
            {
                E.Cast(E.PredOut(target).CastPosition);
            }
            else if (E.IsReady() && Z.GetValue<bool>("spells.e")
                && (target.HasBuffOfType(BuffType.Stun)
                || target.HasBuffOfType(BuffType.Snare)
                || target.HasBuffOfType(BuffType.Taunt)
                || target.HasBuffOfType(BuffType.Fear)
                || target.HasBuffOfType(BuffType.Charm)))
            {
                if (Z.GetValue<bool>("hc.e"))
                    E.CastIfHitchanceEquals(target, HitChance.High);
                else E.Cast(target);
            }

            if (W.IsReady()
                && W.IsInRange(target)
                && Z.GetValue<bool>("spells.w"))
            {
                if ((!target.IsFacing(Player) && Player.IsFacing(target))
                    || Player.CountEnemiesInRange(565f) == 0
                    || (W.GetDamage(target) > target.Health + 10 && Player.GetAutoAttackDamage(target) < target.Health)
                    || (target.HasBuff("Recall")
                    || target.HasBuffOfType(BuffType.Stun)
                    || target.HasBuffOfType(BuffType.Snare)
                    || target.HasBuffOfType(BuffType.Taunt)
                    || target.HasBuffOfType(BuffType.Fear)
                    || target.HasBuffOfType(BuffType.Charm)
                    || target.HasBuffOfType(BuffType.Suppression))
                    || Player.CountAlliesInRange(700f) >= 2)
                {
                    if (Z.GetValue<bool>("hc.w"))
                        W.CastIfHitchanceEquals(target, HitChance.High);
                    else W.Cast(target);
                }
            }
            
            if (R.IsReady()
                && R.IsInRange(target)
                && Z.GetValue<bool>("spells.r")
                && R.GetDamage(target) > target.Health + 20
                && !W.IsInRange(target))
            {
                if (Z.GetValue<bool>("hc.r"))
                    R.CastIfHitchanceEquals(target, HitChance.High);
                else R.Cast(target);
            }
                
        }

        public static void Harass()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

            if (W.IsReady()
                && W.IsInRange(target)
                && Z.GetValue<bool>("spells.w"))
            {
                if (Z.GetValue<bool>("hc.w"))
                    W.CastIfHitchanceEquals(target, HitChance.High);
                else W.Cast(target);
            }
        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var thing = gapcloser.Sender;
            if (thing.IsValidTarget(E.Range) && E.IsReady())
                E.Cast(gapcloser.End);
        }

    }
}
