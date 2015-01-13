using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Drawing;
using Color = System.Drawing.Color;

namespace Blitzcrank___The_Iron_Golem
{
    class Program
    {

        private static string Champion = "Blitzcrank";
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
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != Champion) return;
            Game.PrintChat("<font color=\'#ff5500\'>" + Champion + "</font><font color=\'#00aa00\'> successfully loaded!</font>");
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, Player.AttackRange);
            R = new Spell(SpellSlot.R, 600);

            Q.SetSkillshot(250f, 70, 1800, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(250f, 600, float.MaxValue, false, SkillshotType.SkillshotCircle);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Config = new Menu("Blitzcrank", "Blitzcrank", true);
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);

            Config.AddSubMenu(targetSelectorMenu);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("Use Q", "Use Q")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("Use E", "Use E")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("Use R", "Use R")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("Min Targets For Ult", "Min Targets For Ult")).SetValue(new Slider(2, 1, 5));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Escape/Chase", "Escape/Chase"));
            Config.SubMenu("Escape/Chase").AddItem(new MenuItem("Escape W?", "Escape W?")).SetValue(true);
            Config.SubMenu("Escape/Chase").AddItem(new MenuItem("Active", "Active").SetValue(new KeyBind(71, KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("Packets", "Packets")).SetValue(true);

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("Enable Drawings", "Enable Drawings").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "Draw Q")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawR", "Draw R")).SetValue(true);

            Config.AddToMainMenu();

        }

        private static void OnDraw(EventArgs args)
        {
            if (Config.Item("Enable Drawings").GetValue<bool>())
            {

                if (Config.Item("DrawQ").GetValue<bool>())
                {
                    Drawing.DrawCircle(Player.Position, Q.Range, Color.Blue);
                }

                if (Config.Item("DrawR").GetValue<bool>())
                {
                    Drawing.DrawCircle(Player.Position, 600f, Color.Red);
                }

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


        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Config.Item("Combo").GetValue<KeyBind>().Active)
            {
                Combo();
            }

            if (Config.SubMenu("Escape/Chase").Item("Active").GetValue<KeyBind>().Active)
            {
                Chase();
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

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var minR = Config.Item("Min Targets For Ult").GetValue<Slider>().Value;
            var packets = Config.Item("Packets").GetValue<bool>();

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && Config.SubMenu("Combo").Item("Use Q").GetValue<bool>() == true && !target.HasBuffOfType(BuffType.SpellImmunity) && !target.HasBuffOfType(BuffType.SpellShield))
            {
                Q.Cast(target, packets);
            }

            if (E.IsReady() && target.IsValidTarget(Q.Range) && target.HasBuff("RocketGrab") && Config.SubMenu("Combo").Item("Use E").GetValue<bool>() == true)
            {
                E.Cast();
                Orbwalking.ResetAutoAttackTimer();
                Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }

            if (R.IsReady() && target.IsValidTarget(R.Range) && Config.SubMenu("Combo").Item("Use R").GetValue<bool>() == true && Player.CountEnemysInRange(R.Range) >= minR)
            {
                R.Cast(target, packets);
            }
        }

        private static void Chase()
        { 
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (W.IsReady() && Config.Item("Escape W?").GetValue<bool>() == true && target.IsValidTarget(Q.Range + 150))
            {
                W.Cast();
            }
        }


    }
}


