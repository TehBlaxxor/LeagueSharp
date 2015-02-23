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

            var targetSelectorMenu = new LeagueSharp.Common.Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            root.AddSubMenu(targetSelectorMenu);

            root.AddSubMenu(new LeagueSharp.Common.Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(root.SubMenu("Orbwalking"));

            root.AddSubMenu(new LeagueSharp.Common.Menu("Display Name", "Name"));
            root.SubMenu("Combo").AddItem(new MenuItem("bool", "Bool")).SetValue(true);
            root.AddItem(new MenuItem("stringList", "String List").SetValue(new StringList(new[] { "string1", "string2", "string3" })));
            root.SubMenu("Combo").AddItem(new MenuItem("slider", "Slider")).SetValue(new Slider(2, 1, 5));
            root.SubMenu("Combo").AddItem(new MenuItem("keyBind", "Key Bind").SetValue(new KeyBind(32, KeyBindType.Press)));

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
