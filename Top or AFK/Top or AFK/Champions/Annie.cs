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
    internal class Annie
    {
        private const string Champion = "Annie";
        private static Orbwalking.Orbwalker Orbwalker;
        private static List<Spell> SpellList = new List<Spell>();
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static Menu Config;

        private static Obj_AI_Hero Player;

        public Annie()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.BaseSkinName != Champion) return;

            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, 625);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 600);

            Q.SetTargetted(0.25f, 1400);
            W.SetSkillshot(0.6f, (float)(50 * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.25f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Config = new Menu(Champion, "Annie", true);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("Rrestrict2", "Use R only if stuns")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("Rrestrict", "Use R if stuns X people in target's range: ")).SetValue(new Slider(1,1,5));
            Config.SubMenu("Combo").AddItem(new MenuItem("ActiveCombo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("HarrasQ", "Use Q")).SetValue(true);
            Config.SubMenu("Harass").AddItem(new MenuItem("HarrasW", "Use W")).SetValue(true);
            Config.SubMenu("Harass").AddItem(new MenuItem("CountHarass", "Don't Harass if HP < %").SetValue(new Slider(20, 0, 100)));
            Config.SubMenu("Harass").AddItem(new MenuItem("HarassTg", "Toggle Harass").SetValue<KeyBind>(new KeyBind('T', KeyBindType.Toggle)));
            Config.SubMenu("Harass").AddItem(new MenuItem("ActiveHarass", "Harass").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Lane Clear", "Wave"));
            Config.SubMenu("Wave").AddItem(new MenuItem("UseQWave", "Use Q")).SetValue(true);
            Config.SubMenu("Wave").AddItem(new MenuItem("UseWWave", "Use W")).SetValue(true);
            Config.SubMenu("Wave").AddItem(new MenuItem("ActiveWave", "WaveClear Key").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Passive", "Passive"));
            Config.SubMenu("Passive").AddItem(new MenuItem("passiveQ", "Auto Stack Passive w/ Q").SetValue(false));
            Config.SubMenu("Passive").AddItem(new MenuItem("passiveW", "Auto Stack Passive w/ W").SetValue(false));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("KSq", "Killsteal Q").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("KSe", "Killsteal E").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("KSr", "Killsteal R").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("KS", "Kill Steal")).SetValue(true);
            Config.SubMenu("Misc").AddItem(new MenuItem("autoE", "Auto E")).SetValue(true);
            Config.SubMenu("Misc").AddItem(new MenuItem("Disrespect", "Disrespect")).SetValue(true);

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "Draw Q")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawW", "Draw W")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawR", "Draw R")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleLag", "Lag Free Circles").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleQuality", "Circles Quality").SetValue(new Slider(100, 100, 10)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleThickness", "Circles Thickness").SetValue(new Slider(1, 10, 1)));

            Config.AddToMainMenu();

            Game.OnGameNotifyEvent += Game_OnGameNotifyEvent;
            GameObject.OnCreate += GameObject_OnCreate;
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;


        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (Config.Item("UseRCombo").GetValue<bool>())
            {
                if (Config.Item("Rrestrict2").GetValue<bool>() && HasStun())
                {
                    if (target.IsValidTarget(R.Range) && R.IsReady() && target.CountEnemiesInRange(200f) >= Config.Item("Rrestrict").GetValue<Slider>().Value)
                    {
                        R.Cast(target);
                    }
                }

                if (!Config.Item("Rrestrict2").GetValue<bool>())
                {
                    if (target.IsValidTarget(R.Range) && R.IsReady())
                    {
                        R.Cast(target);
                    }
                }

            }

            if (Config.Item("UseWCombo").GetValue<bool>())
            {
                if (target.IsValidTarget(W.Range - 25) && W.IsReady())
                {
                    W.Cast(target);
                }

            }

            if (Config.Item("UseQCombo").GetValue<bool>())
            {
                if (target.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    Q.Cast(target);
                }

            }

        }

        private static bool CheckFor(Spell spell, Obj_AI_Base target, float spellrange, string menuitem)
        {
            if (spell.IsReady() && target.IsValidTarget(spellrange) && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuff("UndyingRage") && !target.IsDead && Config.Item(menuitem).GetValue<bool>() == true)
            {
                return true;
            }
            else return false;
        }

        public static bool HasStun()
        {
            var passive = Player.Buffs.Where(buff => (buff.Name.ToLower() == "pyromania" || buff.Name.ToLower() == "pyromania_particle"));
            if (passive.Any())
            {
                var pascheck = passive.First();
                if (pascheck.Name.ToLower() == "pyromania_particle")
                { return true; }
                else
                { return false; }
            }
            else return false;
        }

        private static int GetEnemiesAround(Obj_AI_Base unit, float range)
        {
            return unit.CountEnemiesInRange(range);
        }

        private static void OnGameUpdate(EventArgs args)
        {
            {
                if (Player.IsDead) return;
                if (Player.IsRecalling()) return;
                Player = ObjectManager.Player;

                Orbwalker.SetAttack(true);

                if (Config.Item("ActiveCombo").GetValue<KeyBind>().Active)
                {
                    Combo();
                }

                if (Config.Item("passiveQ").GetValue<bool>() == true)
                {
                    pasQstack();
                }

                if (Config.Item("passiveW").GetValue<bool>() == true)
                {
                    pasWstack();
                }

                if (Config.Item("ActiveWave").GetValue<KeyBind>().Active)
                {
                    WaveClear();
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

        private static void pasQstack()
        {
            var minions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);

            if (Q.GetDamage(minions[0]) > minions[0].Health + 20 && !HasStun())
            {
                Q.Cast(minions[0]);
            }
        }

        private static void pasWstack()
        {
            var minions = MinionManager.GetMinions(Player.ServerPosition, W.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);

            if (W.GetDamage(minions[0]) > minions[0].Health + 20 && !HasStun())
            {
                W.Cast(minions[0]);
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
                if (enemy.Health + 20 < W.GetDamage(enemy) && W.IsReady() && enemy.IsValidTarget(W.Range) && Config.Item("KSw").GetValue<bool>() == true)
                {
                    W.Cast(enemy);
                }

                if (enemy.Health + 20 < Q.GetDamage(enemy) && Q.IsReady() && enemy.IsValidTarget(Q.Range) && Config.Item("KSq").GetValue<bool>() == true)
                {
                    Q.Cast(enemy);
                }

                if (enemy.Health + 20 < R.GetDamage(enemy) && R.IsReady() && enemy.IsValidTarget(R.Range) && Config.Item("KSr").GetValue<bool>() == true)
                {
                    R.Cast();
                }

            }
        }

        private static void HarassTg()
        {
            if (Player.HealthPercentage() > Config.Item("CountHarass").GetValue<Slider>().Value)
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                if (CheckFor(Q, target, Q.Range, "HarassQ") && !target.HasBuff("UndyingRage"))
                {
                    Q.Cast(target);
                }

                if (CheckFor(W, target, W.Range, "HarassW") && !target.HasBuff("UndyingRage"))
                {
                    W.Cast(target);
                }
            }
        }

        private static void Harass()
        {
            if (Player.HealthPercentage() > Config.Item("CountHarass").GetValue<Slider>().Value)
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                if (CheckFor(Q, target, Q.Range, "HarassQ") && !target.HasBuff("UndyingRage"))
                {
                    Q.Cast(target);
                }

                if (CheckFor(W, target, W.Range, "HarassW") && !target.HasBuff("UndyingRage"))
                {
                    W.Cast(target);
                }
            }
        }

        static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var UseEAgainstAA = Config.Item("autoE").GetValue<bool>();

            if (UseEAgainstAA && E.IsReady() && sender is Obj_SpellMissile)
            {
                var missile = sender as Obj_SpellMissile;
                if (missile.SpellCaster is Obj_AI_Hero && missile.SpellCaster.IsEnemy && missile.Target.IsMe)
                    E.Cast();
            }
        }


        private static void WaveClear()
        {
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

                if (enemy.Health + 20 < W.GetDamage(enemy) && W.IsReady() && enemy.IsValidTarget(W.Range) && Config.Item("UseWWave").GetValue<bool>() == true)
                {
                    W.Cast(enemy);
                }
            }
        }

        private static void Game_OnGameNotifyEvent(GameNotifyEventArgs args)
        {
            if (args.NetworkId != ObjectManager.Player.NetworkId)
            {
                return;
            }

            if (Config.Item("Disrespect").GetValue<bool>())
            {
                switch (args.EventId)
                {
                    case GameEventId.OnFirstBlood:
                        Game.Say("/laugh");
                        break;

                    case GameEventId.OnKill:
                        Game.Say("/laugh");
                        break;

                    case GameEventId.OnKillWard:
                        Game.Say("/laugh");
                        break;
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

                if (Config.Item("DrawW").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.White,
                        Config.Item("CircleThickness").GetValue<Slider>().Value);
                }

                if (Config.Item("DrawR").GetValue<bool>())
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.White,
                        Config.Item("CircleThickness").GetValue<Slider>().Value);
                }

            }
            else
            {
                if (Config.Item("DrawQ").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.White);
                }
                if (Config.Item("DrawW").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.White);
                }
                if (Config.Item("DrawR").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.White);
                }

            }
        }
    }
}