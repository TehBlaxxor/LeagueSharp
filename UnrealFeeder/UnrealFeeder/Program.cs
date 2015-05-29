using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Menu = LeagueSharp.Common.Menu;
using MenuItem = LeagueSharp.Common.MenuItem;

namespace UnrealFeeder
{
    class Program
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Menu A;

        public static readonly bool[] CheckForBoughtItem = { false, false, false, false, false };

        public static bool Point1Reached;
        public static bool Point2Reached;
        public static bool CycleStarted;

        public static SpellSlot Ghost;

        public static bool IsDead;
        public static bool SaidDeadStuff;

        public static float LastChat;
        public static float LastLaugh;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            Notifications.AddNotification("Loading Unreal Feeder...", 300);

            A = new Menu("Unreal Feeder", "unrealfeeder", true);
            A.AddItem(new MenuItem("root.shouldfeed", "Feeding Enabled").SetValue(false));
            A.AddItem(new MenuItem("root.feedmode", "Feeding Mode:").SetValue(new StringList(new[]
            { "Closest Enemy", "Bottom Lane", "Middle Lane", "Top Lane", "Wait at Dragon", "Wait at Baron", "Most Fed", "Highest Carrying Potential" }
            )));
            A.AddItem(new MenuItem("root.defaultto", "Default To:").SetValue(new StringList(new[] 
            { "Bottom Lane", "Top Lane", "Middle Lane" }
            )));
            A.AddItem(new MenuItem("hehehe1", " "));
            A.AddItem(new MenuItem("root.chat", "Chat at Baron/Dragon").SetValue(false));
            A.AddItem(new MenuItem("root.chat.delay", "Baron/Dragon Chat Delay").SetValue(new Slider(2000, 0, 10000)));
            A.AddItem(new MenuItem("root.chat2", "Chat on Death").SetValue(true));
            A.AddItem(new MenuItem("hehehe2", " "));
            A.AddItem(new MenuItem("root.laugh", "Laugh").SetValue(true));
            A.AddItem(new MenuItem("root.laugh.delay", "Laugh Delay").SetValue(new Slider(500, 0, 10000)));
            A.AddItem(new MenuItem("hehehe3", " "));
            A.AddItem(new MenuItem("root.items", "Buy Speed Items").SetValue(true));
            A.AddItem(new MenuItem("hehehe4", " "));
            A.AddItem(new MenuItem("root.ghost", "Use Ghost").SetValue(false));
            A.AddItem(new MenuItem("hehehe8", " "));
            A.AddItem(new MenuItem("hehehe5", "Made by TehBlaxxor"));
            A.AddItem(new MenuItem("hehehe6", "v1.0.0.0"));
            A.AddItem(new MenuItem("hehehe7", "Site: joduska.me"));
            A.AddToMainMenu();

            Ghost = Player.GetSpellSlot("summonerhaste");

            Game.OnInput += Game_OnInput;
            Game.OnUpdate += Game_OnUpdate;
            CustomEvents.Game.OnGameEnd += Game_OnGameEnd;

            if (Player.Team == GameObjectTeam.Chaos)
            {
                Notifications.AddNotification("Unreal Feeder: Team Chaos", 300);
            }
            else Notifications.AddNotification("Unreal Feeder: Team Order", 300);

            
        }

        static void Game_OnGameEnd(EventArgs args)
        {
            Game.Say("/all Good game lads! :)");
            
        }

        static void Game_OnUpdate(EventArgs args)
        {
            var feedmode = A.Item("root.feedmode").GetValue<StringList>().SelectedIndex;
            var defaultto = A.Item("root.defaultto").GetValue<StringList>().SelectedIndex;

            Vector3 BotTurningPoint1 = new Vector3(12124, 1726, 52);
            Vector3 BotTurningPoint2 = new Vector3(13502, 3494, 51);

            Vector3 TopTurningPoint1 = new Vector3(1454, 11764, 53);
            Vector3 TopTurningPoint2 = new Vector3(3170, 13632, 53);

            Vector3 Dragon = new Vector3(10064, 4646, -71);
            Vector3 Baron = new Vector3(4964, 10380, -71);

            Vector3 ChaosUniversal = new Vector3(14287f, 14383f, 172f);
            Vector3 OrderUniversal = new Vector3(417f, 469f, 182f);

            string[] MsgList = new[] { "wat", "how?", "What?", "how did you manage to do that?", "mate..", "-_-",
                "why?", "lag", "laaaaag", "oh my god this lag is unreal", "rito pls 500 ping", "god bless my ping",
                "if my ping was my iq i'd be smarter than einstein", "what's up with this lag?", "is the server lagging again?",
            "i call black magic" };

            if (IsDead)
            {
                if (!SaidDeadStuff && A.Item("root.chat2").GetValue<bool>())
                {
                    Random r = new Random();
                    Game.Say("/all " + MsgList[r.Next(0, 14)]);
                    SaidDeadStuff = true;
                }
            }

            if (Player.IsDead)
            {
                IsDead = true;
                CycleStarted = false;
                Point1Reached = false;
                Point2Reached = false;
            }
            else
            {
                IsDead = false;
                SaidDeadStuff = false;

                CycleStarted = true;

                if (Player.InFountain())
                {
                    Point1Reached = false;
                    Point2Reached = false;
                }

                if (Player.Distance(BotTurningPoint1) <= 300 || Player.Distance(TopTurningPoint1) <= 300)
                    Point1Reached = true;
                if (Player.Distance(BotTurningPoint2) <= 300 || Player.Distance(TopTurningPoint2) <= 300)
                    Point2Reached = true;
;
            }

            if (A.Item("root.shouldfeed").GetValue<bool>())
            {
                if ((LastLaugh == 0 || LastLaugh < Game.Time) && A.Item("root.laugh").GetValue<bool>())
                {
                    LastLaugh = Game.Time + A.Item("root.laugh.delay").GetValue<Slider>().Value;
                    Game.Say("/laugh");
                }

                if (Ghost != SpellSlot.Unknown
                    && Player.Spellbook.CanUseSpell(Ghost) == SpellState.Ready
                    && A.Item("root.ghost").GetValue<bool>()
                    && Player.InFountain())
                    Player.Spellbook.CastSpell(Ghost);

                if (A.Item("root.items").GetValue<bool>() && Player.InShop())
                {
                    if (Player.Gold >= 325
                        && !CheckForBoughtItem[0])
                    {
                        Player.BuyItem(ItemId.Boots_of_Speed);
                        CheckForBoughtItem[0] = true;
                    }
                    if (Player.Gold >= 475
                        && CheckForBoughtItem[0]
                        && !CheckForBoughtItem[1])
                    {
                        Player.BuyItem(ItemId.Boots_of_Mobility);
                        CheckForBoughtItem[1] = true;
                    }
                    if (Player.Gold >= 475
                        && CheckForBoughtItem[1]
                        && !CheckForBoughtItem[2])
                    {
                        Player.BuyItem(ItemId.Boots_of_Mobility_Enchantment_Homeguard);
                        CheckForBoughtItem[2] = true;
                    } 
                    if (Player.Gold >= 435
                         && CheckForBoughtItem[2]
                         && !CheckForBoughtItem[3])
                    {
                        Player.BuyItem(ItemId.Amplifying_Tome);
                        CheckForBoughtItem[3] = true;
                    }
                    if (Player.Gold >= (850-435)
                         && CheckForBoughtItem[3]
                         && !CheckForBoughtItem[4])
                    {
                        Player.BuyItem(ItemId.Aether_Wisp);
                        CheckForBoughtItem[4] = true;
                    }
                    if (Player.Gold > 1100 
                        && CheckForBoughtItem[4])
                    {
                        Player.BuyItem(ItemId.Zeal);
                    }

                }
                

                switch (feedmode)
                {
                    case 0:
                        if (HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsDead).OrderBy(x => x.Distance(Player.Position)).FirstOrDefault().IsValidTarget())
                        {
                            Player.IssueOrder(GameObjectOrder.MoveTo,
                                HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsDead).OrderBy(x => x.Distance(Player.Position)).FirstOrDefault());
                        }
                        else
                        {
                            switch (defaultto)
                            {
                                case 0:
                                    {
                                        if (Player.Team == GameObjectTeam.Order)
                                        {
                                            if (!Point1Reached)
                                                Player.IssueOrder(GameObjectOrder.MoveTo, BotTurningPoint1);
                                            else if (!Point2Reached)
                                                Player.IssueOrder(GameObjectOrder.MoveTo, BotTurningPoint2);
                                            else if (Point2Reached)
                                                Player.IssueOrder(GameObjectOrder.MoveTo, ChaosUniversal);
                                        }
                                        else
                                        {
                                            if (!Point2Reached)
                                                Player.IssueOrder(GameObjectOrder.MoveTo, BotTurningPoint2);
                                            else if (!Point1Reached)
                                                Player.IssueOrder(GameObjectOrder.MoveTo, BotTurningPoint1);
                                            else if (Point2Reached)
                                                Player.IssueOrder(GameObjectOrder.MoveTo, OrderUniversal);
                                        }
                                    }
                                    break;
                                case 1:
                                    {
                                        if (Player.Team == GameObjectTeam.Order)
                                        {
                                            if (!Point1Reached)
                                                Player.IssueOrder(GameObjectOrder.MoveTo, TopTurningPoint1);
                                            else if (!Point2Reached)
                                                Player.IssueOrder(GameObjectOrder.MoveTo, TopTurningPoint2);
                                            else if (Point2Reached)
                                                Player.IssueOrder(GameObjectOrder.MoveTo, ChaosUniversal);
                                        }
                                        else
                                        {
                                            if (!Point2Reached)
                                                Player.IssueOrder(GameObjectOrder.MoveTo, TopTurningPoint2);
                                            else if (!Point1Reached)
                                                Player.IssueOrder(GameObjectOrder.MoveTo, TopTurningPoint1);
                                            else if (Point2Reached)
                                                Player.IssueOrder(GameObjectOrder.MoveTo, OrderUniversal);
                                        }
                                    }
                                    break;
                                case 2:
                                    {
                                        if (Player.Team == GameObjectTeam.Order)
                                        {
                                            Player.IssueOrder(GameObjectOrder.MoveTo, ChaosUniversal);
                                        }
                                        else
                                        {
                                            Player.IssueOrder(GameObjectOrder.MoveTo, OrderUniversal);
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case 1:
                        {
                            if (Player.Team == GameObjectTeam.Order)
                            {
                                if (!Point1Reached)
                                    Player.IssueOrder(GameObjectOrder.MoveTo, BotTurningPoint1);
                                else if (!Point2Reached)
                                    Player.IssueOrder(GameObjectOrder.MoveTo, BotTurningPoint2);
                                else if (Point2Reached)
                                    Player.IssueOrder(GameObjectOrder.MoveTo, ChaosUniversal);
                            }
                            else
                            {
                                if (!Point2Reached)
                                    Player.IssueOrder(GameObjectOrder.MoveTo, BotTurningPoint2);
                                else if (!Point1Reached)
                                    Player.IssueOrder(GameObjectOrder.MoveTo, BotTurningPoint1);
                                else if (Point2Reached)
                                    Player.IssueOrder(GameObjectOrder.MoveTo, OrderUniversal);

                            }
                        }
                        break;
                    case 2:
                        {
                            if (Player.Team == GameObjectTeam.Order)
                            {
                                Player.IssueOrder(GameObjectOrder.MoveTo, ChaosUniversal);
                            }
                            else
                            {
                                Player.IssueOrder(GameObjectOrder.MoveTo, OrderUniversal);
                            }
                        }
                        break;
                    case 3:
                        {
                            if (Player.Team == GameObjectTeam.Order)
                            {
                                if (!Point1Reached)
                                    Player.IssueOrder(GameObjectOrder.MoveTo, TopTurningPoint1);
                                else if (!Point2Reached)
                                    Player.IssueOrder(GameObjectOrder.MoveTo, TopTurningPoint2);
                                else if (Point2Reached)
                                    Player.IssueOrder(GameObjectOrder.MoveTo, ChaosUniversal);
                            }
                            else
                            {
                                if (!Point2Reached)
                                    Player.IssueOrder(GameObjectOrder.MoveTo, TopTurningPoint2);
                                else if (!Point1Reached)
                                    Player.IssueOrder(GameObjectOrder.MoveTo, TopTurningPoint1);
                                else if (Point2Reached)
                                    Player.IssueOrder(GameObjectOrder.MoveTo, OrderUniversal);
                            }
                        }
                        break;
                    case 4:
                        {
                            if ((LastChat == 0 || LastChat < Game.Time) && A.Item("root.chat").GetValue<bool>()
                                 && Player.Distance(Dragon) <= 300)
                            {
                                LastChat = Game.Time + A.Item("root.chat.delay").GetValue<Slider>().Value;
                                Game.Say("/all Come to dragon!");
                            }
                            Player.IssueOrder(GameObjectOrder.MoveTo, Dragon);

                        }
                        break;
                    case 5:
                        {
                            if ((LastChat == 0 || LastChat < Game.Time) && A.Item("root.chat").GetValue<bool>()
                                && Player.Distance(Baron) <= 300)
                            {
                                LastChat = Game.Time + A.Item("root.chat.delay").GetValue<Slider>().Value;
                                Game.Say("/all Come to baron!");
                            }
                            Player.IssueOrder(GameObjectOrder.MoveTo, Baron);
                        }
                        break;
                    case 6:
                        {
                            if (HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsDead).OrderBy(x => x.ChampionsKilled).LastOrDefault().IsValidTarget())
                            {
                                Player.IssueOrder(GameObjectOrder.MoveTo,
                                    HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsDead).OrderBy(x => x.ChampionsKilled).LastOrDefault());
                            }
                            else
                            {
                                switch (defaultto)
                                {
                                    case 0:
                                        {
                                            if (Player.Team == GameObjectTeam.Order)
                                            {
                                                if (!Point1Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, BotTurningPoint1);
                                                else if (!Point2Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, BotTurningPoint2);
                                                else if (Point2Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, ChaosUniversal);
                                            }
                                            else
                                            {
                                                if (!Point2Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, BotTurningPoint2);
                                                else if (!Point1Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, BotTurningPoint1);
                                                else if (Point2Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, OrderUniversal);
                                            }
                                        }
                                        break;
                                    case 1:
                                        {
                                            if (Player.Team == GameObjectTeam.Order)
                                            {
                                                if (!Point1Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, TopTurningPoint1);
                                                else if (!Point2Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, TopTurningPoint2);
                                                else if (Point2Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, ChaosUniversal);
                                            }
                                            else
                                            {
                                                if (!Point2Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, TopTurningPoint2);
                                                else if (!Point1Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, TopTurningPoint1);
                                                else if (Point2Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, OrderUniversal);
                                            }
                                        }
                                        break;
                                    case 2:
                                        {
                                            if (Player.Team == GameObjectTeam.Order)
                                            {
                                                Player.IssueOrder(GameObjectOrder.MoveTo, ChaosUniversal);
                                            }
                                            else
                                            {
                                                Player.IssueOrder(GameObjectOrder.MoveTo, OrderUniversal);
                                            }
                                        }
                                        break;

                                }
                            }
                        }
                        break;
                    case 7:
                        {
                            if (HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsDead
                                && (x.ChampionName == "Katarina" || x.ChampionName == "Fiora" || x.ChampionName == "Jinx" || x.ChampionName == "Vayne")
                                ).FirstOrDefault().IsValidTarget())
                            {
                                Player.IssueOrder(GameObjectOrder.MoveTo,
                                    HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsDead
                                && (x.ChampionName == "Katarina" || x.ChampionName == "Fiora" || x.ChampionName == "Jinx" || x.ChampionName == "Vayne")
                                ).FirstOrDefault());
                            }
                            else
                            {
                                switch (defaultto)
                                {
                                    case 0:
                                        {
                                            if (Player.Team == GameObjectTeam.Order)
                                            {
                                                if (!Point1Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, BotTurningPoint1);
                                                else if (!Point2Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, BotTurningPoint2);
                                                else if (Point2Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, ChaosUniversal);
                                            }
                                            else
                                            {
                                                if (!Point2Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, BotTurningPoint2);
                                                else if (!Point1Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, BotTurningPoint1);
                                                else if (Point2Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, OrderUniversal);
                                            }
                                        }
                                        break;
                                    case 1:
                                        {
                                            if (Player.Team == GameObjectTeam.Order)
                                            {
                                                if (!Point1Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, TopTurningPoint1);
                                                else if (!Point2Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, TopTurningPoint2);
                                                else if (Point2Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, ChaosUniversal);
                                            }
                                            else
                                            {
                                                if (!Point2Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, TopTurningPoint2);
                                                else if (!Point1Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, TopTurningPoint1);
                                                else if (Point2Reached)
                                                    Player.IssueOrder(GameObjectOrder.MoveTo, OrderUniversal);
                                            }
                                        }
                                        break;
                                    case 2:
                                        {
                                            if (Player.Team == GameObjectTeam.Order)
                                            {
                                                Player.IssueOrder(GameObjectOrder.MoveTo, ChaosUniversal);
                                            }
                                            else
                                            {
                                                Player.IssueOrder(GameObjectOrder.MoveTo, OrderUniversal);
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
        }

        static void Game_OnInput(GameInputEventArgs args)
        {
            if (args.Input == "/getpos")
            {
                args.Process = false;
                Clipboard.SetText(Player.Position.ToString());
                Notifications.AddNotification("Copied position to clipboard.", 500);
            }
        }

    }
}
