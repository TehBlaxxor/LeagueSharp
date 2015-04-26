using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace The_Masterpiece.Plugins
{
    internal class ChampionStartingPoint : BaseChampion
    {
        public static Spell Q, W, E, R;
        public static List<Spell> Spells = new List<Spell>();

        public ChampionStartingPoint()
        {
            Q = new Spell(SpellSlot.Q, 300f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            Spells.Add(Q);
            Spells.Add(W);
            Spells.Add(E);
            Spells.Add(R);

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

        }

        void Game_OnUpdate(EventArgs args)
        {
            if (Menu.Item("themp.kb.combo").GetValue<bool>())
                DoCombo();
            if (Menu.Item("themp.kb.harass").GetValue<bool>())
                DoHarass();
            if (Menu.Item("themp.kb.laneclear").GetValue<bool>())
                DoLaneClear();
            if (Menu.Item("themp.kb.escape").GetValue<bool>())
            {
                GlobalMethods.Flee();
                DoEscape();
            }

        }

        void Drawing_OnDraw(EventArgs args)
        {
            if (!Menu.Item("themp.drawings.draw").GetValue<bool>())
                return;
            
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (Menu.Item("themp.drawings.target").GetValue<Circle>().Active && target != null)
            {
                Render.Circle.DrawCircle(target.Position, 75f, Menu.Item("themp.drawings.target").GetValue<Circle>().Color);
            }

            foreach (var x in Spells.Where(y => Menu.Item("themp.drawings." + y.Slot.ToString().ToLowerInvariant()).GetValue<Circle>().Active))
            {
                Render.Circle.DrawCircle(Player.Position, x.Range, x.IsReady()
                ? System.Drawing.Color.Green
                : System.Drawing.Color.Red
                );
            }
        }

        void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            throw new NotImplementedException();
        }

        void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            throw new NotImplementedException();
        }

        public override void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            throw new NotImplementedException();
        }

        public bool ManaManager()
        {
            return Player.Mana >= Player.MaxMana * (GetValue<Slider>("saveMana").Value / 100);
        }

        private void DoCombo()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            Items();
        }

        private void DoLaneClear()
        {

        }

        private void DoHarass()
        {

        }

        private void DoEscape()
        {

        }

        private void Items()
        {

        }

        public override void Combo(Menu config)
        {
        }

        public override void Harass(Menu config)
        {
        }

        public override void Laneclear(Menu config)
        {
        }

        public override void Misc(Menu config)
        {
        }

        public override void Escape(Menu menu)
        {
        }

        public override void ItemMenu(Menu menu)
        {
        }

        public override void Extra(Menu config)
        {
            var MiscManaSubMenu = new Menu("Misc - Mana Manager", "themp.manamanager");
            {
                MiscManaSubMenu.AddItem(new MenuItem("themp.manamanager.percentage", "% safe for Combo").SetValue(new Slider(50, 0, 100)));
            }
            config.AddSubMenu(MiscManaSubMenu);

            var MiscKeybindsSubMenu = new Menu("Misc - Keybinds", "themp.kb");
            {
                MiscKeybindsSubMenu.AddItem(new MenuItem("themp.kb.combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
                MiscKeybindsSubMenu.AddItem(new MenuItem("themp.kb.harass", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));
                MiscKeybindsSubMenu.AddItem(new MenuItem("themp.kb.laneclear", "LaneClear").SetValue(new KeyBind('V', KeyBindType.Press)));
                MiscKeybindsSubMenu.AddItem(new MenuItem("themp.kb.escape", "Escape").SetValue(new KeyBind('G', KeyBindType.Press)));
            }
            config.AddSubMenu(MiscKeybindsSubMenu);

            var MiscHitchancesSubMenu = new Menu("Misc - Hitchances", "themp.hc");
            {
                
            }
        }

        public override void Drawings(Menu config)
        {
        }
    }
}
