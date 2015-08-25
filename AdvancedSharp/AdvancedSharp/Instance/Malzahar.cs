using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace AdvancedSharp.Instance
{
    internal class Malzahar
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Menu Z;
        public static Spell Q, W, E, R;
        public static SpellSlot Infernus;
        public static bool Enabled = true;
        public static List<Spell> Spells = new List<Spell>();

        public static int[] abilitySequence;
        public static int q = 0, w = 0, e = 0, r = 0;

        public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);

        public static Items.Item TearoftheGoddess = new Items.Item(3070, 0);

        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;

        public static Color Color = Color.Lime;
        public static Color FillColor = Color.Goldenrod;
        public static bool Fill = true;

        private static DamageToUnitDelegate _damageToUnit;

        private static readonly Render.Text Text = new Render.Text(0, 0, "", 14, SharpDX.Color.Red, "monospace");

        public static DamageToUnitDelegate DamageToUnit
        {

            get { return _damageToUnit; }

            set { _damageToUnit = value; }
        }

        public Malzahar()
        {
            Core.LoadWelcomeMessage();

            Q = new Spell(SpellSlot.Q, 900f);
            Q.SetSkillshot(.5f, 30, 1600, false, SkillshotType.SkillshotCircle);
            Spells.Add(Q);

            W = new Spell(SpellSlot.W, 800f);
            W.SetSkillshot(0.50f, 50, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Spells.Add(W);

            E = new Spell(SpellSlot.E, 650f);
            Spells.Add(E);

            R = new Spell(SpellSlot.R, 700f);
            Spells.Add(R);

            Infernus = Player.GetSpellSlot("SummonerDot");

            Z = new Menu("Adv# - Malzahar", "rootmenu", true);

            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Z.AddSubMenu(orbwalkerMenu);

            Menu targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Z.AddSubMenu(targetSelectorMenu);

            Menu MCombo = new Menu("Combo", "combo");
            {
                MCombo.AddItem(new MenuItem("comboQ", "Use Q").SetValue(true));
                MCombo.AddItem(new MenuItem("comboW", "Use W").SetValue(true));
                MCombo.AddItem(new MenuItem("comboE", "Use E").SetValue(true));
                MCombo.AddItem(new MenuItem("comboR", "Use R").SetValue(true));
                MCombo.AddItem(new MenuItem("comboSpacer", " "));
                MCombo.AddItem(new MenuItem("comboMode", "Combo Mode").SetValue(new StringList(new[] { "Fully Automatic", "Normal" })));
                MCombo.AddItem(new MenuItem("underturretr", "Don't Use R Under Turret").SetValue(false));
            }
            Z.AddSubMenu(MCombo);

            Menu MHarass = new Menu("Harass", "harass");
            {
                MHarass.AddItem(new MenuItem("harassQ", "Use Q").SetValue(true));
                MHarass.AddItem(new MenuItem("harassW", "Use W").SetValue(true));
                MHarass.AddItem(new MenuItem("harassE", "Use E").SetValue(true));
            }
            Z.AddSubMenu(MHarass);

            Menu MLC = new Menu("Lane Clear", "lane clear");
            {
                MLC.AddItem(new MenuItem("farmq", "Use Call of The Void (Q)").SetValue(true));
                MLC.AddItem(new MenuItem("farmw", "Use Null Zone (W)").SetValue(true));
                MLC.AddItem(new MenuItem("farme", "Use Malefic Visions (E)").SetValue(true));
            }
            Z.AddSubMenu(MLC);

            Menu MInterrupter = new Menu("Interrupter", "interrupter");
            {
                MInterrupter.AddItem(new MenuItem("interruptq", "Use Call of The Void (Q)").SetValue(true));
                MInterrupter.AddItem(new MenuItem("interruptr", "Use Nether Grasp (R)").SetValue(true));
            }
            Z.AddSubMenu(MInterrupter);

            Menu MDrawings = new Menu("Drawings", "drawings");
            {
                MDrawings.AddItem(new MenuItem("drawings", "Drawings").SetValue(true));
                MDrawings.AddItem(new MenuItem("drawq", "Draw Q").SetValue(true));
                MDrawings.AddItem(new MenuItem("draww", "Draw W").SetValue(true));
                MDrawings.AddItem(new MenuItem("drawe", "Draw E").SetValue(true));
                MDrawings.AddItem(new MenuItem("drawr", "Draw R").SetValue(true));
                MDrawings.AddItem(new MenuItem("drawtg", "Draw Target").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 0, 255, 255))));
                var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo damage").SetValue(true);
                var drawFill =
                    new MenuItem("RushDrawWDamageFill", "Combo Damage Fill").SetValue(new Circle(true, Color.SeaGreen));
                MDrawings.SubMenu("Combo Damage").AddItem(drawDamageMenu);
                MDrawings.SubMenu("Combo Damage").AddItem(drawFill);

                DamageToUnit = GetComboDamage;
                Enabled = drawDamageMenu.GetValue<bool>();
                Fill = drawFill.GetValue<Circle>().Active;
                FillColor = drawFill.GetValue<Circle>().Color;
            }

            Menu Misc = new Menu("Misc", "misc");
            {
                Misc.AddSubMenu(new Menu("Keybinds", "keybinds"));
                Misc.SubMenu("keybinds").AddItem(new MenuItem("combobind", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
                Misc.SubMenu("keybinds").AddItem(new MenuItem("harassbind", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));
                Misc.SubMenu("keybinds").AddItem(new MenuItem("farmbind", "Farm").SetValue(new KeyBind('V', KeyBindType.Press)));
                Misc.AddSubMenu(new Menu("Hit Chances", "hitchances"));
                Misc.SubMenu("hitchances").AddItem(new MenuItem("qhitchance", "Call of The Void (Q)").SetValue(new StringList(new[] { "Medium", "High", "Very High" })));
                Misc.SubMenu("hitchances").AddItem(new MenuItem("whitchance", "Null Zone (W)").SetValue(new StringList(new[] { "Medium", "High", "Very High" })));
                Misc.AddSubMenu(new Menu("Nether Grasp Mode", "nethergraspmode"));
                foreach (var champion in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
                {
                    Misc.SubMenu("nethergraspmode").AddItem(new MenuItem("ng" + champion.ChampionName, "Use Ult On " + champion.ChampionName).SetValue(true));
                }
                if (Infernus != SpellSlot.Unknown)
                    Misc.AddItem(new MenuItem("ignite", "Use Ignite For KS").SetValue(true));
            }
            Z.AddSubMenu(Misc);

            Z.AddItem(new MenuItem("credits1", "Credits:"));
            Z.AddItem(new MenuItem("credits2", "TehBlaxxor - Coding"));

            Z.AddSubMenu(MDrawings);

            Z.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!Z.Item("drawings").GetValue<bool>())
                return;
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (Z.Item("drawtg").GetValue<Circle>().Active && target != null)
            {
                Render.Circle.DrawCircle(target.Position, 75f, Z.Item("drawtg").GetValue<Circle>().Color);
            }
            foreach (var x in Spells.Where(y => Z.Item("draw" + y.Slot.ToString().ToLowerInvariant()).GetValue<Circle>().Active))
            {
                Render.Circle.DrawCircle(Player.Position, x.Range, x.IsReady()
                ? System.Drawing.Color.Green
                : System.Drawing.Color.Red
                );
            }

            if (!Enabled
                || _damageToUnit == null)
            {
                return;
            }

            foreach (var unit in HeroManager.Enemies.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = _damageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width * unit.Health / unit.MaxHealth;

                if (damage > unit.Health)
                {
                    Text.X = (int)barPos.X + XOffset;
                    Text.Y = (int)barPos.Y + YOffset - 13;
                    Text.text = "Killable: " + (unit.Health - damage);
                    Text.OnEndScene();
                }

                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, Color);

                if (Fill)
                {
                    float differenceInHP = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);

                    for (int i = 0; i < differenceInHP; i++)
                    {
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, FillColor);
                    }
                }
            }
        }

        private void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy
                && args.DangerLevel == Interrupter2.DangerLevel.High
                && Z.Item("interruptq").GetValue<bool>()
                && Q.IsReady()
                && sender.IsValidTarget(Q.Range)
                && Player.HealthPercent >= 15)
            {
                Q.CastIfHitchanceEquals(sender, HitChance.Medium);
            }
            else if (sender.IsEnemy
                 && args.DangerLevel == Interrupter2.DangerLevel.High
                 && Z.Item("interruptr").GetValue<bool>()
                 && R.IsReady()
                 && sender.IsValidTarget(R.Range)
                 && !Q.IsReady()
                 && Player.HealthPercent >= 30)
            {
                R.CastOnUnit(sender);
            }
        }

        private void Game_OnUpdate(EventArgs args)
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

            if (Z.Item("combobind").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            else if (Z.Item("harassbind").GetValue<KeyBind>().Active)
            {
                Harass();
            }
            else if (Z.Item("farmbind").GetValue<KeyBind>().Active)
            {
                Farm();
            }
            if (Z.Item("ignite").GetValue<bool>() && Infernus.IsReady())
            {
                Ignite();
            }
        }

        public static void Ignite()
        {
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(600f) && (x.Health / 4) * 3 < Player.GetSummonerSpellDamage(x, Damage.SummonerSpell.Ignite)).OrderBy(x => x.Health))
            {
                Player.Spellbook.CastSpell(Infernus, enemy);
            }
        }

        public static void Combo()
        {
            var ComboModeSelectedIndex = Z.Item("comboMode").GetValue<StringList>().SelectedIndex;
            var QHitChance = Z.Item("qhitchance").GetValue<StringList>().SelectedIndex;
            var WHitChance = Z.Item("whitchance").GetValue<StringList>().SelectedIndex;
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
                    if (Z.Item("ng" + target.ChampionName).GetValue<bool>())
                    {
                        if (Player.UnderTurret() && Z.Item("underturretr").GetValue<bool>())
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
                if (E.IsReady() && target.IsValidTarget(E.Range) && Z.Item("comboE").GetValue<bool>())
                {
                    E.CastOnUnit(target);
                }
                if (Q.IsReady() && target.IsValidTarget(Q.Range) && Z.Item("comboQ").GetValue<bool>())
                {
                    if (QHitChance == 0 || Q.GetDamage(target) > target.Health * 3)
                        Q.Cast(target);
                    else if (QHitChance == 1)
                        Q.CastIfHitchanceEquals(target, HitChance.High);
                    else if (QHitChance == 2)
                        Q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                }
                if (W.IsReady() && target.IsValidTarget(W.Range) && Z.Item("comboW").GetValue<bool>())
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
                    if (Z.Item("comboR").GetValue<bool>() && Z.Item("ng" + target.ChampionName).GetValue<bool>())
                    {
                        if (Player.UnderTurret() && Z.Item("underturretr").GetValue<bool>())
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
            if (Z.Item("comboW").GetValue<bool>() && W.IsReady())
                return false;
            else return true;
        }
        public static bool ComboQ()
        {
            if (Z.Item("comboQ").GetValue<bool>() && Q.IsReady())
                return false;
            else return true;
        }
        public static bool ComboE()
        {
            if (Z.Item("comboE").GetValue<bool>() && E.IsReady())
                return false;
            else return true;
        }

        public static void Harass()
        {
            var QHitChance = Z.Item("qhitchance").GetValue<StringList>().SelectedIndex;
            var WHitChance = Z.Item("whitchance").GetValue<StringList>().SelectedIndex;
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (E.IsReady() && target.IsValidTarget(E.Range) && Z.Item("harassE").GetValue<bool>())
                E.CastOnUnit(target);
            if (Q.IsReady() && target.IsValidTarget(Q.Range) && Z.Item("harassQ").GetValue<bool>())
            {
                if (QHitChance == 0)
                    Q.Cast(target);
                if (QHitChance == 1)
                    Q.CastIfHitchanceEquals(target, HitChance.High);
                if (QHitChance == 2)
                    Q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
            }
            if (W.IsReady() && target.IsValidTarget(W.Range) && Z.Item("harassW").GetValue<bool>())
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
            if (E.IsReady() && Z.Item("farme").GetValue<bool>())
            {
                E.CastOnUnit(minion);
            }
            if (Q.IsReady() && Z.Item("farmq").GetValue<bool>())
            {
                Q.Cast(QFarmLocation.Position);
            }
            if (W.IsReady() && Z.Item("farmw").GetValue<bool>())
            {
                W.Cast(WFarmLocation.Position);
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

    }
}
