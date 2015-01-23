#region
using System;
using System.Collections;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using System.Collections.Generic;
using System.Threading;
#endregion

namespace TopOrAFK.Champions
{
    internal class Template
    {
        private const string Champion = "champname";
        private static Orbwalking.Orbwalker Orbwalker;
        private static List<Spell> SpellList = new List<Spell>();
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static Menu Config;
        private static Items.Item item;

        private static Obj_AI_Hero Player;

        public Template()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.BaseSkinName != Champion) return;

            Q = new Spell(SpellSlot.Q, 930);
            W = new Spell(SpellSlot.W, 320);
            E = new Spell(SpellSlot.E, 225);
            R = new Spell(SpellSlot.R, 0);

            Q.SetSkillshot(0.50f, 75f, 1500f, true, SkillshotType.SkillshotLine);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            item = new Items.Item(3143, 490f);


            Config = new Menu(Champion, "champname", true);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseItems", "Use Items")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("ActiveCombo", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("CountHarass", "Don't Harass if HP < %").SetValue(new Slider(30, 100, 0)));
            Config.SubMenu("Harass").AddItem(new MenuItem("Harass Tog", "Use Harass (toggle)").SetValue<KeyBind>(new KeyBind('T', KeyBindType.Toggle)));
            Config.SubMenu("Harass").AddItem(new MenuItem("ActiveHarass", "Harass!").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Jungle Clear", "Jungle"));
            Config.SubMenu("Jungle").AddItem(new MenuItem("UseQClear", "Use Q")).SetValue(true);
            Config.SubMenu("Jungle").AddItem(new MenuItem("UseWClear", "Use W")).SetValue(true);
            Config.SubMenu("Jungle").AddItem(new MenuItem("UseEClear", "Use E")).SetValue(true);
            Config.SubMenu("Jungle").AddItem(new MenuItem("ActiveClear", "Jungle Key").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Wave Clear", "Wave"));
            Config.SubMenu("Wave").AddItem(new MenuItem("UseQWave", "Use Q")).SetValue(true);
            Config.SubMenu("Wave").AddItem(new MenuItem("UseWWave", "Use W")).SetValue(true);
            Config.SubMenu("Wave").AddItem(new MenuItem("UseEWave", "Use E")).SetValue(true);
            Config.SubMenu("Wave").AddItem(new MenuItem("ActiveWave", "WaveClear Key").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("KS", "Killsteal Q").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("Rsave", "Life saving Ultimate").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Rhp", "R Saving Ult % health").SetValue(new Slider(30, 100, 0)));

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "Draw Q")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawW", "Draw W")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleLag", "Lag Free Circles").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleQuality", "Circles Quality").SetValue(new Slider(100, 100, 10)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleThickness", "Circles Thickness").SetValue(new Slider(1, 10, 1)));

            Config.AddToMainMenu();

            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;


        }



        private static void OnGameUpdate(EventArgs args)
        {
            {
                if (Player.IsDead) return;

                Player = ObjectManager.Player;

                Orbwalker.SetAttack(true);

                if (Config.Item("ActiveCombo").GetValue<KeyBind>().Active)
                {
                    Combo();
                }
                if (Config.Item("ActiveClear").GetValue<KeyBind>().Active)
                {
                    JungleClear();
                }
                if (Config.Item("ActiveWave").GetValue<KeyBind>().Active)
                {
                    WaveClear();
                }
                if (Config.Item("ActiveHarass").GetValue<KeyBind>().Active)
                {
                    Harass();
                }
                if (Config.Item("Harass Tog").GetValue<KeyBind>().Active)
                {
                    HarassTog();
                }
                if (Config.Item("Rsave").GetValue<bool>())
                {
                    Rsave();
                }
                if (Config.Item("KS").GetValue<bool>())
                {
                    Killsteal();
                }
            }
        }

        private static void Killsteal()
        {
            
        }

        private static void HarassTog()
        {
            
        }

        private static void Harass()
        {
            
        }

        private static void Combo()
        {
            
        }

        private static void Rsave()
        {
            

        }
        private static void WaveClear()
        {
            
        }

        private static void JungleClear()
        {

        }

        private static int CountR(Obj_AI_Base target)
        {
            return 1;
        }
        private static void OnDraw(EventArgs args)
        {
            
        }
    }
}