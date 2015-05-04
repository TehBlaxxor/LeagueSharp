using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Rek_Sai
{
    class Program
    {
        private const string CHAMPION = "RekSai";
        private static Obj_AI_Hero Player = ObjectManager.Player;
        public static Spell Q1, W1, E1, Q2, W2, E2;
        public static SpellSlot Flash, Ignite;
        public static List<Spell> BurrowedSpells = new List<Spell>();
        public static List<Spell> UnburrowedSpells = new List<Spell>();
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {

            if (!Player.ChampionName.ToLowerInvariant().Contains("rek"))
                return;

            Utils.ClearConsole();

            Others.Print("TehBlaxxor's Rek'Sai successfully loaded!", Others.PrintType.Message);

            Q1 = new Spell(SpellSlot.Q, 300);
            UnburrowedSpells.Add(Q1);
            W1 = new Spell(SpellSlot.W, 250);
            UnburrowedSpells.Add(W1);
            E1 = new Spell(SpellSlot.E, 250);
            UnburrowedSpells.Add(E1);
            Q2 = new Spell(SpellSlot.Q, 1500, TargetSelector.DamageType.Magical);
            Q2.SetSkillshot(0.125f, 60, 4000, true, SkillshotType.SkillshotLine);
            BurrowedSpells.Add(Q2);
            W2 = new Spell(SpellSlot.W, 250);
            BurrowedSpells.Add(W2);
            E2 = new Spell(SpellSlot.E, 750);
            E2.SetSkillshot(0, 60, 1600, false, SkillshotType.SkillshotLine);
            BurrowedSpells.Add(E2);

            Flash = Player.GetSpellSlot("SummonerFlash");
            Ignite = Player.GetSpellSlot("SummonerDot");

            AssemblyMenu.Setup();

            Utility.HpBarDamageIndicator.DamageToUnit = Others.GetDamageOn;
            Utility.HpBarDamageIndicator.Color = System.Drawing.Color.White;
            Utility.HpBarDamageIndicator.Enabled = true;

            Obj_AI_Hero.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.AfterAttack += Modes.Orbwalking_AfterAttack;
        }


        static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            if (args.SData.Name.ToLowerInvariant() == "reksaiq")
                Orbwalking.ResetAutoAttackTimer();

            if (args.SData.Name.ToLowerInvariant().Contains("reksaiqatt")
                || args.SData.Name.ToLowerInvariant() == "reksaie")
                Utility.DelayAction.Add(500, () => Orbwalking.ResetAutoAttackTimer());
        }

        static void Game_OnUpdate(EventArgs args)
        {
            if (AssemblyMenu.Menu.Item("reksai.disable").GetValue<bool>())
                return;

            Menu menu = AssemblyMenu.Menu;
            if (menu.Item("reksai.others.keybinds.combo").GetValue<KeyBind>().Active)
                Modes.Combo();
            else if (menu.Item("reksai.others.keybinds.harass").GetValue<KeyBind>().Active)
                Modes.Harass();
            else if (menu.Item("reksai.others.keybinds.lane").GetValue<KeyBind>().Active)
                Modes.LaneClear();
            else if (menu.Item("reksai.others.keybinds.jungle").GetValue<KeyBind>().Active)
                Modes.JungleClear();
            else if (menu.Item("reksai.others.keybinds.escape").GetValue<KeyBind>().Active)
                Modes.Escape();
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (!AssemblyMenu.Menu.Item("reksai.draw.draw").GetValue<bool>() 
                || AssemblyMenu.Menu.Item("reksai.disable").GetValue<bool>())
                return;

            Obj_AI_Hero target = TargetSelector.GetTarget(Q2.Range, TargetSelector.DamageType.Physical);
            if (AssemblyMenu.Menu.Item("reksai.draw.tg").GetValue<bool>())
                Render.Circle.DrawCircle(target.Position,
                    75f,
                    System.Drawing.Color.Red);

            if (Player.IsBurrowed())
            {
                foreach (Spell spell in BurrowedSpells
                                    .Where(x =>
                                        AssemblyMenu.Menu.Item("reksai.draw." + x.Slot.ToString().ToLowerInvariant() + "2").GetValue<bool>()))
                {
                    Render.Circle.DrawCircle(Player.Position, spell.Range, spell.IsReady()
                ? System.Drawing.Color.Green
                : System.Drawing.Color.Red
                );
                }
            }
            else
            {
                foreach (Spell spell in UnburrowedSpells
                                    .Where(x =>
                                        AssemblyMenu.Menu.Item("reksai.draw." + x.Slot.ToString().ToLowerInvariant() + "1").GetValue<bool>()))
                {
                    Render.Circle.DrawCircle(Player.Position, spell.Range, spell.IsReady()
                ? System.Drawing.Color.Green
                : System.Drawing.Color.Red
                );
                }
            }
        }
    }
}
