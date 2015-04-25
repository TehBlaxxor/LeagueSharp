using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using SharpDX;
using LeagueSharp.Common;
using The_Masterpiece.Handlers;

namespace The_Masterpiece.Plugins
{
    internal abstract class BaseChampion
    {
        public Obj_AI_Hero Player = ObjectManager.Player;
        public Menu Menu { get; internal set; }
        public Orbwalking.Orbwalker Orbwalker { get; internal set; }
        protected BaseChampion()
        {
            CreateMenu();

            Utility.HpBarDamageIndicator.DamageToUnit = DamageToUnit;
            Utility.HpBarDamageIndicator.Enabled = true;

            Orbwalking.AfterAttack += OnAfterAttack;
        }


        private float DamageToUnit(Obj_AI_Hero hero)
        {
            return GetComboDamage(hero);
        }

        public virtual float GetComboDamage(Obj_AI_Hero target)
        {
            return 0;
        }

        private void CreateMenu()
        {
            Menu = new Menu("The Masterpiece - " + ObjectManager.Player.BaseSkinName, "themp", true);

            var tsMenu = new Menu("Target Selector", "themp.ts");
            TargetSelector.AddToMenu(tsMenu);
            Menu.AddSubMenu(tsMenu);

            var orbwalkMenu = new Menu("Orbwalker", "themp.orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
            Menu.AddSubMenu(orbwalkMenu);

            var comboMenu = new Menu("Combo", "themp.combo");
            Combo(comboMenu);
            Menu.AddSubMenu(comboMenu);

            var harassMenu = new Menu("Harass", "themp.harass");
            Harass(harassMenu);
            Menu.AddSubMenu(harassMenu);

            var laneclearMenu = new Menu("LaneClear", "themp.laneclear");
            Laneclear(laneclearMenu);
            Menu.AddSubMenu(laneclearMenu);

            var escapeMenu = new Menu("Escape", "themp.escape");
            escapeMenu.AddItem(new MenuItem("themp.escape.active", "Enabled").SetValue(new KeyBind('G', KeyBindType.Press)));
            Escape(escapeMenu);
            Menu.AddSubMenu(escapeMenu);

            var miscMenu = new Menu("Misc", "themp.misc");
            Misc(miscMenu);
            Menu.AddSubMenu(miscMenu);

            var extraMenu = new Menu("Extra", "themp.extra");
            Extra(extraMenu);
            Menu.AddSubMenu(extraMenu);

            var itemMenu = new Menu("Items", "themp.items");
            ItemMenu(itemMenu);
            Menu.AddSubMenu(itemMenu);

            var summonersMenu = new Menu("Summoner Spells", "themp.summoners");
            if (SpellHandler.Ghost.Exists())
                summonersMenu.AddItem(new MenuItem("themp.ghost", "Use Ghost").SetValue(true));
            if (SpellHandler.Barrier.Exists())
                summonersMenu.AddItem(new MenuItem("themp.barrier", "Use Barrier").SetValue(true));
            if (SpellHandler.Cleanse.Exists())
                summonersMenu.AddItem(new MenuItem("themp.clarity", "Use Clarity").SetValue(true));
            if (SpellHandler.Exhaust.Exists())
                summonersMenu.AddItem(new MenuItem("themp.exhaust", "Use Exhaust").SetValue(true));
            if (SpellHandler.Flash.Exists())
                summonersMenu.AddItem(new MenuItem("themp.flash", "Use Flash").SetValue(true));
            if (SpellHandler.Heal.Exists())
                summonersMenu.AddItem(new MenuItem("themp.heal", "Use Heal").SetValue(true));
            if (SpellHandler.Ignite.Exists())
                summonersMenu.AddItem(new MenuItem("themp.ignite", "Use Ignite").SetValue(true));
            Menu.AddSubMenu(summonersMenu);

            var pm = new Menu("Potion Control", "themp.pm");
            new PotionHandler().Load(pm);
            Menu.AddSubMenu(pm);

            var drawingMenu = new Menu("Drawings", "themp.drawings");
            drawingMenu.AddItem(new MenuItem("themp.drawings.draw", "Enable").SetValue(true));
            Drawings(drawingMenu);
            Menu.AddSubMenu(drawingMenu);

            Menu.AddToMainMenu();
        }

        public T GetValue<T>(string name)
        {
            return Menu.Item(name).GetValue<T>();
        }

        public virtual void Combo(Menu menu) { }

        public virtual void Harass(Menu menu) { }

        public virtual void Laneclear(Menu menu) { }

        public virtual void ItemMenu(Menu menu) { }

        public virtual void Misc(Menu menu) { }

        public virtual void Extra(Menu menu) { }

        public virtual void Drawings(Menu menu) { }

        public virtual void OnAfterAttack(AttackableUnit unit, AttackableUnit target) { }

        public virtual void Escape(Menu menu) { }
    }
}
