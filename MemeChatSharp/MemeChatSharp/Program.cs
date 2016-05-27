using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using SharpDX;
using LeagueSharp.Common;

namespace MemeChatSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Game.OnInput += Game_OnInput;
        }

        private static void Game_OnInput(GameInputEventArgs args)
        {
            if (args.Input.StartsWith("/lenny "))
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[1] + " ( ͡° ͜ʖ ͡°)"); //╠═══╣Lets build a ladder╠═══╣
            }
            if (args.Input.StartsWith("/all lenny "))// ¯\_(ツ)_/¯
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[2] + " ( ͡° ͜ʖ ͡°)");
            }
            if (args.Input == "/ladder")
            {
                args.Process = false;
                Game.Say("╠═══╣Lets build a ladder╠═══╣");
            }
            if (args.Input == "/all ladder")
            {
                args.Process = false;
                Game.Say("/all ╠═══╣Lets build a ladder╠═══╣");
            }
            if (args.Input.StartsWith("/shrug "))
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[1] + @" ¯\_(ツ)_/¯"); //╠═══╣Lets build a ladder╠═══╣
            }
            if (args.Input.StartsWith("/all shrug "))// ¯\_(ツ)_/¯
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[2] + @" ¯\_(ツ)_/¯");
            }
            // ̿̿ ̿̿ ̿̿ ̿'̿'\̵͇̿̿\з= ( ▀ ͜͞ʖ▀) =ε/̵͇̿̿/’̿’̿ ̿ ̿̿ ̿̿ ̿̿ - terminator
            // ʕ•ᴥ•ʔ - bear
            // (▀̿Ĺ̯▀̿ ̿) - agent
            // (ノಠ益ಠ)ノ彡┻━┻ - tableflip
            // (•_•) ( •_•)>⌐■-■ (⌐■_■) - glasseson
            // ▄︻̷̿┻̿═━一 - sniper
            // ( ͡°( ͡° ͜ʖ( ͡° ͜ʖ ͡°)ʖ ͡°) ͡°) - lennyarmy
            // (ง ͠° ͟ل͜ ͡°)ง - tusl
            if (args.Input.StartsWith("/terminator "))
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[1] + @"  ̿̿ ̿̿ ̿'̿'\̵͇̿̿\з= ( ▀ ͜͞ʖ▀) =ε/̵͇̿̿/’̿’̿ ̿ ̿̿ ̿̿ ̿̿ "); //╠═══╣Lets build a ladder╠═══╣
            }
            if (args.Input.StartsWith("/all terminator "))// ¯\_(ツ)_/¯
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[2] + @"  ̿̿ ̿̿ ̿'̿'\̵͇̿̿\з= ( ▀ ͜͞ʖ▀) =ε/̵͇̿̿/’̿’̿ ̿ ̿̿ ̿̿ ̿̿ ");
            }
            if (args.Input.StartsWith("/bear "))
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[1] + @" ʕ•ᴥ•ʔ"); //╠═══╣Lets build a ladder╠═══╣
            }
            if (args.Input.StartsWith("/all bear "))// ¯\_(ツ)_/¯
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[2] + @" ʕ•ᴥ•ʔ");
            }
            if (args.Input.StartsWith("/glasses "))
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[1] + @" (▀̿Ĺ̯▀̿ ̿)"); //╠═══╣Lets build a ladder╠═══╣
            }
            if (args.Input.StartsWith("/all glasses "))// ¯\_(ツ)_/¯
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[2] + @" (▀̿Ĺ̯▀̿ ̿)");
            }
            if (args.Input.StartsWith("/tableflip ") || args.Input.StartsWith("/tf "))
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[1] + @" (ノಠ益ಠ)ノ彡┻━┻"); //╠═══╣Lets build a ladder╠═══╣
            }
            if (args.Input.StartsWith("/all tableflip ") || args.Input.StartsWith("/all tf "))// ¯\_(ツ)_/¯
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[2] + @" (ノಠ益ಠ)ノ彡┻━┻");
            }
            if (args.Input.StartsWith("/glasseson "))
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[1] + @" (•_•) ( •_•)>⌐■-■ (⌐■_■)"); //╠═══╣Lets build a ladder╠═══╣
            }
            if (args.Input.StartsWith("/all glasseson "))// ¯\_(ツ)_/¯
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[2] + @" (•_•) ( •_•)>⌐■-■ (⌐■_■)");
            }
            if (args.Input.StartsWith("/sniper "))
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[1] + @" ▄︻̷̿┻̿═━一"); //╠═══╣Lets build a ladder╠═══╣
            }
            if (args.Input.StartsWith("/all sniper "))// ¯\_(ツ)_/¯
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[2] + @" ▄︻̷̿┻̿═━一");
            }
            if (args.Input.StartsWith("/lennyarmy "))
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[1] + @" ( ͡°( ͡° ͜ʖ( ͡° ͜ʖ ͡°)ʖ ͡°) ͡°)"); //╠═══╣Lets build a ladder╠═══╣
            }
            if (args.Input.StartsWith("/all lennyarmy "))// ¯\_(ツ)_/¯
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[2] + @" ( ͡°( ͡° ͜ʖ( ͡° ͜ʖ ͡°)ʖ ͡°) ͡°)");
            }
            if (args.Input.StartsWith("/tusl "))
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[1] + @" (ง ͠° ͟ل͜ ͡°)ง"); //╠═══╣Lets build a ladder╠═══╣
            }
            if (args.Input.StartsWith("/all tusl "))// ¯\_(ツ)_/¯
            {
                args.Process = false;
                string[] splits = args.Input.Split(' ');
                Game.Say(splits[2] + @" (ง ͠° ͟ل͜ ͡°)ง");
            }
        }
    }
}
