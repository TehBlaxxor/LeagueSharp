using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Item = LeagueSharp.Common.Items.Item;

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
                        Messages.OnAssemblyLoad();
                    }
                    break;
                case "malzahar":
                    {
                        new Plugins.Malzahar();
                        Messages.OnAssemblyLoad();
                    }
                    break;
                case "lucian":
                    {
                        new Plugins.Lucian();
                        Messages.OnAssemblyLoad();
                    }
                    break;
                //case "ryze":
                //    {
                        //new Plugins.Ryze();
                        //Messages.OnAssemblyLoad();
                //    }
                //    break;
                default:
                    {
                       GlobalMethods.Print("Champion not supported! If you believe this is in error, please report on the forums.", GlobalEnums.MessageType.WARNING);
                    }
                    break;
            }
        }

        public static Item Tear = new Item(3070);
        public static Item ScarTear = new Item(3073);
        public static Item Archangel = new Item(3003);
        public static Item ScarArchangel = new Item(3007);
        public static Item Manamune = new Item(3004);
        public static Item ScarManamune = new Item(3008);
        public static bool StackingItemOwned(Obj_AI_Hero Hero)
        {
            return Tear.IsOwned(Hero) || Archangel.IsOwned(Hero) || Manamune.IsOwned(Hero)
                    || ScarTear.IsOwned(Hero) || ScarArchangel.IsOwned(Hero) || ScarManamune.IsOwned(Hero);
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
