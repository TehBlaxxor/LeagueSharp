using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using Color = System.Drawing.Color;

namespace Irelia
{
    class Program
    {
        private static string Champion = "Irelia";
        private static Obj_AI_Hero Player = ObjectManager.Player;
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static List<Spell> SpellList = new List<Spell>();
        private static Menu Config;
        private static Orbwalking.Orbwalker Orbwalker;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserOnEnemyGapcloser;

        }

        private static void AntiGapcloserOnEnemyGapcloser(ActiveGapcloser gc)
        {
            if (gc.Sender.IsValidTarget() && Config.Item("gapcloserE").GetValue<bool>() && E.IsReady())
            {
                E.Cast(gc.Sender);
            }
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != Champion) return;
            Game.PrintChat("<font color=\'#ff5500\'>" + Champion + "</font><font color=\'#00aa00\'> by TehBlaxxor successfully loaded!</font>");
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, Player.AttackRange);
            E = new Spell(SpellSlot.E, 425);
            R = new Spell(SpellSlot.R, 1000);

            R.SetSkillshot(250f, 65, 1600, false, SkillshotType.SkillshotLine);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Config = new Menu("Irelia", "Irelia", true);
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);

            Config.AddSubMenu(targetSelectorMenu);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("comboQ", "Use Q")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("comboW", "Use W")).SetValue(false);
            Config.SubMenu("Combo").AddItem(new MenuItem("comboE", "Use E")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("comboR", "Use R")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("comboRrestrict", "Use R if < 50%")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("comboErestrict", "Use E if stun")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Lane Clear", "Lane Clear"));
            Config.SubMenu("Escape/Chase").AddItem(new MenuItem("laneQ", "Use Q")).SetValue(true);
            Config.SubMenu("Escape/Chase").AddItem(new MenuItem("laneE", "Use E")).SetValue(true);
            Config.SubMenu("Escape/Chase").AddItem(new MenuItem("Lane Clear", "Lane Clear").SetValue(new KeyBind(86, KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Chase", "Chase"));
            Config.SubMenu("Escape/Chase").AddItem(new MenuItem("chaseQ", "Use Q")).SetValue(true);
            Config.SubMenu("Escape/Chase").AddItem(new MenuItem("chaseE", "Use E")).SetValue(true);
            Config.SubMenu("Escape/Chase").AddItem(new MenuItem("Chase", "Chase").SetValue(new KeyBind(71, KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Escape/Chase").AddItem(new MenuItem("harE", "Auto Harras w/ E")).SetValue(true);

            Config.AddSubMenu(new Menu("Gapcloser", "Gapcloser"));
            Config.SubMenu("Escape/Chase").AddItem(new MenuItem("gapcloserE", "Gapclose with E")).SetValue(true);


            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("Packets", "Packets")).SetValue(true);
            Config.SubMenu("Misc").AddItem(new MenuItem("Auto R if HP < 25%", "Auto R if HP < 25%").SetValue(false));
 
            Config.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("ksQ", "Using Q")).SetValue(true);
            Config.SubMenu("KillSteal").AddItem(new MenuItem("ksE", "Using E")).SetValue(true);
            Config.SubMenu("KillSteal").AddItem(new MenuItem("ksR", "Using R")).SetValue(true);
            Config.SubMenu("KillSteal").AddItem(new MenuItem("Steal Kills", "Steal Kills")).SetValue(false);

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("Enable Drawings", "Enable Drawings").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "Draw Q")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawE", "Draw E")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawR", "Draw R")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("AArange", "Draw AA")).SetValue(true);

            Config.AddToMainMenu();

        }

        public static void minionGapclose(Obj_AI_Hero target)
        {
            if (Player.Position.Distance(target.Position) < E.Range || !Q.IsReady()) return;
            var minion1 =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(a => a.IsValidTarget(Q.Range) && a.IsEnemy && Q.GetDamage(a) > a.Health && a.Position.Distance(target.Position) < a.Position.Distance(Player.Position))
                    .OrderBy(a => a.Position.Distance(target.Position)).FirstOrDefault();
            Q.Cast(minion1);
        }

        public static void GTarget(Spell spell, TargetSelector.DamageType dmgtype)
        {
            TargetSelector.GetTarget(spell.Range, dmgtype);
        }

        public static float GetHPPercentage(Obj_AI_Hero hero)
        {
            return hero.Health / hero.MaxHealth * 100;
        }

        public static void Combo()
        {

            var packets = Config.Item("Packets").GetValue<bool>();
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (target.HasBuffOfType(BuffType.Invulnerability)) return;

            minionGapclose(target);

            if (Config.Item("comboErestrict").GetValue<bool>() == true)
            {
                if (E.IsReady() && GetHPPercentage(Player) < GetHPPercentage(target) && target.IsValidTarget(E.Range) && Config.Item("comboE").GetValue<bool>() == true)
                {
                    E.Cast(target, packets);
                }
            }
            else if (Config.Item("comboErestrict").GetValue<bool>() == false)
            {
                if (E.IsReady() && target.IsValidTarget(E.Range) && Config.Item("comboE").GetValue<bool>() == true)
                {
                    E.Cast(target, packets);
                }
            }
            if (W.IsReady() && target.IsValidTarget(Player.AttackRange) && Config.Item("comboW").GetValue<bool>() == true)
            {
                W.Cast();
            }

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && Config.Item("comboQ").GetValue<bool>() == true)
            {
                Q.Cast(target, packets);
            }

            if (Config.Item("comboRrestrict").GetValue<bool>() == true)
            {
                if (R.IsReady() && GetHPPercentage(Player) < 25 && target.IsValidTarget(R.Range) && Config.Item("comboR").GetValue<bool>() == true)
                {
                    R.Cast(target, packets);
                }
            }
            else if (Config.Item("comboRrestrict").GetValue<bool>() == false)
            {
                if (R.IsReady() && target.IsValidTarget(R.Range) && Config.Item("comboR").GetValue<bool>() == true)
                {
                    R.Cast(target, packets);
                }
            }

            
        }

        public static void Farm()
        {
            foreach (
                var minion in
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(minion => minion.Team != Player.Team)
                        .Where(minion => minion.IsValidTarget(Q.Range))
                )
            {
                if (minion.Health + 15 < Q.GetDamage(minion) && minion.IsValidTarget(Q.Range) && Config.Item("laneQ").GetValue<bool>() == true)
                {
                    Q.Cast(minion);
                }

                if (minion.Health + 15 < E.GetDamage(minion) && minion.IsValidTarget(E.Range) && Config.Item("laneE").GetValue<bool>() == true)
                {
                    E.Cast(minion);
                }
            }
        }

        public static void Chase()
        {
            var packets = Config.Item("Packets").GetValue<bool>();
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (target.HasBuffOfType(BuffType.Invulnerability)) return;

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && Config.Item("comboQ").GetValue<bool>() == true)
            {
                Q.Cast(target, packets);
            }

            if (E.IsReady() && target.IsValidTarget(E.Range) && !target.HasBuffOfType(BuffType.SpellImmunity) && !target.HasBuffOfType(BuffType.SpellShield) && Config.Item("comboE").GetValue<bool>() == true)
            {
                E.Cast(target, packets);
            }
        }

        public static void KS()
        {
            var packets = Config.Item("Packets").GetValue<bool>();

            foreach (
                var enemy in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(enemy => enemy.Team != Player.Team)
                        .Where(
                            enemy =>
                                !enemy.HasBuff("UndyingRage") && !enemy.HasBuff("JudicatorIntervention") && !enemy.HasBuffOfType(BuffType.Invulnerability) &&
                                enemy.IsValidTarget(R.Range))
                )
            {
                if (enemy.Health + 20 < Q.GetDamage(enemy) && Q.IsReady() && enemy.IsValidTarget(Q.Range) && Config.Item("ksQ").GetValue<bool>() == true)
                {
                    Q.Cast(enemy, packets);
                }

                if (enemy.Health + 20 < E.GetDamage(enemy) && E.IsReady() && enemy.IsValidTarget(E.Range) && Config.Item("ksE").GetValue<bool>() == true)
                {
                    E.Cast(enemy, packets);
                }

                if (enemy.Health + 20 < R.GetDamage(enemy) && R.IsReady() && enemy.IsValidTarget(R.Range) && Config.Item("ksR").GetValue<bool>() == true)
                {
                    R.Cast(enemy, packets);
                }
            }
        }

        public static void AutoR()
        {
            var packets = Config.Item("Packets").GetValue<bool>();
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            if (GetHPPercentage(Player) < 25 && R.IsReady() && target.IsValidTarget(R.Range) && !target.HasBuffOfType(BuffType.Invulnerability) && Config.Item("Auto R if HP < 25%").GetValue<bool>() == true)
            {
                R.Cast(target, packets);
            }
        }


        public static void HarassE()
        {
            var packets = Config.Item("Packets").GetValue<bool>();
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (E.IsReady() && target.IsValidTarget(E.Range) && !target.HasBuffOfType(BuffType.Invulnerability) && Config.Item("harE").GetValue<bool>() == true)
            {
                E.Cast(target, packets);
            }
        }
        private static void OnDraw(EventArgs args)
        {
            if (Config.Item("Enable Drawings").GetValue<bool>())
            {

                if (Config.Item("DrawQ").GetValue<bool>())
                {
                    Drawing.DrawCircle(Player.Position, Q.Range, Color.Blue);
                }

                if (Config.Item("DrawE").GetValue<bool>())
                {
                    Drawing.DrawCircle(Player.Position, E.Range, Color.Blue);
                }

                if (Config.Item("DrawR").GetValue<bool>())
                {
                    Drawing.DrawCircle(Player.Position, 1000f, Color.Red);
                }

                if (Config.Item("AArange").GetValue<bool>())
                {
                    Drawing.DrawCircle(Player.Position, Player.AttackRange, Color.Green);
                }

            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Config.Item("Combo").GetValue<KeyBind>().Active)
            {
                Combo();
            }

            if (Config.Item("harE").GetValue<bool>() == true)
            {
                HarassE();
            }

            if (Config.Item("Auto R if HP < 25%").GetValue<bool>() == true)
            {
                AutoR();
            }

            if (Config.SubMenu("Lane Clear").Item("Lane Clear").GetValue<KeyBind>().Active)
            {
                Farm();
            } 
            
            if (Config.SubMenu("Chase").Item("Active").GetValue<KeyBind>().Active)
            {
                Chase();
            }

            if (Config.SubMenu("KillSteal").Item("Steal Kills").GetValue<bool>() == true)
            {
                KS();
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.None:
                    if (Config.SubMenu("Escape/Chase").Item("Active").GetValue<KeyBind>().Active)
                    {
                        Flee();
                    }
                    break;

            }
        
        }

        public static void Flee()
        {

            MoveTo(Game.CursorPos);

        }

        private static readonly Random RandomPos = new Random(DateTime.Now.Millisecond);
        public static void MoveTo(Vector3 pos)
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Player.ServerPosition.Extend(pos, (RandomPos.NextFloat(0.6f, 1) + 0.2f) * 300));
        }


    }
}
