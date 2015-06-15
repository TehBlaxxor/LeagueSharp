using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AdvancedSharp.Instance
{
    class Orbwalker
    {
        public static Menu Z;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Orbwalking.Orbwalker Orbwalkerino;
        public Orbwalker()
        {
            Core.LoadWelcomeMessage(true);

            Z = new Menu("Advanced#", "root", true);
            Menu OrbwalkerM = new Menu("Orbwalker", "Orbwalker");
            Z.AddSubMenu(OrbwalkerM);
            Orbwalkerino = new Orbwalking.Orbwalker(OrbwalkerM);
            Z.AddToMainMenu();
        }
    }
}
