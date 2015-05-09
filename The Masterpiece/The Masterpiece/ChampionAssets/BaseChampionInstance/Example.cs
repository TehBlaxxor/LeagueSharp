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
    public class Example
    {
        public static LMenu Config = Menu.Config;
        public static List<Spell> SpellList = Spells.SpellList;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Spell Q = Spells.Q, W = Spells.W, E = Spells.E, R = Spells.R;
        public Example()
        {

            Messages.OnAssemblyLoad();
            Menu.Load("Example", "menu.example");
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;

            Utility.HpBarDamageIndicator.DamageToUnit = GetDamageTo;
            Utility.HpBarDamageIndicator.Enabled = Config.GetValue<bool>("draw.dmg");
        }

        public static float GetDamageTo(Obj_AI_Hero hero)
        {
            double damage = 0d;
            if (Q.IsReady() && Config.GetValue<bool>("combo.r"))
                damage += Q.GetDamage(hero);
            if (W.IsReady() && Config.GetValue<bool>("combo.r"))
                damage += W.GetDamage(hero);
            if (E.IsReady() && Config.GetValue<bool>("combo.r"))
                damage += E.GetDamage(hero);
            if (R.IsReady() && Config.GetValue<bool>("combo.r"))
                damage += R.GetDamage(hero);

            return (float)damage;
        }

        void Drawing_OnDraw(EventArgs args)
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
        }

        void Game_OnUpdate(EventArgs args)
        {
            if (Config.Item("extra.ks").GetValue<bool>())
                KS();
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
                Lane();
            if (Config.Item("extra.kb.last").GetValue<bool>())
                Last();
        }

        public static void KS() { }
        public static void Combo() { }
        public static void Harass() { }
        public static void Escape() { }
        public static void Lane() { }
        public static void Last() { }
    }
}
