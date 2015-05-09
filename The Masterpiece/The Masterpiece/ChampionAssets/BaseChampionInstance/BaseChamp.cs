using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using LMenu = LeagueSharp.Common.Menu;

namespace The_Masterpiece.ChampionAssets.BaseChampionInstance
{
    abstract class BaseChamp
    {
        public LMenu Config = Menu.Config;
        public List<Spell> SpellList = Spells.SpellList;//Dont rly like spell arrays!
        public Obj_AI_Hero Player = ObjectManager.Player;
        public Spell Q = Spells.Q, W = Spells.W, E = Spells.E, R = Spells.R;

        protected BaseChamp(String name)
        {
            Messages.OnAssemblyLoad();
            Menu.Load(name, "menu." + name);
            Spells.Load();
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;

            //Later add all other callbacks so they would be accesable to champ plugins

            Utility.HpBarDamageIndicator.DamageToUnit = Player.GetDamageTo;
            Utility.HpBarDamageIndicator.Enabled = Config.GetValue<bool>("draw.dmg");
        }

        

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!Config.Item("draw").GetValue<bool>())
                return;

            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (Config.Item("draw.target").GetValue<Circle>().Active && target != null)
            {
                Render.Circle.DrawCircle(target.Position, 75f, Config.GetValue<Circle>("draw.target").Color);
            }

            foreach (var x in SpellList.Where(y => Config.Item("draw." + y.Slot.ToString().ToLowerInvariant()) != null
                && Config.GetValue<Circle>("draw." + y.Slot.ToString().ToLowerInvariant()).Active))
            {
                Render.Circle.DrawCircle(Player.Position, x.Range, x.IsReady()
                ? System.Drawing.Color.Green
                : System.Drawing.Color.Red
                );
            }

            Draw();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            CalcPerFrame();
            if (Config.Item("extra.ks").GetValue<bool>())
                KillSteal();
            if (Config.Item("extra.kb.combo").GetValue<bool>())
                Combo();
            if (Config.Item("extra.kb.harass").GetValue<bool>())
                Harass();
            if (Config.Item("extra.kb.escape").GetValue<bool>())
            {
                Escape();
                GlobalMethods.Flee();
            }
            if (Config.Item("extra.kb.lane").GetValue<bool>())
                LaneClear();
            if (Config.Item("extra.kb.last").GetValue<bool>())
                LastHit();
        }


        public abstract void KillSteal();
        public abstract void Combo();
        public abstract void Harass();
        public abstract void Escape();
        public abstract void LaneClear();
        public abstract void LastHit();

        public abstract void Draw();
        public abstract void CalcPerFrame();
    }
}
