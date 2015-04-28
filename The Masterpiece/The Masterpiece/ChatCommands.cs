using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using Assembly = System.Reflection.Assembly;

namespace The_Masterpiece
{
    internal class ChatCommands
    {
        internal static void Game_OnInput(GameInputEventArgs args)
        {
            #region Command Setup
            if (args.Input.StartsWith("#"))
                args.Process = false;
            else return;

            string[] arguments = args.Input.Split(' ');
            string command = arguments[0].Substring(1);
            #endregion

            #region Commands
            switch (command)
            {
                case "rundev":
                    {
                        Program.Permission = GlobalEnums.RunningMode.DEVELOPER;
                    }
                    break;
                case "load":
                    {
                        var champion = arguments[1].ToString();
                        if (champion != null && Program.Permission == GlobalEnums.RunningMode.DEVELOPER)
                        {
                            switch (champion.ToLowerInvariant())
                            {
                                case "vayne":
                                    {
                                        new Plugins.Vayne();
                                        Game.PrintChat("load");
                                    }
                                    break;
                                case "malzahar":
                                    {
                                        new Plugins.Malzahar();
                                        Game.PrintChat("load");
                                    }
                                    break;
                                case "lucian":
                                        {
                                            new Plugins.Lucian();
                                            Game.PrintChat("load");
                                        }
                                    break;
                            }
                        }
                    }
                    break;
                case "version":
                    Game.PrintChat("Version is: " + Assembly.GetExecutingAssembly().GetName().Version, GlobalEnums.MessageType.NORMAL);
                    break;
                case "buffs":
                    {
                        if (arguments[1].ToLowerInvariant() == ObjectManager.Player.ChampionName.ToLowerInvariant())
                        {
                            foreach (var Bufferino in ObjectManager.Player.Buffs)
                            {
                                Console.Write("CASTER: {0} || COUNT: {1} || DISPLAY_NAME: {2} || NAME: {3} || TYPE: {4}", Bufferino.Caster, Bufferino.Count, Bufferino.DisplayName, Bufferino.Name, Bufferino.Type);
                            }
                        }
                        else if (arguments[1] != null)
                        {
                            foreach (var Player in ObjectManager.Get<Obj_AI_Hero>().Where(x => !x.IsMe))
                            {
                                if (Player.ChampionName.ToLowerInvariant() == arguments[1].ToLowerInvariant())
                                {
                                    foreach (var Bufferino in ObjectManager.Player.Buffs)
                                    {
                                        Console.Write("CASTER: {0} || COUNT: {1} || DISPLAY_NAME: {2} || NAME: {3} || TYPE: {4}", Bufferino.Caster, Bufferino.Count, Bufferino.DisplayName, Bufferino.Name, Bufferino.Type);
                                    }
                                }
                            }
                        }
                        break;
                    }
                default:
                    GlobalMethods.Print("Unknown command, please retry!", GlobalEnums.MessageType.ERROR);
                    break;
            }
            #endregion
        }
    }
}
