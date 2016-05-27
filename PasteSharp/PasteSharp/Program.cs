using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Windows.Forms;

namespace PasteSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Game.OnInput += Game_OnInput;
        }

        private static void Game_OnInput(GameInputEventArgs args)
        {
            if (args.Input.StartsWith("/paste ") || args.Input.StartsWith("/p "))
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(Clipboard.GetText() + " " + splits[1]);
            }
            if (args.Input == "/paste" || args.Input == "/p")
            {
                args.Process = false;
                Game.Say(Clipboard.GetText());
            }
            if (args.Input.Contains("*paste*") || args.Input.Contains("*p*"))
            {
                args.Process = false;
                Game.Say(args.Input.Replace("*paste*", Clipboard.GetText()).Replace("*p*", Clipboard.GetText()));
            }
        }
    }
}
