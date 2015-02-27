using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Template
{
    class Menu
    {
        public static LeagueSharp.Common.Menu root;
        private static Orbwalking.Orbwalker Orbwalker;

        public static void Initialize()
        {
            root = new LeagueSharp.Common.Menu(Settings.ASSEMBLY_NAME, "menu", true);

            var targetSelectorMenu = new LeagueSharp.Common.Menu("[Teh] Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            root.AddSubMenu(targetSelectorMenu);

            root.AddSubMenu(new LeagueSharp.Common.Menu("[Teh] Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(root.SubMenu("Orbwalking"));

            root.AddSubMenu(new LeagueSharp.Common.Menu("[Teh] Combo", "Combo"));
            root.SubMenu("Combo").AddItem(new MenuItem("comboQ", "Use Transfusion (Q)")).SetValue(true);
            root.SubMenu("Combo").AddItem(new MenuItem("comboW", "Use Sanguine Pool (W)")).SetValue(true);
            root.SubMenu("Combo").AddItem(new MenuItem("comboE", "Use Tides of Blood (E)")).SetValue(true);
            root.SubMenu("Combo").AddItem(new MenuItem("comboR", "Use Hemoplague (R)")).SetValue(true);
            root.SubMenu("Combo").AddItem(new MenuItem("spacer", " "));
            root.SubMenu("Combo").AddItem(new MenuItem("modW", "Use W").SetValue(new StringList(new[] {"If Enemy Killable", "If Enemy In Range"})));
            root.SubMenu("Combo").AddItem(new MenuItem("modE", "Min. Enemies for E")).SetValue(new Slider(1, 1, 5));
            root.SubMenu("Combo").AddItem(new MenuItem("modR", "Use R").SetValue(new StringList(new[] {"If Enemy Killable", "If Enemy In Range"})));

            root.AddSubMenu(new LeagueSharp.Common.Menu("[Teh] Harass", "Harass"));
            root.SubMenu("Harass").AddItem(new MenuItem("harassQ", "Use Transfusion (Q)")).SetValue(true);
            root.SubMenu("Harass").AddItem(new MenuItem("harassE", "Use Tides of Blood (E)")).SetValue(true);
            root.SubMenu("Harass").AddItem(new MenuItem("harassToggler", "Toggle Harass")).SetValue(new KeyBind('H', KeyBindType.Toggle));

            root.AddSubMenu(new LeagueSharp.Common.Menu("[Teh] Kill Steal", "KS"));
            root.SubMenu("Harass").AddItem(new MenuItem("ks", "Try to KillSteal")).SetValue(true);
            root.SubMenu("Harass").AddItem(new MenuItem("ksQ", "Use Transfusion (Q)")).SetValue(true);
            root.SubMenu("Harass").AddItem(new MenuItem("ksE", "Use Tides of Blood (E)")).SetValue(true);
            root.SubMenu("Harass").AddItem(new MenuItem("ksIgnite", "Use Ignite")).SetValue(false);

            root.AddSubMenu(new LeagueSharp.Common.Menu("[Teh] Lane Clear", "LaneClear"));
            root.SubMenu("LaneClear").AddItem(new MenuItem("laneClearQ", "Use Transfusion (Q)")).SetValue(true);
            root.SubMenu("LaneClear").AddItem(new MenuItem("laneClearW", "Use Sanguine Pool (W)")).SetValue(true);
            root.SubMenu("LaneClear").AddItem(new MenuItem("laneClearE", "Use Tides of Blood (E)")).SetValue(true);
            root.SubMenu("LaneClear").AddItem(new MenuItem("spacer2", " "));
            root.SubMenu("LaneClear").AddItem(new MenuItem("modW2", "Min. Minions for W")).SetValue(new Slider(5, 1, 10));
            root.SubMenu("LaneClear").AddItem(new MenuItem("modE2", "Min. Minions for E")).SetValue(new Slider(3, 1, 10));

            root.AddSubMenu(new LeagueSharp.Common.Menu("[Teh] Last Hit", "LastHit"));
            root.SubMenu("LastHit").AddItem(new MenuItem("lastHitQ", "Use Transfusion (Q)")).SetValue(true);
            root.SubMenu("LastHit").AddItem(new MenuItem("lastHitE", "Use Transfusion (E)")).SetValue(true);
            root.SubMenu("LastHit").AddItem(new MenuItem("spacer3", " "));
            root.SubMenu("LastHit").AddItem(new MenuItem("modE3", "Min. Minions for E")).SetValue(new Slider(1, 1, 10));

            root.AddSubMenu(new LeagueSharp.Common.Menu("[Teh] Drawings", "Drawings"));
            root.SubMenu("LastHit").AddItem(new MenuItem("drawQ", "Draw Transfusion (Q) range")).SetValue(true);
            root.SubMenu("LastHit").AddItem(new MenuItem("drawW", "Draw Sanguine Pool (W) range")).SetValue(true);
            root.SubMenu("LastHit").AddItem(new MenuItem("drawE", "Draw Tides of Blood (E) range")).SetValue(true);
            root.SubMenu("LastHit").AddItem(new MenuItem("drawR", "Draw Hemoplague (R) range")).SetValue(true);
            root.SubMenu("LastHit").AddItem(new MenuItem("drawOn", "Enable Drawings")).SetValue(true);

            root.AddSubMenu(new LeagueSharp.Common.Menu("[Teh] Misc", "Misc"));
            root.SubMenu("Misc").AddItem(new MenuItem("modHP", "Save Health Percentage for Combo")).SetValue(new Slider(35, 1, 100));
            root.SubMenu("LastHit").AddItem(new MenuItem("spacer4", " "));
            root.SubMenu("Misc").AddItem(new MenuItem("surviveW", "Auto W")).SetValue(true);
            root.SubMenu("Misc").AddItem(new MenuItem("surviveWHP", "Auto W HP Percentage")).SetValue(new Slider(10, 1, 100));

            root.AddToMainMenu();

            Console.WriteLine("Menu initialized successfully!");
           
        }

        public static bool GetBool(string MenuItemName)
        {
            if (Menu.root.Item(MenuItemName).GetValue<bool>() == true)
            { return true; }
            else
            { return false; }
        }

        public static float GetSlider(string MenuItemName)
        {
            return Menu.root.Item(MenuItemName).GetValue<Slider>().Value;
        }

        public static int GetSelectedStringListIndex(string MenuItemName)
        {
            return Menu.root.Item(MenuItemName).GetValue<StringList>().SelectedIndex;
        }

        public static bool IsKeyBindActive(string KeyBindMenuItemName)
        {
            return Menu.root.Item(KeyBindMenuItemName).GetValue<KeyBind>().Active;
        }
    }
}
