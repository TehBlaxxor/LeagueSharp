using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AbilitySight
{
    class Program
    {
        public static Spell Rkatarina;
        public static Spell Rmorgana;
        public static Spell Qevelynn;
        public static Spell Wtryndamere;
        public static Spell Wkennen;
        public static string version = "1.0.0.0";
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Menu menu;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }
        static void Game_OnGameLoad(EventArgs args)
        {
            menu = new Menu("AbilitySight", "AbilitySight", true);
            menu.AddItem(new MenuItem("Enabled", "Enabled").SetValue(true));
            menu.AddSubMenu(new Menu("Developed by TehBlaxxor @ joduska.me", "Developed by TehBlaxxor @ joduska.me"));

            menu.AddToMainMenu();


            if (Player.BaseSkinName == "Katarina")
            {
                Rkatarina = new Spell(SpellSlot.R, 550);
                InitDrawing(Rkatarina);
                Game.PrintChat("<font color='#881df2'>AbilitySight</font> loaded successfully for Katarina!");
            }
            else if (Player.BaseSkinName == "Morgana")
            {
                Rmorgana = new Spell(SpellSlot.R, 600);
                InitDrawing(Rmorgana);
                Game.PrintChat("<font color='#881df2'>AbilitySight</font> loaded successfully for Morgana!");
            }
            else if (Player.BaseSkinName == "Evelynn")
            {
                Qevelynn = new Spell(SpellSlot.Q, 500);
                InitDrawing(Qevelynn);
                Game.PrintChat("<font color='#881df2'>AbilitySight</font> loaded successfully for Evelynn!");
            }
            else if (Player.BaseSkinName == "Tryndamere")
            {
                Wtryndamere = new Spell(SpellSlot.W, 400);
                InitDrawing(Wtryndamere);
                Game.PrintChat("<font color='#881df2'>AbilitySight</font> loaded successfully for Tryndamere!");
            }
            else if (Player.BaseSkinName == "Kennen")
            {
                Wkennen = new Spell(SpellSlot.W, 800);
                InitDrawing(Wkennen);
                Game.PrintChat("<font color='#881df2'>AbilitySight</font> loaded successfully for Kennen!");
            }
            else
            {
                menu.AddSubMenu(new Menu("Champion not supported!", "Champion not supported!"));
                Game.PrintChat("Failed to load AbilitySight for " + Player.BaseSkinName + " (champion not supported).");
            }
            Game.OnGameUpdate += Game_OnGameUpdate; // adds OnGameUpdate (Same as onTick in bol)

            
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
        }

        static bool CheckForMinions(Spell z)
        {
            var nrminions = MinionManager.GetMinions(z.Range, MinionTypes.All, MinionTeam.Enemy).Count;

            if (nrminions < 1)
            {
                return false;
            }
            else return true;
            

        }

        static bool CheckForEnemies(Spell x)
        {
            if (x.IsReady())
            {
                return true;
            }
            else return false;

        }

        public static void InitDrawing(Spell y)
        {
            var text = new Render.Text("", Player, new Vector2(20, 50), 18, new ColorBGRA(255, 255, 255, 255));
            if (Render.OnScreen(Drawing.WorldToScreen(Player.Position)) && menu.Item("Enabled").GetValue<bool>() == true) ;
            {
                text.Visible = true;
            }
            text.TextUpdate += () =>
            {
                if (menu.Item("Enabled").GetValue<bool>() == false)
                {
                    return "";
                }

                else if (Player.BaseSkinName == "Kennen" && CheckForMinions(y) == false && CheckForEnemies(y))
                {
                    return "Enemies have been found!";
                }
                else if (Player.BaseSkinName != "Kennen" && CheckForEnemies(y))
                {
                    return "Enemies have been found!";
                }
                else if (y.Level == 0)
                {
                    if (Player.Level >= 3 && Player.BaseSkinName == "Kennen")
                    {
                        return "You should get your spell.";
                    }
                    if (Player.Level >= 1 && Player.BaseSkinName == "Evelynn")
                    {
                        return "You should get your spell.";
                    }
                    else if (Player.Level > 6)
                    {
                        return "Do you play on getting your damn spell?";
                    }
                    else return "Spell not acquired.";
                }


                else return "";
            };


            text.OutLined = true;
            text.Add();

        }

    }
}