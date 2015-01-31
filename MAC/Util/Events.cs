using LeagueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAC.Util
{
    class Events
    {
        public class Game
        {
            public delegate void OnGameStartet(EventArgs args);

            static Game()
            {
                LeagueSharp.Game.OnGameProcessPacket += OnGameProcessPacket;
            }

            public static event OnGameStartet OnGameStart;

            private static void OnGameProcessPacket(GamePacketEventArgs args)
            {
                if (LeagueSharp.Game.Mode != GameMode.Running || OnGameStart == null)
                    return;
                OnGameStart(new EventArgs());
                LeagueSharp.Game.OnGameProcessPacket -= OnGameProcessPacket;
            }
        }
    }
}
