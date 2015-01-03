using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Katarina_s_Assassin_Sight
{
    class Program
    {
        public static string ChampName = "Katarina";
        public static string version = "1.0.0.0";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Spell R;
        public static Menu menu;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampName) return;
            R = new Spell(SpellSlot.R, 550);
            menu = new Menu("Katarina's Assassin Sight", "Katarina's Assassin Sight", true);
            menu.AddItem(new MenuItem("Enabled", "Enabled").SetValue(true));
            menu.AddSubMenu(new Menu("Developed by TehBlaxxor @ joduska.me", "Developed by TehBlaxxor @ joduska.me"));
            
            menu.AddToMainMenu();

            InitDrawing();

            Game.OnGameUpdate += Game_OnGameUpdate; // adds OnGameUpdate (Same as onTick in bol)

            Game.PrintChat("<font color='#881df2'>Katarina's Assassin Sight</font> loaded successfully!");
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
        }
        
        static bool CheckForEnemies()
        {
            if (R.IsReady())
            {
                return true;
            }
            else return false;

        }

        public static void InitDrawing()
        {
            var text = new Render.Text("", Player, new Vector2(20, 50), 18, new ColorBGRA(255, 255, 255, 255));
            if (Render.OnScreen(Drawing.WorldToScreen(Player.Position)) && menu.Item("Enabled").GetValue<bool>() == true);
            {
                text.Visible = true;
            }
            text.TextUpdate += () =>
            {
                if (menu.Item("Enabled").GetValue<bool>() == false)
                {
                    return "";
                }
                else if (CheckForEnemies())
                {
                    return "An enemy has been detected!";
                }
                else if (R.Level == 0)
                {
                    if (Player.Level >= 6)
                    {
                        return "Do you play on getting your damn ultimate?";
                    }
                    else return "Ultimate not acquired.";
                }


                else return "Unable to detect enemies.";
            };


                text.OutLined = true;
                text.Add();
            
        }

    }
}
