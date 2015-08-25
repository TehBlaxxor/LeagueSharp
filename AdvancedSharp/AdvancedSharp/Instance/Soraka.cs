using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;

namespace AdvancedSharp.Instance
{
    internal class Soraka
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Menu Z;
        public static Orbwalking.Orbwalker Orbwalker;
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        public static Items.Item mikael = new Items.Item(3222);

        public static void OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Soraka")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 950);
            W = new Spell(SpellSlot.W, 550);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.5f, 300, 1750, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.5f, 70f, 1750, false, SkillshotType.SkillshotCircle);

            Z = new Menu("Adv# - Soraka", "root", true);
            Menu MOrb = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(MOrb);
            Z.AddSubMenu(MOrb);

            // Target Selector
            var tsMenu = new Menu("Target Selector", "ssTS");
            TargetSelector.AddToMenu(tsMenu);
            Z.AddSubMenu(tsMenu);


            // Combo
            var comboMenu = new Menu("Combo", "ssCombo");
            comboMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            Z.AddSubMenu(comboMenu);

            // Harass
            var harassMenu = new Menu("Harass", "ssHarass");
            harassMenu.AddItem(new MenuItem("useQHarass", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("useEHarass", "Use E").SetValue(true));
            Z.AddSubMenu(harassMenu);

            // Lane Clear
            var clearMenu = new Menu("Lane Clear", "ssClear");
            clearMenu.AddItem(new MenuItem("useQLane", "Use Q").SetValue(true));
            clearMenu.AddItem(new MenuItem("lanepercent", "% to lane clear").SetValue(new Slider(15)));


            // Drawing
            var drawingMenu = new Menu("Drawing", "ssDrawing");
            drawingMenu.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(true));
            drawingMenu.AddItem(new MenuItem("drawW", "Draw W").SetValue(true));
            drawingMenu.AddItem(new MenuItem("drawE", "Draw E").SetValue(true));
            Z.AddSubMenu(drawingMenu);

            // Misc
            var miscMenu = new Menu("Misc", "ssMisc");
            miscMenu.AddItem(new MenuItem("useQGapcloser", "Q on Gapcloser").SetValue(true));
            miscMenu.AddItem(new MenuItem("useEGapcloser", "E on Gapcloser").SetValue(true));
            miscMenu.AddItem(new MenuItem("useEdashcloser", "E on Dash").SetValue(true));
            miscMenu.AddItem(new MenuItem("autoR", "Auto use R").SetValue(true));
            miscMenu.AddItem(new MenuItem("autoRPercent", "% to R").SetValue(new Slider(15)));
            miscMenu.AddItem(new MenuItem("eInterrupt", "Use E to Interrupt").SetValue(true));
            Z.AddSubMenu(miscMenu);

            foreach (var hero in HeroManager.Allies)
            {
                if (!hero.IsMe)
                {
                    Z.AddItem(new MenuItem("healop" + hero.ChampionName, hero.ChampionName))
                        .SetValue(new StringList(new[] {"Heal", "Don't Heal"}, 1));

                    Z.AddItem(new MenuItem("hpsettings" + hero.ChampionName, "% Hp to").SetValue(new Slider(20)));

                    Z.AddItem(new MenuItem("hprest", "Min %HP To heal").SetValue(new Slider(20)));
                }
            }

            Z.AddItem(new MenuItem("credits1", "Credits:"));
            Z.AddItem(new MenuItem("credits3", "Hoes - Coding"));

            Z.AddToMainMenu();


            Interrupter2.OnInterruptableTarget += InterrupterOnOnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserOnOnEnemyGapcloser;
            Game.OnUpdate += GameOnOnGameUpdate;
            Drawing.OnDraw += DrawingOnOnDraw;
            CustomEvents.Unit.OnDash += Unit_OnDash;
        }

        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var qSpell = Z.Item("useEdashcloser").GetValue<bool>();

            if (!sender.IsEnemy)
            {
                return;
            }

            if (sender.NetworkId == target.NetworkId)
            {
                if (qSpell)
                {

                    if (Q.IsReady()
                        && args.EndPos.Distance(Player) < Q.Range)
                    {
                        var delay = (int) (args.EndTick - Game.Time - Q.Delay - 0.1f);
                        if (delay > 0)
                        {
                            Utility.DelayAction.Add(delay*1000, () => Q.Cast(args.EndPos));
                        }
                        else
                        {
                            Q.Cast(args.EndPos);
                        }
                        if (Q.IsReady()
                            && args.EndPos.Distance(Player) < Q.Range)
                        {
                            if (delay > 0)
                            {
                                Utility.DelayAction.Add(delay*1000, () => Q.Cast(args.EndPos));
                            }
                            else
                            {
                                E.Cast(args.EndPos);
                            }
                        }
                    }
                }
            }
        }


        private static
            void DrawingOnOnDraw(EventArgs args)
        {
            var drawQ = Z.Item("drawQ").GetValue<bool>();
            var drawW = Z.Item("drawW").GetValue<bool>();
            var drawE = Z.Item("drawE").GetValue<bool>();

            var p = ObjectManager.Player.Position;

            if (drawQ)
            {
                Render.Circle.DrawCircle(p, Q.Range, Q.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawW)
            {
                Render.Circle.DrawCircle(p, W.Range, W.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawE)
            {
                Render.Circle.DrawCircle(p, E.Range, E.IsReady() ? Color.Aqua : Color.Red);
            }
        }

        private static void GameOnOnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
            }
            Mikaels();
            if (Z.Item("autoW").GetValue<bool>())
            {
                AutoW();
            }

            if (Z.Item("autoR").GetValue<bool>())
            {
                AutoR();
            }
        }

        static double UnitIsImmobileUntil(Obj_AI_Base unit)
        {
            var time =
                unit.Buffs.Where(buff => buff.IsActive && Game.Time <= buff.EndTime &&
                        (buff.Type == BuffType.Charm
                        || buff.Type == BuffType.Knockup
                        || buff.Type == BuffType.Suppression
                        || buff.Type == BuffType.Stun
                        || buff.Type == BuffType.Snare)).Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return (time - Game.Time);
        }

        private static void Mikaels()
        {
            foreach (var friend in
                from friend in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => !x.IsEnemy)
                        .Where(x => !x.IsMe).Where(friend => W.IsInRange(friend.ServerPosition, W.Range))
                select friend)
            {

                if (UnitIsImmobileUntil(friend) > 0.5f
                    && mikael.IsReady())
                {
                    mikael.Cast(friend);
                }
            }
        }
        private static void AutoR()
        {
            if (!R.IsReady())
            {
                return;
            }

            foreach (var friend in
                ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly).Where(x => !x.IsDead).Where(x => !x.IsZombie))
            {
                var health = Z.Item("autoRPercent").GetValue<Slider>().Value;

                if (friend.HealthPercent
                    <= health)
                {
                    R.Cast();
                }
            }
        }
        private static void AutoW()
        {
            if (!W.IsReady())
            {
                return;
            }

            if (Player.HealthPercent < Z.Item("hprest").GetValue<Slider>().Value)
                return;

            foreach (var hero in HeroManager.Allies)
            {
                if (Z.Item("healop" + hero.ChampionName).GetValue<StringList>().SelectedIndex == 0
                    && hero.HealthPercent <= Z.Item("hpsettings" + hero.ChampionName).GetValue<Slider>().Value)
                {
                    W.CastOnUnit(hero);
                }
            }
        }
        private static void Combo()
        {
            var useQ = Z.Item("useQ").GetValue<bool>();
            var useE = Z.Item("useE").GetValue<bool>();
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            if (useQ 
                && Q.IsReady())
            {
                Q.Cast(target);
            }

            if (useE 
                && E.IsReady())
            {
                E.Cast(target);
            }
        }
        private static void Harass()
        {
            var useQ = Z.Item("useQHarass").GetValue<bool>();
            var useE = Z.Item("useEHarass").GetValue<bool>();
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target == null)
            {
                return;
            }

            if (useQ && Q.IsReady())
            {
                Q.Cast(target);
            }

            if (useE && E.IsReady())
            {
                E.Cast(target);
            }
        }
        private static void AntiGapcloserOnOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var unit = gapcloser.Sender;

            if (Z.Item("useQGapcloser").GetValue<bool>() && unit.IsValidTarget(Q.Range) && Q.IsReady())
            {
                Q.Cast(unit);
            }

            if (Z.Item("useEGapcloser").GetValue<bool>() && unit.IsValidTarget(E.Range) && E.IsReady())
            {
                E.Cast(unit);
            }
        }

        private static void InterrupterOnOnPossibleToInterrupt(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Z.Item("eInterrupt").GetValue<bool>()
                || !sender.IsValidTarget(E.Range)
                || !E.IsReady())
                return;

            {
                E.Cast(sender);
            }
        }

        private static void LaneClear()
        {
            var qSpell = Z.Item("useQLane").GetValue<bool>();
            var minMana = Z.Item("lanepercent").GetValue<Slider>().Value;


            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (Player.ManaPercent <= minMana)
                return;
            foreach (var minion in minionCount)
            {
                if (qSpell
                    && Q.IsReady()
                    && minion.IsValidTarget(Q.Range))
                {
                    Q.Cast(minion);
                }                
            }
        }
            
    }
}
