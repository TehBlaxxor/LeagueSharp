using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

//using TehSM;

namespace TehRyze
{

    public class HtmlColor
    {
        public const string AliceBlue = "#F0F8FF";
        public const string AntiqueWhite = "#FAEBD7";
        public const string Aqua = "#00FFFF";
        public const string Aquamarine = "#7FFFD4";
        public const string Azure = "#F0FFFF";
        public const string Beige = "#F5F5DC";
        public const string Bisque = "#FFE4C4";
        public const string Black = "#000000";
        public const string BlanchedAlmond = "#FFEBCD";
        public const string Blue = "#0000FF";
        public const string BlueViolet = "#8A2BE2";
        public const string Brown = "#A52A2A";
        public const string BurlyWood = "#DEB887";
        public const string CadetBlue = "#5F9EA0";
        public const string Chartreuse = "#7FFF00";
        public const string Chocolate = "#D2691E";
        public const string Coral = "#FF7F50";
        public const string CornflowerBlue = "#6495ED";
        public const string Cornsilk = "#FFF8DC";
        public const string Crimson = "#DC143C";
        public const string Cyan = "#00FFFF";
        public const string DarkBlue = "#00008B";
        public const string DarkCyan = "#008B8B";
        public const string DarkGoldenRod = "#B8860B";
        public const string DarkGray = "#A9A9A9";
        public const string DarkGreen = "#006400";
        public const string DarkKhaki = "#BDB76B";
        public const string DarkMagenta = "#8B008B";
        public const string DarkOliveGreen = "#556B2F";
        public const string DarkOrange = "#FF8C00";
        public const string DarkOrchid = "#9932CC";
        public const string DarkRed = "#8B0000";
        public const string DarkSalmon = "#E9967A";
        public const string DarkSeaGreen = "#8FBC8F";
        public const string DarkSlateBlue = "#483D8B";
        public const string DarkSlateGray = "#2F4F4F";
        public const string DarkTurquoise = "#00CED1";
        public const string DarkViolet = "#9400D3";
        public const string DeepPink = "#FF1493";
        public const string DeepSkyBlue = "#00BFFF";
        public const string DimGray = "#696969";
        public const string DodgerBlue = "#1E90FF";
        public const string FireBrick = "#B22222";
        public const string FloralWhite = "#FFFAF0";
        public const string ForestGreen = "#228B22";
        public const string Fuchsia = "#FF00FF";
        public const string Gainsboro = "#DCDCDC";
        public const string GhostWhite = "#F8F8FF";
        public const string Gold = "#FFD700";
        public const string GoldenRod = "#DAA520";
        public const string Gray = "#808080";
        public const string Green = "#008000";
        public const string GreenYellow = "#ADFF2F";
        public const string HoneyDew = "#F0FFF0";
        public const string HotPink = "#FF69B4";
        public const string IndianRed = "#CD5C5C";
        public const string Indigo = "#4B0082";
        public const string Ivory = "#FFFFF0";
        public const string Khaki = "#F0E68C";
        public const string Lavender = "#E6E6FA";
        public const string LavenderBlush = "#FFF0F5";
        public const string LawnGreen = "#7CFC00";
        public const string LemonChiffon = "#FFFACD";
        public const string LightBlue = "#ADD8E6";
        public const string LightCoral = "#F08080";
        public const string LightCyan = "#E0FFFF";
        public const string LightGoldenRodYellow = "#FAFAD2";
        public const string LightGray = "#D3D3D3";
        public const string LightGreen = "#90EE90";
        public const string LightPink = "#FFB6C1";
        public const string LightSalmon = "#FFA07A";
        public const string LightSeaGreen = "#20B2AA";
        public const string LightSkyBlue = "#87CEFA";
        public const string LightSlateGray = "#778899";
        public const string LightSteelBlue = "#B0C4DE";
        public const string LightYellow = "#FFFFE0";
        public const string Lime = "#00FF00";
        public const string LimeGreen = "#32CD32";
        public const string Linen = "#FAF0E6";
        public const string Magenta = "#FF00FF";
        public const string Maroon = "#800000";
        public const string MediumAquaMarine = "#66CDAA";
        public const string MediumBlue = "#0000CD";
        public const string MediumOrchid = "#BA55D3";
        public const string MediumPurple = "#9370DB";
        public const string MediumSeaGreen = "#3CB371";
        public const string MediumSlateBlue = "#7B68EE";
        public const string MediumSpringGreen = "#00FA9A";
        public const string MediumTurquoise = "#48D1CC";
        public const string MediumVioletRed = "#C71585";
        public const string MidnightBlue = "#191970";
        public const string MintCream = "#F5FFFA";
        public const string MistyRose = "#FFE4E1";
        public const string Moccasin = "#FFE4B5";
        public const string NavajoWhite = "#FFDEAD";
        public const string Navy = "#000080";
        public const string OldLace = "#FDF5E6";
        public const string Olive = "#808000";
        public const string OliveDrab = "#6B8E23";
        public const string Orange = "#FFA500";
        public const string OrangeRed = "#FF4500";
        public const string Orchid = "#DA70D6";
        public const string PaleGoldenRod = "#EEE8AA";
        public const string PaleGreen = "#98FB98";
        public const string PaleTurquoise = "#AFEEEE";
        public const string PaleVioletRed = "#DB7093";
        public const string PapayaWhip = "#FFEFD5";
        public const string PeachPuff = "#FFDAB9";
        public const string Peru = "#CD853F";
        public const string Pink = "#FFC0CB";
        public const string Plum = "#DDA0DD";
        public const string PowderBlue = "#B0E0E6";
        public const string Purple = "#800080";
        public const string Red = "#FF0000";
        public const string RosyBrown = "#BC8F8F";
        public const string RoyalBlue = "#4169E1";
        public const string SaddleBrown = "#8B4513";
        public const string Salmon = "#FA8072";
        public const string SandyBrown = "#F4A460";
        public const string SeaGreen = "#2E8B57";
        public const string SeaShell = "#FFF5EE";
        public const string Sienna = "#A0522D";
        public const string Silver = "#C0C0C0";
        public const string SkyBlue = "#87CEEB";
        public const string SlateBlue = "#6A5ACD";
        public const string SlateGray = "#708090";
        public const string Snow = "#FFFAFA";
        public const string SpringGreen = "#00FF7F";
        public const string SteelBlue = "#4682B4";
        public const string Tan = "#D2B48C";
        public const string Teal = "#008080";
        public const string Thistle = "#D8BFD8";
        public const string Tomato = "#FF6347";
        public const string Turquoise = "#40E0D0";
        public const string Violet = "#EE82EE";
        public const string Wheat = "#F5DEB3";
        public const string White = "#FFFFFF";
        public const string WhiteSmoke = "#F5F5F5";
        public const string Yellow = "#FFFF00";
        public const string YellowGreen = "#9ACD32";
    }
    class Program
    {
       // private List<string> Skins = new List<string>();
        //private int SelectedSkin;
        //private bool Initialize = true;
        public static string ChampName = "Ryze";
        public static string version = "1.0.0.0";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Base Player = ObjectManager.Player; // Instead of typing ObjectManager.Player you can just type Player
        public static Spell Q, W, E, R;
        public static Items.Item DFG;
        //private static M_TargetSelector TS;
        private static readonly List<Spell> spellList = new List<Spell>();
        //public static SkinManager SkinManager;
        public static Menu menu;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampName) return;

            Q = new Spell(SpellSlot.Q, 625);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R);
            spellList.AddRange(new[] { Q, W, E, R });
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
            menu.SubMenu("Escape").AddItem(new MenuItem("escR", "Use R").SetValue(true));
            menu.SubMenu("Escape").AddItem(new MenuItem("escW", "Use W").SetValue(true));
            menu.SubMenu("Escape").AddItem(new MenuItem("EscapeActive", "Escape").SetValue(new KeyBind(71, KeyBindType.Press)));


            menu.AddSubMenu(new Menu("Harass", "Harass"));
            menu.SubMenu("Harass").AddItem(new MenuItem("harQ", "Use Q").SetValue(true));
            menu.SubMenu("Harass").AddItem(new MenuItem("harW", "Use W").SetValue(false));
            menu.SubMenu("Harass").AddItem(new MenuItem("harE", "Use E").SetValue(false));
            menu.SubMenu("Harass").AddItem(new MenuItem("HarassActive", "Harass").SetValue(new KeyBind(67, KeyBindType.Press)));

            //InitializeSkinManager();

            //SkinManager.AddToMenu(ref menu);

            //Exploits
            menu.AddItem(new MenuItem("NOFACE", "No-Face Exploit").SetValue(true));
            //Make the menu visible
            menu.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw; // Add onDraw
            Game.OnGameUpdate += Game_OnGameUpdate; // adds OnGameUpdate (Same as onTick in bol)

            Game.PrintChat("<font color='#881df2'>Teh" + ChampName + " version " + version + "</font> loaded successfully!");
        }

        //public void AddToMenu(ref Menu menu)
        //{
        //    if (Skins.Count > 0)
         //   {
        //        menu.AddSubMenu(new Menu("Skin Changer", "Skin Changer"));
        //        menu.SubMenu("Skin Changer").AddItem(new MenuItem("Skin_" + ObjectManager.Player.ChampionName + "_enabled", "Enable skin changer").SetValue(true));
        //        menu.SubMenu("Skin Changer").AddItem(new MenuItem("Skin_" + ObjectManager.Player.ChampionName + "_select", "Skins").SetValue(new StringList(Skins.ToArray())));
        //        SelectedSkin = menu.Item("Skin_" + ObjectManager.Player.ChampionName + "_select").GetValue<StringList>().SelectedIndex;
       //     }
       // }

        //private static void InitializeSkinManager()
        //{
          //  SkinManager = new SkinManager();
           // SkinManager.Add("Classic Ryze");
           // SkinManager.Add("Human Ryze");
           // SkinManager.Add("Tribal Ryze");
          //  SkinManager.Add("Uncle Ryze");
          //  SkinManager.Add("Triumphant Ryze");
          //  SkinManager.Add("Professor Ryze");
          //  SkinManager.Add("Zombie Ryze");
          //  SkinManager.Add("Dark Crystal Ryze");
          //  SkinManager.Add("Pirate Ryze");
        // }

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

            //SkinManager.Update();

        }

        static void Drawing_OnDraw(EventArgs args)
        {
            // Spell ranges
            foreach (var spell in spellList)
            {
                // Regular spell ranges
                var circleEntry = menu.Item("drawRange" + spell.Slot).GetValue<Circle>();
                if (circleEntry.Active)
                    Utility.DrawCircle(Player.Position, spell.Range, circleEntry.Color);
            }
        }

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;

            if (target.IsValidTarget(Q.Range) && Q.IsReady() && menu.SubMenu("Combo").Item("useQ").GetValue<bool>() == true)
            {
                Q.Cast(target, menu.Item("NOFACE").GetValue<bool>());

            }
            if (target.IsValidTarget(W.Range) && W.IsReady() && menu.SubMenu("Combo").Item("useW").GetValue<bool>() == true)
            {
                W.Cast(target, menu.Item("NOFACE").GetValue<bool>());
            }
            if (target.IsValidTarget(E.Range) && E.IsReady() && menu.SubMenu("Combo").Item("useE").GetValue<bool>() == true)
            {
                E.Cast(target, menu.Item("NOFACE").GetValue<bool>());
            }
            if (R.IsReady() && Player.HealthPercentage() < 75  && menu.SubMenu("Combo").Item("useR").GetValue<bool>() == true)
            {
                R.Cast();
            }
        }
        public static void Escape()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;

            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (target.IsValidTarget(Q.Range) && R.IsReady() && menu.SubMenu("Escape").Item("escR").GetValue<bool>() == true)
            {
                R.Cast();
            }

            if (target.IsValidTarget(W.Range) && W.IsReady() && menu.SubMenu("Escape").Item("escW").GetValue<bool>() == true)
            {
                W.Cast(target, menu.Item("NOFACE").GetValue<bool>());
            }
        }

        public static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;

            if (target.IsValidTarget(Q.Range) && Q.IsReady() && menu.SubMenu("Harass").Item("harQ").GetValue<bool>() == true)
            {
                Q.Cast(target, menu.Item("NOFACE").GetValue<bool>());
            }

            if (target.IsValidTarget(W.Range) && W.IsReady() && menu.SubMenu("Harass").Item("harW").GetValue<bool>() == true)
            {
                W.Cast(target, menu.Item("NOFACE").GetValue<bool>());
            }

            if (target.IsValidTarget(E.Range) && E.IsReady() && menu.SubMenu("Harass").Item("harE").GetValue<bool>() == true)
            {
                E.Cast(target, menu.Item("NOFACE").GetValue<bool>());
            }

        }
    }
}