using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace The_Masterpiece.Handlers
{
    internal class PotionHandler
    {
        private const string HP_POT_NAME = "RegenerationPotion";
        private const string MP_POT_NAME = "FlaskOfCrystalWater";
        private const string MP2_POT_NAME = "ItemCrystalFlask";
        private const string MP3_POT_NAME = "ItemMiniRegenPotion";
        private const int HP_ID = 2003;
        private const int MP_ID = 2004;
        private const int HP_ID2 = 2010;
        private const int MP_ID3 = 2041;

        private static Menu menu;

        public void Load(Menu config)
        {
            config.AddItem(new MenuItem("themp.healthpots.active", "Use Health Pot").SetValue(true));
            config.AddItem(new MenuItem("themp.healthpots.health", "Health %").SetValue(new Slider(35, 1)));
            config.AddItem(new MenuItem("sseperator", "       "));
            config.AddItem(new MenuItem("themp.manapots.active", "Use Mana Pot").SetValue(true));
            config.AddItem(new MenuItem("themp.manapots.mana", "Mana %").SetValue(new Slider(35, 1)));

            menu = config;

            Game.OnUpdate += OnGameUpdate;
        }

        private void OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.HasBuff("Recall") || ObjectManager.Player.InFountain() && ObjectManager.Player.InShop())
                return;

            try
            {
                if (menu.Item("themp.healthpots.active").GetValue<bool>())
                {
                    if (ObjectManager.Player.HealthPercentage() <= menu.Item("themp.healthpots.health").GetValue<Slider>().Value)
                    {
                        if (Items.CanUseItem(HP_ID) && Items.HasItem(HP_ID))
                        {
                            Items.UseItem(HP_ID);
                        }
                        if (Items.CanUseItem(HP_ID2) && Items.HasItem(HP_ID2))
                        {
                            Items.UseItem(HP_ID2);
                        }
                        if (Items.CanUseItem(MP_ID3) && Items.HasItem(MP_ID3))
                        {
                            Items.UseItem(MP_ID3);
                        }
                    }
                }
                if (menu.Item("themp.manapots.active").GetValue<bool>())
                {
                    if (ObjectManager.Player.ManaPercentage() <= menu.Item("themp.manapots.mana").GetValue<Slider>().Value)
                    {
                        Game.PrintChat("Manapotsareworking");
                        if (Items.CanUseItem(MP_ID) && Items.HasItem(MP_ID))
                        {
                            Items.UseItem(MP_ID);
                        }
                        if (Items.CanUseItem(MP_ID3) && Items.HasItem(MP_ID3))
                        {
                            Items.UseItem(MP_ID3);
                        }
                    }
                }
            }

            catch (Exception)
            {
                Game.PrintChat("Error using potions D:");
            }
        }
    }
}
