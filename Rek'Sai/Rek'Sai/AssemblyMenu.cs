using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Rek_Sai
{
    class AssemblyMenu
    {
        private static Obj_AI_Hero Player = ObjectManager.Player;
        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;

        public static void Setup()
        {
            Menu = new Menu("Rek'Sai, the Void Deathbringer", "mainmenu", true);

            var orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Menu.AddSubMenu(targetSelectorMenu);

            var ComboMenu = new Menu("Combo", "reksai.combo");
            {
                ComboMenu.AddItem(new MenuItem("reksai.combo.q1", "Use Queen's Wrath (Q1)").SetValue(true));
                ComboMenu.AddItem(new MenuItem("reksai.combo.w1", "Switch Forms (W1)").SetValue(true));
                ComboMenu.AddItem(new MenuItem("reksai.combo.e1", "Use Furious Bite (E1)").SetValue(true));
                AddSpacerTo(ComboMenu);
                ComboMenu.AddItem(new MenuItem("reksai.combo.q2", "Use Prey Seeker (Q2)").SetValue(true));
                ComboMenu.AddItem(new MenuItem("reksai.combo.w2", "Knock 'em Up (W2)").SetValue(true));
                ComboMenu.AddItem(new MenuItem("reksai.combo.e2", "Use Queen's Wrath (W2)").SetValue(true));
                AddSpacerTo(ComboMenu);
                if (Program.Ignite != SpellSlot.Unknown)
                {
                    ComboMenu.AddItem(new MenuItem("reksai.combo.flash", "Use Flash").SetValue(false));
                }
                if (Program.Ignite != SpellSlot.Unknown)
                {
                    ComboMenu.AddItem(new MenuItem("reksai.combo.ignite", "Use Ignite").SetValue(true));
                }
            }
            Menu.AddSubMenu(ComboMenu);

            var HarassMenu = new Menu("Harass", "reksai.harass");
            {
                HarassMenu.AddItem(new MenuItem("reksai.harass.e1", "Use Furious Bite (E1)").SetValue(true));
                AddSpacerTo(HarassMenu);
                HarassMenu.AddItem(new MenuItem("reksai.combo.q2", "Use Prey Seeker (Q2)").SetValue(true));
            }
            Menu.AddSubMenu(HarassMenu);

            var LaneMenu = new Menu("Lane Clear", "reksai.lane");
            {
                LaneMenu.AddItem(new MenuItem("reksai.lane.q1", "Use Queen's Wrath (Q1)").SetValue(true));
                LaneMenu.AddItem(new MenuItem("reksai.lane.e1", "Use Furious Bite (E1)").SetValue(true));
                AddSpacerTo(LaneMenu);
                LaneMenu.AddItem(new MenuItem("reksai.lane.q1.count", "Use Q if it hits").SetValue(new Slider(2, 1, 10)));
            }
            Menu.AddSubMenu(LaneMenu);

            var JungleMenu = new Menu("Jungle Clear", "reksai.jungle");
            {
                JungleMenu.AddItem(new MenuItem("reksai.jungle.q1", "Use Queen's Wrath (Q1)").SetValue(true));
                JungleMenu.AddItem(new MenuItem("reksai.jungle.w1", "Borrow (W1)").SetValue(true));
                JungleMenu.AddItem(new MenuItem("reksai.jungle.e1", "Use Furious Bite (E1)").SetValue(true));
                AddSpacerTo(JungleMenu);
                JungleMenu.AddItem(new MenuItem("reksai.jungle.q2", "Use Prey Seeker (Q2)").SetValue(true));
                JungleMenu.AddItem(new MenuItem("reksai.jungle.w2", "Knock 'em Up (W2)").SetValue(true));
            }
            Menu.AddSubMenu(JungleMenu);

            var ItemMenu = new Menu("Items", "reksai.items");
            {
                ItemMenu.AddItem(new MenuItem("reksai.items.hydra", "Use Tiamat / Hydra").SetValue(true));
                ItemMenu.AddItem(new MenuItem("reksai.items.botrk", "Use BotRK / Cutlass").SetValue(true));
                ItemMenu.AddItem(new MenuItem("reksai.items.omen", "Use Randuin's Omen").SetValue(true));
            }
            Menu.AddSubMenu(ItemMenu);

            var EscapeMenu = new Menu("Escape", "reksai.escape");
            {
                EscapeMenu.AddItem(new MenuItem("reksai.escape.w1", "Borrow (W1)").SetValue(true));
                AddSpacerTo(EscapeMenu);
                EscapeMenu.AddItem(new MenuItem("reksai.escape.e2", "Tunnel to Mouse (E2)").SetValue(true));
            }
            Menu.AddSubMenu(EscapeMenu);

            var RageMenu = new Menu("Rage Manager", "reksai.rage");
            {
                RageMenu.AddItem(new MenuItem("reksai.rage.e1", "Only Cast E at 100 Rage (E1)").SetValue(false));
                RageMenu.AddItem(new MenuItem("reksai.rage.w1", "Try to live with Burrow (W1)").SetValue(true));
            }
            Menu.AddSubMenu(RageMenu);

            var DrawingsMenu = new Menu("Drawings Manager", "reksai.draw");
            {
                DrawingsMenu.AddItem(new MenuItem("reksai.draw.draw", "Draw Spells").SetValue(true));
                DrawingsMenu.AddItem(new MenuItem("reksai.draw.tg", "Draw Target").SetValue(true));
                AddSpacerTo(DrawingsMenu);
                DrawingsMenu.AddItem(new MenuItem("reksai.draw.q1", "Draw Queen's Wrath (Q1)").SetValue(true));
                DrawingsMenu.AddItem(new MenuItem("reksai.draw.w1", "Draw Burrow (W1)").SetValue(true));
                DrawingsMenu.AddItem(new MenuItem("reksai.draw.e1", "Draw Furious Bite (E1)").SetValue(true));
                AddSpacerTo(DrawingsMenu);
                DrawingsMenu.AddItem(new MenuItem("reksai.draw.q2", "Draw Prey Seeker (Q2)").SetValue(true));
                DrawingsMenu.AddItem(new MenuItem("reksai.draw.w2", "Draw Unborrow (W2)").SetValue(true));
                DrawingsMenu.AddItem(new MenuItem("reksai.draw.e2", "Draw Tunnel (E2)").SetValue(true));
            }
            Menu.AddSubMenu(DrawingsMenu);

            var OthersMenu = new Menu("Others", "reksai.others");
            {
                var KeyBinds = new Menu("KeyBinds", "reksai.others.keybinds");
                {
                    KeyBinds.AddItem(new MenuItem("reksai.others.keybinds.combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
                    KeyBinds.AddItem(new MenuItem("reksai.others.keybinds.harass", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));
                    KeyBinds.AddItem(new MenuItem("reksai.others.keybinds.lane", "LaneClear").SetValue(new KeyBind('V', KeyBindType.Press)));
                    KeyBinds.AddItem(new MenuItem("reksai.others.keybinds.jungle", "JungleClear").SetValue(new KeyBind('V', KeyBindType.Press)));
                    KeyBinds.AddItem(new MenuItem("reksai.others.keybinds.escape", "Escape").SetValue(new KeyBind('G', KeyBindType.Press)));
                }
                OthersMenu.AddSubMenu(KeyBinds);
                var HitChances = new Menu("HitChances", "reksai.others.hitchances");
                {
                    HitChances.AddItem(new MenuItem("reksai.others.hitchances.q2", "Prey Seeker (Q2)").SetValue(new StringList(new[] { "Normal", "High" })));
                    HitChances.AddItem(new MenuItem("reksai.others.hitchances.e2", "Tunnel (E2)").SetValue(new StringList(new[] { "Normal", "High" }, 1)));
                }
                OthersMenu.AddSubMenu(HitChances);
                OthersMenu.AddItem(new MenuItem("reksai.disable", "Orbwalker Only").SetValue(false));
            }
            Menu.AddSubMenu(OthersMenu);

            Menu.AddToMainMenu();
        }

        private static void AddSpacerTo(Menu menu)
        {
            menu.AddItem(new MenuItem("spacer" + Game.ClockTime, " "));
        }
    }
}
