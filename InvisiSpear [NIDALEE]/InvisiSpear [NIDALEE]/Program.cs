using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace InvisiSpear__NIDALEE_
{
    class Program
    {
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Spell Q;
        public static List<Spell> SpellList = new List<Spell>();

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {

            if (Player.BaseSkinName.ToLowerInvariant() != "nidalee")
                return;

            Q = new Spell(SpellSlot.Q, 1500f);
            Q.SetSkillshot(0.125f, 40f, 1300f, true, SkillshotType.SkillshotLine);
            SpellList.Add(Q);

            Config = new Menu("Nidalee - Invisible Spear", "MENU QQ", true);

            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Config.AddSubMenu(new Menu("Invisible Spear", "mainmenu"));
            Config.SubMenu("mainmenu").AddItem(new MenuItem("invisispear.toggle", "Use Invisible Spear").SetValue(true));
            Config.SubMenu("mainmenu").AddItem(new MenuItem("invisispear.mode", "Mode").SetValue(new StringList(new[] { "Delayed", "Instant" })));
            Config.AddToMainMenu();

            Orbwalking.AfterAttack += Orbwalking_OnAttack;
        }

        static void Orbwalking_OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Player.IsMelee() || !Config.Item("invisispear.toggle").GetValue<bool>() || target != TargetSelector.GetTarget(1500f, TargetSelector.DamageType.Magical))
                return;

            if (Config.Item("invisispear.mode").GetValue<StringList>().SelectedIndex == 0)
            {
                Game.PrintChat("INVISIBLE SPEAR ATTEMPT!, delaying by " + Game.Ping);
                Utility.DelayAction.Add(Game.Ping, () => Q.CastIfHitchanceEquals(TargetSelector.GetTarget(1500f, TargetSelector.DamageType.Magical), HitChance.High));
            }
            else
            {
                Game.PrintChat("INVISIBLE SPEAR ATTEMPT!, immediate casting");
                Utility.DelayAction.Add(1, () => Q.CastIfHitchanceEquals(TargetSelector.GetTarget(1500f, TargetSelector.DamageType.Magical), HitChance.High));
            }
        }
    }
}
