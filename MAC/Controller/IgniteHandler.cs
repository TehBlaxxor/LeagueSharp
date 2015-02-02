using LeagueSharp;
using LeagueSharp.Common;
using MAC.Model;
using MAC.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAC.Controller
{
    internal class ItemHandler : Utilitario
    {
        private const int IgniteRange = 600;
        private SpellSlot _igniteSlot;
        private Menu _menu;

        public override void Load(Menu config)
        {
            config.AddItem(new MenuItem("igniteKill", "Use Ignite if Killable").SetValue(true));
            config.AddItem(new MenuItem("igniteMinRange", "Min Range to cast ignite").SetValue(new Slider(400, 0, 600)));
            config.AddItem(new MenuItem("igniteKS", "Use Ignite KS").SetValue(true));

            _menu = config;
            _igniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");

            Game.OnGameUpdate += GameOnOnGameUpdate;
        }

        private void GameOnOnGameUpdate(EventArgs args)
        {
            var igniteKill = _menu.Item("igniteKill").GetValue<bool>();
            var igniteKs = _menu.Item("igniteKS").GetValue<bool>();
            var igniteRange = _menu.Item("igniteMinRange").GetValue<Slider>().Value;

            if (!_igniteSlot.IsReady())
                return;

            if (igniteKill)
            {
                var igniteKillableEnemy = HeroManager.Enemies.Find(x => x.IsValidTarget(IgniteRange) && ObjectManager.Player.GetSummonerSpellDamage(x, Damage.SummonerSpell.Ignite) > x.Health);
                if (igniteKillableEnemy == null)
                {
                    return;
                }
                if (igniteKillableEnemy.Distance(GameControl.MyHero.Position) < igniteRange)
                {
                    return;
                }
                    ObjectManager.Player.Spellbook.CastSpell(_igniteSlot, igniteKillableEnemy);
            }

            if (!igniteKs)
            {
                return;
            }
            var enemy = HeroManager.Enemies.Find(x => x.IsValidTarget(IgniteRange) &&  x.Health <= ObjectManager.Player.GetSummonerSpellDamage(x, Damage.SummonerSpell.Ignite) / 5);
            if (enemy != null)
            {
                ObjectManager.Player.Spellbook.CastSpell(_igniteSlot, enemy);
            }
        }
    }
}