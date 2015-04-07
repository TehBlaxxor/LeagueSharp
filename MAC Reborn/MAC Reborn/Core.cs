using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using MAC_Reborn.Extras;

namespace MAC_Reborn
{
    class Core
    {
        internal static Obj_AI_Hero Player = ObjectManager.Player;
        internal static Menu Config;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static bool DevMode = false;
        internal static string Version = "1.0.0.0";

        internal static void Initialize()
        {
            try
            {
                Game.OnInput += Game_OnInput;
                Game.PrintChat("<font color = \"#00E5EE\">MAC:r by</font> <font color = \"#FF3300\">TehBlaxxor</font> <font color = \"#00E5EE\">is attempting to initialize..</font>");

                try
                {
                    var assembly = Type.GetType("MAC_Reborn.Champions." + Player.ChampionName);

                    if (assembly != null)
                    {
                        DynamicInitializer.NewInstance(assembly);
                        Core.PrintChat("<font color = \"#FF3300\">" + Player.ChampionName + "</font> <font color = \"#00E5EE\">was successfully loaded!</font>", PrintType.Custom);
                    }
                        
                }
                catch (NullReferenceException)
                {
                    Game.PrintChat("<font color = \"#FF3300\">" + Player.ChampionName + "</font> <font color = \"#00E5EE\">is not supported! Orbwalker loaded!</font>");
                    Config = new Menu("MAC:r - " + Player.ChampionName, "menu", true);

                    Config.AddSubMenu(new Menu("Target Selector", "TS"));
                    Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));

                    TargetSelector.AddToMenu(Config.SubMenu("TS"));
                    Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));
                }



                Config.AddToMainMenu();
            }
            catch (Exception e)
            {
                    Core.WriteException(e);
            }
        }

        internal static void Game_OnInput(GameInputEventArgs args)
        {
            if (args.Input.StartsWith("/"))
            {
                string[] splits = args.Input.Split(' ');
                if (splits[0] == "/~!dev")
                {
                    DevMode = true;
                    Core.PrintChat("Developer Mode is now active!", PrintType.Message);
                }
                if (splits[0] == "/load" && DevMode)
                {
                    var toLoad = Type.GetType("MAC_Reborn.Champions." + splits[1]);

                    if (toLoad != null)
                        DynamicInitializer.NewInstance(toLoad);
                }

                if (splits[0] == "/version")
                {
                    Core.PrintChat("You are using version " + Version, PrintType.Message);
                }

            }
        }

        internal enum PrintType
        {
            Message,
            Warning,
            Error,
            Custom
        }

        internal static void PrintChat(string message, PrintType type)
        {
            if (type == PrintType.Error)
            {
                Game.PrintChat("<font color = \"#FF0000\">" + message + "</font>");
            }
            else if (type == PrintType.Message)
            {
                Game.PrintChat("<font color = \"#00E5EE\">" + message + "</font>");
            }
            else if (type == PrintType.Warning)
            {
                Game.PrintChat("<font color = \"#FFA500\">" + message + "</font>");
            }
            else if (type == PrintType.Custom)
            {
                Game.PrintChat(message);
            }
            else Console.WriteLine("lol ur so bad lrn 2 code dumass");
        }

        internal static void WriteException(Exception exception)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("============EXCEPTION=CAUGHT============");
            Console.WriteLine("========================================");
            Console.WriteLine(exception);
            Console.WriteLine("==========PRINTING=STACKTRACE===========");
            Console.WriteLine(exception.StackTrace);
            Console.WriteLine("============PRINTING=MESSAGE============");
            Console.WriteLine(exception.Message);
            Console.WriteLine("=============PRINTING=DATA==============");
            Console.WriteLine(exception.Data);
            Console.WriteLine("========================================");
            Console.WriteLine("============END=OF=EXCEPTION============");
            Console.WriteLine("========================================");
        }
    }
}
