using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Extra_Utility
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            try
            {
                new Mods.Skin_Changer();
                new Mods.Anti_Flame();
            }
            catch (Exception e)
            {
                Console.Write(e.Data); Console.Write(e.StackTrace); Console.Write(e.Source); Console.Write(e.Message);
            }  
        }
    }
}
