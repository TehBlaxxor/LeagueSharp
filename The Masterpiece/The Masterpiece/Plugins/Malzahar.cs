using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Masterpiece.Handlers;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace The_Masterpiece.Plugins
{
    internal class Malzahar : BaseChampion
    {
        public static Spell Q, W, E, R;
        public static List<Spell> Spells = new List<Spell>();

        public Malzahar()
        {
            Q = new Spell(SpellSlot.Q, 900f);
            Q.SetSkillshot(.5f, 30, 1600, false, SkillshotType.SkillshotCircle);
            W = new Spell(SpellSlot.W, 800f);
            W.SetSkillshot(0.50f, 50, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E = new Spell(SpellSlot.E, 650f);
            R = new Spell(SpellSlot.R, 700f);

            Spells.Add(Q);
            Spells.Add(W);
            Spells.Add(E);
            Spells.Add(R);

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsChannelingImportantSpell() || Player.IsCastingInterruptableSpell())
            {
                Orbwalker.SetMovement(false);
                Orbwalker.SetAttack(false);
            }
            else
            {
                Orbwalker.SetMovement(true);
                Orbwalker.SetAttack(true);
            }

            if (Menu.Item("themp.kb.combo").GetValue<KeyBind>().Active)
                DoCombo();
            if (Menu.Item("themp.kb.harass").GetValue<KeyBind>().Active)
                DoHarass();
            if (Menu.Item("themp.kb.laneclear").GetValue<KeyBind>().Active)
                DoLaneClear();
            if (Menu.Item("themp.kb.escape").GetValue<KeyBind>().Active)
            {
                GlobalMethods.Flee();
            }

        }

        void Drawing_OnDraw(EventArgs args)
        {
            if (!Menu.Item("themp.drawings.draw").GetValue<bool>())
                return;

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
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

        void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy
                            && args.DangerLevel == Interrupter2.DangerLevel.High
                            && Menu.Item("themp.interrupt.q").GetValue<bool>()
                            && Q.IsReady()
                            && sender.IsValidTarget(Q.Range))
            {
                Q.CastIfHitchanceEquals(sender, HitChance.Medium);
            }
            else if (sender.IsEnemy
                 && args.DangerLevel == Interrupter2.DangerLevel.High
                 && Menu.Item("themp.interrupt.r").GetValue<bool>()
                 && R.IsReady()
                 && sender.IsValidTarget(R.Range)
                 && !Q.IsReady())
            {
                R.CastOnUnit(sender);
            }
        }

        public bool ManaManager()
        {
            return Player.Mana >= Player.MaxMana * (GetValue<Slider>("themp.manamanager.percentage").Value / 100);
        }

        public override float GetComboDamage(Obj_AI_Hero enemy)
        {
            double damage = 0d;

            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W) * 3;

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);

            return (float)damage;
        }

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

        private void DoCombo()
        {
            /*UseItems();
            UseSummoners();*/
            var ComboModeSelectedIndex = Menu.Item("themp.combo.mode").GetValue<StringList>().SelectedIndex;
            var QHitChance = Menu.Item("themp.hitchance.q").GetValue<StringList>().SelectedIndex;
            var WHitChance = Menu.Item("themp.hitchance.w").GetValue<StringList>().SelectedIndex;
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            // underturretr, comboQ
            if (ComboModeSelectedIndex == 0)
            {
                //Fully Automatic Combo Code
                if (E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.CastOnUnit(target);
                }
                if (Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    if (QHitChance == 0 || Q.GetDamage(target) > target.Health * 3)
                        Q.Cast(target);
                    else if (QHitChance == 1)
                        Q.CastIfHitchanceEquals(target, HitChance.High);
                    else if (QHitChance == 2)
                        Q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                }
                if (W.IsReady() && target.IsValidTarget(W.Range))
                {
                    if (target.HasBuffOfType(BuffType.Knockup)
                        || target.HasBuffOfType(BuffType.Fear)
                        || target.HasBuffOfType(BuffType.Slow)
                        || target.HasBuffOfType(BuffType.Snare)
                        || target.HasBuffOfType(BuffType.Stun)
                        || target.HasBuffOfType(BuffType.Suppression)
                        || target.HasBuffOfType(BuffType.Taunt)
                        || R.IsReady() && target.IsValidTarget(R.Range)
                        )
                    {
                        if (WHitChance == 0 || W.GetDamage(target) > target.Health * 3)
                            W.Cast(target);
                        else if (WHitChance == 1)
                            W.CastIfHitchanceEquals(target, HitChance.High);
                        else if (WHitChance == 2)
                            W.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                    }
                }
                if (R.IsReady() && target.IsValidTarget(R.Range) && !W.IsReady() && !Q.IsReady() && !E.IsReady()
                    || R.IsReady() && target.IsValidTarget(R.Range) && R.GetDamage(target) > (target.Health / 4) * 3
                    )
                {
                    if (Menu.Item("ng" + target.ChampionName).GetValue<bool>())
                    {
                        if (Player.UnderTurret() && Menu.Item("underturretr").GetValue<bool>())
                        {
                            R.CastOnUnit(target);
                        }
                        else if (!Player.UnderTurret())
                        {
                            R.CastOnUnit(target);
                        }
                    }
                }

            }
            else if (ComboModeSelectedIndex == 1)
            {
                //Normal Combo Code
                if (E.IsReady() && target.IsValidTarget(E.Range) && Menu.Item("themp.combo.E").GetValue<bool>())
                {
                    E.CastOnUnit(target);
                }
                if (Q.IsReady() && target.IsValidTarget(Q.Range) && Menu.Item("themp.combo.Q").GetValue<bool>())
                {
                    if (QHitChance == 0 || Q.GetDamage(target) > target.Health * 3)
                        Q.Cast(target);
                    else if (QHitChance == 1)
                        Q.CastIfHitchanceEquals(target, HitChance.High);
                    else if (QHitChance == 2)
                        Q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                }
                if (W.IsReady() && target.IsValidTarget(W.Range) && Menu.Item("themp.combo.W").GetValue<bool>())
                {
                    if (target.HasBuffOfType(BuffType.Knockup)
                        || target.HasBuffOfType(BuffType.Fear)
                        || target.HasBuffOfType(BuffType.Slow)
                        || target.HasBuffOfType(BuffType.Snare)
                        || target.HasBuffOfType(BuffType.Stun)
                        || target.HasBuffOfType(BuffType.Suppression)
                        || target.HasBuffOfType(BuffType.Taunt)
                        || R.IsReady() && target.IsValidTarget(R.Range)
                        )
                    {
                        if (WHitChance == 0 || W.GetDamage(target) > target.Health * 3)
                            W.Cast(target);
                        else if (WHitChance == 1)
                            W.CastIfHitchanceEquals(target, HitChance.High);
                        else if (WHitChance == 2)
                            W.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                    }
                }
                if (R.IsReady() && target.IsValidTarget(R.Range) && ComboW() && ComboQ() && ComboE()
                    || R.IsReady() && target.IsValidTarget(R.Range) && R.GetDamage(target) > (target.Health / 4) * 3
                    )
                {
                    if (Menu.Item("themp.combo.R").GetValue<bool>() && Menu.Item("ng" + target.ChampionName).GetValue<bool>())
                    {
                        if (Player.UnderTurret() && Menu.Item("underturretr").GetValue<bool>())
                        {
                            R.CastOnUnit(target);
                        }
                        else if (!Player.UnderTurret())
                        {
                            R.CastOnUnit(target);
                        }
                    }
                }
            }
        }

        private bool ComboW()
        {
            if (Menu.Item("themp.combo.W").GetValue<bool>() && W.IsReady())
                return false;
            else return true;
        }
        private bool ComboQ()
        {
            if (Menu.Item("themp.combo.Q").GetValue<bool>() && Q.IsReady())
                return false;
            else return true;
        }
        private bool ComboE()
        {
            if (Menu.Item("themp.combo.E").GetValue<bool>() && E.IsReady())
                return false;
            else return true;
        }

        private void DoLaneClear()
        {
            if (!ManaManager())
                return;
            Obj_AI_Minion minion = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.IsValidTarget(E.Range)).OrderBy(x => x.Health).FirstOrDefault();
            MinionManager.FarmLocation QFarmLocation =
                     Q.GetCircularFarmLocation(
                     MinionManager.GetMinionsPredictedPositions(MinionManager.GetMinions(Q.Range),
                     Q.Delay, Q.Width, Q.Speed,
                     Player.Position, Q.Range,
                     false, SkillshotType.SkillshotLine), Q.Width);
            MinionManager.FarmLocation WFarmLocation =
                     W.GetCircularFarmLocation(MinionManager.GetMinionsPredictedPositions(MinionManager.GetMinions(W.Range),
                     W.Delay, W.Width, W.Speed,
                     Player.Position, W.Range,
                     false, SkillshotType.SkillshotLine), Q.Width);
            if (E.IsReady() && Menu.Item("themp.laneclear.E").GetValue<bool>())
            {
                E.CastOnUnit(minion);
            }
            if (Q.IsReady() && Menu.Item("themp.laneclear.Q").GetValue<bool>())
            {
                Q.Cast(QFarmLocation.Position);
            }
            if (W.IsReady() && Menu.Item("themp.laneclear.W").GetValue<bool>())
            {
                W.Cast(WFarmLocation.Position);
            }
        }

        private void DoHarass()
        {
            if (!ManaManager())
                return;
            var QHitChance = Menu.Item("themp.hitchance.q").GetValue<StringList>().SelectedIndex;
            var WHitChance = Menu.Item("themp.hitchance.w").GetValue<StringList>().SelectedIndex;
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (E.IsReady() && target.IsValidTarget(E.Range) && Menu.Item("themp.harass.E").GetValue<bool>())
                E.CastOnUnit(target);
            if (Q.IsReady() && target.IsValidTarget(Q.Range) && Menu.Item("themp.harass.Q").GetValue<bool>())
            {
                if (QHitChance == 0)
                    Q.Cast(target);
                if (QHitChance == 1)
                    Q.CastIfHitchanceEquals(target, HitChance.High);
                if (QHitChance == 2)
                    Q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }
            if (W.IsReady() && target.IsValidTarget(W.Range) && Menu.Item("themp.harass.W").GetValue<bool>())
            {
                if (WHitChance == 0)
                    W.Cast(target);
                if (WHitChance == 1)
                    W.CastIfHitchanceEquals(target, HitChance.High);
                if (WHitChance == 2)
                    W.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }
        
        }

        private void UseItems()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (ItemHandler.Righteous.CanCast()
                && Menu.Item("themp.items.righteous").GetValue<bool>())
            {
                if (Player.CountAlliesInRange(ItemHandler.Righteous.Range) >= 2
                    && target != null
                    && !target.IsFacing(Player))
                    ItemHandler.Righteous.Instance.Cast();
            }
            if (ItemHandler.Randuin.CanCast(target)
                && Menu.Item("themp.items.randuin").GetValue<bool>())
            {
                if (target.IsFacing(Player)
                    && !Player.IsFacing(target))
                    ItemHandler.Randuin.Instance.Cast();
                else if (!target.IsFacing(Player)
                    && Player.IsFacing(target))
                    ItemHandler.Randuin.Instance.Cast();
            }
            if (ItemHandler.Zhonya.CanCast()
                && Menu.Item("themp.items.zhonya").GetValue<bool>()
                && Player.HealthPercent <= 10
                && Player.CountEnemiesInRange(Q.Range) >= 3)
                ItemHandler.Zhonya.Instance.Cast();

            if (ItemHandler.Gunblade.CanCast()
                && Menu.Item("themp.items.hextech").GetValue<bool>()
                && Player.HealthPercent <= 90
                && target.IsValidTarget(ItemHandler.Revolver.Range))
                ItemHandler.Gunblade.Instance.Cast(target);

            if (ItemHandler.Revolver.CanCast()
                && Menu.Item("themp.items.hextech").GetValue<bool>()
                && Player.HealthPercent <= 90
                && target.IsValidTarget(ItemHandler.Revolver.Range))
                ItemHandler.Revolver.Instance.Cast(target);

            if (ItemHandler.Seraph.CanCast()
                && Menu.Item("themp.items.seraph").GetValue<bool>()
                && Player.HealthPercent <= 30
                && Player.ManaPercent >= 60
                && Player.CountEnemiesInRange(Q.Range) >= 3)
                ItemHandler.Seraph.Instance.Cast(Player);


        }

        public override void Combo(Menu config)
        {
            config.AddItem(new MenuItem("themp.combo.Q", "Use Call of The Void (Q)").SetValue(true));
            config.AddItem(new MenuItem("themp.combo.W", "Use Null Zone (W)").SetValue(true));
            config.AddItem(new MenuItem("themp.combo.E", "Use Malefic Visions (E)").SetValue(true));
            config.AddItem(new MenuItem("themp.combo.R", "Use Nether Grasp (R)").SetValue(true));
            config.AddItem(new MenuItem("comboSpacer", " "));
            config.AddItem(new MenuItem("themp.combo.mode", "Combo Mode").SetValue(new StringList(new[] { "Fully Automatic", "Normal" })));
            config.AddItem(new MenuItem("underturretr", "Don't Use R Under Turret").SetValue(false));
        }

        public override void Harass(Menu config)
        {
            config.AddItem(new MenuItem("themp.harass.Q", "Use Call of The Void (Q)").SetValue(true));
            config.AddItem(new MenuItem("themp.harass.W", "Use Null Zone (W)").SetValue(true));
            config.AddItem(new MenuItem("themp.harass.E", "Use Malefic Visions (E)").SetValue(true));
        }

        public override void Laneclear(Menu config)
        {
            config.AddItem(new MenuItem("themp.laneclear.Q", "Use Call of The Void (Q)").SetValue(true));
            config.AddItem(new MenuItem("themp.laneclear.W", "Use Null Zone (W)").SetValue(true));
            config.AddItem(new MenuItem("themp.laneclear.E", "Use Malefic Visions (E)").SetValue(true));
        }

        public override void Misc(Menu config)
        {
            config.AddItem(new MenuItem("themp.interrupt.q", "Use Call of The Void (Q)").SetValue(true));
            config.AddItem(new MenuItem("themp.interrupt.r", "Use Nether Grasp (R)").SetValue(true));
        }

        public override void Escape(Menu menu)
        {
            menu.AddItem(new MenuItem("nofuncs", "No spell supported."));
        }

        public override void ItemMenu(Menu menu)
        {
            menu.AddItem(new MenuItem("themp.items.randuin", "Use Randuin's Omen").SetValue(true));
            menu.AddItem(new MenuItem("themp.items.zhonya", "Use Zhonya's Hourglass").SetValue(true));
            menu.AddItem(new MenuItem("themp.items.hextech", "Use Hextech Gunblade").SetValue(true));
            menu.AddItem(new MenuItem("themp.items.seraph", "Use Seraph's Embrace").SetValue(true));
            menu.AddItem(new MenuItem("themp.items.righteous", "Use Righteous Glory").SetValue(true));
        }

        public override void Extra(Menu config)
        {
            var MiscManaSubMenu = new Menu("Extra - Mana Manager", "themp.manamanager");
            {
                MiscManaSubMenu.AddItem(new MenuItem("themp.manamanager.percentage", "% safe for Combo").SetValue(new Slider(50, 0, 100)));
            }
            config.AddSubMenu(MiscManaSubMenu);

            var MiscKeybindsSubMenu = new Menu("Extra - Keybinds", "themp.kb");
            {
                MiscKeybindsSubMenu.AddItem(new MenuItem("themp.kb.combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
                MiscKeybindsSubMenu.AddItem(new MenuItem("themp.kb.harass", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));
                MiscKeybindsSubMenu.AddItem(new MenuItem("themp.kb.laneclear", "LaneClear").SetValue(new KeyBind('V', KeyBindType.Press)));
                MiscKeybindsSubMenu.AddItem(new MenuItem("themp.kb.escape", "Escape").SetValue(new KeyBind('G', KeyBindType.Press)));
            }
            config.AddSubMenu(MiscKeybindsSubMenu);

            var MiscHitchancesSubMenu = new Menu("Extra - Hitchances", "themp.hc");
            {
                MiscHitchancesSubMenu.AddItem(new MenuItem("themp.hitchance.q", "Call of The Void (Q)").SetValue(new StringList(new[] { "Medium", "High", "Very High" })));
                MiscHitchancesSubMenu.AddItem(new MenuItem("themp.hitchance.w", "Null Zone (W)").SetValue(new StringList(new[] { "Medium", "High", "Very High" })));
            }
            config.AddSubMenu(MiscHitchancesSubMenu);

            var MiscNetherGraspSubMenu = new Menu("Extra - Nethergrasp", "themp.ng");
            {
                foreach (var champion in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
                {
                    MiscNetherGraspSubMenu.AddItem(new MenuItem("ng" + champion.ChampionName, "Use Ult On " + champion.ChampionName).SetValue(true));
                }
            }
        }

        public override void Drawings(Menu config)
        {
            config.AddItem(new MenuItem("themp.drawings.draw", "Drawings").SetValue(true));
            config.AddItem(new MenuItem("themp.drawings.q", "Draw Q").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            config.AddItem(new MenuItem("themp.drawings.w", "Draw W").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            config.AddItem(new MenuItem("themp.drawings.e", "Draw E").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            config.AddItem(new MenuItem("themp.drawings.r", "Draw R").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            config.AddItem(new MenuItem("themp.drawings.target", "Draw Target").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 0, 255, 255))));
        }
    }
}
