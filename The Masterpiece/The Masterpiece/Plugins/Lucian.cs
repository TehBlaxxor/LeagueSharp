using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using The_Masterpiece.Handlers;

namespace The_Masterpiece.Plugins
{
    internal class Lucian : BaseChampion
    {
        public static Spell Q, W, E, R;
        public static List<Spell> Spells = new List<Spell>();

        public Lucian()
        {
            Q = new Spell(SpellSlot.Q, 575f);
            Q.SetSkillshot(0.35f, Player.Spellbook.GetSpell(SpellSlot.Q).SData.LineWidth, float.MaxValue, false, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 1000f);
            W.SetSkillshot(0.30f, 80f, 1600f, true, SkillshotType.SkillshotLine);
            E = new Spell(SpellSlot.E, 425f);
            R = new Spell(SpellSlot.R, 1400f);

            Spells.Add(Q);
            Spells.Add(W);
            Spells.Add(E);
            Spells.Add(R);

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

        }

        void Game_OnUpdate(EventArgs args)
        {
            doKillSteal();
            if (Menu.Item("themp.kb.combo").GetValue<KeyBind>().Active)
                DoCombo();
            if (Menu.Item("themp.kb.harass").GetValue<KeyBind>().Active)
                DoHarass();
            if (Menu.Item("themp.kb.laneclear").GetValue<KeyBind>().Active)
                DoLaneClear();
            if (Menu.Item("themp.kb.escape").GetValue<KeyBind>().Active)
            {
                GlobalMethods.Flee();
                DoEscape();
            }

        }

        #region Detuks' Code
        internal enum QhitChance
        {
            himself = 0,
            easy = 1,
            medium = 2,
            hard = 3,
            wontHit = 4
        }

        public static MathUtils.Polygon getPolygonOn(Obj_AI_Base target, float bonusW = 0)
        {
            List<Vector2> points = new List<Vector2>();
            Vector2 rTpos = Prediction.GetPrediction(target, 0.10f).UnitPosition.To2D();
            Vector2 startP = ObjectManager.Player.ServerPosition.To2D();
            Vector2 endP = startP.Extend(rTpos, 1100 + bonusW);

            Vector2 p = (rTpos - startP);
            var per = p.Perpendicular().Normalized() * (Q.Width / 2 + bonusW);
            points.Add(startP + per);
            points.Add(startP - per);
            points.Add(endP - per);
            points.Add(endP + per);

            return new MathUtils.Polygon(points);
        }

        public static QhitChance hitChOnTarg(Obj_AI_Base target, Obj_AI_Base onTarg)
        {
            if (target.NetworkId == onTarg.NetworkId)
                return QhitChance.himself;

            var poly = getPolygonOn(onTarg, target.BoundingRadius * 0.6f);
            var predTarPos = Prediction.GetPrediction(target, 0.35f).UnitPosition.To2D();
            var nowPos = target.Position.To2D();

            bool nowInside = poly.pointInside(nowPos);
            bool predInsode = poly.pointInside(predTarPos);

            if (nowInside && predInsode)
                return QhitChance.easy;
            if (predInsode)
                return QhitChance.medium;
            if (nowInside)
                return QhitChance.hard;

            return QhitChance.wontHit;
        }

        public static bool targValidForQ(Obj_AI_Base targ)
        {
            if (targ.MagicImmune || targ.IsDead || !targ.IsTargetable)
                return false;
            if (targ.IsAlly)
                return false;
            var dist = targ.Position.To2D().Distance(ObjectManager.Player.Position.To2D(), true);
            var realQRange = Q.Range + targ.BoundingRadius;
            if (dist > realQRange * realQRange)
                return false;
            return true;
        }

        public bool useQonTarg(Obj_AI_Base target, QhitChance hitChance)
        {
            if (!Q.IsReady() || 
                (Menu.Item("themp.kb.combo").GetValue<KeyBind>().Active 
                && !Menu.Item("themp.combo.q").GetValue<bool>())
                || (Menu.Item("themp.kb.harass").GetValue<KeyBind>().Active
                && !Menu.Item("themp.harass.q").GetValue<bool>()))
                return false;

            if (targValidForQ(target))
            {
                Q.CastOnUnit(target);
                return true;
            }

            var bestQon =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(targValidForQ)
                    .OrderBy(hit => hitChOnTarg(target, hit))
                    .FirstOrDefault();
            if (bestQon != null && hitChOnTarg(target, bestQon) <= hitChance)
            {
                Q.CastOnUnit(bestQon);
                return true;
            }
            return false;
        }

        public static Vector3 getQpredictionWithDelay(Obj_AI_Base target, float delay)
        {
            var res = Q.GetPrediction(target);
            var del = Prediction.GetPrediction(target, delay);

            var dif = del.UnitPosition - target.Position;
            return res.CastPosition + dif;
        }

        public void doProKillSteal(Obj_AI_Base target)
        {
            var dashSpeed = E.Range / (700 + Player.MoveSpeed);
            var targetPred = getQpredictionWithDelay(target, dashSpeed).To2D();
            var mins =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(m => m.Distance(Player.Position, true) < 900*900)
                    .OrderByDescending(mini => mini.Distance(Player.Position, true));
            Console.WriteLine(mins.Count());
            foreach (var min in mins)
            {

                var minPred = Prediction.GetPrediction(min, dashSpeed);

                var inter = MathUtils.getCicleLineInteraction(minPred.UnitPosition.To2D(), targetPred, Player.Position.To2D(), E.Range);

                var best = inter.getBestInter(target);
                if (best.X == 0)
                    return;

                E.Cast(best);

            }
        }

        public void killIfPossible(Obj_AI_Base target)
        {
            try
            {
                if (Menu.Item("themp.ks").GetValue<bool>())
                {
                    var dist = target.Distance(Player.Position) - target.BoundingRadius + 10;
                    var tHP = target.Health;
                    var qDmg = Q.GetDamage(target);
                    //Console.WriteLine(tHP+" - "+qDmg);
                    var wDmg = W.GetDamage(target);
                    var passive = Player.GetAutoAttackDamage(target) * 1.45f;



                    if (qDmg - 20 > tHP && E.IsReady() && Q.IsReady() && dist < 1100 + E.Range && dist > Q.Range + 100)
                    {
                        doProKillSteal(target);
                        Orbwalking.Orbwalk(target, Game.CursorPos);
                    }

                    if (qDmg > tHP && Q.IsReady() && dist < 1150)
                    {
                        useQonTarg(target, QhitChance.medium);
                        return;
                    }


                    if (wDmg > tHP && W.IsReady() && dist < W.Range)
                    {
                        W.CastIfHitchanceEquals(target, HitChance.Medium);
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        public void doKillSteal()
        {
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(ene => ene.IsEnemy && ene.IsValidTarget(1500)))
            {
                killIfPossible(enemy);
            }
        }

        #endregion


        void Drawing_OnDraw(EventArgs args)
        {
            if (!Menu.Item("themp.drawings.draw").GetValue<bool>())
                return;
            
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (Menu.Item("themp.drawings.target").GetValue<Circle>().Active && target != null)
            {
                Render.Circle.DrawCircle(target.Position, 75f, Menu.Item("themp.drawings.target").GetValue<Circle>().Color);
            }

            foreach (var x in Spells.Where(y => Menu.Item("themp.drawings." + y.Slot.ToString().ToLowerInvariant()).GetValue<Circle>().Active))
            {
                Render.Circle.DrawCircle(Player.Position, x.Range, x.IsReady()
                ? System.Drawing.Color.Green
                : System.Drawing.Color.Red
                );
            }
        }

        private void ManageE(Obj_AI_Hero target, bool gapcloser)
        {
            var distto = Player.Distance(target.Position);

            if (Passive())
                Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            // combo e
            if (Menu.Item("themp.combo.e").GetValue<bool>() 
                && Menu.Item("themp.kb.combo").GetValue<KeyBind>().Active)
            {
                if (!Q.IsInRange(target)
                    && E.IsReady()
                    && Q.IsReady()
                    && distto < Q.Range + E.Range
                    && Menu.Item("themp.combo.q").GetValue<bool>())
                {
                    E.Cast(target.Position);
                }

                if (!Passive()
                    && E.IsReady())
                {
                    if (distto < Player.AttackRange)
                        E.Cast(Game.CursorPos);
                    else if (distto < E.Range + Player.AttackRange)
                        E.Cast(target.Position);
                }

            }


            // gapcloser e
            if (Menu.Item("themp.gapcloser.e").GetValue<bool>() 
                && gapcloser 
                && E.IsReady())
            {
                CastEAwayFrom(target);
            }

            // escape e
            if (Menu.Item("themp.escape.e").GetValue<bool>() 
                && Menu.Item("themp.kb.escape").GetValue<KeyBind>().Active
                && E.IsReady())
            {
                E.Cast(Game.CursorPos);
            }
        }

        public override float GetComboDamage(Obj_AI_Hero target)
        {
            double damage = 0d;
            if (R.IsReady())
                damage += R.GetDamage(target);
            if (Q.IsReady() && Menu.Item("themp.combo.q").GetValue<bool>())
                damage += Q.GetDamage(target);
            if (W.IsReady() && Menu.Item("themp.combo.w").GetValue<bool>())
                damage += W.GetDamage(target);
            damage += Player.GetAutoAttackDamage(target) * 3;

            return (float)damage;
        }

        private bool Passive()
        {
            return Player.Buffs.Any(b => b.Name.ToLowerInvariant() == "lucianpassivebuff");
        }

        private bool IsInTower(Vector2 bestpositioneune)
        {
            foreach (var tower in ObjectManager.Get<Obj_AI_Turret>()
                .Where(turr => turr.IsEnemy
                    && bestpositioneune.Distance(turr.Position.To2D()) < Player.BoundingRadius + 850))
            {
                if (tower != null)
                    return true;
            }
            return false;
        }

        private bool CastEAwayFrom(Obj_AI_Hero noob)
        {
            if (noob == null)
                return false;

            var extendingpos = Player.Position.To2D() - (noob.Position - Player.Position).To2D();
            var castingpos = Player.Position.To2D().Extend(extendingpos, E.Range);

            if (!IsInTower(castingpos))
            {
                E.Cast(castingpos);
                return true;
            }

            return false;
        }

        void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            ManageE(gapcloser.Sender, true);
        }


        public bool ManaManager()
        {
            return Player.Mana >= Player.MaxMana * (GetValue<Slider>("themp.manamanager.percentage").Value / 100);
        }

        private void DoCombo()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            ManageE(target, false);

            if (Q.IsInRange(target)
                && Menu.Item("themp.combo.q").GetValue<bool>()
                && Q.IsReady())
                useQonTarg(target, QhitChance.medium);

            if ((!Passive()
                && Menu.Item("themp.combo.w").GetValue<bool>()
                && W.IsReady()
                && target.Distance(Player.Position) < Q.Range)
                ||
                (W.GetDamage(target) > (target.Health/4) * 3
                && Menu.Item("themp.combo.w").GetValue<bool>()
                && W.IsReady()
                && W.IsInRange(target)))
            {
                if (Menu.Item("themp.hc.w").GetValue<StringList>().SelectedIndex == 0)
                    W.Cast(target);
                else if (Menu.Item("themp.hc.w").GetValue <StringList>().SelectedIndex == 1)
                    W.CastIfHitchanceEquals(target, HitChance.High);
            }
           // UseItems();
            UseSummoners(); 
   
        }


        private void DoLaneClear()
        {
            if (ManaManager())
            {

                var minions = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
                List<Vector2> MinionPositions = MinionManager.GetMinionsPredictedPositions(minions,
                    W.Delay,
                    W.Width,
                    W.Speed,
                    Player.Position,
                    W.Range,
                    true,
                    SkillshotType.SkillshotCircle);
                MinionManager.FarmLocation FarmLocation = Q.GetLineFarmLocation(MinionPositions);
                if (FarmLocation.MinionsHit >= 3
                    && W.IsReady()
                    && Menu.Item("themp.laneclear.w").GetValue<bool>())
                    W.Cast(FarmLocation.Position);

                if (Q.IsReady()
                    && Menu.Item("themp.laneclear.q").GetValue<bool>()
                    && minions[0].Health < Q.GetDamage(minions[0])
                    && Q.IsInRange(minions[0]))
                {
                    useQonTarg(minions[0], QhitChance.medium);
                }


            }
        }

        private void DoHarass()
        {
            if (ManaManager())
            {
                Obj_AI_Hero target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (Menu.Item("themp.harass.q").GetValue<bool>() && Q.IsReady())
                {
                    if (Menu.Item("themp.harass.q.smart").GetValue<bool>())
                    {
                        useQonTarg(target, QhitChance.medium);
                    }
                    else
                    {
                        if (Q.IsInRange(target))
                            useQonTarg(target, QhitChance.medium);
                    }
                }
                if (Menu.Item("themp.harass.w").GetValue<bool>() && W.IsReady() && W.IsInRange(target))
                {
                    if (Menu.Item("themp.hc.w").GetValue<StringList>().SelectedIndex == 0)
                        W.Cast(target);
                    else if (Menu.Item("themp.hc.w").GetValue<StringList>().SelectedIndex == 1)
                        W.CastIfHitchanceEquals(target, HitChance.High);
                }
            }
        }

        private void DoEscape()
        {
            ManageE(TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical), false);
        }

        //private void UseItems()
        //{
        //    var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);

        //    if (ItemHandler.BotRK.CanCast(target)
        //        && Menu.Item("themp.botrk").GetValue<bool>()
        //        && Player.HealthPercent <= 99)
        //        ItemHandler.BotRK.Instance.Cast(target);
            

        //    if (ItemHandler.Randuin.CanCast(target)
        //        && Menu.Item("themp.randuin").GetValue<bool>())
        //    {
        //        if (target.IsFacing(Player)
        //            && !Player.IsFacing(target))
        //            ItemHandler.Randuin.Instance.Cast();
        //        else if (!target.IsFacing(Player)
        //            && Player.IsFacing(target))
        //            ItemHandler.Randuin.Instance.Cast();
        //    }

        //    if (ItemHandler.Righteous.CanCast()
        //        && Menu.Item("themp.righteous").GetValue<bool>())
        //    {
        //        if (Player.CountAlliesInRange(ItemHandler.Righteous.Range) >= 2
        //            && target != null
        //            && !target.IsFacing(Player))
        //            ItemHandler.Righteous.Instance.Cast();
        //    }

        //    if (ItemHandler.Mikael.CanCast()
        //        && Menu.Item("themp.mikael").GetValue<bool>())
        //    {
        //        if (Player.HasBuffOfType(BuffType.Charm)
        //            || Player.HasBuffOfType(BuffType.Stun)
        //            || Player.HasBuffOfType(BuffType.Suppression)
        //            || Player.HasBuffOfType(BuffType.Taunt)
        //            || Player.HasBuffOfType(BuffType.Fear)
        //            || Player.HasBuffOfType(BuffType.Blind)
        //            || Player.HasBuffOfType(BuffType.Snare))
        //            ItemHandler.Mikael.Instance.Cast();
        //    }

        //    if (ItemHandler.QSS.CanCast()
        //        && Menu.Item("themp.qss").GetValue<bool>())
        //    {
        //        if (Player.HasBuffOfType(BuffType.Charm)
        //            || Player.HasBuffOfType(BuffType.Stun)
        //            || Player.HasBuffOfType(BuffType.Suppression)
        //            || Player.HasBuffOfType(BuffType.Taunt)
        //            || Player.HasBuffOfType(BuffType.Fear)
        //            || Player.HasBuffOfType(BuffType.Blind)
        //            || Player.HasBuffOfType(BuffType.Snare))
        //            ItemHandler.QSS.Instance.Cast();

        //    }

        //    if (ItemHandler.Scimitar.CanCast()
        //         && Menu.Item("themp.scimitar").GetValue<bool>())
        //    {
        //        if (Player.HasBuffOfType(BuffType.Charm)
        //            || Player.HasBuffOfType(BuffType.Stun)
        //            || Player.HasBuffOfType(BuffType.Suppression)
        //            || Player.HasBuffOfType(BuffType.Taunt)
        //            || Player.HasBuffOfType(BuffType.Fear)
        //            || Player.HasBuffOfType(BuffType.Blind)
        //            || Player.HasBuffOfType(BuffType.Snare))
        //            ItemHandler.Scimitar.Instance.Cast();

        //    }
        //}

        private void UseSummoners()
        {
            var inNeedOfHealthAlly = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly && x.Distance(Player.Position) < SpellHandler.Heal.Range).FirstOrDefault();
            var inNeedOfManaAlly = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly && x.Distance(Player.Position) < SpellHandler.Clarity.Range).FirstOrDefault();
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
            if (GetValue<bool>("themp.ghost"))
            {
                if (SpellHandler.Flash.SSpellSlot != SpellSlot.Unknown
                    && !SpellHandler.Flash.IsReady()
                    || SpellHandler.Flash.SSpellSlot == SpellSlot.Unknown
                    || SpellHandler.Flash.SSpellSlot == SpellSlot.Unknown
                    && Player.Distance(target.Position) > Player.AttackRange + 425f)
                {
                    if (target.Health < Player.GetAutoAttackDamage(target) * 2 && SpellHandler.Ghost.IsReady())
                        SpellHandler.Ghost.Cast();
                }
            }

            if (GetValue<bool>("themp.barrier"))
            {
                if (target != null
                    && Player.HealthPercent < 20
                    && SpellHandler.Barrier.IsReady())
                    SpellHandler.Barrier.Cast();
            }

            if (GetValue<bool>("themp.clarity"))
            {
                if (Player.ManaPercent < 20
                    && target != null
                    && SpellHandler.Clarity.IsReady())
                    SpellHandler.Clarity.Cast();

                else if (inNeedOfManaAlly.ManaPercent < 20
                    && target != null
                    && SpellHandler.Clarity.IsReady())
                    SpellHandler.Clarity.Cast(inNeedOfManaAlly);
            }

            if (GetValue<bool>("themp.exhaust"))
            {
                if (target.IsValidTarget(SpellHandler.Exhaust.Range)
                    && SpellHandler.Exhaust.IsReady())
                    SpellHandler.Exhaust.Cast(target);

            }

            if (GetValue<bool>("themp.flash"))
            {
                if (target.Health < Player.GetAutoAttackDamage(target)
                    && target.IsValidTarget(Player.AttackRange + 400f)
                    && !target.IsValidTarget(Player.AttackRange)
                    && SpellHandler.Flash.IsReady())
                    SpellHandler.Flash.Cast(target.Position);
            }

            if (GetValue<bool>("themp.heal"))
            {
                if (Player.HealthPercent < 20
                    && target != null
                    && SpellHandler.Heal.IsReady())
                    SpellHandler.Heal.Cast();

                else if (inNeedOfHealthAlly.HealthPercent < 20
                    && target != null
                    && SpellHandler.Heal.IsReady())
                    SpellHandler.Heal.Cast(inNeedOfHealthAlly);
            }

            if (GetValue<bool>("themp.ignite"))
            {
                if ((target.Health / 4) * 3 < Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)
                    && target.IsValidTarget(SpellHandler.Ignite.Range)
                    && SpellHandler.Ignite.IsReady())
                    SpellHandler.Ignite.Cast(target);
            }
        }

        public override void Combo(Menu config)
        {
            config.AddItem(new MenuItem("themp.combo.q", "Use Piercing Light (Q)").SetValue(true));
            config.AddItem(new MenuItem("themp.combo.w", "Use Ardent Blaze (W)").SetValue(true));
            config.AddItem(new MenuItem("themp.combo.e", "Use Relentless Pursuit (E)").SetValue(true));
        }

        public override void Harass(Menu config)
        {
            config.AddItem(new MenuItem("themp.harass.q.smart", "Use Smart Q").SetValue(true));
            config.AddItem(new MenuItem("themp.harass.q", "Use Piercing Light (Q)").SetValue(true));
            config.AddItem(new MenuItem("themp.harass.w", "Use Ardent Blaze (W)").SetValue(true));
        }

        public override void Laneclear(Menu config)
        {
            config.AddItem(new MenuItem("themp.laneclear.q", "Use Piercing Light (Q)").SetValue(true));
            config.AddItem(new MenuItem("themp.laneclear.w", "Use Ardent Blaze (W)").SetValue(true));
        }

        public override void Misc(Menu config)
        {
            config.AddItem(new MenuItem("themp.gapcloser.e", "Relentless Pursuit (E) antigapcloser").SetValue(true));
            config.AddItem(new MenuItem("themp.ks", "Try to KS").SetValue(true));
        }

        public override void Escape(Menu menu)
        {
            menu.AddItem(new MenuItem("themp.escape.e", "Use Relentless Pursuit (E)").SetValue(true));
        }

        public override void ItemMenu(Menu menu)
        {
            menu.AddItem(new MenuItem("themp.botrk", "Use BoTRK").SetValue(true));
            menu.AddItem(new MenuItem("themp.randuin", "Use Randuin's Omen").SetValue(true));
            menu.AddItem(new MenuItem("themp.righteous", "Use Righteous Glory").SetValue(true));
            menu.AddItem(new MenuItem("themp.mikael", "Use Mikael's Crucible").SetValue(true));
            menu.AddItem(new MenuItem("themp.qss", "Use QSS").SetValue(true));
            menu.AddItem(new MenuItem("themp.scimitar", "Use Mercurial Scimitar").SetValue(true));
        }

        public override void Extra(Menu config)
        {
            var MiscManaSubMenu = new Menu("Misc - Mana Manager", "themp.manamanager");
            {
                MiscManaSubMenu.AddItem(new MenuItem("themp.manamanager.percentage", "% safe for Combo").SetValue(new Slider(50, 0, 100)));
            }
            config.AddSubMenu(MiscManaSubMenu);

            var MiscKeybindsSubMenu = new Menu("Misc - Keybinds", "themp.kb");
            {
                MiscKeybindsSubMenu.AddItem(new MenuItem("themp.kb.combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
                MiscKeybindsSubMenu.AddItem(new MenuItem("themp.kb.harass", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));
                MiscKeybindsSubMenu.AddItem(new MenuItem("themp.kb.laneclear", "LaneClear").SetValue(new KeyBind('V', KeyBindType.Press)));
                MiscKeybindsSubMenu.AddItem(new MenuItem("themp.kb.escape", "Escape").SetValue(new KeyBind('G', KeyBindType.Press)));
            }
            config.AddSubMenu(MiscKeybindsSubMenu);

            var MiscHitchancesSubMenu = new Menu("Misc - Hitchances", "themp.hc");
            {
                MiscHitchancesSubMenu.AddItem(new MenuItem("themp.hc.w", "Ardent Blaze (W)").SetValue(new StringList(new[] { "Normal", "High" })));
            }
            config.AddSubMenu(MiscHitchancesSubMenu);
        }

        public override void Drawings(Menu config)
        {
            config.AddItem(new MenuItem("themp.drawings.draw", "Drawings").SetValue(true));
            config.AddItem(new MenuItem("themp.drawings.target", "Draw Target").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            config.AddItem(new MenuItem("themp.drawings.q", "Draw Piercing Light (Q)").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            config.AddItem(new MenuItem("themp.drawings.w", "Draw Ardent Blaze (W)").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            config.AddItem(new MenuItem("themp.drawings.e", "Draw Relentless Pursuit (E)").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            config.AddItem(new MenuItem("themp.drawings.r", "Draw The Culling (R)").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
        }
    }
}
