using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace The_Masterpiece
{
    internal class GlobalMethods
    {
        internal static void LoadChampion()
        {
            switch (ObjectManager.Player.ChampionName.ToLowerInvariant())
            {
                case "vayne":
                    {
                        new Plugins.Vayne();
                        GlobalMethods.Print("Vayne loaded! Enjoy your experience!", GlobalEnums.MessageType.NORMAL);
                    }
                    break;
                case "malzahar":
                    {
                        new Plugins.Malzahar();
                        GlobalMethods.Print("Malzahar loaded! Enjoy your experience!", GlobalEnums.MessageType.NORMAL);
                    }
                    break;
                case "lucian":
                    {
                        new Plugins.Lucian();
                        GlobalMethods.Print("Lucian loaded! Enjoy your experience!", GlobalEnums.MessageType.NORMAL);
                    }
                    break;
                default:
                    {
                       GlobalMethods.Print("Champion not supported! If you believe this is in error, please report on the forums.", GlobalEnums.MessageType.WARNING);
                    }
                    break;
            }
        }

        private static readonly Random RandomPos = new Random(DateTime.Now.Millisecond);
        internal static void MoveTo(Vector3 pos)
        {
            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, ObjectManager.Player.ServerPosition.Extend(pos, (RandomPos.NextFloat(0.6f, 1) + 0.2f) * 300));
        }
        public static void Flee()
        {
            MoveTo(Game.CursorPos);
        }


        internal static void Print(string message, GlobalEnums.MessageType messagetype)
        {
            switch (messagetype)
            {
                case GlobalEnums.MessageType.CUSTOM:
                    Game.PrintChat(message);
                    break;
                case GlobalEnums.MessageType.NORMAL:
                    Game.PrintChat("<font color = \"#EEAD0E\">[ MASTERPIECE/MSG ] </font>" + message);
                    break;
                case GlobalEnums.MessageType.WARNING:
                    Game.PrintChat("<font color = \"#FFA500\">[ MASTERPIECE/WRN ] </font>" + message);
                    break;
                case GlobalEnums.MessageType.ERROR:
                    Game.PrintChat("<font color = \"#FF0000\">[ MASTERPIECE/ERR ] </font>" + message);
                    break;
            }
        }
    }

    internal class GlobalEnums
    {
        internal enum MessageType
        {
            CUSTOM,
            NORMAL,
            ERROR,
            WARNING
        }

        internal enum RunningMode
        {
            USER,
            DEVELOPER
        }
    }
}
