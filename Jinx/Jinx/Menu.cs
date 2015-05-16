using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Jinx
{
    public static class MenuUtils
    {
        public static Menu Z;
        public static Orbwalking.Orbwalker Orbwalker;
        public static void Create()
        {
            Z = new Menu("Jinx", "jinxerino so op", true);

            var tsMenu = new Menu("Target Selector", "macTS");
            TargetSelector.AddToMenu(tsMenu);
            Z.AddSubMenu(tsMenu);

            var orbwalkMenu = new Menu("Orbwalker", "macOrbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
            Z.AddSubMenu(orbwalkMenu);

            Menu sub1 = new Menu("Spell Usage", "spells");
            sub1.AddItem(MI("Switch Weapons", "spells.q").SetValue(true));
            sub1.AddItem(MI("Use Zapper", "spells.w").SetValue(true));
            sub1.AddItem(MI("Use Traps", "spells.e").SetValue(true));
            sub1.AddItem(MI("Defensive Traps", "spells.e.def").SetValue(false));
            sub1.AddItem(MI("Use Ultimate", "spells.r").SetValue(true));
            sub1.AddItem(MI("Assisted Ult", "spells.r.manual").SetValue(new KeyBind('G', KeyBindType.Press)));
            sub1.AddItem(MI("Assisted Ult Mode", "spells.r.mode").SetValue(new StringList(new[] { "Lowest HP", "Closest", "Most Damage", "Multiple Hit" })));
            Z.AddSubMenu(sub1);

            Menu sub2 = new Menu("Hit Chances", "hc");
            sub2.AddItem(MI("W - High Hitchance", "hc.w").SetValue(true));
            sub2.AddItem(MI("E - High Hitchance", "hc.e").SetValue(true));
            sub2.AddItem(MI("R - High Hitchance", "hc.r").SetValue(true));
            Z.AddSubMenu(sub2);

            Menu sub3 = new Menu("Fishbones Usage", "rocket");
            sub3.AddItem(MI("Lane Clear - Q2R", "rocket.lane.1").SetValue(true));
            sub3.AddItem(MI("Lane Clear - Q Multiple", "rocket.lane.2").SetValue(true));
            sub3.AddItem(MI("Lane Clear - Min. Minions", "rocket.lane").SetValue(new Slider(2, 1, 20)));
            Z.AddSubMenu(sub3);

            Menu sub4 = new Menu("Drawings", "draw");
            sub4.AddItem(MI("Disable Drawings", "draw.no").SetValue(false));
            sub4.AddItem(MI("Draw Q", "draw.q").SetValue(true));
            sub4.AddItem(MI("Draw W", "draw.w").SetValue(true));
            sub4.AddItem(MI("Draw E", "draw.e").SetValue(true));
            sub4.AddItem(MI("Draw R", "draw.r").SetValue(true));
            sub4.AddItem(MI("Draw W Prediction", "draw.pred.w").SetValue(true));
            sub4.AddItem(MI("Draw E Prediction", "draw.pred.e").SetValue(true));
            sub4.AddItem(MI("Draw Target", "draw.tg").SetValue(true));
            Z.AddSubMenu(sub4);


            Z.AddItem(new MenuItem("killsecure", "Attempt Kill Securing").SetValue(true));

            Z.AddToMainMenu();
            Messages.OnMenuCreation();
        }

        public static T GetValue<T>(this Menu m, string name)
        {
            return m.Item(name).GetValue<T>();
        }

        public static MenuItem MI(string displayName, string name)
        {
            return new MenuItem(name, displayName);
        }
    }
}
