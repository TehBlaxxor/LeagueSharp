using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace TehMalzahar
{
    class Malzahar
    {
        public static Spell Q, W, E, R;
        public static SpellSlot Infernus; // ignite xd

        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static Orbwalking.Orbwalker Orbwalker;
        public static Menu Config;

        public static List<Spell> Spells = new List<Spell>();


        public Malzahar()
        {
            Q = new Spell(SpellSlot.Q, 900f);
            Q.SetSkillshot(.5f, 30, 1600, false, SkillshotType.SkillshotCircle);

            W = new Spell(SpellSlot.W, 800f);
            W.SetSkillshot(0.50f, 50, float.MaxValue, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 650f);
            R = new Spell(SpellSlot.R, 700f);
            Infernus = Player.GetSpellSlot("SummonerDot");


            Config = new Menu("TehMalzahar", "MainMenu", true);

            var orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Config.AddSubMenu(orbwalkerMenu);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            var comboMenu = new Menu("Combo", "combomenu");
            comboMenu.AddItem(new MenuItem("comboQ", "Use Call of The Void (Q)").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboW", "Use Null Zone (W)").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboE", "Use Malefic Visions (E)").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboR", "Use Nether Grasp (R)").SetValue(true));
            comboMenu.AddItem(new MenuItem("comboSpacer", " "));
            comboMenu.AddItem(new MenuItem("comboMode", "Combo Mode").SetValue(new StringList(new[] { "Fully Automatic", "Normal" })));
            comboMenu.AddItem(new MenuItem("underturretr", "Don't Use R Under Turret").SetValue(false));
            Config.AddSubMenu(comboMenu);

            var harassMenu = new Menu("Harass", "harassmenu");
            harassMenu.AddItem(new MenuItem("harassQ", "Use Call of The Void (Q)").SetValue(true));
            harassMenu.AddItem(new MenuItem("harassW", "Use Null Zone (W)").SetValue(true));
            harassMenu.AddItem(new MenuItem("harassE", "Use Malefic Visions (E)").SetValue(true));
            Config.AddSubMenu(harassMenu);

            var farmMenu = new Menu("Lane Clear / Farm", "farmmenu");
            farmMenu.AddItem(new MenuItem("farmq", "Use Call of The Void (Q)").SetValue(true));
            farmMenu.AddItem(new MenuItem("farmw", "Use Null Zone (W)").SetValue(true));
            farmMenu.AddItem(new MenuItem("farme", "Use Malefic Visions (E)").SetValue(true));
            Config.AddSubMenu(farmMenu);

            var interrupterMenu = new Menu("Interrupting Spells", "interruptermenu");
            interrupterMenu.AddItem(new MenuItem("interruptq", "Use Call of The Void (Q)").SetValue(true));
            interrupterMenu.AddItem(new MenuItem("interruptr", "Use Nether Grasp (R)").SetValue(true));
            Config.AddSubMenu(interrupterMenu);

            var drawingsMenu = new Menu("Drawings", "drawingsmenu");
            drawingsMenu.AddItem(new MenuItem("drawings", "Drawings").SetValue(true));
            drawingsMenu.AddItem(new MenuItem("drawq", "Draw Q").SetValue(true));
            drawingsMenu.AddItem(new MenuItem("draww", "Draw W").SetValue(true));
            drawingsMenu.AddItem(new MenuItem("drawe", "Draw E").SetValue(true));
            drawingsMenu.AddItem(new MenuItem("drawr", "Draw R").SetValue(true));
            drawingsMenu.AddItem(new MenuItem("drawtg", "Draw Target").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 0, 255, 255))));
            MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
            MenuItem drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true, System.Drawing.Color.FromArgb(90, 255, 169, 4)));
            drawingsMenu.AddItem(drawComboDamageMenu);
            drawingsMenu.AddItem(drawFill);
            DamageIndicator.DamageToUnit = GetComboDamage;
            DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
            DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
            DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
            drawComboDamageMenu.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };
            drawFill.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };
            Config.AddSubMenu(drawingsMenu);

            var miscMenu = new Menu("Others", "miscmenu");
            miscMenu.AddSubMenu(new Menu("Keybinds", "keybinds"));
            miscMenu.SubMenu("keybinds").AddItem(new MenuItem("combobind", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            miscMenu.SubMenu("keybinds").AddItem(new MenuItem("harassbind", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));
            miscMenu.SubMenu("keybinds").AddItem(new MenuItem("farmbind", "Farm").SetValue(new KeyBind('V', KeyBindType.Press)));
            miscMenu.AddSubMenu(new Menu("Hit Chances", "hitchances"));
            miscMenu.SubMenu("hitchances").AddItem(new MenuItem("qhitchance", "Call of The Void (Q)").SetValue(new StringList(new[] { "Medium", "High", "Very High" })));
            miscMenu.SubMenu("hitchances").AddItem(new MenuItem("whitchance", "Null Zone (W)").SetValue(new StringList(new[] { "Medium", "High", "Very High" })));
            miscMenu.AddSubMenu(new Menu("Nether Grasp Mode", "nethergraspmode"));
            foreach (var champion in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
            {
                miscMenu.SubMenu("nethergraspmode").AddItem(new MenuItem("ng" + champion.Name, "Use Ult On " + champion.Name).SetValue(true));
            }
            if (Infernus != SpellSlot.Unknown)
                miscMenu.AddItem(new MenuItem("ignite", "Use Ignite For KS").SetValue(true));
            Config.AddSubMenu(miscMenu);

            Config.AddToMainMenu();

            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        void Drawing_OnDraw(EventArgs args)
        {
            if (!Config.Item("drawings").GetValue<bool>())
                return;
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (Config.Item("drawtg").GetValue<Circle>().Active && target != null)
            {
                Render.Circle.DrawCircle(target.Position, 75f, Config.Item("drawtg").GetValue<Circle>().Color);
            }
            foreach (var x in Spells.Where(y => Config.Item("draw" + y.Slot.ToString().ToLowerInvariant()).GetValue<Circle>().Active))
            {
                Render.Circle.DrawCircle(Player.Position, x.Range, x.IsReady()
                ? System.Drawing.Color.Green
                : System.Drawing.Color.Red
                );
            }
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

            if (Config.Item("combobind").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            else if (Config.Item("harassbind").GetValue <KeyBind>().Active)
            {
                Harass();
            }
            else if (Config.Item("farmbind").GetValue <KeyBind>().Active)
            {
                Farm();
            }
            if (Config.Item("ignite").GetValue<bool>() && Infernus.IsReady())
            {
                Ignite();
            }
        }

        private static float GetComboDamage(Obj_AI_Base enemy)
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

        public static void Ignite()
        {
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(600f) && (x.Health/4)*3 < Player.GetSummonerSpellDamage(x, Damage.SummonerSpell.Ignite)).OrderBy(x => x.Health))
            {
                Player.Spellbook.CastSpell(Infernus, enemy);   
            }
        }

        public static void Combo()
        {
            var ComboModeSelectedIndex = Config.Item("comboMode").GetValue<StringList>().SelectedIndex;
            var QHitChance = Config.Item("qhitchance").GetValue<StringList>().SelectedIndex;
            var WHitChance = Config.Item("whitchance").GetValue<StringList>().SelectedIndex;
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
                    || R.IsReady() && target.IsValidTarget(R.Range) && R.GetDamage(target) > (target.Health/4)*3
                    )
                {
                    if (Config.Item("ng" + target.ChampionName).GetValue<bool>())
                    {
                        if (Player.UnderTurret() && Config.Item("underturretr").GetValue<bool>())
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
                if (E.IsReady() && target.IsValidTarget(E.Range) && Config.Item("comboE").GetValue<bool>())
                {
                    E.CastOnUnit(target);
                }
                if (Q.IsReady() && target.IsValidTarget(Q.Range) && Config.Item("comboQ").GetValue<bool>())
                {
                    if (QHitChance == 0 || Q.GetDamage(target) > target.Health * 3)
                        Q.Cast(target);
                    else if (QHitChance == 1)
                        Q.CastIfHitchanceEquals(target, HitChance.High);
                    else if (QHitChance == 2)
                        Q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                }
                if (W.IsReady() && target.IsValidTarget(W.Range) && Config.Item("comboW").GetValue<bool>())
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
                    if (Config.Item("comboR").GetValue<bool>() && Config.Item("ng" + target.ChampionName).GetValue<bool>())
                    {
                        if (Player.UnderTurret() && Config.Item("underturretr").GetValue<bool>())
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

        public static bool ComboW()
        {
            if (Config.Item("comboW").GetValue<bool>() && W.IsReady())
                return false;
            else return true;
        }
        public static bool ComboQ()
        {
            if (Config.Item("comboQ").GetValue<bool>() && Q.IsReady())
                return false;
            else return true;
        }
        public static bool ComboE()
        {
            if (Config.Item("comboE").GetValue<bool>() && E.IsReady())
                return false;
            else return true;
        }

        public static void Harass()
        {
            var QHitChance = Config.Item("qhitchance").GetValue<StringList>().SelectedIndex;
            var WHitChance = Config.Item("whitchance").GetValue<StringList>().SelectedIndex;
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (E.IsReady() && target.IsValidTarget(E.Range) && Config.Item("harassE").GetValue<bool>())
                E.CastOnUnit(target);
            if (Q.IsReady() && target.IsValidTarget(Q.Range) && Config.Item("harassQ").GetValue<bool>())
            {
                if (QHitChance == 0)
                    Q.Cast(target);
                if (QHitChance == 1)
                    Q.CastIfHitchanceEquals(target, HitChance.High);
                if (QHitChance == 2)
                    Q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }
            if (W.IsReady() && target.IsValidTarget(W.Range) && Config.Item("harassW").GetValue<bool>())
            {
                if (WHitChance == 0)
                    W.Cast(target);
                if (WHitChance == 1)
                    W.CastIfHitchanceEquals(target, HitChance.High);
                if (WHitChance == 2)
                    W.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }
        }

        public static void Farm()
        {
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
            if (E.IsReady() && Config.Item("farme").GetValue<bool>())
            {
                E.CastOnUnit(minion);
            }
            if (Q.IsReady() && Config.Item("farmq").GetValue<bool>())
            {
                Q.Cast(QFarmLocation.Position);
            }
            if (W.IsReady() && Config.Item("farmw").GetValue<bool>())
            {
                W.Cast(WFarmLocation.Position);
            }
        }

        void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy
                && args.DangerLevel == Interrupter2.DangerLevel.High
                && Config.Item("interruptq").GetValue<bool>()
                && Q.IsReady()
                && sender.IsValidTarget(Q.Range))
            {
                Q.CastIfHitchanceEquals(sender, HitChance.Medium);
            }
            else if (sender.IsEnemy
                 && args.DangerLevel == Interrupter2.DangerLevel.High
                 && Config.Item("interruptr").GetValue<bool>()
                 && R.IsReady()
                 && sender.IsValidTarget(R.Range)
                 && !Q.IsReady())
            {
                R.CastOnUnit(sender);
            }
        }
    }
}
