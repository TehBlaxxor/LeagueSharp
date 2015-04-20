using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using GameItem = LeagueSharp.Common.Items.Item;
using SharpDX;

namespace Rek_Sai
{
    public static class Modes
    {
        public static GameItem Tiamat = ItemData.Tiamat_Melee_Only.GetItem();
        public static GameItem Hydra = ItemData.Ravenous_Hydra_Melee_Only.GetItem();
        public static GameItem Cutlass = ItemData.Bilgewater_Cutlass.GetItem();
        public static GameItem BotRK = ItemData.Blade_of_the_Ruined_King.GetItem();
        public static GameItem Omen = ItemData.Randuins_Omen.GetItem();
        public static Menu menu = AssemblyMenu.Menu;
        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(1500f, TargetSelector.DamageType.Physical);
            ManageW();
            Items();
            if (menu.Item("reksai.combo.q1").GetValue<bool>()
                && !Player.IsBurrowed()
                && target.IsValidTarget(Program.Q1.Range)
                && Program.Q1.IsReady())
            {
                Program.Q1.Cast();
            }
            else if (menu.Item("reksai.combo.q2").GetValue<bool>()
                && Player.IsBurrowed()
                && target.IsValidTarget(Program.Q2.Range)
                && Program.Q2.IsReady())
            {
                if (menu.Item("reksai.others.hitchances.q2").GetValue<StringList>().SelectedIndex == 0)
                {
                    Program.Q2.Cast(target);
                }
                else if (menu.Item("reksai.others.hitchances.q2").GetValue<StringList>().SelectedIndex == 1)
                {
                    Program.Q2.CastIfHitchanceEquals(target, HitChance.High);
                }
            }
            else if (menu.Item("reksai.combo.e1").GetValue<bool>()
                && !Player.IsBurrowed()
                && target.IsValidTarget(Program.E1.Range)
                && Program.E1.IsReady()
                || menu.Item("reksai.rage.e1").GetValue<bool>()
                && !Player.IsBurrowed()
                && target.IsValidTarget(Program.E1.Range)
                && Program.E1.IsReady()
                && Player.ManaPercent == 100)
            {
                if (!Player.QActive() || Player.ManaPercent == 100)
                    Program.E1.CastOnUnit(target);
            }
            else if (menu.Item("reksai.combo.e2").GetValue<bool>()
                && Player.IsBurrowed()
                && target.IsValidTarget(Program.E2.Range)
                && Program.E2.IsReady())
            {
                if (menu.Item("reksai.others.hitchances.e2").GetValue<StringList>().SelectedIndex == 0)
                    Program.E2.Cast(target);
                else if (menu.Item("reksai.others.hitchances.e2").GetValue<StringList>().SelectedIndex == 1)
                    Program.E2.CastIfHitchanceEquals(target, HitChance.High);
            }
            if (menu.Item("reksai.combo.flash").GetValue<bool>()
                && Player.IsBurrowed()
                && target.IsValidTarget(575f)
                && Program.W2.IsReady()
                && !Program.E2.IsReady()
                && !target.IsValidTarget(Program.Q1.Range)
                && Program.Flash.IsReady())
            {
                Player.Spellbook.CastSpell(Program.Flash, target.ServerPosition);
            }

            if (menu.Item("reksai.combo.ignite").GetValue<bool>()
                && target.IsValidTarget(600f)
                && Program.Ignite.IsReady()
                && !target.IsValidTarget(Program.Q1.Range)
                && Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) / 4 * 3 > target.Health)
            {
                Player.Spellbook.CastSpell(Program.Ignite, target);
            }
        }

        public static void Harass()
        {
            var target = TargetSelector.GetTarget(1500f, TargetSelector.DamageType.Physical);
            if (!Player.IsBurrowed())
            {
                if (menu.Item("reksai.harass.e1").GetValue<bool>()
                    && Program.E1.IsReady()
                    && target.IsValidTarget(Program.E1.Range)
                    || menu.Item("reksai.rage.e1").GetValue<bool>()
                    && Player.ManaPercent == 100
                    && Program.E1.IsReady()
                    && target.IsValidTarget(Program.E1.Range))
                    Program.E1.CastOnUnit(target);
            }
            if (Player.IsBurrowed())
            {
                if (menu.Item("reksai.harass.q2").GetValue<bool>()
                    && Program.Q2.IsReady()
                    && target.IsValidTarget(Program.Q2.Range))
                {
                    if (menu.Item("reksai.others.hitchances.q2").GetValue<StringList>().SelectedIndex == 0)
                        Program.Q2.Cast(target);
                    else if (menu.Item("reksai.others.hitchances.q2").GetValue<StringList>().SelectedIndex == 1)
                        Program.Q2.CastIfHitchanceEquals(target, HitChance.High);
                }
            }
        }
        
        public static void LaneClear()
        {
            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.Distance(Player.Position) <= 250f);
            if (menu.Item("reksai.lane.q1").GetValue<bool>()
                && minions.Count() >= menu.Item("reksai.lane.q1.count").GetValue<Slider>().Value
                && !Player.IsBurrowed()
                && Program.Q1.IsReady())
            {
                Program.Q1.Cast();
            }
            if (menu.Item("reksai.lane.e1").GetValue<bool>()
                && !Player.IsBurrowed()
                && Program.E1.IsReady()
                && minions.OrderBy(x => x.Health).FirstOrDefault() != null
                && minions.OrderBy(x => x.Health).FirstOrDefault().IsValidTarget(Program.E1.Range)
                && minions.OrderBy(x => x.Health).FirstOrDefault().Health < Program.E1.GetDamage(minions.OrderBy(x => x.Health).FirstOrDefault())
                && Player.IsWindingUp)
            {
                Program.E1.CastOnUnit(minions.OrderBy(x => x.Health).FirstOrDefault());
            }
        }

        public static void JungleClear()
        {
            var badmonster = MinionManager.GetMinions(500f, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            ManageW();
            if (Player.IsBurrowed() && badmonster != null)
            {
                if (menu.Item("reksai.jungle.q2").GetValue<bool>()
                    && Program.Q2.IsReady()
                    && badmonster.IsValidTarget(Program.Q2.Range))
                    Program.Q2.Cast(badmonster);
            }
            if (!Player.IsBurrowed() && badmonster != null)
            {
                if (menu.Item("reksai.jungle.q1").GetValue<bool>()
                    && Program.Q1.IsReady()
                    && badmonster.IsValidTarget(Program.Q1.Range))
                    Program.Q1.Cast();

                if (menu.Item("reksai.jungle.e1").GetValue<bool>()
                    && Program.E1.IsReady()
                    && badmonster.IsValidTarget(Program.E1.Range))
                {
                    if (!Player.QActive() || Player.ManaPercent == 100)
                        Program.E1.CastOnUnit(badmonster);
                }
            }
        }

        public static void Escape()
        {
            Others.MoveTo(Game.CursorPos);
            if (Player.IsBurrowed()
                && menu.Item("reksai.escape.e2").GetValue<bool>()
                && Program.E2.IsReady())
                Program.E2.Cast(Game.CursorPos);
            if (!Player.IsBurrowed()
                && menu.Item("reksai.escape.w1").GetValue<bool>()
                && Program.W1.IsReady())
                Program.W1.Cast();
        }

        public static void Items()
        {
            var target = TargetSelector.GetTarget(1500f, TargetSelector.DamageType.Physical);
            if (menu.Item("reksai.items.hydra").GetValue<bool>())
            {
                if (LeagueSharp.Common.Items.HasItem(Hydra.Id, Player) 
                    && Hydra.IsReady() 
                    && target.IsValidTarget(Hydra.Range))
                    Hydra.Cast();
                else if (LeagueSharp.Common.Items.HasItem(Tiamat.Id, Player) 
                    && Tiamat.IsReady() 
                    && target.IsValidTarget(Tiamat.Range))
                    Tiamat.Cast();
            }
            if (menu.Item("reksai.items.botrk").GetValue<bool>())
            {
                if (LeagueSharp.Common.Items.HasItem(BotRK.Id, Player) 
                    && BotRK.IsReady() 
                    && target.IsValidTarget(BotRK.Range))
                    BotRK.Cast(target);
                else if (LeagueSharp.Common.Items.HasItem(Cutlass.Id, Player) 
                    && Cutlass.IsReady() 
                    && target.IsValidTarget(Cutlass.Range))
                    Cutlass.Cast(target);
            }
            if (menu.Item("reksai.items.omen").GetValue<bool>())
            {
                if (LeagueSharp.Common.Items.HasItem(Omen.Id, Player) 
                    && Omen.IsReady() 
                    && target.IsValidTarget(Omen.Range))
                {
                    if (Player.IsFacing(target) 
                        && !target.IsFacing(Player) 
                        || !Player.IsFacing(target) 
                        && target.IsFacing(Player))
                        Omen.Cast();
                }
            }
        }

        public static void ManageW()
        {
            if (!Player.IsBurrowed())
            {
                if (menu.Item("reksai.rage.w1").GetValue<bool>()
                        && Program.W1.IsReady()
                        && Player.HealthPercentage() < 9
                        && Player.Mana > 0)
                    {
                        Program.W1.Cast();
                    }

                if (menu.Item("reksai.others.keybinds.combo").GetValue<KeyBind>().Active)
                {
                    if (!Program.Q1.ShouldBeCasted()
                        && !Program.E1.ShouldBeCasted()
                        && Program.W1.IsReady()
                        && menu.Item("reksai.combo.w1").GetValue<bool>()
                        && !Player.QActive())
                    {
                        Program.W1.Cast();
                    }
                }

                else if (menu.Item("reksai.others.keybinds.jungle").GetValue<KeyBind>().Active)
                {
                    if (!Program.Q1.ShouldBeCasted()
                        && !Program.E1.ShouldBeCasted()
                        && Program.W1.IsReady()
                        && menu.Item("reksai.jungle.w1").GetValue<bool>()
                        && !Player.QActive())
                    {
                        Program.W1.Cast();
                    }
                }
            }
            else if (Player.IsBurrowed())
            {
                if (Program.W2.IsReady()
                    && Player.CountEnemiesInRange(Program.W2.Range) > 3)
                {
                    Others.Print("Ideal knockup: " + Player.CountEnemiesInRange(Program.W2.Range) + " enemies. Overriding W permissions.", Others.PrintType.Warning);
                }

                var target = TargetSelector.GetTarget(1500f, TargetSelector.DamageType.Physical);
                if (menu.Item("reksai.others.keybinds.combo").GetValue<KeyBind>().Active)
                {
                    if (!Program.Q2.ShouldBeCasted()
                        && Program.W2.IsReady()
                        && menu.Item("reksai.combo.w2").GetValue<bool>()
                        && Player.IsUnder(target))
                    {
                        Program.W2.Cast();
                    }
                }

                else if (menu.Item("reksai.others.keybinds.jungle").GetValue<KeyBind>().Active)
                {
                    var bestest = MinionManager.GetMinions(Program.Q1.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
                    if (!Program.Q2.ShouldBeCasted()
                        && Program.W2.IsReady()
                        && menu.Item("reksai.jungle.w2").GetValue<bool>()
                        && Player.IsUnder(bestest)
                        && bestest != null)
                    {
                        Program.W2.Cast();
                    }
                }
            }
        }
    }
}
