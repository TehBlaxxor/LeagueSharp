#region
using System;
using System.Collections;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using System.Collections.Generic;
using System.Threading;
#endregion

namespace TopOrAFK.Champions
{
    internal class Aatrox
    {
        private const string Champion = "Aatrox";
        private static Orbwalking.Orbwalker Orbwalker;
        private static List<Spell> SpellList = new List<Spell>();
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static Menu Config;
        private static Items.Item item;

        private static Obj_AI_Hero Player;

        public Aatrox()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.BaseSkinName != Champion) return;

            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, Player.AttackRange);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R, 350);

            Q.SetSkillshot(Q.Instance.SData.SpellCastTime, 250f, 2000f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(Q.Instance.SData.SpellCastTime, 35f, 1250f, false, SkillshotType.SkillshotLine);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Config = new Menu(Champion, "Aatrox", true);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQECombo", "Use QE when needed")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("ActiveCombo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Items", "Items"));
            Config.SubMenu("Items").AddItem(new MenuItem("BOTRK", "Use BotRK")).SetValue(true);
            Config.SubMenu("Items").AddItem(new MenuItem("OMEN", "Use Randuin's Omen")).SetValue(true);
            Config.SubMenu("Items").AddItem(new MenuItem("GBLADE", "Use Youumu's Ghostblade")).SetValue(true);
            Config.SubMenu("Items").AddItem(new MenuItem("useItems", "Use Items")).SetValue(true);

            Config.AddSubMenu(new Menu("Blood Thrist / Price", "BloodThirst"));
            Config.SubMenu("BloodThirst").AddItem(new MenuItem("switchW", "Automatically W")).SetValue(true);
            Config.SubMenu("BloodThirst").AddItem(new MenuItem("useWHealth", "W if HP % < or >").SetValue(new Slider(35, 20, 100)));

            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("HarassE", "Use E")).SetValue(true);
            Config.SubMenu("Harass").AddItem(new MenuItem("HarassTg", "Toggle Harass").SetValue<KeyBind>(new KeyBind('T', KeyBindType.Toggle)));
            Config.SubMenu("Harass").AddItem(new MenuItem("ActiveHarass", "Harass").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Jungle Clear", "Jungle"));
            Config.SubMenu("Jungle").AddItem(new MenuItem("UseQClear", "Use Q")).SetValue(true);
            Config.SubMenu("Jungle").AddItem(new MenuItem("UseEClear", "Use E")).SetValue(true);
            Config.SubMenu("Jungle").AddItem(new MenuItem("ActiveClear", "Jungle").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Lane Clear", "Wave"));
            Config.SubMenu("Wave").AddItem(new MenuItem("UseQWave", "Use Q")).SetValue(true);
            Config.SubMenu("Wave").AddItem(new MenuItem("QMode", "Q to Mouse")).SetValue(true);
            Config.SubMenu("Wave").AddItem(new MenuItem("UseEWave", "Use E")).SetValue(true);
            Config.SubMenu("Wave").AddItem(new MenuItem("ActiveWave", "Lane Clear").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("KSq", "Killsteal Q").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("KSe", "Killsteal E").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("KSr", "Killsteal R").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("KS", "Kill Steal").SetValue(true));

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "Draw Q")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawE", "Draw E")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawR", "Draw R")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawAA", "Draw AA Range")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleLag", "Lag Free Circles").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleQuality", "Circles Quality").SetValue(new Slider(100, 100, 10)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleThickness", "Circles Thickness").SetValue(new Slider(1, 10, 1)));

            Config.AddToMainMenu();

            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;


        }


        private static bool CheckFor(Spell spell, Obj_AI_Base target, float spellrange, string menuitem)
        {
            if (spell.IsReady() && target.IsValidTarget(spellrange) && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuff("UndyingRage") && !target.IsDead && Config.Item(menuitem).GetValue<bool>() == true)
            {
                return true;
            }
            else return false;
        }

        private static void WControl()
        {
            if (Config.Item("switchW").GetValue<bool>())
            {
                if (Player.Health > (Player.MaxHealth * (Config.Item("useWHealth").GetValue<Slider>().Value / 100f)) &&
                    W.Instance.SData.Name.ToLowerInvariant() == "aatroxw")
                {
                    W.Cast();
                }
                else if (Player.Health < (Player.MaxHealth * (Config.Item("useWHealth").GetValue<Slider>().Value / 100f)) &&
                         W.Instance.SData.Name.ToLowerInvariant() == "aatroxw2")
                {
                    W.Cast();
                }
            }
        }

        private static void OnGameUpdate(EventArgs args)
        {
            {
                if (Player.IsDead) return;

                Player = ObjectManager.Player;

                Orbwalker.SetAttack(true);

                if (Config.Item("switchW").GetValue<bool>() == true)
                {
                    WControl();
                }

                if (Config.Item("ActiveCombo").GetValue<KeyBind>().Active)
                {
                    Combo();
                    UseItems();
                }
                if (Config.Item("ActiveClear").GetValue<KeyBind>().Active)
                {
                    JungleClear();
                }
                if (Config.Item("ActiveWave").GetValue<KeyBind>().Active)
                {
                    LaneClear();
                }
                if (Config.Item("ActiveHarass").GetValue<KeyBind>().Active)
                {
                    Harass();
                }
                if (Config.Item("HarassTg").GetValue<KeyBind>().Active)
                {
                    HarassTg();
                }

                if (Config.Item("KS").GetValue<bool>())
                {
                    Killsteal();
                }
            }
        }

        private static void UseItems()
        {
            if (Config.Item("useItems").GetValue<bool>())
            {
                if (Config.Item("BOTRK").GetValue<bool>())
                {
                    var botrk = Items.HasItem((int)ItemId.Blade_of_the_Ruined_King);
                    var cutlass = Items.HasItem((int)ItemId.Bilgewater_Cutlass);
                    var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                    
                    if (botrk || cutlass)
                    {
                        if (botrk)
                        {
                            Items.UseItem((int)ItemId.Blade_of_the_Ruined_King, target);
                        }
                        else
                        {
                            Items.UseItem((int)ItemId.Bilgewater_Cutlass, target);
                        }
                    }
                }

                if (Config.Item("OMEN").GetValue<bool>())
                {
                    var omen = Items.HasItem((int)ItemId.Randuins_Omen);

                    if (omen)
                    {
                        var champcount = 0;

                        foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy && !x.IsDead && x.IsValidTarget(R.Range)))
                        {
                            champcount++;
                        }

                        if (champcount >= 1)
                        {
                            if (Items.CanUseItem((int)ItemId.Randuins_Omen))
                            {
                                Items.UseItem((int)ItemId.Randuins_Omen);
                            }
                        }
                    }
                }

                if (Config.Item("GBLADE").GetValue<bool>())
                {
                    var gblade = Items.HasItem((int)ItemId.Youmuus_Ghostblade);

                    if (gblade)
                    {
                        if (Items.CanUseItem((int)ItemId.Youmuus_Ghostblade))
                        {
                            Items.UseItem((int)ItemId.Youmuus_Ghostblade);
                        }
                    }
                }
            }
        }

        private static void Killsteal()
        {
            foreach (
                var enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(enemy => enemy.Team != Player.Team)
                        .Where(
                            enemy =>
                                !enemy.HasBuff("UndyingRage") && !enemy.HasBuff("JudicatorIntervention") && !enemy.HasBuffOfType(BuffType.Invulnerability))
                )
            {
                if (enemy.Health + 20 < Q.GetDamage(enemy) && Q.IsReady() && enemy.IsValidTarget(Q.Range) && Config.Item("KSq").GetValue<bool>() == true)
                {
                    Q.Cast(enemy);
                }

                if (enemy.Health + 20 < E.GetDamage(enemy) && E.IsReady() && enemy.IsValidTarget(E.Range) && Config.Item("KSe").GetValue<bool>() == true)
                {
                    E.Cast(enemy);
                }

                if (enemy.HealthPercentage() < 5 && R.IsReady() && enemy.IsValidTarget(R.Range) && Config.Item("KSr").GetValue<bool>() == true)
                {
                    R.Cast();
                }

            }
        }

        private static void HarassTg()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (CheckFor(E, target, E.Range, "HarassE") && !target.HasBuff("UndyingRage"))
            {
                E.Cast(target);
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (CheckFor(E, target, E.Range, "HarassE") && !target.HasBuff("UndyingRage"))
            {
                E.Cast(target);
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (target.HasBuff("UndyingRage")) return;

            if (Config.Item("UseQECombo").GetValue<bool>() == true && Player.Distance(target.Position) < E.Range + 590 && !target.IsValidTarget(E.Range + 100))
            {
                if (Q.IsReady() && !target.IsDead && Config.Item("UseQCombo").GetValue<bool>() == true)
                { Q.Cast(target); }

                if (CheckFor(E, target, E.Range, "UseECombo"))
                { E.Cast(target); }
            }

            if (target.IsValidTarget(Q.Range))
            {
                if (CheckFor(Q, target, Q.Range, "UseQCombo"))
                { Q.Cast(target); }

                if (CheckFor(E, target, E.Range, "UseECombo"))
                { E.Cast(target); }
            }
        }

        private static void LaneClear()
        { 
            if (Config.Item("QMode").GetValue<bool>() == true)
            {
                var minions = MinionManager.GetMinions(Game.CursorPos, 650, MinionTypes.All, MinionTeam.Enemy);

                if (minions.Count > 3)
                {
                    if (Player.Distance(minions[0].Position) < Q.Range)
                    {
                        Q.Cast(Game.CursorPos);
                    }
                }
            }
            foreach (
                    var enemy in
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(enemy => enemy.Team != Player.Team)
                    )
                {
                    if (enemy.Health + 20 < Q.GetDamage(enemy) && Q.IsReady() && enemy.IsValidTarget(Q.Range) && Config.Item("UseQWave").GetValue<bool>() == true)
                    {
                        Q.Cast(enemy);
                    }

                    if (enemy.Health + 20 < E.GetDamage(enemy) && E.IsReady() && enemy.IsValidTarget(E.Range) && Config.Item("UseEWave").GetValue<bool>() == true)
                    {
                        E.Cast(enemy);
                    }
                }
            }

        private static void JungleClear()
        {
            var junglenoobs = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var UseQClear = Config.Item("UseQClear").GetValue<bool>();
            var UseEClear = Config.Item("UseEClear").GetValue<bool>();

            if (junglenoobs.Count > 0 && Player.Distance(junglenoobs[0].Position) < Q.Range)
            {
                if (UseQClear && Q.IsReady() && junglenoobs[0].IsValidTarget() && Player.Distance(junglenoobs[0].Position) <= Q.Range)
                {
                    Q.Cast(junglenoobs[0].Position);
                }

                if (UseEClear && E.IsReady() && junglenoobs[0].IsValidTarget() && Player.Distance(junglenoobs[0].Position) <= E.Range)
                {
                    E.Cast(junglenoobs[0].Position);
                }

            }



        }

        private static void OnDraw(EventArgs args)
        {
            if (Config.Item("CircleLag").GetValue<bool>())
            {
                if (Config.Item("DrawQ").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.White,
                        Config.Item("CircleThickness").GetValue<Slider>().Value);
                }

                if (Config.Item("DrawE").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.White,
                        Config.Item("CircleThickness").GetValue<Slider>().Value);
                }

                if (Config.Item("DrawR").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.White,
                        Config.Item("CircleThickness").GetValue<Slider>().Value);
                }

                if (Config.Item("DrawAA").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.White,
                        Config.Item("CircleThickness").GetValue<Slider>().Value);
                }
            }
            else
            {
                if (Config.Item("DrawQ").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.White);
                }
                if (Config.Item("DrawAA").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.White);
                }
                if (Config.Item("DrawE").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.White);
                }
                if (Config.Item("DrawR").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.White);
                }

            }
        }
    }
}