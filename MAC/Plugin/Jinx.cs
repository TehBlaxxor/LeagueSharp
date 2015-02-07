using LeagueSharp;
using LeagueSharp.Common;
using MAC.Model;
using MAC.Util;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAC.Plugin
{
    internal class Jinx : PluginModel
    {

        public Spell Q;
        public Spell W;
        public Spell E;
        public Spell R;

        public Jinx()
        {
            Q = new Spell(SpellSlot.Q);

            W = new Spell(SpellSlot.W, 1500f);
            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);

            E = new Spell(SpellSlot.E, 900f);
            E.SetSkillshot(0.7f, 120f, 1750f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 25000f);
            R.SetSkillshot(0.6f, 140f, 1700f, false, SkillshotType.SkillshotLine);

            Game.OnGameUpdate += GameOnOnGameUpdate;
            Drawing.OnDraw += DrawingOnOnDraw;

            MiscControl.PrintChat(MiscControl.stringColor("Jinx Loaded", MiscControl.TableColor.Red));
        }

        public bool RocketLauncherActive
        {
            get { return Player.AttackRange > 565f; }
        }

        public float RocketLauncherBonusRange
        {
            get { return Player.Spellbook.GetSpell(SpellSlot.Q).Level * 25 + 50; }
        }

        private void DrawingOnOnDraw(EventArgs args)
        {
            var drawQ = GetBool("drawQ");
            var drawW = GetBool("drawW");
            var drawE = GetBool("drawE");
            var drawR = GetBool("drawR");

            var comboTypeIndex = Menu.Item("comboType").GetValue<StringList>().SelectedIndex;

            var target = TargetSelector.GetTarget(GetValue<Slider>("rmaxrange").Value, TargetSelector.DamageType.Physical);

            var wts = Drawing.WorldToScreen(Player.Position);

            var p = Player.Position;

            if (GetBool("disableAll"))
            {
                return;
            }

            if (drawQ)
            {
                if (RocketLauncherActive)
                {
                    Render.Circle.DrawCircle(p, 565f + RocketLauncherBonusRange, Q.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);
                }

                else if (!RocketLauncherActive)
                {
                    Render.Circle.DrawCircle(p, 565f, Q.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);
                }
            }

            if (drawW)
            {
                Render.Circle.DrawCircle(p, W.Range, W.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);
            }

            if (drawE)
            {
                Render.Circle.DrawCircle(p, E.Range, E.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);
            }

            if (drawR)
            {
                Render.Circle.DrawCircle(p, GetValue<Slider>("rmaxrange").Value, R.IsReady() ? System.Drawing.Color.Aqua : System.Drawing.Color.Red);
            }

            if (GetBool("drawComboType"))
                switch (comboTypeIndex)
                {
                    case 0:
                        Drawing.DrawText(wts[0] - 35, wts[1] + 10, System.Drawing.Color.White, "Manual Combo Selected");
                        break;
                    case 1:
                        Drawing.DrawText(wts[0] - 35, wts[1] + 10, System.Drawing.Color.Red, "Advanced Combo Selected");
                        break;
                    case 2:
                        Drawing.DrawText(wts[0] - 35, wts[1] + 10, System.Drawing.Color.Gold, "Automatic Combo Selected");
                        break;
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
                    Combar();
                    break;
            }
        }

        public bool IsEnemyBeingRekt(Obj_AI_Hero hero)
        {
            
            var comboTypeIndex = Menu.Item("comboType").GetValue<StringList>().SelectedIndex;

            if (comboTypeIndex == 2 || comboTypeIndex == 0)
            {
                if (hero.HasBuffOfType(BuffType.Blind) || hero.HasBuffOfType(BuffType.Charm) || hero.HasBuffOfType(BuffType.Fear) || hero.HasBuffOfType(BuffType.Slow) || hero.HasBuffOfType(BuffType.Snare) || hero.HasBuffOfType(BuffType.Stun) || hero.HasBuffOfType(BuffType.Suppression) || hero.HasBuffOfType(BuffType.Taunt))
                {
                    return true;
                }
                else return false;
            }

            else if (comboTypeIndex == 1)
            {
                if (hero.HasBuffOfType(BuffType.Blind) && GetBool("Blind"))
                {
                    return true;
                }
                else if (hero.HasBuffOfType(BuffType.Fear) && GetBool("Fear"))
                {
                    return true;
                }
                else if (hero.HasBuffOfType(BuffType.Slow) && GetBool("Slow"))
                {
                    return true;
                }
                else if (hero.HasBuffOfType(BuffType.Snare) && GetBool("Snare"))
                {
                    return true;
                }
                else if (hero.HasBuffOfType(BuffType.Stun) && GetBool("Stun"))
                {
                    return true;
                }
                else if (hero.HasBuffOfType(BuffType.Suppression) && GetBool("Suppression"))
                {
                    return true;
                }
                else if (hero.HasBuffOfType(BuffType.Taunt) && GetBool("Taunt"))
                {
                    return true;
                }
                else return false;
            }
            else return false;
        }

        private void Combar()
        {
            var gosutarget = TargetSelector.GetTarget(1500f, TargetSelector.DamageType.Physical);
            var target = TargetSelector.GetTarget(3000f, TargetSelector.DamageType.Physical);

            var comboTypeIndex = Menu.Item("comboType").GetValue<StringList>().SelectedIndex;

            /*
                Automatic mode ignore all your combo configs 
             */

            if (comboTypeIndex == 2)
            {

                var damageR = Player.GetSpellDamage(gosutarget, SpellSlot.R);
                if (Q.IsReady() && gosutarget.IsValidTarget(565f) && RocketLauncherActive)
                {
                    Q.Cast();
                }
                if (Q.IsReady() && gosutarget.IsValidTarget(565f + RocketLauncherBonusRange) && !gosutarget.IsValidTarget(565f) && !RocketLauncherActive)
                {
                    Q.Cast();
                }

                if (R.IsReady() && gosutarget.Health < damageR + 40 && gosutarget.IsValidTarget(1500f))
                {
                    R.CastIfHitchanceEquals(gosutarget, HitChance.Medium, false);
                }

                if (E.IsReady() && !gosutarget.IsMoving && !gosutarget.HasBuffOfType(BuffType.SpellShield) && gosutarget.IsValidTarget(E.Range) || E.IsReady() && IsEnemyBeingRekt(gosutarget) && gosutarget.IsValidTarget(E.Range))
                {
                    E.Cast(gosutarget);
                }

                if (CanUseItem(3142) && Player.Distance(gosutarget.Position) < 565f)
                    UseItem(3142);

                if ((Player.Health / Player.MaxHealth) * 100 < (gosutarget.Health / gosutarget.MaxHealth) * 100 && (CanUseItem(3153) || CanUseItem(3144)))
                {
                    UseItem(3144, gosutarget);

                    UseItem(3153, gosutarget);
                }

                if (W.IsReady() && W.IsInRange(gosutarget))
                {
                    W.CastIfHitchanceEquals(gosutarget, HitChance.High, false);
                }

            }

            /* End of Gosu Mode */

            /**
             * Advanced Combo Mode 
             */

            if (comboTypeIndex == 1)
            {

                var damageR = Player.GetSpellDamage(target, SpellSlot.R);

                if (GetBool("comboQ"))
                {
                    if (Q.IsReady() && target.IsValidTarget(565f) && RocketLauncherActive)
                    {
                        Q.Cast();
                    }
                    if (Q.IsReady() && target.IsValidTarget(565f + RocketLauncherBonusRange) && !target.IsValidTarget(565f) && !RocketLauncherActive)
                    {
                        Q.Cast();
                    }
                }

                if (R.IsReady() && target.Health < damageR + 40 && target.IsValidTarget(GetValue<Slider>("rmaxrange").Value) && GetBool("comboR"))
                {
                    R.CastIfHitchanceEquals(target, HitChance.Medium, false);
                }

                if (E.IsReady() && !target.IsMoving && !target.HasBuffOfType(BuffType.SpellShield) && GetBool("comboE") && target.IsValidTarget(E.Range) || E.IsReady() && IsEnemyBeingRekt(target) && target.IsValidTarget(E.Range) && GetBool("comboE"))
                {
                    E.Cast(target);
                }

                if (CanUseItem(3142) && Player.Distance(target.Position) < Player.AttackRange && GetBool("YoumuuC"))
                    UseItem(3142);

                if ((Player.Health / Player.MaxHealth) * 100 < (target.Health / target.MaxHealth) * 100 && (CanUseItem(3153) || CanUseItem(3144)) && GetBool("BotrkC"))
                {
                    UseItem(3144, target);

                    UseItem(3153, target);
                }

                if (W.IsReady() && W.IsInRange(target) && GetBool("comboW"))
                {
                    W.CastIfHitchanceEquals(target, HitChance.High, false);
                }

            }

            /* End of Advanced Combo Mode */

            /**
             * Manual Combo Mode 
             */

            if (comboTypeIndex == 0)
            {
                if (Q.IsReady() && target.IsValidTarget(565f) && RocketLauncherActive)
                {
                    Q.Cast();
                }
                if (Q.IsReady() && target.IsValidTarget(565f + RocketLauncherBonusRange) && !target.IsValidTarget(565f) && !RocketLauncherActive)
                {
                    Q.Cast();
                }

                if (E.IsReady() && !target.IsMoving && !target.HasBuffOfType(BuffType.SpellShield) && target.IsValidTarget(E.Range) || E.IsReady() && IsEnemyBeingRekt(target) && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }

            }

            /* End of Manual Combo Mode */

        }

        private void Harass()
        {
            // get ma target
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

            // if w is ready, we want to harass with w, we have enuogh mana saved & target is in w's range
            if (W.IsReady() && GetBool("harassW") && manaManager() && W.IsInRange(target))
            {
                // gosh darn it i'm getting bored
                // w cast on the nooblet
                W.CastIfHitchanceEquals(target, HitChance.High, false);
            }

        }

        private void LaneClear()
        {
            // i think rockets is enough, orbwalker already got that going
        }

        public override void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            // get combotypeindex
            var comboTypeIndex = Menu.Item("comboType").GetValue<StringList>().SelectedIndex;

            // if we are in combo mode
            if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Combo)
            {
                // if we are using gosu mode, w is ready and target is in w's range
                if (comboTypeIndex == 2 && W.IsReady() && W.IsInRange(target))
                {
                    // hit the noob if the hit chance is high
                    W.CastIfHitchanceEquals((Obj_AI_Hero)target, HitChance.High, false);
                }
                    // else if we aren't using gosu mode, we have w checked in cnofiguration, w is ready and target is in w's range
                else if (comboTypeIndex < 2 && GetBool("comboW") && W.IsReady() && W.IsInRange(target))
                {
                    // then hit the noob if the hit chance is high
                    Q.CastIfHitchanceEquals((Obj_AI_Hero)target, HitChance.High, false);
                }
            }

                // else if we are in harass mode
            else if (OrbwalkerMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                // if we enabled w for harass, w is ready, we have enough mana saved and target is in w's range
                if (GetBool("harassW") && W.IsReady() && manaManager() && W.IsInRange(target))
                {
                    // hit the nerd haha
                    W.CastIfHitchanceEquals((Obj_AI_Hero)target, HitChance.High, false);
                }
            }
            else if (OrbwalkerMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                // if we are requried to use rocket launcher in lane clear mode and q is ready
                if (GetBool("laneClearQ") && Q.IsReady())
                {
                    // if we aren't using rocket launcher and we don't need to worry about mana,
                    if (!RocketLauncherActive && manaManager())
                    {
                        // then switch to rocket launcher
                        Q.Cast();
                    }
                    // if we are using rocket launcher and we don't have enough mana saved
                    if (RocketLauncherActive && !manaManager())
                    {
                        // then switch back to minigun
                        Q.Cast();
                    }
                }
                // if we don't need to use q in lane clear and q is ready
                if (!GetBool("laneClearQ") && Q.IsReady())
                {
                    // check if we are using rocket launcher
                    if (RocketLauncherActive)
                    {
                        // switch to minigun if we are using rocket auncher
                        Q.Cast();
                    }
                }
            }
        }

        public bool manaManager()
        {
            // get the slider value of mana that needs to be saved
            var mana = GetValue<Slider>("saveMana").Value;

            // if we have enough mana
            if (Player.Mana >= Player.MaxMana * (mana / 100))
            {
                // then return true
                return true;
            }
            // if failed the previous check, return false
            return false;
        }

        public int enemiesInRange(Obj_AI_Hero obj, float range)
        {
            // i didn't even touch this lol
            var nearEnemies =
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => x.IsEnemy)
                        .Where(x => !x.IsDead)
                        .Where(x => x.Distance(obj.Position) <= range);
            return nearEnemies.Count();
        }

        public override float GetComboDamage(Obj_AI_Hero enemy)
        {
            // create damage instance and set value
            var damage = 0d;

            // bla bla calculate damage of items and skills bla bla
            if (Items.HasItem(3077) && Items.CanUseItem(3077))
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Tiamat);
            if (Items.HasItem(3074) && Items.CanUseItem(3074))
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Hydra);
            if (Items.HasItem(3153) && Items.CanUseItem(3153))
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Botrk);
            if (Items.HasItem(3144) && Items.CanUseItem(3144))
                damage += Player.GetItemDamage(enemy, Damage.DamageItems.Bilgewater);
            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);
            if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);

            // add 2 auto attacks because we're adc
            damage += Player.GetAutoAttackDamage(enemy, true) * 2;

            // give me the amount of damage
            return (float)damage;
        }

        bool isUnderEnemyTurret(Vector3 Position)
        {
            // i didn't touch this either lol
            foreach (var tur in ObjectManager.Get<Obj_AI_Turret>().Where(turr => turr.IsEnemy && (turr.Health != 0)))
            {
                if (tur.Distance(Position) <= 975f) return true;
            }
            return false;
        }


        public override void Combo(Menu config)
        {
            config.AddItem(new MenuItem("comboQ", "Use Q").SetValue(true));
            config.AddItem(new MenuItem("comboW", "Use W").SetValue(true));
            config.AddItem(new MenuItem("comboE", "Use E Automatically").SetValue(true));
            config.AddItem(new MenuItem("comboR", "Use R").SetValue(true));
        }

        public override void Harass(Menu config)
        {
            config.AddItem(new MenuItem("harassW", "Use W").SetValue(false));
        }

        public override void Laneclear(Menu config)
        {
            config.AddItem(new MenuItem("laneClearQ", "Use Rocket Launcher").SetValue(true));
        }

        public override void Misc(Menu config)
        {
            config.AddItem(new MenuItem("comboType", "Combo Type").SetValue(new StringList(new[] { "Normal", "Advanced", "Gosu" }, 2)));
            config.AddItem(new MenuItem("rmaxrange", "R Cast Range").SetValue(new Slider(1500, 565, 3000)));
        }

        public override void Extra(Menu config)
        {
            var MiscMSubMenu = new Menu("Misc - Mana Manager", "MiscM");
            {
                MiscMSubMenu.AddItem(new MenuItem("saveMana", "% safe for Combo").SetValue(new Slider(50, 0, 100)));
            }

            config.AddSubMenu(MiscMSubMenu);

            var EControl = new Menu("E Control - Buffs", "EControl");
            {
                EControl.AddItem(new MenuItem("Blind", "Blind").SetValue(true));
                EControl.AddItem(new MenuItem("Charm", "Charm").SetValue(true));
                EControl.AddItem(new MenuItem("Fear", "Fear").SetValue(true));
                EControl.AddItem(new MenuItem("Slow", "Slow").SetValue(true));
                EControl.AddItem(new MenuItem("Snare", "Snare").SetValue(true));
                EControl.AddItem(new MenuItem("Stun", "Stun").SetValue(true));
                EControl.AddItem(new MenuItem("Suppression", "Suppression").SetValue(true));
                EControl.AddItem(new MenuItem("Taunt", "Taunt").SetValue(true));

            }

            config.AddSubMenu(EControl);
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
