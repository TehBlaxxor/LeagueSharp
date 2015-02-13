using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace DevTool
{
    class Program
    {
        private static Obj_AI_Hero Player = ObjectManager.Player;

        static void Main(string[] args)
        {
            Game.OnGameInput += Game_OnGameInput;
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        public static void print(string msg, string type)
        {
            if (type.ToLowerInvariant() == "normal")
            {
                Game.PrintChat("<b><font color=\'#d92ee8\'>[MSG] DevTool: </font></b>" + msg);
            }
            else if (type.ToLowerInvariant() == "error")
            {
                Game.PrintChat("<b><font color=\'#ff5500\'>[ERR] DevTool: </font></b>" + msg);
            }
            else if (type.ToLowerInvariant() == "warning")
            {
                Game.PrintChat("<b><font color=\'#ff5500\'>[WRN] DevTool: </font></b>" + msg);
            }
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            Game.PrintChat("<b><font color=\'#d92ee8\'>TehBlaxxor's DevTool successfully loaded!</font></b>");
        }

        public static int countbuffs(Obj_AI_Hero hero)
        {
            int buffcount = 0;
            foreach (var Buff in hero.Buffs)
            {
                buffcount = buffcount + 1;
            }

            return buffcount;
        }

        public static Obj_AI_Hero GetNearestTarget(Vector3 pos)
        {
            return ObjectManager.Get<Obj_AI_Hero>().OrderBy(x => pos.Distance(x.ServerPosition)).FirstOrDefault();
        }

        private static void Game_OnGameInput(GameInputEventArgs args)
        {
            if (args.Input.StartsWith("#"))
            {
                args.Process = false;

                if (args.Input.ToLowerInvariant().Contains("print buffs") && !args.Input.ToLowerInvariant().Contains("print buffs nearest"))
                {
                    print(countbuffs(Player) + " buffs have been found:", "normal");
                    foreach (var Buff in Player.Buffs)
                    {
                        Game.PrintChat("<font color=\'#1fdb35\'> Buff Name: </font>" + Buff.Name);
                        Game.PrintChat("<font color=\'#1fdb35\'> Buff Display Name: </font>" + Buff.DisplayName);
                        Game.PrintChat("<font color=\'#1fdb35\'> Buff Type: </font>" + Buff.Type);
                        Game.PrintChat("__________________________");
                    }
                }

                else if (args.Input.ToLowerInvariant().Contains("print buffs nearest"))
                {
                    print(countbuffs(GetNearestTarget(Game.CursorPos)) + " buffs have been found:", "normal");
                    foreach (var Buff in GetNearestTarget(Game.CursorPos).Buffs)
                    {
                        Game.PrintChat("<font color=\'#1fdb35\'> Buff Name: </font>" + Buff.Name);
                        Game.PrintChat("<font color=\'#1fdb35\'> Buff Display Name: </font>" + Buff.DisplayName);
                        Game.PrintChat("<font color=\'#1fdb35\'> Buff Type: </font>" + Buff.Type);
                        Game.PrintChat("__________________________");
                    }
                }
                else if (args.Input.ToLowerInvariant().Contains("champname"))
                {
                    print("Your champion is named " + Player.ChampionName, "normal");
                    print("Your champion's base skin is named " + Player.BaseSkinName, "normal");
                    print("Your champion's current skin name is " + Player.SkinName, "normal");
                }

                else if (args.Input.ToLowerInvariant().Contains("print position") && !args.Input.ToLowerInvariant().Contains("print position cursor"))
                {
                    print("Your position is: " + Player.Position, "normal");
                }

                else if (args.Input.ToLowerInvariant().Contains("print position cursor"))
                {
                    print("Your cursor's position is: " + Game.CursorPos, "normal");
                }
                else if (args.Input.ToLowerInvariant().Contains("print petinfo"))
                {
                    if (Player.Pet != null)
                    {
                        var pet = Player.Pet;
                        print("Pet found! Printing info:", "normal");
                        Game.PrintChat("Pet name is: " + pet.Name);
                        Game.PrintChat("Pet position is: " + pet.Position);
                        Game.PrintChat("Pet type is: " + pet.Type);
                    }
                    else print("Pet not found!", "error");
                }

                else { print("Command not found!", "error"); }
                
                
            }
        }    
    }
}
