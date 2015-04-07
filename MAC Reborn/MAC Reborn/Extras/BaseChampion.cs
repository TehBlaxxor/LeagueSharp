using LeagueSharp;
using LeagueSharp.Common;
using MAC_Reborn.Extras;
using MAC_Reborn.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAC_Reborn.Extras
{
    internal abstract class BaseChampion
    {
            protected BaseChampion()
            {
                LoadMenu();

                Utility.HpBarDamageIndicator.DamageToUnit = DamageToUnit;
                Utility.HpBarDamageIndicator.Enabled = true;

                Orbwalking.AfterAttack += OnAfterAttack;
            }

            public Menu Menu { get; internal set; }
            public Orbwalking.Orbwalker Orbwalker { get; internal set; }

            public Orbwalking.OrbwalkingMode OrbwalkerMode
            {
                get { return Orbwalker.ActiveMode; }
            }

            public bool Packets
            {
                get { return false; }
            }

            public static Obj_AI_Hero Player
            {
                get { return Core.Player; }
            }

            private float DamageToUnit(Obj_AI_Hero hero)
            {
                return GetComboDamage(hero);
            }

            private void LoadMenu()
            {
                Menu = new Menu("MAC - " + Player.ChampionName, "mac", true);

                var tsMenu = new Menu("Target Selector", "macTS");
                TargetSelector.AddToMenu(tsMenu);
                Menu.AddSubMenu(tsMenu);

                var orbwalkMenu = new Menu("Orbwalker", "macOrbwalker");
                Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
                Menu.AddSubMenu(orbwalkMenu);

                var comboMenu = new Menu("Combo", "macCombo");
                Combo(comboMenu);
                Menu.AddSubMenu(comboMenu);

                var harassMenu = new Menu("Harass", "macHarass");
                Harass(harassMenu);
                Menu.AddSubMenu(harassMenu);

                var laneclearMenu = new Menu("LaneClear", "macLaneclear");
                Laneclear(laneclearMenu);
                Menu.AddSubMenu(laneclearMenu);

                var miscMenu = new Menu("Misc", "macMisc");
                miscMenu.AddItem(new MenuItem("packets", "Use packets").SetValue(true));
                Misc(miscMenu);
                Menu.AddSubMenu(miscMenu);

                var extraMenu = new Menu("Extra", "macExtra");
                Extra(extraMenu);
                Menu.AddSubMenu(extraMenu);

                var itemMenu = new Menu("Items and Summoners", "Items");
                itemMenu.AddItem(new MenuItem("BotrkC", "Use Botrk / Bilgewater").SetValue(true));
                itemMenu.AddItem(new MenuItem("YoumuuC", "Use Youmuu").SetValue(true));
                Menu.AddSubMenu(itemMenu);


                if (Player.GetSpellSlot("SummonerDot") != SpellSlot.Unknown)
                {
                    var igniteMenu = new Menu("Ignite/Incendiar", "macIgnite");
                    new IgniteHandler().Load(igniteMenu);
                    Menu.AddSubMenu(igniteMenu);
                }

                var pmUtilitario = new Menu("Potion Control", "macPM");
                new PotionHandler().Load(pmUtilitario);
                Menu.AddSubMenu(pmUtilitario);

                var drawingMenu = new Menu("Drawings", "macDrawing");
                Drawings(drawingMenu);
                Menu.AddSubMenu(drawingMenu);

                Menu.AddToMainMenu();
            }

            public T GetValue<T>(string name)
            {
                return Menu.Item(name).GetValue<T>();
            }

            public bool GetBool(string name)
            {
                return GetValue<bool>(name);
            }

            public virtual float GetComboDamage(Obj_AI_Hero target)
            {
                return 0;
            }

            public Spell GetSpell(List<Spell> spellList, SpellSlot slot)
            {
                return spellList.First(x => x.Slot == slot);
            }

            #region Virtuals

            public virtual void Combo(Menu config)
            {
            }

            public virtual void Harass(Menu config)
            {
            }

            public virtual void Laneclear(Menu config)
            {
            }

            public virtual void ItemMenu(Menu config)
            {
            }

            public virtual void Misc(Menu config)
            {
            }

            public virtual void Extra(Menu config)
            {
            }

            public virtual void Drawings(Menu config)
            {
            }

            public virtual void UseItem(int id, Obj_AI_Hero target = null)
            {
                if (Items.HasItem(id) && Items.CanUseItem(id))
                {
                    Items.UseItem(id, target);
                }
            }

            public virtual bool CanUseItem(int id)
            {
                return Items.HasItem(id) && Items.CanUseItem(id);
            }

            public virtual void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
            {
            }

            #endregion Virtuals
        }
    }
