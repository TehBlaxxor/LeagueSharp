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
    internal class Skin_Changer : ModBase
    {
        public override string ModName { get { return "Skin Changer"; } }
        public override string ModVersion { get { return "1.0.0"; } }
        public static string Model;
        public static int Skin;
        public static Menu Config;

        #region Models - Credit to Trees
        public static List<string> ModelList = new List<string>
        {
            "Aatrox",
            "Ahri",
            "Akali",
            "Alistar",
            "Amumu",
            "AncientGolem",
            "Anivia",
            "AniviaEgg",
            "AniviaIceblock",
            "Annie",
            "AnnieTibbers",
            "ARAMChaosNexus",
            "ARAMChaosTurretFront",
            "ARAMChaosTurretInhib",
            "ARAMChaosTurretNexus",
            "ARAMChaosTurretShrine",
            "ARAMOrderNexus",
            "ARAMOrderTurretFront",
            "ARAMOrderTurretInhib",
            "ARAMOrderTurretNexus",
            "ARAMOrderTurretShrine",
            "AramSpeedShrine",
            "AscRelic",
            "AscWarpIcon",
            "AscXerath",
            "Ashe",
            "Azir",
            "AzirSoldier",
            "AzirSunDisc",
            "AzirTowerClicker",
            "AzirUltSoldier",
            "Bard",
            "BardFollower",
            "BardHealthShrine",
            "BardPickup",
            "BardPickupNoIcon",
            "Blitzcrank",
            "BlueTrinket",
            "Blue_Minion_Basic",
            "Blue_Minion_MechCannon",
            "Blue_Minion_MechMelee",
            "Blue_Minion_Wizard",
            "Brand",
            "Braum",
            "brush_A_SR",
            "brush_B_SR",
            "brush_C_SR",
            "brush_D_SR",
            "brush_E_SR",
            "brush_F_SR",
            "brush_SRU_A",
            "brush_SRU_B",
            "brush_SRU_C",
            "brush_SRU_D",
            "brush_SRU_E",
            "brush_SRU_F",
            "brush_SRU_G",
            "brush_SRU_H",
            "brush_SRU_I",
            "brush_SRU_J",
            "Caitlyn",
            "CaitlynTrap",
            "Cassiopeia",
            "Cassiopeia_Death",
            "ChaosInhibitor",
            "ChaosInhibitor_D",
            "ChaosNexus",
            "ChaosTurretGiant",
            "ChaosTurretNormal",
            "ChaosTurretShrine",
            "ChaosTurretTutorial",
            "ChaosTurretWorm",
            "ChaosTurretWorm2",
            "Chogath",
            "Corki",
            "crystal_platform",
            "Darius",
            "DestroyedInhibitor",
            "DestroyedNexus",
            "DestroyedTower",
            "Diana",
            "Dragon",
            "Draven",
            "DrMundo",
            "Elise",
            "EliseSpider",
            "EliseSpiderling",
            "Evelynn",
            "Ezreal",
            "Ezreal_cyber_1",
            "Ezreal_cyber_2",
            "Ezreal_cyber_3",
            "FiddleSticks",
            "Fiora",
            "Fizz",
            "FizzBait",
            "FizzShark",
            "Galio",
            "Gangplank",
            "Garen",
            "GhostWard",
            "GiantWolf",
            "Gnar",
            "GnarBig",
            "Golem",
            "GolemODIN",
            "Gragas",
            "Graves",
            "GreatWraith",
            "HA_AP_BannerMidBridge",
            "HA_AP_BridgeLaneStatue",
            "HA_AP_Chains",
            "HA_AP_Chains_Long",
            "HA_AP_ChaosTurret",
            "HA_AP_ChaosTurret2",
            "HA_AP_ChaosTurret3",
            "HA_AP_ChaosTurretRubble",
            "HA_AP_ChaosTurretShrine",
            "HA_AP_ChaosTurretTutorial",
            "HA_AP_Cutaway",
            "HA_AP_HealthRelic",
            "HA_AP_Hermit",
            "HA_AP_Hermit_Robot",
            "HA_AP_HeroTower",
            "HA_AP_OrderCloth",
            "HA_AP_OrderShrineTurret",
            "HA_AP_OrderTurret",
            "HA_AP_OrderTurret2",
            "HA_AP_OrderTurret3",
            "HA_AP_OrderTurretRubble",
            "HA_AP_OrderTurretTutorial",
            "HA_AP_PeriphBridge",
            "HA_AP_Poro",
            "HA_AP_PoroSpawner",
            "HA_AP_ShpNorth",
            "HA_AP_ShpSouth",
            "HA_AP_Viking",
            "HA_FB_HealthRelic",
            "Hecarim",
            "Heimerdinger",
            "HeimerTBlue",
            "HeimerTYellow",
            "Irelia",
            "Janna",
            "JarvanIV",
            "JarvanIVStandard",
            "JarvanIVWall",
            "Jax",
            "Jayce",
            "Jinx",
            "JinxMine",
            "Kalista",
            "KalistaAltar",
            "KalistaSpawn",
            "Karma",
            "Karthus",
            "Kassadin",
            "Katarina",
            "Kayle",
            "Kennen",
            "Khazix",
            "KingPoro",
            "KINGPORO_HiddenUnit",
            "KINGPORO_PoroFollower",
            "KogMaw",
            "KogMawDead",
            "Leblanc",
            "LeeSin",
            "Leona",
            "LesserWraith",
            "Lissandra",
            "Lizard",
            "LizardElder",
            "Lucian",
            "Lulu",
            "LuluCupcake",
            "LuluDragon",
            "LuluFaerie",
            "LuluKitty",
            "LuluLadybug",
            "LuluPig",
            "LuluSnowman",
            "LuluSquill",
            "Lux",
            "Malphite",
            "Malzahar",
            "MalzaharVoidling",
            "Maokai",
            "MaokaiSproutling",
            "MasterYi",
            "MissFortune",
            "MonkeyKing",
            "MonkeyKingClone",
            "MonkeyKingFlying",
            "Mordekaiser",
            "Morgana",
            "Nami",
            "Nasus",
            "NasusUlt",
            "Nautilus",
            "Nidalee",
            "Nidalee_Cougar",
            "Nidalee_Spear",
            "Nocturne",
            "Nunu",
            "OdinBlueSuperminion",
            "OdinCenterRelic",
            "OdinChaosTurretShrine",
            "OdinClaw",
            "OdinCrane",
            "OdinMinionGraveyardPortal",
            "OdinMinionSpawnPortal",
            "OdinNeutralGuardian",
            "OdinOpeningBarrier",
            "OdinOrderTurretShrine",
            "OdinQuestBuff",
            "OdinQuestIndicator",
            "OdinRedSuperminion",
            "OdinRockSaw",
            "OdinShieldRelic",
            "OdinSpeedShrine",
            "OdinTestCubeRender",
            "Odin_Blue_Minion_Caster",
            "Odin_Drill",
            "Odin_Lifts_Buckets",
            "Odin_Lifts_Crystal",
            "Odin_Minecart",
            "Odin_Red_Minion_Caster",
            "Odin_skeleton",
            "Odin_SoG_Chaos",
            "Odin_SOG_Chaos_Crystal",
            "Odin_SoG_Order",
            "Odin_SOG_Order_Crystal",
            "Odin_Windmill_Gears",
            "Odin_Windmill_Propellers",
            "Olaf",
            "OlafAxe",
            "OrderInhibitor",
            "OrderInhibitor_D",
            "OrderNexus",
            "OrderTurretAngel",
            "OrderTurretDragon",
            "OrderTurretNormal",
            "OrderTurretNormal2",
            "OrderTurretShrine",
            "OrderTurretTutorial",
            "Orianna",
            "OriannaBall",
            "OriannaNoBall",
            "Pantheon",
            "Poppy",
            "Quinn",
            "QuinnValor",
            "Rammus",
            "RammusDBC",
            "RammusPB",
            "redDragon",
            "Red_Minion_Basic",
            "Red_Minion_MechCannon",
            "Red_Minion_MechMelee",
            "Red_Minion_Wizard",
            "RekSai",
            "RekSaiTunnel",
            "Renekton",
            "Rengar",
            "Riven",
            "Rumble",
            "Ryze",
            "Sejuani",
            "Shaco",
            "ShacoBox",
            "Shen",
            "Shop",
            "ShopKeeper",
            "ShopMale",
            "Shyvana",
            "ShyvanaDragon",
            "SightWard",
            "Singed",
            "Sion",
            "Sivir",
            "Skarner",
            "SmallGolem",
            "Sona",
            "SonaDJGenre01",
            "SonaDJGenre02",
            "SonaDJGenre03",
            "Soraka",
            "SpellBook1",
            "SRUAP_Building",
            "SRUAP_ChaosInhibitor",
            "sruap_chaosinhibitor_rubble",
            "SRUAP_ChaosNexus",
            "Sruap_Chaosnexus_Rubble",
            "Sruap_Esports_Banner",
            "sruap_flag",
            "SRUAP_MageCrystal",
            "sruap_mage_vines",
            "SRUAP_OrderInhibitor",
            "sruap_orderinhibitor_rubble",
            "SRUAP_OrderNexus",
            "Sruap_Ordernexus_Rubble",
            "Sruap_Pali_Statue_Banner",
            "SRUAP_Turret_Chaos1",
            "sruap_turret_chaos1_rubble",
            "SRUAP_Turret_Chaos2",
            "SRUAP_Turret_Chaos3",
            "SRUAP_Turret_Chaos3_Test",
            "SRUAP_Turret_Chaos4",
            "SRUAP_Turret_Chaos5",
            "SRUAP_Turret_Order1",
            "SRUAP_Turret_Order1_Rubble",
            "SRUAP_Turret_Order2",
            "SRUAP_Turret_Order3",
            "SRUAP_Turret_Order3_Test",
            "SRUAP_Turret_Order4",
            "SRUAP_Turret_Order5",
            "Sru_Antlermouse",
            "SRU_Baron",
            "SRU_BaronSpawn",
            "SRU_Bird",
            "SRU_Blue",
            "SRU_BlueMini",
            "SRU_BlueMini2",
            "Sru_Butterfly",
            "SRU_ChaosMinionMelee",
            "SRU_ChaosMinionRanged",
            "SRU_ChaosMinionSiege",
            "SRU_ChaosMinionSuper",
            "Sru_Crab",
            "Sru_CrabWard",
            "SRU_Dragon",
            "Sru_Dragonfly",
            "sru_dragon_prop",
            "Sru_Duck",
            "SRU_Es_Banner",
            "Sru_Es_Bannerplatform_Chaos",
            "Sru_Es_Bannerplatform_Order",
            "Sru_Es_Bannerwall_Chaos",
            "Sru_Es_Bannerwall_Order",
            "SRU_Gromp",
            "Sru_Gromp_Prop",
            "SRU_Krug",
            "SRU_KrugMini",
            "Sru_Lizard",
            "SRU_Murkwolf",
            "SRU_MurkwolfMini",
            "SRU_OrderMinionMelee",
            "SRU_OrderMinionRanged",
            "SRU_OrderMinionSiege",
            "SRU_OrderMinionSuper",
            "SRU_Razorbeak",
            "SRU_RazorbeakMini",
            "SRU_Red",
            "SRU_RedMini",
            "SRU_RiverDummy",
            "Sru_Snail",
            "SRU_SnailSpawner",
            "SRU_Spiritwolf",
            "sru_storekeepernorth",
            "sru_storekeepersouth",
            "SRU_WallVisionBearer",
            "SummonerBeacon",
            "Summoner_Rider_Chaos",
            "Summoner_Rider_Order",
            "Swain",
            "SwainBeam",
            "SwainNoBird",
            "SwainRaven",
            "Syndra",
            "SyndraOrbs",
            "SyndraSphere",
            "Talon",
            "Taric",
            "Teemo",
            "TeemoMushroom",
            "TestCube",
            "TestCubeRender",
            "TestCubeRender10Vision",
            "TestCubeRenderwCollision",
            "Thresh",
            "ThreshLantern",
            "Tristana",
            "Trundle",
            "TrundleWall",
            "Tryndamere",
            "TT_Brazier",
            "TT_Buffplat_L",
            "TT_Buffplat_R",
            "TT_Chains_Bot_Lane",
            "TT_Chains_Order_Base",
            "TT_Chains_Order_Periph",
            "TT_Chains_Xaos_Base",
            "TT_ChaosInhibitor",
            "TT_ChaosInhibitor_D",
            "TT_ChaosTurret1",
            "TT_ChaosTurret2",
            "TT_ChaosTurret3",
            "TT_ChaosTurret4",
            "TT_DummyPusher",
            "TT_Flytrap_A",
            "TT_Nexus_Gears",
            "TT_NGolem",
            "TT_NGolem2",
            "TT_NWolf",
            "TT_NWolf2",
            "TT_NWraith",
            "TT_NWraith2",
            "TT_OrderInhibitor",
            "TT_OrderInhibitor_D",
            "TT_OrderTurret1",
            "TT_OrderTurret2",
            "TT_OrderTurret3",
            "TT_OrderTurret4",
            "TT_Relic",
            "TT_Shopkeeper",
            "TT_Shroom_A",
            "TT_SpeedShrine",
            "TT_Speedshrine_Gears",
            "TT_Spiderboss",
            "TT_SpiderLayer_Web",
            "TT_Tree1",
            "TT_Tree_A",
            "Tutorial_Blue_Minion_Basic",
            "Tutorial_Blue_Minion_Wizard",
            "Tutorial_Red_Minion_Basic",
            "Tutorial_Red_Minion_Wizard",
            "TwistedFate",
            "Twitch",
            "Udyr",
            "UdyrPhoenix",
            "UdyrPhoenixUlt",
            "UdyrTiger",
            "UdyrTigerUlt",
            "UdyrTurtle",
            "UdyrTurtleUlt",
            "UdyrUlt",
            "Urf",
            "Urgot",
            "Varus",
            "Vayne",
            "Veigar",
            "Velkoz",
            "Vi",
            "Viktor",
            "ViktorSingularity",
            "VisionWard",
            "Vladimir",
            "VoidGate",
            "VoidSpawn",
            "VoidSpawnTracer",
            "Volibear",
            "Warwick",
            "wolf",
            "Worm",
            "Wraith",
            "Xerath",
            "XerathArcaneBarrageLauncher",
            "XinZhao",
            "Yasuo",
            "YellowTrinket",
            "YellowTrinketUpgrade",
            "Yonkey",
            "Yorick",
            "YorickDecayedGhoul",
            "YorickRavenousGhoul",
            "YorickSpectralGhoul",
            "YoungLizard",
            "Zac",
            "ZacRebirthBloblet",
            "Zed",
            "ZedShadow",
            "Ziggs",
            "Zilean",
            "Zyra",
            "ZyraGraspingPlant",
            "ZyraPassive",
            "ZyraSeed",
            "ZyraThornPlant"
        };
        #endregion

        public Skin_Changer()
        {
            Notifications.OnModLoaded(ModName);
        }
        public override void LoadMenu()
        {
            Config = new Menu("EU - " + ModName, "eu." + ModName.Replace(" ", string.Empty).ToLowerInvariant() + new Random().Next(0, 133333337), true);
            Config.AddItem(new MenuItem("eu.Skin Changer.enabled", "Enabled").SetValue(new StringList(new[] {" Disabled "})));
            Config.AddItem(new MenuItem("version" + new Random().Next(0, 133333337), "Version: " + ModVersion));
            Config.AddItem(new MenuItem("site" + new Random().Next(0, 133333337), "Visit joduska.me!"));
            Config.AddToMainMenu();

        }

        public static bool ModEnabled { get { return Config.Item("eu.Skin Changer.enabled").GetValue<bool>(); } }

        public override void Game_OnInput(LeagueSharp.GameInputEventArgs args)
        {
            /*if (!Config.Item("eu.Skin Changer.enabled").GetValue<bool>() || args.Input != string.Empty)
            {
                Notifications.OnModelChanged("nob");
                foreach (var item in Config.Items)
                {
                    Console.Write(item.Name + "|" + item.DisplayName);
                }
                return;
            }
            */
            try
            {
                var trial = args.Input + " ";
                if (args.Input.StartsWith("#"))
                {
                    args.Process = false;
                    string[] cmdargs = trial.Split(' ');

                    if (cmdargs[0].Equals("#setskin"))
                    {
                        if (cmdargs[2] != string.Empty)
                        {
                            foreach (var hero in HeroManager.AllHeroes)
                            {
                                if (cmdargs[2].ToLowerInvariant() == hero.ChampionName.ToLowerInvariant()
                                    || hero.Name.ToLowerInvariant().StartsWith(cmdargs[2].ToLowerInvariant()))
                                {
                                    hero.SetSkin(hero.BaseSkinName, Convert.ToInt32(cmdargs[1]));
                                    Notifications.OnSkinChanged(Convert.ToInt32(cmdargs[1]).ToString());
                                }
                            }
                        }
                        else
                        {
                            Player.SetSkin(Player.BaseSkinName, Convert.ToInt32(cmdargs[1]));
                            Model = Player.BaseSkinName;
                            Skin = Convert.ToInt32(cmdargs[1]);
                            Notifications.OnSkinChanged(Convert.ToInt32(cmdargs[1]).ToString());
                        }

                    }

                    else if (cmdargs[0].Equals("#setmodel"))
                    {
                        if (ModelList.Contains(cmdargs[1]))
                        {
                            if (cmdargs[2] != string.Empty)
                            {
                                foreach (var hero in HeroManager.AllHeroes)
                                {
                                    if (cmdargs[2].ToLowerInvariant() == hero.ChampionName.ToLowerInvariant()
                                        || hero.Name.ToLowerInvariant().StartsWith(cmdargs[2].ToLowerInvariant()))
                                    {
                                        hero.SetSkin(cmdargs[1], 0);
                                        Notifications.OnModelChanged(cmdargs[1]);
                                    }
                                }
                            }
                            else
                            {
                                Model = cmdargs[1];
                                Skin = 0;
                                Player.SetSkin(cmdargs[1], 0);
                            }
                        }
                        else { Notifications.OnFailedModel(); }
                    }
                    else if (cmdargs[0].Equals("#baron"))
                    {
                        if (args.Input.Contains("old"))
                        {
                            Model = "Worm";
                            Skin = 0;
                            Player.SetSkin("Worm", 0);
                            Notifications.OnModelChanged("Worm");
                        }
                        else
                        {
                            Model = "SRU_Baron";
                            Skin = 0;
                            Player.SetSkin("SRU_Baron", 0);
                            Notifications.OnModelChanged("SRU_Baron");
                        }
                    }
                    else if (cmdargs[0].Equals("#dragon"))
                    {
                        if (args.Input.Contains("old"))
                        {
                            Model = "Dragon";
                            Skin = 0;
                            Player.SetSkin("Dragon", 0);
                            Notifications.OnModelChanged("Dragon");
                        }
                        else 
                        {
                            Model = "SRU_Dragon";
                            Skin = 0;
                            Player.SetSkin("SRU_Dragon", 0);
                            Notifications.OnModelChanged("SRU_Dragon");
                        }
                    }
                    else if (cmdargs[0].Equals("#spider"))
                    {
                        Model = "TT_Spiderboss";
                        Skin = 0;
                        Player.SetSkin("TT_Spiderboss", 0);
                        Notifications.OnModelChanged("TT_Spiderboss");
                    }
                    else if (cmdargs[0].Equals("#red"))
                    {
                        if (args.Input.Contains("mini"))
                        {
                            Model = "SRU_RedMini";
                            Skin = 0;
                            Player.SetSkin("SRU_RedMini", 0);
                            Notifications.OnModelChanged("SRU_RedMini");
                        }
                        else
                        {
                            Model = "SRU_Red";
                            Skin = 0;
                            Player.SetSkin("SRU_Red", 0);
                            Notifications.OnModelChanged("SRU_Red");
                        }
                    }
                    else if (cmdargs[0].Equals("#blue"))
                    {
                        if (args.Input.Contains("mini"))
                        {
                            Model = "SRU_BlueMini";
                            Skin = 0;
                            Player.SetSkin("SRU_BlueMini", 0);
                            Notifications.OnModelChanged("SRU_BlueMini");
                        }
                        else
                        {
                            Model = "SRU_Blue";
                            Skin = 0;
                            Player.SetSkin("SRU_Blue", 0);
                            Notifications.OnModelChanged("SRU_Blue");
                        }
                    }
                    else if (cmdargs[0].Equals("#shroom") || cmdargs[0].Equals("#teemoshroom") || cmdargs[0].Equals("#mushroom"))
                    {
                        Model = "TeemoMushroom";
                        Skin = 0;
                        Player.SetSkin("TeemoMushroom", 0);
                        Notifications.OnModelChanged("TeemoMushroom");
                    }
                    else if (cmdargs[0].Equals("#duck"))
                    {
                        Model = "Sru_Duck";
                        Skin = 0;
                        Player.SetSkin("Sru_Duck", 0);
                        Notifications.OnModelChanged("Sru_Duck");
                    }
                    else if (cmdargs[0].Equals("#crab"))
                    {
                        Model = "Sru_Crab";
                        Skin = 0;
                        Player.SetSkin("Sru_Crab", 0);
                        Notifications.OnModelChanged("Sru_Crab");
                    }
                    else if (cmdargs[0].Equals("#urf"))
                    {
                        Model = "Urf";
                        Skin = 0;
                        Player.SetSkin("Urf", 0);
                        Notifications.OnModelChanged("Urf");
                    }
                    else { Notifications.OnFakeInput();
                    args.Process = false;
                    }
                }
            }
            catch (NullReferenceException e)
            {
                Console.Write(e.StackTrace + " || " + e.Message + " || " + e.Source);
            }
        }

        public override void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead && ModEnabled)
                Player.SetSkin(Model, Skin);
        }

    }
}
