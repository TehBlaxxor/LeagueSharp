using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using LMenu = LeagueSharp.Common.Menu;

namespace The_Masterpiece.ChampionAssets.BaseChampionInstance
{
    public static class Menu
    {
        public static LMenu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static void Load(string displayName, string name)
        {
            Config = new LMenu(displayName, name, true);

            var TSMenu = new LMenu("Target Selector", "ts");
            TargetSelector.AddToMenu(TSMenu);
            Config.AddSubMenu(TSMenu);

            var OrbMenu = new LMenu("Orbwalker", "orb");
            Orbwalker = new Orbwalking.Orbwalker(OrbMenu);
            Config.AddSubMenu(OrbMenu);

            var Combo = new LMenu("Combo", "combo");
            Config.SubMenu("combo").AddItem(MI("Use Q", "combo.q").SetValue(true));
            Config.AddSubMenu(Combo);

            var Harass = new LMenu("Harass", "harass");
            Harass.AddItem(MI("harass.q", "Use Q").SetValue(true));
            Config.AddSubMenu(Harass);

            var LaneClear = new LMenu("Lane Clear", "lane");
            LaneClear.AddItem(MI("lane.q", "Use Q").SetValue(true));
            Config.AddSubMenu(LaneClear);

            var LastHit = new LMenu("Last Hit", "last");
            LastHit.AddItem(MI("last.q", "Use Q").SetValue(true));
            Config.AddSubMenu(LastHit);

            var Escape = new LMenu("Escape", "escape");
            Escape.AddItem(MI("escape.q", "Use Q").SetValue(true));
            Config.AddSubMenu(Escape);

            var Extra = new LMenu("Extra", "extra");
            Extra.AddItem(new MenuItem("extra.autolevel", "Auto Level Spells").SetValue(true));
            Extra.AddItem(new MenuItem("extra.ks", "Attemp to Steal Kills").SetValue(true));
            Extra.AddItem(new MenuItem("extra.mm", "Save % Mana For Combo").SetValue(new Slider(30, 0, 100)));
            Extra.AddItem(new MenuItem("extra.gc", "Anti Gap Closer").SetValue(true));
            Extra.AddSubMenu(new LMenu("Hitchances", "extra.hc"));
            Extra.SubMenu("extra.hc").AddItem(new MenuItem("extra.hc.q", "Q").SetValue(new StringList(new[] { "Normal", "High" })));
            Extra.AddSubMenu(new LMenu("Keybinds", "extra.kb"));
            Extra.SubMenu("extra.kb").AddItem(MI("extra.kb.combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            Extra.SubMenu("extra.kb").AddItem(MI("extra.kb.harass", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));
            Extra.SubMenu("extra.kb").AddItem(MI("extra.kb.escape", "Escape").SetValue(new KeyBind('G', KeyBindType.Press)));
            Extra.SubMenu("extra.kb").AddItem(MI("extra.kb.lane", "LaneClear").SetValue(new KeyBind('V', KeyBindType.Press)));
            Extra.SubMenu("extra.kb").AddItem(MI("extra.kb.last", "LastHit").SetValue(new KeyBind('X', KeyBindType.Press)));
            Config.AddSubMenu(Extra);

            var Drawings = new LMenu("Drawings", "drawings");
            Drawings.AddItem(MI("draw", "Use Drawings").SetValue(true));
            Drawings.AddItem(MI("draw.q", "Draw Q").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Drawings.AddItem(MI("draw.w", "Draw W").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Drawings.AddItem(MI("draw.e", "Draw E").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Drawings.AddItem(MI("draw.r", "Draw R").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Drawings.AddItem(MI("draw.target", "Draw Target").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Drawings.AddItem(MI("draw.dmg", "Draw Damage").SetValue(true));
            Config.AddSubMenu(Drawings);


            Config.AddToMainMenu();



        }

        public static T GetValue<T>(this LMenu menu, string name)
        {
            return menu.Item(name).GetValue<T>();
        }

        private static MenuItem MI(string displayName, string name)
        {
            return new MenuItem(name, displayName);
        }

    }
}
