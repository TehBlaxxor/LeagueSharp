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
            else Game.PrintChat("Unexpected type!");
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
                        Game.PrintChat("--------------------------");
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
                        Game.PrintChat("--------------------------");                    }
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
                        Game.PrintChat("Pet return radius: " + Player.PetReturnRadius);
                        Game.PrintChat("Last Pet Spawned ID: " + Player.AI_LastPetSpawnedID);
                    }
                    else print("Pet not found!", "error");
                }
                else if (args.Input.ToLowerInvariant().Contains("print iteminfo"))
                {
                    if (Player.InventoryItems.Count() > 0)
                    {
                        print("Printing items' info:", "normal");
                        foreach (var Item in Player.InventoryItems)
                        {
                            Game.PrintChat("Item Display Name: " + Item.DisplayName);
                            Game.PrintChat("Item Name: " + Item.Name);
                            Game.PrintChat("Item ID: " + Item.Id);
                            Game.PrintChat("Item Stacks: " + Item.Stacks);
                            Game.PrintChat("--------------------------");
                        }
                    }
                    else print("You do not own any items!", "error");
                }
                else if (args.Input.ToLowerInvariant().Contains("print attacktype"))
                {
                    if (Player.IsMelee())
                    {
                        print("Champion is melee!", "normal");
                    }
                    else if (!Player.IsMelee())
                    {
                        print("Champion is ranged!", "normal");
                    }
                    else print("Unexpected error!", "error");
                }
                else if (args.Input.ToLowerInvariant().Contains("print largest critstrike"))
                {
                    if (Player.LargestCriticalStrike != 0)
                    {
                        print("Your largest critical strike is " + Player.LargestCriticalStrike, "normal");
                    }
                    else print("You haven't critically striked yet!", "error");
                }
                else if (args.Input.ToLowerInvariant() == "print my info")
                {
                    print("Printing info:", "normal");
                    Game.PrintChat("Armor: " + Player.Armor);
                    Game.PrintChat("Flat Magic Reduction: " + Player.FlatMagicReduction);
                    Game.PrintChat("Gold: " + Player.Gold);
                    Game.PrintChat("Current Gold: " + Player.GoldCurrent);
                    Game.PrintChat("Gold Earned: " + Player.GoldEarned);
                    Game.PrintChat("Has Bot AI: " + Player.HasBotAI);
                    Game.PrintChat("HP: " + Player.Health + "/" + Player.MaxHealth);
                    Game.PrintChat("HP Percentage: " + Player.HealthPercentage());
                    Game.PrintChat("Mana: " + Player.Mana + "/" + Player.MaxMana);
                    Game.PrintChat("Mana Percentage: " + Player.ManaPercentage());

                }

                else if (args.Input.ToLowerInvariant().Contains("print my info 2"))
                {
                    Game.PrintChat("HP Regen Rate: " + Player.HPRegenRate); //
                    Game.PrintChat("Is In Fountain: " + Player.InFountain());
                    Game.PrintChat("Is In Shop: " + Player.InShop());
                    Game.PrintChat("Is Bot: " + Player.IsBot);
                    Game.PrintChat("Is Player Dead?: " + Player.IsDead);
                    Game.PrintChat("Is Immovable: " + Player.IsImmovable);
                    Game.PrintChat("Is Invulnerable: " + Player.IsInvulnerable);
                    Game.PrintChat("Is Moving: " + Player.IsMoving);
                    Game.PrintChat("Is Pacified: " + Player.IsPacified);
                    Game.PrintChat("Is Recalling: " + Player.IsRecalling());
                }

                else if (args.Input.ToLowerInvariant().Contains("print my info 3"))
                {
                    Game.PrintChat("Is Rooted: " + Player.IsRooted);//
                    Game.PrintChat("Is Stunned: " + Player.IsStunned);
                    Game.PrintChat("Is Targetable: " + Player.IsTargetable);
                    Game.PrintChat("Is Visible: " + Player.IsVisible);
                    Game.PrintChat("Is Winding Up: " + Player.IsWindingUp);
                    Game.PrintChat("Is Zombie: " + Player.IsZombie);
                    Game.PrintChat("Last Spell Casted: " + Player.LastCastedSpellName());
                    Game.PrintChat("Last Spell's Target: " + Player.LastCastedSpellTarget());
                    Game.PrintChat("Last Pause Position: " + Player.LastPausePosition);
                    Game.PrintChat("Level: " + Player.Level);
                }

                else if (args.Input.ToLowerInvariant().Contains("print my info 4"))
                {
                    Game.PrintChat("Level Cap: " + Player.LevelCap);//
                    Game.PrintChat("Lifesteal Immune: " + Player.LifestealImmune);
                    Game.PrintChat("Longest Time Alive: " + Player.LongestTimeSpentLiving);
                    Game.PrintChat("Magic Immune: " + Player.MagicImmune);
                    Game.PrintChat("Network ID: " + Player.NetworkId);
                    Game.PrintChat("Wards Killed: " + Player.WardsKilled);
                    Game.PrintChat("Wards Placed: " + Player.WardsPlaced);

                }
                else if (args.Input.ToLowerInvariant().Contains("print game info"))
                {
                    print("Printing game info:", "normal");
                    Game.PrintChat("Clock Time: " + Game.ClockTime);
                    Game.PrintChat("ID: " + Game.Id);
                    Game.PrintChat("IP: " + Game.IP);
                    Game.PrintChat("Map ID: " + Game.MapId);
                    Game.PrintChat("Mode: " + Game.Mode);
                    Game.PrintChat("Ping: " + Game.Ping);
                    Game.PrintChat("Time: " + Game.Time);
                    Game.PrintChat("Version: " + Game.Version);

                }
                else if (args.Input.ToLowerInvariant().Contains("ping"))
                {
                    print("Ping: " + Game.Ping, "normal");
                }

                else { print("Command not found!", "error"); }
                
            }
        }    
    }
}
