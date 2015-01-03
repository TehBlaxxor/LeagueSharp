using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace TehGaren___Demacia_s_Heart
{
    class Program
    {
        public static string ChampName = "Garen";
        public static string version = "1.0.0.0";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Base Player = ObjectManager.Player; // Instead of typing ObjectManager.Player you can just type Player
        public static Spell Q, W, E, R;
        public static Menu menu;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampName) return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 165);
            R = new Spell(SpellSlot.R, 400);
            menu = new Menu("Teh" + ChampName, ChampName, true);
            menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(menu.SubMenu("Orbwalker"));
            var ts = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(ts);
            menu.AddSubMenu(ts);
            menu.AddSubMenu(new Menu("Combo", "Combo"));
            menu.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            menu.SubMenu("Combo").AddItem(new MenuItem("useW", "Use W").SetValue(true));
            menu.SubMenu("Combo").AddItem(new MenuItem("useE", "Use E").SetValue(true));
            menu.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R").SetValue(true));
            menu.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));


            menu.AddSubMenu(new Menu("Escape", "Escape"));
            menu.SubMenu("Escape").AddItem(new MenuItem("escQ", "Use Q").SetValue(true));
            menu.SubMenu("Escape").AddItem(new MenuItem("escW", "Use W").SetValue(true));
            menu.SubMenu("Escape").AddItem(new MenuItem("EscapeActive", "Escape").SetValue(new KeyBind(71, KeyBindType.Press)));


            menu.AddSubMenu(new Menu("Harass", "Harass"));
            menu.SubMenu("Harass").AddItem(new MenuItem("harE", "Use E").SetValue(true));
            menu.SubMenu("Harass").AddItem(new MenuItem("HarassActive", "Harass").SetValue(new KeyBind(67, KeyBindType.Press)));

            menu.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw; 
            Game.OnGameUpdate += Game_OnGameUpdate;

            Game.PrintChat("<font color='#881df2'>Teh" + ChampName + " version " + version + "</font> loaded successfully!");
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


        static void Game_OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.None:
                    if (menu.SubMenu("Escape").Item("EscapeActive").GetValue<KeyBind>().Active)
                    {
                        Flee();
                    }
                    break;
            }
            if (menu.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }

            if (menu.Item("EscapeActive").GetValue<KeyBind>().Active)
            {
                Escape();
            }

            if (menu.Item("HarassActive").GetValue<KeyBind>().Active)
            {
                Harass();
            }



        }

        static void Drawing_OnDraw(EventArgs args)
        {
        }

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target == null) return;
            bool qcasted = false;

            if (target.IsValidTarget(E.Range) && Q.IsReady() && menu.SubMenu("Combo").Item("useQ").GetValue<bool>() == true)
            {
                Q.Cast();
                qcasted = true;

            }
            if (target.IsValidTarget(R.Range) && W.IsReady() && menu.SubMenu("Combo").Item("useW").GetValue<bool>() == true)
            {
                W.Cast();
            }
            if (target.IsValidTarget(E.Range) && E.IsReady() &&  qcasted == true && menu.SubMenu("Combo").Item("useE").GetValue<bool>() == true)
            {
                E.Cast();
                qcasted = false;
            }
            if (R.IsReady() && target.IsValidTarget(R.Range) && R.GetDamage(target) > target.Health + target.HPRegenRate && menu.SubMenu("Combo").Item("useR").GetValue<bool>() == true)
            {
                R.Cast(target);
            }
        }
        public static void Escape()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target == null) return;

            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (target.IsValidTarget(R.Range * 2) && Q.IsReady() && menu.SubMenu("Escape").Item("escQ").GetValue<bool>() == true)
            {
                Q.Cast();
            }

            if (target.IsValidTarget(R.Range * 2) && W.IsReady() && menu.SubMenu("Escape").Item("escW").GetValue<bool>() == true)
            {
                W.Cast();
            }
        }

        public static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target == null) return;

            if (target.IsValidTarget(E.Range) && E.IsReady() && menu.SubMenu("Harass").Item("harE").GetValue<bool>() == true)
            {
                E.Cast();
            }

        }
    }
}
