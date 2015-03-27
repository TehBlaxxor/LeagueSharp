using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace TehOrianna
{
    class Orianna
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static SpellSlot IS;
        public static List<Spell> SpellList = new List<Spell>();
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static string infernus = "Tod Re Nommus";

        public static string ReverseString(string s)
        {
            char[] arr = s.ToLowerInvariant().Replace(" ", "").ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        public static float GetComboDamage(Obj_AI_Hero enemy)
        {
            double damage = (double)Player.GetAutoAttackDamage(enemy) * 3;
            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);
            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);
            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);
            if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R) * 8;
            if (IS.IsReady() && IS != SpellSlot.Unknown)
                damage += (float)Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            return (float)damage;
        }

        private static bool WillHitE(Obj_AI_Base enemy, Obj_AI_Hero ally)
        {
            return E.WillHit(enemy, Prediction.GetPrediction(ally, ally.Distance(OriannasBall.BallPos) / E.Speed - Game.Ping / 2.0f).CastPosition);
        }

        static Orianna()
        {
            IS = ObjectManager.Player.GetSpellSlot(ReverseString(infernus));

            Q = new Spell(SpellSlot.Q, 825f);
            Q.SetSkillshot(0f, 80f, 1200f, false, SkillshotType.SkillshotLine);
            SpellList.Add(Q);
            W = new Spell(SpellSlot.W, 255f);
            W.SetSkillshot(0.25f, 0f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            SpellList.Add(W);
            E = new Spell(SpellSlot.E, 1095f);
            E.SetSkillshot(0.25f, 80f, 1700f, false, SkillshotType.SkillshotLine);
            SpellList.Add(E);
            R = new Spell(SpellSlot.R, 410f);
            R.SetSkillshot(0.6f, 0f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            SpellList.Add(R);

            Menu.Runnerino();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

        }

        static void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel == Interrupter2.DangerLevel.High && R.IsReady() && sender.Distance(OriannasBall.BallPos) < R.Range && Menu.Config.Item("tehorianna.interrupter.r").GetValue<bool>())
                R.Cast();
        }


        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (OriannasBall.BallPos == Player.Position && Player.Distance(gapcloser.End) < W.Range)
            {
                W.Cast();
            }
            else if (gapcloser.Start.Distance(OriannasBall.BallPos) < R.Range && gapcloser.Start.Distance(Player.Position) > 749f && R.WillHit(gapcloser.Sender, OriannasBall.BallPos))
            {
                R.Cast();
            }
        }

        static void Game_OnUpdate(EventArgs args)
        {

            if (Menu.Config.Item("tehorianna.killsteal.active").GetValue<bool>())
                KillSteal();

            if (Menu.Config.Item("tehorianna.harass.toggle").GetValue<KeyBind>().Active)
                Harass();

            StayAlive();

            switch (Menu.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }
        }

        private static void StayAlive()
        {
            Obj_AI_Hero ally = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly && !x.IsDead && x.Distance(Player.Position) < E.Range).OrderBy(x => x.Health).FirstOrDefault();
            if (ally.HealthPercentage() < 8)
            {
                E.Cast(ally);
            }
        }

        public static Obj_AI_Hero xd(Obj_AI_Hero rere)
        {
            Obj_AI_Hero hero = rere;
            foreach (var ally in ObjectManager.Get<Obj_AI_Hero>().Where(y => y.IsAlly && !y.IsDead && E.IsInRange(y)))
            {
                if (WillHitE(rere, ally))
                {
                    hero = ally;
                }
                else if (!WillHitE(hero, rere))
                {
                    hero = rere;
                }

            }

            return hero;
        }

        private static void KillSteal()
        {
            Obj_AI_Hero kstarget = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy && !x.IsDead && x.IsValidTarget(Q.Range)).OrderBy(x => x.Health).FirstOrDefault();

            if (!Menu.Config.Item("tehorianna.killsteal.active").GetValue<bool>())
                return;

            if (kstarget.Health + 25 < Q.GetDamage(kstarget) && Q.IsReady() && Menu.Config.Item("tehorianna.killsteal.q").GetValue<bool>())
            {
                Q.Cast(kstarget);
            }
            else if (kstarget.Health + 25 < W.GetDamage(kstarget) && kstarget.Distance(OriannasBall.BallPos) < W.Range && W.IsReady() && Menu.Config.Item("tehorianna.killsteal.w").GetValue<bool>())
            {
                W.Cast();
            }
            else if (WillHitE(kstarget, xd(kstarget)) && E.IsReady() && E.GetDamage(kstarget) > kstarget.Health + 25 && Menu.Config.Item("tehorianna.killsteal.e").GetValue<bool>())
            {
                E.Cast(xd(kstarget));
            }
            else if (kstarget.Health + 25 < R.GetDamage(kstarget) && kstarget.Distance(OriannasBall.BallPos) < R.Range && R.IsReady() && Menu.Config.Item("tehorianna.killsteal.r").GetValue<bool>())
            {
                R.Cast();
            }
            else if (Player.GetSummonerSpellDamage(kstarget, Damage.SummonerSpell.Ignite) > kstarget.Health + 25 && IS != SpellSlot.Unknown && Menu.Config.Item("tehorianna.killsteal.infernus").GetValue<bool>())
            {
                Player.Spellbook.CastSpell(IS, kstarget);
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (Q.IsReady() && Menu.Config.Item("tehorianna.combo.q").GetValue<bool>())
            {
                Q.Cast(target);
            }
            if (W.IsReady() && target.Distance(OriannasBall.BallPos) < W.Range && OriannasBall.BallPos.CountEnemiesInRange(W.Range) >= Menu.Config.Item("tehorianna.combo.wcondition").GetValue<Slider>().Value && Menu.Config.Item("tehorianna.combo.w").GetValue<bool>())
            {
                W.Cast();
            }
            if (WillHitE(target, xd(target)) && E.IsReady() && Menu.Config.Item("tehorianna.combo.e").GetValue<bool>())
            {
                E.Cast(xd(target));
            }
            if (R.IsReady() && target.Distance(OriannasBall.BallPos) < R.Range && OriannasBall.BallPos.CountEnemiesInRange(R.Range) >= Menu.Config.Item("tehorianna.combo.rcondition").GetValue<Slider>().Value && Menu.Config.Item("tehorianna.combo.r").GetValue<bool>())
            {
                R.Cast();
            }
        }

        private static void LastHit()
        {
            if (Player.ManaPercentage() < Menu.Config.Item("tehorianna.mana.manager").GetValue<Slider>().Value)
                return;

            var wkillable = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && !x.IsDead && x.Health + 10 < W.GetDamage(x) && x.Distance(OriannasBall.BallPos) < W.Range);
            var killable = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && !x.IsDead && x.Health + 10 < Q.GetDamage(x) && x.IsValidTarget(Q.Range)).OrderBy(z => z.Health).FirstOrDefault();

            if (Q.IsReady() && Menu.Config.Item("tehorianna.lasthit.q").GetValue<bool>() && killable != null)
                Q.Cast(killable);

            if (W.IsReady() && Menu.Config.Item("tehorianna.lasthit.w").GetValue<bool>() && wkillable.Count() >= Menu.Config.Item("tehorianna.lasthit.wcondition").GetValue<Slider>().Value)
                W.Cast();
        }

        private static void LaneClear()
        {
            if (Player.ManaPercentage() < Menu.Config.Item("tehorianna.mana.manager").GetValue<Slider>().Value)
                return;
            if (Menu.Config.Item("tehorianna.laneclear.mode").GetValue<StringList>().SelectedIndex == 0)
            {
                if (Q.IsReady() && Menu.Config.Item("tehorianna.laneclear.q").GetValue<bool>())
                {
                    List<Obj_AI_Base> minionList = MinionManager.GetMinions(Q.Range);
                    var qCast = MinionManager.GetBestLineFarmLocation(minionList.Select(minion => minion.Position.To2D()).ToList(), Q.Width, Q.Range);

                    Q.Cast(qCast.Position);
                }
            }
            else if (Menu.Config.Item("tehorianna.laneclear.mode").GetValue<StringList>().SelectedIndex == 1)
            {
                if (Q.IsReady() && Menu.Config.Item("tehorianna.laneclear.q").GetValue<bool>())
                {
                    var minions = MinionManager.GetMinions(Game.CursorPos, 50f, MinionTypes.All, MinionTeam.Enemy);
                    if (minions.Count() > 0)
                        Q.Cast(Game.CursorPos);
                }
            }
            if (W.IsReady() && Menu.Config.Item("tehorianna.laneclear.w").GetValue<bool>())
            {
                var minions = MinionManager.GetMinions(OriannasBall.BallPos, W.Range, MinionTypes.All, MinionTeam.Enemy);
                if (minions.Count() > 0)
                    W.Cast();
            }

        }

        private static void Harass()
        {
            if (Player.ManaPercentage() < Menu.Config.Item("tehorianna.mana.manager").GetValue<Slider>().Value)
                return;
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && Menu.Config.Item("tehorianna.harass.q").GetValue<bool>())
            {
                Q.Cast(target);
            }
            if (W.IsReady() && target.Distance(OriannasBall.BallPos) < W.Range && Menu.Config.Item("tehorianna.harass.w").GetValue<bool>())
            {
                W.Cast();
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (!Menu.Config.Item("tehorianna.drawings.toggle").GetValue<bool>())
            {
                return;
            }
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (Menu.Config.Item("tehorianna.drawings.target").GetValue<Circle>().Active && target != null)
            {
                Render.Circle.DrawCircle(target.Position, 75f, Menu.Config.Item("tehorianna.drawings.target").GetValue<Circle>().Color);
            }
            foreach (var x in SpellList.Where(y => Menu.Config.Item("tehorianna.drawings." + y.Slot.ToString().ToLowerInvariant()).GetValue<Circle>().Active))
            {
                if (x.Slot == SpellSlot.Q || x.Slot == SpellSlot.E)
                {
                    Render.Circle.DrawCircle(Player.Position, x.Range, x.IsReady()
                    ? System.Drawing.Color.Green
                    : System.Drawing.Color.Red
                    );
                }
                else if (x.Slot == SpellSlot.W || x.Slot == SpellSlot.R)
                {
                    Render.Circle.DrawCircle(OriannasBall.BallPosDrawSpot, x.Range, x.IsReady()
                    ? System.Drawing.Color.Green
                    : System.Drawing.Color.Red
                    );
                }
            }
        }
    }
}
