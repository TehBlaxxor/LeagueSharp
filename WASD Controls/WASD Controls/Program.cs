using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace WASD_Controls
{
    class Program
    {
        public static Menu Config;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static bool Enabled = true;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            Game.OnInput += Game_OnInput;
        }

        static void Game_OnInput(GameInputEventArgs args)
        {
            switch (args.Input)
            {
                case "#disable":
                    Enabled = false;
                    break;
                case "#enable":
                    Enabled = true;
                    break;
                default:
                    return;
            }
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            Config = new Menu("WASD Controls", "wasd", true);

            Config.AddItem(new MenuItem("w", "Up").SetValue(new KeyBind('Y', KeyBindType.Press)));
            Config.AddItem(new MenuItem("a", "Left").SetValue(new KeyBind('G', KeyBindType.Press)));
            Config.AddItem(new MenuItem("s", "Down").SetValue(new KeyBind('H', KeyBindType.Press)));
            Config.AddItem(new MenuItem("d", "Right").SetValue(new KeyBind('J', KeyBindType.Press)));
            Config.AddItem(new MenuItem("autodisable", "Automatically Disable Spells").SetValue(true));
            Config.AddItem(new MenuItem("summoner", "Disable D Summoner Spell").SetValue(false));
            Config.AddItem(new MenuItem("summoner2", "Disable F Summoner Spell").SetValue(false));
            Config.AddItem(new MenuItem("summoner3", "Disable Q").SetValue(false));
            Config.AddItem(new MenuItem("summoner4", "Disable W").SetValue(false));
            Config.AddItem(new MenuItem("summoner5", "Disable E").SetValue(false));
            Config.AddItem(new MenuItem("summoner6", "Disable R").SetValue(false));
            Config.AddToMainMenu();

            Game.PrintChat("WASD Controls Loaded!");

            Game.OnUpdate += Game_OnUpdate;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        public static bool Check(string menuitem)
        {
            return (Config.Item(menuitem).GetValue<KeyBind>().Key == 'Q'
                || Config.Item(menuitem).GetValue<KeyBind>().Key == 'W'
                || Config.Item(menuitem).GetValue<KeyBind>().Key == 'E'
                || Config.Item(menuitem).GetValue<KeyBind>().Key == 'R'
                || Config.Item(menuitem).GetValue<KeyBind>().Key == 'D'
                || Config.Item(menuitem).GetValue<KeyBind>().Key == 'F') && Config.Item(menuitem).GetValue<KeyBind>().Active;
        }

        static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!sender.Owner.IsMe || Enabled == false)
                return;

            if (Config.Item("autodisable").GetValue<bool>())
            {
                if (Check("w") || Check("a") || Check("s") || Check("d"))
                    args.Process = false;
            }

            if (args.Slot == SpellSlot.Summoner1 && Config.Item("summoner").GetValue<bool>())
            {
                args.Process = false;
            }
            else if (args.Slot == SpellSlot.Summoner2 && Config.Item("summoner2").GetValue<bool>())
            {
                args.Process = false;
            }
            else if (args.Slot == SpellSlot.Q && Config.Item("summoner3").GetValue<bool>())
            {
                args.Process = false;
            }
            else if (args.Slot == SpellSlot.W && Config.Item("summoner4").GetValue<bool>())
            {
                args.Process = false;
            }
            else if (args.Slot == SpellSlot.E && Config.Item("summoner5").GetValue<bool>())
            {
                args.Process = false;
            }
            else if (args.Slot == SpellSlot.R && Config.Item("summoner6").GetValue<bool>())
            {
                args.Process = false;
            }
        }

        static void Game_OnUpdate(EventArgs args)
        {
            if (Enabled == false)
                return;

            if (Config.Item("w").GetValue<KeyBind>().Active 
                && Config.Item("a").GetValue<KeyBind>().Active)
            {
                MoveTo(-200, 200, 0);
            }
            else if (Config.Item("w").GetValue<KeyBind>().Active 
                && Config.Item("d").GetValue<KeyBind>().Active)
            {
                MoveTo(200, 200, 0);
            }
            else if (Config.Item("s").GetValue<KeyBind>().Active 
                && Config.Item("a").GetValue<KeyBind>().Active)
            {
                MoveTo(-200, -200, 0);
            }
            else if (Config.Item("s").GetValue<KeyBind>().Active 
                && Config.Item("d").GetValue<KeyBind>().Active)
            {
                MoveTo(200, -200, 0);
            }
            else if (Config.Item("w").GetValue<KeyBind>().Active)
            {
                MoveTo(0, 200, 0);
            }
            else if (Config.Item("s").GetValue<KeyBind>().Active)
            {
                MoveTo(0, -200, 0);
            }
            else if (Config.Item("d").GetValue<KeyBind>().Active)
            {
                MoveTo(200, 0, 0);
            }
            else if (Config.Item("a").GetValue<KeyBind>().Active)
            {
                MoveTo(-200, 0, 0);
            }
        }

        public static void MoveTo(float X, float Y, float Z)
        {
            Vector3 realpos = new Vector3(Player.Position.X + X, Player.Position.Y + Y, Player.Position.Z + Z);

            Player.IssueOrder(GameObjectOrder.MoveTo, realpos);
        }

    }
}
