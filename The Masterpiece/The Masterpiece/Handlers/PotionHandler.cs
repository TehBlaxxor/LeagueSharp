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
        private const int HP_ID = 2003;
        private const int MP_ID = 2004;
        private static Menu menu;

        public void Load(Menu config)
        {
            config.AddItem(new MenuItem("themp.healthpots.active", "Use Health Pot").SetValue(true));
            config.AddItem(new MenuItem("themp.healthpots.health", "Health %").SetValue(new Slider(35, 1)));
            config.AddItem(new MenuItem("sseperator", "       "));
            config.AddItem(new MenuItem("themp.manapots.active", "Use Mana Pot").SetValue(true));
            config.AddItem(new MenuItem("themp.manapots.mana", "Mana %").SetValue(new Slider(35, 1)));

            menu = config;

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            var useHp = menu.Item("themp.healthpots.active").GetValue<bool>();
            var useMp = menu.Item("themp,manapots.active").GetValue<bool>();
            if (ObjectManager.Player.IsRecalling() || ObjectManager.Player.InFountain() || ObjectManager.Player.InShop())
            {
                return;
            }

            if (useHp && ObjectManager.Player.HealthPercent <= menu.Item("themp.healthpots.health").GetValue<Slider>().Value &&
                !HasHealthPotActive())
            {
                if (Items.CanUseItem(HP_ID) && Items.HasItem(HP_ID))
                {
                    Items.UseItem(HP_ID);
                }
            }

            if (!useMp ||
                !(ObjectManager.Player.ManaPercent <= menu.Item("themp.manapots.mana").GetValue<Slider>().Value) ||
                HasMannaPutActive()) return;

            if (Items.CanUseItem(MP_ID) && Items.HasItem(MP_ID))
            {
                Items.UseItem(MP_ID);
            }
        }

        private static bool HasHealthPotActive()
        {
            return ObjectManager.Player.HasBuff(HP_POT_NAME, true);
        }

        private static bool HasMannaPutActive()
        {
            return ObjectManager.Player.HasBuff(MP_POT_NAME, true);
        }
    }
}
