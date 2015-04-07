using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using MAC_Reborn.Handlers;
using MAC_Reborn.Extras;
using Color = System.Drawing.Color;

namespace MAC_Reborn.Champions
{
    internal class Ezreal : BaseChampion
    {
        public Spell Q;
        public Spell W;
        public Spell E;
        public Spell R;
        public static List<Spell> SpellList = new List<Spell>();

        public Ezreal()
        {
            Q = new Spell(SpellSlot.Q, 1200);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 475);
            R = new Spell(SpellSlot.R, 3000f);

            Q.SetSkillshot(0.25f, 50f, 2000f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1.2f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            SpellList.Add(Q); SpellList.Add(W); SpellList.Add(E); SpellList.Add(R);

            Game.OnUpdate += GameOnOnGameUpdate;
            Drawing.OnDraw += DrawingOnOnDraw;
        }

        internal static float RCastTime = 0;

        private void DrawingOnOnDraw(EventArgs args)
        {
            var comboTypeIndex = Menu.Item("comboType").GetValue<StringList>().SelectedIndex;
            var target = TargetSelector.GetTarget(1300, TargetSelector.DamageType.Physical);
            var wts = Drawing.WorldToScreen(Player.Position);
            var p = Player.Position;

            if (GetBool("disableAll"))
                return;

            foreach (var spell in SpellList)
            {
                if (GetBool("draw" + spell.Slot))
                    Render.Circle.DrawCircle(p, spell.Range, spell.IsReady() ? Color.Aqua : Color.Red);
            }

            if (GetBool("drawComboType"))
            {
                switch (comboTypeIndex)
                {
                    case 0:
                        Drawing.DrawText(wts[0] - 35, wts[1] + 10, System.Drawing.Color.White, "Manual Combo Selected");
                        break;
                    case 1:
                        Drawing.DrawText(wts[0] - 35, wts[1] + 10, System.Drawing.Color.Red, "Advanced Combo Selected");
                        break;
                    case 2:
                        Drawing.DrawText(wts[0] - 35, wts[1] + 10, System.Drawing.Color.Gold, "Automatic Combo");
                        break;
                }
            }
        }

        private void GameOnOnGameUpdate(EventArgs args)
        {
            switch (OrbwalkerMode)
            {
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;

                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical);

            var comboTypeIndex = Menu.Item("comboType").GetValue<StringList>().SelectedIndex;

            List<Vector2> waypoints = target.GetWaypoints();
            float S2S = ((target.MoveSpeed * W.Delay) + (Player.Distance(target.ServerPosition) / W.Speed)) * 6 - W.Width;
            float B2F = ((target.MoveSpeed * W.Delay) + (Player.Distance(target.ServerPosition) / W.Speed)) * 5;


            /*
                Automatic mode ignore all your combo configs 
             */

            if (comboTypeIndex == 2)
            {
                if (Q.IsReady() && Q.IsInRange(target) && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                    if (target.IsValidTarget(Player.AttackRange - 25f) 
                        || target.IsValidTarget(Player.AttackRange) && target.Health < Player.GetAutoAttackDamage(target))
                    {
                        Orbwalking.ResetAutoAttackTimer();
                        Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                }


                if (S2S > Player.Distance(waypoints.Last<Vector2>().To3D()) 
                    || S2S > Player.Distance(target.Position))
                    W.CastIfHitchanceEquals(target, HitChance.High, true);
                else if (target.Path.Count() < 2
                    && (target.ServerPosition.Distance(waypoints.Last<Vector2>().To3D()) > S2S
                    || 0 - B2F > (Player.Distance(waypoints.Last<Vector2>().To3D()) - ObjectManager.Player.Distance(target.Position))
                    || (Player.Distance(waypoints.Last<Vector2>().To3D()) - ObjectManager.Player.Distance(target.Position)) > (target.MoveSpeed * W.Delay)
                    || target.Path.Count() == 0))
                     {
                     if (ObjectManager.Player.Distance(waypoints.Last<Vector2>().To3D()) <= ObjectManager.Player.Distance(target.Position))
                        {
                            if (ObjectManager.Player.Distance(target.ServerPosition) < W.Range - ((target.MoveSpeed * W.Delay) + (Player.Distance(target.ServerPosition) / W.Speed)))
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                        }
                     else
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                        }

                     }

                if (E.GetDamage(target) > target.Health + target.HPRegenRate && E.IsReady() && target.IsValidTarget(750f))
                {
                    E.Cast(target);
                    if (target.IsValidTarget(Player.AttackRange - 25f)
                        || target.IsValidTarget(Player.AttackRange) && target.Health < Player.GetAutoAttackDamage(target))
                    {
                        Orbwalking.ResetAutoAttackTimer();
                        Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                }

                if (R.GetDamage(target) > target.Health && R.IsReady() && target.IsValidTarget(R.Range))
                    R.CastIfHitchanceEquals(target, HitChance.High);
                else if (target.Path.Count() >= 3 && R.IsReady() && target.IsValidTarget(R.Range))
                    R.CastIfHitchanceEquals(target, HitChance.VeryHigh);

                if (CanUseItem(3142) && Player.Distance(target.Position) < Player.AttackRange)
                    UseItem(3142);

                if ((Player.Health / Player.MaxHealth) * 100 < (target.Health / target.MaxHealth) * 100 && (CanUseItem(3153) || CanUseItem(3144)))
                {
                    UseItem(3144, target);
                    UseItem(3153, target);
                }

            }

            /* End of Gosu Mode */

            /**
             * Advanced Combo Mode 
             */

            if (comboTypeIndex == 1)
            {

                if (Q.IsReady() && Q.IsInRange(target) && target.IsValidTarget(Q.Range) && GetBool("comboQ"))
                {
                    Q.Cast(target);
                    if (target.IsValidTarget(Player.AttackRange - 25f)
                        || target.IsValidTarget(Player.AttackRange) && target.Health < Player.GetAutoAttackDamage(target))
                    {
                        Orbwalking.ResetAutoAttackTimer();
                        Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                }


                if ((S2S > Player.Distance(waypoints.Last<Vector2>().To3D())
                    || S2S > Player.Distance(target.Position)) && GetBool("comboW"))
                    W.CastIfHitchanceEquals(target, HitChance.High, true);
                else if ((target.Path.Count() < 2
                    && (target.ServerPosition.Distance(waypoints.Last<Vector2>().To3D()) > S2S
                    || 0 - B2F > (Player.Distance(waypoints.Last<Vector2>().To3D()) - ObjectManager.Player.Distance(target.Position))
                    || (Player.Distance(waypoints.Last<Vector2>().To3D()) - ObjectManager.Player.Distance(target.Position)) > (target.MoveSpeed * W.Delay)
                    || target.Path.Count() == 0)) && GetBool("comboW"))
                {
                    if (ObjectManager.Player.Distance(waypoints.Last<Vector2>().To3D()) <= ObjectManager.Player.Distance(target.Position))
                    {
                        if (ObjectManager.Player.Distance(target.ServerPosition) < W.Range - ((target.MoveSpeed * W.Delay) + (Player.Distance(target.ServerPosition) / W.Speed)))
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                    }
                    else
                    {
                        W.CastIfHitchanceEquals(target, HitChance.High, true);
                    }

                }

                if (E.GetDamage(target) > target.Health + target.HPRegenRate && E.IsReady() && target.IsValidTarget(750f) && GetBool("comboE"))
                {
                    E.Cast(target);
                    if (target.IsValidTarget(Player.AttackRange - 25f)
                        || target.IsValidTarget(Player.AttackRange) && target.Health < Player.GetAutoAttackDamage(target))
                    {
                        Orbwalking.ResetAutoAttackTimer();
                        Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                }

                if (R.GetDamage(target) > target.Health && R.IsReady() && target.IsValidTarget(R.Range) && GetBool("comboR"))
                    R.CastIfHitchanceEquals(target, HitChance.High);
                else if (target.Path.Count() >= 3 && R.IsReady() && target.IsValidTarget(R.Range) && GetBool("comboR"))
                    R.CastIfHitchanceEquals(target, HitChance.VeryHigh);

                if (CanUseItem(3142) && Player.Distance(target.Position) < Player.AttackRange && GetBool("YoumuuC"))
                    UseItem(3142);

                if ((Player.Health / Player.MaxHealth) * 100 < (target.Health / target.MaxHealth) * 100 && (CanUseItem(3153) || CanUseItem(3144)) && GetBool("BotrkC"))
                {
                    UseItem(3144, target);
                    UseItem(3153, target);
                }

            }

            /* End of Advanced Combo Mode */

            /**
             * Manual Combo Mode 
             */

            if (comboTypeIndex == 0)
            {
                if (E.GetDamage(target) > target.Health + target.HPRegenRate && E.IsReady() && target.IsValidTarget(750f) && GetBool("comboE"))
                {
                    E.Cast(target);
                    if (target.IsValidTarget(Player.AttackRange - 25f)
                        || target.IsValidTarget(Player.AttackRange) && target.Health < Player.GetAutoAttackDamage(target))
                    {
                        Orbwalking.ResetAutoAttackTimer();
                        Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                }
            }

            /* End of Manual Combo Mode */

        }

        private void Harass()
        {
            var target = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical);

            if (Q.IsReady() && GetBool("harassQ") && manaManager() && target.IsValidTarget(Q.Range))
                Q.CastIfHitchanceEquals(target, HitChance.High);

            if (W.IsReady() && GetBool("harassW") && manaManager() && target.IsValidTarget(W.Range))
                W.CastIfHitchanceEquals(target, HitChance.High);

            if (E.IsReady() && GetBool("harassE") && manaManager() && target.IsValidTarget(750f))
            {
                E.Cast(target);
                if (target.IsValidTarget(Player.AttackRange - 25f)
                    || target.IsValidTarget(Player.AttackRange) && target.Health < Player.GetAutoAttackDamage(target))
                {
                    Orbwalking.ResetAutoAttackTimer();
                    Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }
            }

        }

        private void LaneClear()
        {
            MinionManager.FarmLocation farmLocation =
                    R.GetLineFarmLocation(
                    MinionManager.GetMinionsPredictedPositions(MinionManager.GetMinions(R.Range),
                    R.Delay, R.Width, R.Speed,
                    Player.Position, R.Range,
                    false, SkillshotType.SkillshotLine), R.Width);
            var target = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical);
            var minionforq = ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsEnemy && minion.Health < Q.GetDamage(minion) && minion.IsValidTarget(Q.Range)).OrderBy(minion => minion.Distance(Player.Position)).FirstOrDefault();


            if (Q.IsReady() && GetBool("laneClearQ") && manaManager() && minionforq != null)
            {
                Q.Cast(minionforq);
            }
            if (R.IsReady() && GetBool("laneClearR") && manaManager() && farmLocation.MinionsHit >= 20)
            {
                R.Cast(farmLocation.Position);
            }

        }

        public override void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var comboTypeIndex = Menu.Item("comboType").GetValue<StringList>().SelectedIndex;

            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Q.IsReady() && Q.IsInRange(target) && target.IsValidTarget(Q.Range))
                {
                    Q.Cast((Obj_AI_Hero)target);
                    if (target.IsValidTarget(Player.AttackRange - 25f)
                        || target.IsValidTarget(Player.AttackRange) && target.Health < Player.GetAutoAttackDamage((Obj_AI_Hero)target))
                    {
                        Orbwalking.ResetAutoAttackTimer();
                        Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                }
                else if (Q.IsReady() && Q.IsInRange(target) && target.IsValidTarget(Q.Range) && GetBool("comboQ"))
                {
                    Q.Cast((Obj_AI_Hero)target);
                    if (target.IsValidTarget(Player.AttackRange - 25f)
                        || target.IsValidTarget(Player.AttackRange) && target.Health < Player.GetAutoAttackDamage((Obj_AI_Hero)target))
                    {
                        Orbwalking.ResetAutoAttackTimer();
                        Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                }
            }
            else if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (Q.IsReady() && GetBool("harassQ") && manaManager() && target.IsValidTarget(Q.Range))
                    Q.CastIfHitchanceEquals((Obj_AI_Hero)target, HitChance.High);
            }
            else if (OrbwalkerMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var minionforq = ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsEnemy && minion.Health < Q.GetDamage(minion) && minion.IsValidTarget(Q.Range)).OrderBy(minion => minion.Distance(Player.Position)).FirstOrDefault();
                if (Q.IsReady() && GetBool("laneClearE") && manaManager() && minionforq != null)
                {
                    Q.Cast(minionforq);
                }

            }
        }

        public bool manaManager()
        {
            var mana = GetValue<Slider>("saveMana").Value;

            if (Player.Mana >= Player.MaxMana * (mana / 100))
            {
                return true;
            }

            return false;
        }

        public int enemiesInRange(Obj_AI_Hero obj, float range)
        {
            return obj.CountEnemiesInRange(range);
        }

        public override float GetComboDamage(Obj_AI_Hero enemy)
        {
            var damage = 0d;

            if (Items.HasItem(3077) && Items.CanUseItem(3077))
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Tiamat);
            if (Items.HasItem(3074) && Items.CanUseItem(3074))
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Hydra);
            if (Items.HasItem(3153) && Items.CanUseItem(3153))
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Botrk);
            if (Items.HasItem(3144) && Items.CanUseItem(3144))
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Bilgewater);
            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q) * 1.3;
            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);
            if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);

            damage += Player.GetAutoAttackDamage(enemy, true) * 2;

            return (float)damage;
        }

        bool isUnderEnemyTurret(Vector3 Position)
        {
            return Position.UnderTurret(true);
        }


        public override void Combo(Menu config)
        {
            config.AddItem(new MenuItem("comboQ", "Use Q").SetValue(true));
            config.AddItem(new MenuItem("comboW", "Use W").SetValue(true));
            config.AddItem(new MenuItem("comboE", "Use E").SetValue(true));
            config.AddItem(new MenuItem("comboR", "Use R").SetValue(true));
        }

        public override void Harass(Menu config)
        {
            config.AddItem(new MenuItem("harassQ", "Use Q").SetValue(true));
            config.AddItem(new MenuItem("harassW", "Use W").SetValue(true));
            config.AddItem(new MenuItem("harassE", "Use E").SetValue(false));
        }

        public override void Laneclear(Menu config)
        {
            config.AddItem(new MenuItem("laneClearQ", "Use Q").SetValue(true));
            config.AddItem(new MenuItem("laneClearR", "Use R").SetValue(true));

        }

        public override void Misc(Menu config)
        {
            config.AddItem(new MenuItem("comboType", "Combo Type").SetValue(new StringList(new[] { "Normal", "Advanced", "Gosu" }, 2)));
            config.AddItem(new MenuItem("qRange", "Custom Q Range").SetValue(new Slider((int)Q.Range, 50, (int)Q.Range)));
        }

        public override void Extra(Menu config)
        {
            var MiscMSubMenu = new Menu("Misc - Mana Manager", "MiscM");
            {
                MiscMSubMenu.AddItem(new MenuItem("saveMana", "% safe for Combo").SetValue(new Slider(50, 0, 100)));
            }

            config.AddSubMenu(MiscMSubMenu);

        }

        public override void Drawings(Menu config)
        {
            config.AddItem(new MenuItem("disableAll", "Disable All Drawings").SetValue(false));
            config.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(true));
            config.AddItem(new MenuItem("drawW", "Draw W").SetValue(true));
            config.AddItem(new MenuItem("drawE", "Draw E").SetValue(true));
            config.AddItem(new MenuItem("drawR", "Draw R").SetValue(true));
            config.AddItem(new MenuItem("drawComboType", "Draw Combo Type").SetValue(true));
        }
    }
}
