using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Extra_Utility.Mods
{
    internal class Anti_Flame : ModBase
    {
        public override string ModName { get { return "Anti Flame"; } }
        public override string ModVersion { get { return "1.0.0"; } }
        public Anti_Flame()
        {
            Notifications.OnModLoaded(ModName);
        }

        public static bool ModEnabled { get { return Config.Item("eu.Anti Flame.enabled").GetValue<bool>(); } }

        public override void LoadSettings(Menu menu)
        {
            menu.AddItem(new MenuItem("eu.Anti Flame.enabled", "Enabled").SetValue(true));
        }

        public override void Game_OnInput(LeagueSharp.GameInputEventArgs args)
        {
            if (!ModEnabled)
                return;

            if (args.Input.ToLowerInvariant() == "fuck")
            {
                args.Process = false;
                Game.Say("cookies");
            }
            else if (args.Input.ToLowerInvariant().Contains("fuck"))
            {
                args.Process = false;
                Game.Say("i like ponies");
            }
            else if (args.Input.ToLowerInvariant().Contains("shit"))
            {
                args.Process = false;
                Game.Say("pancake");
            }
            else if (args.Input.ToLowerInvariant().Contains("nigger"))
            {
                args.Process = false;
                Game.Say("i am such a nice person!");
            }
            else if (args.Input.ToLowerInvariant().Contains("report"))
            {
                args.Process = false;
                Game.Say("good job guys, we'll win this!");
            }
            else if (args.Input.ToLowerInvariant().Contains("ass"))
            {
                args.Process = false;
                Game.Say("i do like me some ice cream");
            }
            else if (args.Input.ToLowerInvariant().Contains("pussy")
                || args.Input.ToLowerInvariant().Contains("vagina")
                || args.Input.ToLowerInvariant().Contains("dick")
                || args.Input.ToLowerInvariant().Contains("penis"))
            {
                args.Process = false;
                Game.Say("hi guys");
            }
            else if (args.Input.ToUpperInvariant() == args.Input)
            {
                args.Process = false;
                Game.Say(args.Input.Substring(0, 1).ToUpperInvariant() + args.Input.Substring(1).ToLowerInvariant());
            }
        }
    }
}
