using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using menu = LeagueSharp.Common.Menu;

namespace TehOrianna
{
    class Menu
    {
        public static menu Config;
        public static Orbwalking.Orbwalker Orbwalker;

        public static void Runnerino()
        {
            Config = new menu("TehOrianna", "TehOrianna", true);

            Config.AddSubMenu(new menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            var targetSelectorMenu = new menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Config.AddSubMenu(new menu("TehOrianna - Combo", "tehorianna.combo"));
            Config.SubMenu("tehorianna.combo").AddItem(new MenuItem("tehorianna.combo.q", "Use Q").SetValue(true));
            Config.SubMenu("tehorianna.combo").AddItem(new MenuItem("tehorianna.combo.w", "Use W").SetValue(true));
            Config.SubMenu("tehorianna.combo").AddItem(new MenuItem("tehorianna.combo.e", "Use E").SetValue(true));
            Config.SubMenu("tehorianna.combo").AddItem(new MenuItem("tehorianna.combo.r", "Use R").SetValue(true));
            Config.SubMenu("tehorianna.combo").AddItem(new MenuItem("tehorianna.combo.spacer", " "));
            Config.SubMenu("tehorianna.combo").AddItem(new MenuItem("tehorianna.combo.wcondition", "W Minimum Enemies").SetValue(new Slider(1,1,5)));
            Config.SubMenu("tehorianna.combo").AddItem(new MenuItem("tehorianna.combo.rcondition", "R Minimum Enemies").SetValue(new Slider(2,1,5)));

            Config.AddSubMenu(new menu("TehOrianna - Harass", "tehorianna.harass"));
            Config.SubMenu("tehorianna.harass").AddItem(new MenuItem("tehorianna.harass.q", "Use Q").SetValue(true));
            Config.SubMenu("tehorianna.harass").AddItem(new MenuItem("tehorianna.harass.w", "Use W").SetValue(true));
            Config.SubMenu("tehorianna.harass").AddItem(new MenuItem("tehorianna.harass.spacer", " "));
            Config.SubMenu("tehorianna.harass").AddItem(new MenuItem("tehorianna.harass.toggle", "Toggle Harass").SetValue<KeyBind>(new KeyBind('I', KeyBindType.Toggle)));

            Config.AddSubMenu(new menu("TehOrianna - Last Hit", "tehorianna.lasthit"));
            Config.SubMenu("tehorianna.lasthit").AddItem(new MenuItem("tehorianna.lasthit.q", "Use Q").SetValue(true));
            Config.SubMenu("tehorianna.lasthit").AddItem(new MenuItem("tehorianna.lasthit.w", "Use W").SetValue(true));
            Config.SubMenu("tehorianna.lasthit").AddItem(new MenuItem("tehorianna.lasthit.spacer", " "));
            Config.SubMenu("tehorianna.lasthit").AddItem(new MenuItem("tehorianna.lasthit.wcondition", "W Minimum Killed").SetValue(new Slider(2,1,10)));

            Config.AddSubMenu(new menu("TehOrianna - Lane Clear", "tehorianna.laneclear"));
            Config.SubMenu("tehorianna.laneclear").AddItem(new MenuItem("tehorianna.laneclear.q", "Use Q").SetValue(true));
            Config.SubMenu("tehorianna.laneclear").AddItem(new MenuItem("tehorianna.laneclear.w", "Use W").SetValue(true));
            Config.SubMenu("tehorianna.laneclear").AddItem(new MenuItem("tehorianna.laneclear.spacer", " "));
            Config.SubMenu("tehorianna.laneclear").AddItem(new MenuItem("tehorianna.laneclear.mode", "LaneClear Mode").SetValue(new StringList(new[] { "Best Farm Location", "Cursor" })));

            Config.AddSubMenu(new menu("TehOrianna - Kill Steal", "tehorianna.killsteal"));
            Config.SubMenu("tehorianna.killsteal").AddItem(new MenuItem("tehorianna.killsteal.active", "Kill Steal").SetValue(true));
            Config.SubMenu("tehorianna.killsteal").AddItem(new MenuItem("tehorianna.killsteal.q", "Use Q").SetValue(true));
            Config.SubMenu("tehorianna.killsteal").AddItem(new MenuItem("tehorianna.killsteal.w", "Use W").SetValue(true));
            Config.SubMenu("tehorianna.killsteal").AddItem(new MenuItem("tehorianna.killsteal.e", "Use E").SetValue(true));
            Config.SubMenu("tehorianna.killsteal").AddItem(new MenuItem("tehorianna.killsteal.r", "Use R").SetValue(true));
            if (Orianna.IS != SpellSlot.Unknown)
                Config.SubMenu("tehorianna.killsteal").AddItem(new MenuItem("tehorianna.killsteal.infernus", "Use Ignite").SetValue(true));
            else
                NotificationManager.ShowNotification("Failed to get SummonerDot!", System.Drawing.Color.Red);

            Config.AddSubMenu(new menu("TehOrianna - Mana Manager", "tehorianna.mana"));
            Config.SubMenu("tehorianna.mana").AddItem(new MenuItem("tehorianna.mana.manager", "Mininimum Mana %").SetValue(new Slider(25,0,100)));

            Config.AddSubMenu(new menu("TehOrianna - Drawings", "tehorianna.drawings"));
            Config.SubMenu("tehorianna.drawings").AddItem(new MenuItem("tehorianna.drawings.toggle", "Drawings").SetValue(true));
            Config.SubMenu("tehorianna.drawings").AddItem(new MenuItem("tehorianna.drawings.q", "Draw Q").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("tehorianna.drawings").AddItem(new MenuItem("tehorianna.drawings.w", "Draw W").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("tehorianna.drawings").AddItem(new MenuItem("tehorianna.drawings.e", "Draw E").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("tehorianna.drawings").AddItem(new MenuItem("tehorianna.drawings.r", "Draw R").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("tehorianna.drawings").AddItem(new MenuItem("tehorianna.drawings.target", "Draw Target").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 0, 255, 255))));
            MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
            MenuItem drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true, System.Drawing.Color.FromArgb(90, 255, 169, 4)));
            Config.SubMenu("tehorianna.drawings").AddItem(drawComboDamageMenu);
            Config.SubMenu("tehorianna.drawings").AddItem(drawFill);
            DamageIndicator.DamageToUnit = Orianna.GetComboDamage;
            DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
            DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
            DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
            drawComboDamageMenu.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };
            drawFill.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };

            Config.AddSubMenu(new menu("TehOrianna - Interrupter", "tehorianna.interrupter"));
            Config.SubMenu("tehorianna.interrupter").AddItem(new MenuItem("tehorianna.interrupter.r", "Use R").SetValue(true));

            Config.AddSubMenu(new menu("Assembly Info", "tehorianna.info"));
            Config.SubMenu("tehorianna.info").AddItem(new MenuItem("TK/info/author", "Author: TehBlaxxor"));
            Config.SubMenu("tehorianna.info").AddItem(new MenuItem("TK/info/edition", "Edition: TB:r"));
            Config.SubMenu("tehorianna.info").AddItem(new MenuItem("TK/info/version", "5.6.1.1"));

            Config.AddToMainMenu();

        }
    }
}
