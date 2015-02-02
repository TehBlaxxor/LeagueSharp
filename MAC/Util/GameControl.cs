using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAC.Util
{
    class GameControl
    {
        public static String version = "i'm not good with versions :( - tehblaxxor";
        public static Obj_AI_Hero MyHero = ObjectManager.Player;

        public static void LoadPlugin()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            var plugin = Type.GetType("MAC.Plugin." + ObjectManager.Player.ChampionName);

            if (plugin == null)
            {
                Game.PrintChat(MiscControl.stringColor(ObjectManager.Player.ChampionName, MiscControl.TableColor.Red) + " not found. Loading OrbWalker");
                MiscControl.LoadOrbwalker();
                return;
            }
            else
            {
                Game.PrintChat(MiscControl.stringColor(ObjectManager.Player.ChampionName, MiscControl.TableColor.RoyalBlue) + " loaded, thanks for using MAC.");
            }

            Activator.CreateInstance(plugin);
        }

        private static void CurrentDomainOnUnhandledException(object sender,
            UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Console.WriteLine(((Exception)unhandledExceptionEventArgs.ExceptionObject).Message);
            Game.PrintChat("Fatal error occured! Report on forum!");
        }

        public class EnemyInfo
        {
            public Obj_AI_Hero Player;
            public int LastSeen;
            public int LastPinged;

            public EnemyInfo(Obj_AI_Hero player)
            {
                Player = player;
            }
        }

    }
}
