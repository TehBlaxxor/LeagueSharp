using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;

namespace AdvancedSharp.Instance
{
    /*
    Planserino:
    - Q
    When Killable.
    When in Range for E (take in count Q+E damage, allied forces, Q+2E if mana)
    If hits >=3 enemies
    
    - W
    When low health and running
    On gapcloser
    Q on cd, target not poisoned + in range for E
    When killable
    If hits >=3 enemies
    
    - E
    Humanizer
    Full yolo xd
    Killable
    
    - R
    Hits X enemies
    X enemies facing
    
    
    - Kill Secure
    E
    */

    internal class Cassiopeia
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Menu Z;
        public static Spell Q, W, E, R;
        public static List<Spell> Spells = new List<Spell>();
        public static float LastQ = 0;
        public static float LastE = 0;
        public static int[] abilitySequence;
        public static int q = 0, w = 0, e = 0, r = 0;

        public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);

        public static Items.Item TearoftheGoddess = new Items.Item(3070, 0);

        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;

        public static Color Color = Color.Lime;
        public static Color FillColor = Color.Goldenrod;
        public static bool Fill = true;

        public static bool Enabled = true;
        private static DamageToUnitDelegate _damageToUnit;

        private static readonly Render.Text Text = new Render.Text(0, 0, "", 14, SharpDX.Color.Red, "monospace");

        public static DamageToUnitDelegate DamageToUnit
        {

            get { return _damageToUnit; }

            set { _damageToUnit = value; }
        }


        public Cassiopeia()
        {
            Core.LoadWelcomeMessage();

            Q = new Spell(SpellSlot.Q, 850f);
            Spells.Add(Q);
            Q.SetSkillshot(0.75f, Q.Instance.SData.CastRadius, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W = new Spell(SpellSlot.W, 850f);
            Spells.Add(W);
            W.SetSkillshot(0.5f, W.Instance.SData.CastRadius, W.Instance.SData.MissileSpeed, false,
                SkillshotType.SkillshotCircle);
            E = new Spell(SpellSlot.E, 700f);
            Spells.Add(E);
            E.SetTargetted(0.2f, float.MaxValue);
            R = new Spell(SpellSlot.R, 825f);
            Spells.Add(R);
            R.SetSkillshot(0.3f, (float) (80*Math.PI/180), float.MaxValue, false, SkillshotType.SkillshotCone);

            abilitySequence = new int[] {1, 3, 3, 2, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2};

            Z = new Menu("Adv# - Cassiopeia", "root", true);

            Menu MOrb = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(MOrb);
            Z.AddSubMenu(MOrb);

            Menu MTS = new Menu("Target Selector", "TS");
            TargetSelector.AddToMenu(MTS);
            Z.AddSubMenu(MTS);

            Menu MCombo = new Menu("Combo", "combo");
            {
                MCombo.AddItem(new MenuItem("combo.q", "Use Q").SetValue(true));
                MCombo.AddItem(new MenuItem("combo.w", "Use W").SetValue(true));
                MCombo.AddItem(new MenuItem("combo.e", "Use E").SetValue(true));
                MCombo.AddItem(new MenuItem("combo.r", "Use R").SetValue(true));
                MCombo.AddItem(new MenuItem("combo.spacer1", " "));
                MCombo.AddItem(new MenuItem("combo.r.minfacing", "R Minimum Facing").SetValue(new Slider(1, 1, 5)));
                MCombo.AddItem(new MenuItem("combo.r.minhit", "R Minimum Hit").SetValue(new Slider(1, 1, 5)));
                MCombo.AddItem(new MenuItem("combo.r.smart", "Smart Ultimate").SetValue(true));
            }
            Z.AddSubMenu(MCombo);

            Menu MHarass = new Menu("Harass", "harass");
            {
                MHarass.AddItem(new MenuItem("harass.q", "Use Q").SetValue(true));
                MHarass.AddItem(new MenuItem("harass.w", "Use W").SetValue(true));
                MHarass.AddItem(new MenuItem("harass.e", "Use E").SetValue(true));
                MHarass.AddItem(new MenuItem("harass.spacer1", " "));
                MHarass.AddItem(new MenuItem("harass.e.restriction", "E Only If Poisoned").SetValue(true));
            }
            Z.AddSubMenu(MHarass);

            Menu MLH = new Menu("Last Hit", "lasthit");
            {
                MLH.AddItem(new MenuItem("lasthit.e", "Use E").SetValue(true));
                MLH.AddItem(new MenuItem("lasthit.e.auto", "Use E Automatically").SetValue(false));
            }
            Z.AddSubMenu(MLH);

            Menu MLC = new Menu("Lane Clear", "laneclear");
            {
                MLC.AddItem(new MenuItem("laneclear.q", "Use Q").SetValue(true));
                MLC.AddItem(new MenuItem("laneclear.w", "Use W").SetValue(true));
                MLC.AddItem(new MenuItem("laneclear.e", "Use E").SetValue(true));
                MLC.AddItem(new MenuItem("laneclear.spacer1", " "));
                MLC.AddItem(new MenuItem("laneclear.e.restriction", "E Only If Poisoned").SetValue(true));
                MLC.AddItem(new MenuItem("laneclear.e.lasthit", "E Only If Last Hit").SetValue(true));
                MLC.AddItem(new MenuItem("laneclear.w.restriction", "W Minimum Hit").SetValue(new Slider(3, 1, 10)));
            }
            Z.AddSubMenu(MLC);

            Menu Misc = new Menu("Misc", "misc");
            {
                Misc.AddItem(new MenuItem("misc.manamenagertm", "Restrict Mana Usage").SetValue(true));
                Misc.AddItem(new MenuItem("misc.manamenagertm.slider", "Minimum Mana").SetValue(new Slider(35, 0, 95)));
                Misc.AddItem(new MenuItem("misc.spacer1", " "));
                Misc.AddItem(new MenuItem("misc.itemstack", "Item Stacking").SetValue(true)); 
                Misc.AddItem(new MenuItem("misc.spacer3", " "));
                Misc.AddItem(new MenuItem("misc.qks", "Kill Secure with Q").SetValue(true));
                Misc.AddItem(new MenuItem("misc.wks", "Kill Secure with W").SetValue(true));
                Misc.AddItem(new MenuItem("misc.eks", "Kill Secure with E").SetValue(true));
                Misc.AddItem(new MenuItem("misc.edelay", "E Delay").SetValue(new Slider(0, 0, 500)));
                Misc.AddItem(new MenuItem("misc.spacer4", " "));
                Misc.AddItem(new MenuItem("misc.autospells", "Auto Level Spells").SetValue(true)); 
                Misc.AddItem(new MenuItem("misc.spacer5", " "));
                Misc.AddItem(new MenuItem("misc.gc", "W on gap closer").SetValue(true));
                Misc.AddItem(new MenuItem("misc.gc.hp", "if HP < ").SetValue(new Slider(30)));
                Misc.AddItem(new MenuItem("misc.aablock", "Auto Attack Block in combo").SetValue(false));


            }
            Z.AddSubMenu(Misc);

            Menu Drawings = new Menu("Drawings", "drawings");
            {
                Drawings.AddItem(new MenuItem("draw", "Drawings").SetValue(true));
                Drawings.AddItem(new MenuItem("draw.q", "Draw Q Range").SetValue(true));
                Drawings.AddItem(new MenuItem("draw.w", "Draw W Range").SetValue(true));
                Drawings.AddItem(new MenuItem("draw.e", "Draw E Range").SetValue(true));
                Drawings.AddItem(new MenuItem("draw.r", "Draw R Range").SetValue(true));
                Drawings.AddItem(new MenuItem("draw.tg", "Draw Target").SetValue(true));
                var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo damage").SetValue(true);
                var drawFill =
                    new MenuItem("RushDrawWDamageFill", "Combo Damage Fill").SetValue(new Circle(true, Color.SeaGreen));
                Drawings.SubMenu("Combo Damage").AddItem(drawDamageMenu);
                Drawings.SubMenu("Combo Damage").AddItem(drawFill);

                DamageToUnit = GetComboDamage;
                Enabled = drawDamageMenu.GetValue<bool>();
                Fill = drawFill.GetValue<Circle>().Active;
                FillColor = drawFill.GetValue<Circle>().Color;
            }
            Z.AddSubMenu(Drawings);

            Z.AddItem(new MenuItem("credits1", "Credits:"));
            Z.AddItem(new MenuItem("credits2", "TehBlaxxor - Coding"));
            Z.AddItem(new MenuItem("credits3", "Hoes - Coding"));
            Z.AddItem(new MenuItem("credits4", "Hawk - Testing"));


            Z.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;

        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q)
                LastQ = Environment.TickCount;
            if (args.Slot == SpellSlot.E)
                LastE = Environment.TickCount;
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Z.Item("misc.gc").GetValue<bool>()
                && W.IsReady()
                && Player.HealthPercent <= Z.Item("misc.gc.hp").GetValue<Slider>().Value
                && gapcloser.Sender.IsValidTarget())
            {
                W.Cast(gapcloser.End);
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!Enabled 
                || _damageToUnit == null)
            {
                return;
            }

            foreach (var unit in HeroManager.Enemies.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = _damageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage)/unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width*percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width*unit.Health/unit.MaxHealth;

                if (damage > unit.Health)
                {
                    Text.X = (int) barPos.X + XOffset;
                    Text.Y = (int) barPos.Y + YOffset - 13;
                    Text.text = "Killable: " + (unit.Health - damage);
                    Text.OnEndScene();
                }

                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, Color);

                if (Fill)
                {
                    float differenceInHP = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + (107*percentHealthAfterDamage);

                    for (int i = 0; i < differenceInHP; i++)
                    {
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, FillColor);
                    }
                }
            }

            if (!Z.Item("draw").GetValue<bool>())
                return;

            var target = TargetSelector.GetTarget(850f, TargetSelector.DamageType.Magical);
            if (Z.Item("draw.tg").GetValue<bool>() && target.IsValidTarget())
            {
                Render.Circle.DrawCircle(target.Position, 75f, System.Drawing.Color.DarkRed);
            }

            foreach (var x in Spells.Where(x => Z.Item("draw." + x.Slot.ToString().ToLowerInvariant()).GetValue<bool>())
                )
            {
                Render.Circle.DrawCircle(Player.Position, x.Range, x.IsReady()
                    ? System.Drawing.Color.Green
                    : System.Drawing.Color.Red
                    );
            }
        }

        public static void AutoE()
        {
            if (Z.Item("misc.manamenagertm").GetValue<bool>() &&
                Z.Item("misc.manamenagertm.slider").GetValue<Slider>().Value > Player.ManaPercent)
                return;

            if (!Z.Item("lasthit.e.auto").GetValue<bool>() || !E.IsReady())
                return;

            foreach (
                var unit in
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            x =>
                                x.IsEnemy && !x.IsDead && E.IsInRange(x) && E.GetDamage(x) > x.Health + 5 &&
                                x.IsValidTarget()))
            {
                E.CastOnUnit(unit);
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            Orbwalker.SetAttack(true);
            KS();
            TearStack();
            AutoLevel();
            var target = TargetSelector.GetTarget(900f, TargetSelector.DamageType.Magical);
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                {
                    if (Player.Distance(target.Position) > 440)
                        Orbwalker.SetAttack(false);
                    else
                        Orbwalker.SetAttack(true);
                }
                    Orbwalker.SetAttack(true);
                    Combo();
                    AABlock();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;



            }
        }

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(900f, TargetSelector.DamageType.Magical);

            if (!target.IsValidTarget())
                return;
            if (Z.Item("combo.q").GetValue<bool>())
            {
                if ((target.Health + 50 < Q.GetDamage(target) && Q.IsReady())
                    ||
                    (!target.HasBuffOfType(BuffType.Poison) && E.IsInRange(target) && E.IsReady() && Q.IsReady() &&
                     Player.Mana >= Q.Instance.ManaCost + 2*E.Instance.ManaCost)
                    || (!target.HasBuffOfType(BuffType.Poison) && E.Level < 1 && Q.IsReady() && Q.IsInRange(target))
                    ||
                    (Q.IsReady() && E.IsReady() && E.IsInRange(target) &&
                     target.Health + 25 < Q.GetDamage(target) + E.GetDamage(target) &&
                     Player.Mana >= Q.Instance.ManaCost + E.Instance.ManaCost))
                    Q.Cast(target);
            }
            if (Z.Item("combo.w").GetValue<bool>())
            {
                if ((Player.HealthPercent <= 25 && !Player.IsFacing(target) && target.IsFacing(Player) &&
                     target.IsValidTarget((W.Range/3)*2) && W.IsReady() && target.MoveSpeed >= Player.MoveSpeed)
                    ||
                    (!target.HasBuffOfType(BuffType.Poison) && Q.Delay*1000 + LastQ < Environment.TickCount &&
                     !Q.IsReady() && W.IsReady() && E.IsReady() && E.IsInRange(target) &&
                     Player.Mana >= W.Instance.ManaCost + 2*E.Instance.ManaCost)
                    ||
                    (!target.HasBuffOfType(BuffType.Poison) && Q.Delay*1000 + LastQ < Environment.TickCount &&
                     !Q.IsReady() && W.IsReady() && E.IsReady() && E.IsInRange(target) &&
                     Player.Mana >= W.Instance.ManaCost + E.Instance.ManaCost &&
                     W.GetDamage(target) + E.GetDamage(target) > target.Health + 25)
                    ||
                    (!target.HasBuffOfType(BuffType.Poison) && Q.Delay*1000 + LastQ < Environment.TickCount &&
                     (!Q.IsReady() || Q.GetDamage(target) < target.Health + 25) && W.IsReady() && W.IsInRange(target) &&
                     W.GetDamage(target) > target.Health + 25))
                    W.Cast(target);
            }
            if (Z.Item("combo.e").GetValue<bool>())
            {
                if ((target.HasBuffOfType(BuffType.Poison) && E.IsReady() && target.IsValidTarget(E.Range) &&
                     Environment.TickCount > LastE + Z.Item("misc.edelay").GetValue<Slider>().Value)
                    || (E.IsReady() && target.IsValidTarget(E.Range) && target.Health + 25 < E.GetDamage(target)))
                    E.CastOnUnit(target);
            }

            EasyRLogic();
            SmartR();

        }

        public static void LaneClear()
        {
            if (Z.Item("misc.manamenagertm").GetValue<bool>() &&
                Z.Item("misc.manamenagertm.slider").GetValue<Slider>().Value > Player.ManaPercent)
                return;

            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

            var jungleMinion = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Team == GameObjectTeam.Neutral
                                                                             && !x.IsDead
                                                                             &&
                                                                             x.Distance(ObjectManager.Player.Position) <=
                                                                             Q.Range)
                .OrderBy(x => x.MaxHealth)
                .FirstOrDefault();
            if (jungleMinion != null)
            {
                return;
            }

            MinionManager.FarmLocation QFarmLocation =
                Q.GetCircularFarmLocation(
                    MinionManager.GetMinionsPredictedPositions(MinionManager.GetMinions(Q.Range),
                        Q.Delay, Q.Width, Q.Speed,
                        Player.Position, Q.Range,
                        false, SkillshotType.SkillshotCircle), Q.Width);

            MinionManager.FarmLocation WFarmLocation =
                W.GetCircularFarmLocation
                    (MinionManager.GetMinionsPredictedPositions(MinionManager.GetMinions(W.Range),
                        W.Delay, W.Width, W.Speed,
                        Player.Position, W.Range,
                        false, SkillshotType.SkillshotCircle), W.Width);

            foreach (var minion in minionCount)
            {
                if (minion == null)
                    return;

                if (Z.Item("laneclear.q").GetValue<bool>()
                    && Q.IsReady()
                    && Player.Mana >= Q.Instance.ManaCost + 2*E.Instance.ManaCost)
                    Q.Cast(QFarmLocation.Position);

                var whit =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(x => x.Distance(WFarmLocation.Position) <= W.Width && !x.IsDead && x.IsEnemy);
                if (Z.Item("laneclear.w").GetValue<bool>() && W.IsReady() &&
                    whit.Count() >= Z.Item("laneclear.w.restriction").GetValue<Slider>().Value)
                    W.Cast(WFarmLocation.Position);

                var emin =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(x => !x.IsDead && x.IsEnemy && E.IsInRange(x))
                        .OrderBy(x => x.Health)
                        .FirstOrDefault();

                if (Z.Item("laneclear.e").GetValue<bool>() && E.IsReady())
                {
                    if ((emin.HasBuffOfType(BuffType.Poison)
                        && !Z.Item("laneclear.e.lasthit").GetValue<bool>())

                        || (!emin.HasBuffOfType(BuffType.Poison)
                            && !Z.Item("laneclear.e.restriction").GetValue<bool>())

                        || (emin.HasBuffOfType(BuffType.Poison)
                        && Z.Item("laneclear.e.lasthit").GetValue<bool>()
                        && emin.Health <= E.GetDamage(emin)))
                        E.CastOnUnit(emin);
                }
            }
        }


        public static void JungleClear()
        {
            var jungle = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (!jungle.Any())
                return;

            var bigjungle = jungle.First();

            if (Q.IsReady() &&
                bigjungle.IsValidTarget(Q.Range))
            {
                Q.Cast(bigjungle);
            }

            if (E.IsReady()
                && bigjungle.HasBuffOfType(BuffType.Poison)
                && bigjungle.IsValidTarget(E.Range))
            {
                E.Cast(bigjungle);
            }

            if (W.IsReady() 
                && bigjungle.IsValidTarget(W.Range))
            {
                W.Cast(bigjungle);
            }

        }
    

    public static void LastHit()
        {
            if (Z.Item("misc.manamenagertm").GetValue<bool>() && Z.Item("misc.manamenagertm.slider").GetValue<Slider>().Value > Player.ManaPercent)
                return;

            if (!Z.Item("lasthit.e").GetValue<bool>() || !E.IsReady())
                return;

            foreach (var min in ObjectManager.Get<Obj_AI_Minion>().Where(x =>
                E.IsInRange(x) 
                && !x.IsDead 
                && x.IsEnemy 
                && x.Health + 5 < E.GetDamage(x)))
            {
                E.CastOnUnit(min);
            }
        }

        private static void EasyRLogic()
        {

            var rTarget = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            var rSpell = Z.Item("combo.r").GetValue<bool>();
            var rminhitSpell = Z.Item("combo.r.minhit").GetValue<Slider>().Value;
            var rfaceSpell = Z.Item("combo.r.minfacing").GetValue<Slider>().Value;

            List<Obj_AI_Hero> targets = HeroManager.Enemies.Where(o => R.WillHit(o, rTarget.Position)
                                                                       && o.Distance(Player.Position) < 600).ToList();

            var facing =
                HeroManager.Enemies.Where(
                    x => R.WillHit(x, rTarget.Position)
                         && x.IsFacing(Player)
                         && !x.IsDead
                         && x.IsValidTarget(600));

            if (rSpell)
            {
                if ((targets.Count() >= rminhitSpell
                    || facing.Count() >= rfaceSpell)
                    && R.IsReady()
                    && rTarget.Health >= (Q.GetDamage(rTarget) + 2*E.GetDamage(rTarget) + R.GetDamage(rTarget)))
                {
                    R.Cast(rTarget);
                }
            }
        }

        private static void SmartR()
        {
            var srSpell = Z.Item("combo.r.smart").GetValue<bool>();
            var rTarget = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            if (Player.CountEnemiesInRange(1200) == 1)

                return;

            if (srSpell)
            {
                if (rTarget.IsValidTarget(500))
                {
                    if (rTarget.IsFacing(Player))
                    {
                        if (Player.HealthPercent + 25 <= rTarget.HealthPercent
                            && R.IsReady())
                        {
                            R.Cast(rTarget);
                        }

                        if (rTarget.HasBuffOfType(BuffType.Poison)
                            && E.IsReady() && R.IsReady()
                            && Player.Mana >= (2*E.Instance.ManaCost + R.Instance.ManaCost)
                            && rTarget.Health < (E.GetDamage(rTarget)*4 + R.GetDamage(rTarget)))
                        {
                            R.Cast(rTarget);
                        }
                        if (!rTarget.HasBuffOfType(BuffType.Poison)
                            && Q.IsReady() && E.IsReady() && R.IsReady()
                            && rTarget.Health < (Q.GetDamage(rTarget) + E.GetDamage(rTarget)*4 + R.GetDamage(rTarget))
                            && Player.Mana >= (Q.Instance.ManaCost + 2*E.Instance.ManaCost + R.Instance.ManaCost))
                        {
                            R.Cast(rTarget);
                        }    
                    }
                }
            }
        }

        public static void Harass()
        {
            if (Z.Item("misc.manamenagertm").GetValue<bool>() && Z.Item("misc.manamenagertm.slider").GetValue<Slider>().Value > Player.ManaPercent)
                return;
            var target = TargetSelector.GetTarget(850f, TargetSelector.DamageType.Magical);
            if (Z.Item("harass.q").GetValue<bool>() && Q.IsReady() && Q.IsInRange(target))
                Q.Cast(target);
            if (Z.Item("harass.w").GetValue<bool>() && W.IsReady() && W.IsInRange(target))
                W.Cast(target);
            if (Z.Item("harass.e").GetValue<bool>() && E.IsReady() && E.IsInRange(target) && Environment.TickCount > LastE + Z.Item("misc.edelay").GetValue<Slider>().Value
                && ((target.HasBuffOfType(BuffType.Poison) && Z.Item("harass.e.restriction").GetValue<bool>())
                || (!target.HasBuffOfType(BuffType.Poison) && !Z.Item("harass.e.restriction").GetValue<bool>())))
                E.CastOnUnit(target);
        }

        public static void KS()
        {
            if (E.IsReady() && Z.Item("misc.eks").GetValue<bool>())
            {
                foreach (var x in HeroManager.Enemies.Where(x => !x.IsDead
                    && x.IsValidTarget(E.Range)
                    && x.Health + 10 < E.GetDamage(x)))
                {
                    E.CastOnUnit(x);
                }
            }

            if (Q.IsReady() && Z.Item("misc.qks").GetValue<bool>())
            {
                foreach (var x in HeroManager.Enemies.Where(x => !x.IsDead
                    && x.IsValidTarget(Q.Range)
                    && x.Health + 25 < Q.GetDamage(x)))
                {
                    Q.Cast(x);
                }
            }

            if (W.IsReady() && Z.Item("misc.wks").GetValue<bool>())
            {
                foreach (var x in HeroManager.Enemies.Where(x => !x.IsDead
                    && x.IsValidTarget(W.Range)
                    && x.Health + 25 < W.GetDamage(x)))
                {
                    if ((!Q.IsReady() && !x.HasBuffOfType(BuffType.Poison) && Q.Delay * 1000 + LastQ < Environment.TickCount)
                        || (Q.IsReady() && Q.GetDamage(x) < x.Health))
                        W.Cast(x);
                }
            }
            
        }

        private static void TearStack()
        {

            if (!Z.Item("misc.itemstack").GetValue<bool>()
                || !Player.InFountain())
                return;

            if (ItemData.Tear_of_the_Goddess.Stacks.Equals(750)
                || Items.HasItem(ItemData.Seraphs_Embrace.Id)
                || ItemData.Archangels_Staff.Stacks.Equals(750))
            { 
                return;
            }

            if (Q.IsReady()
                && ((Items.HasItem(ItemData.Tear_of_the_Goddess.Id)
                     || Items.HasItem(ItemData.Archangels_Staff.Id))))
            {
                Q.Cast(Player.Position.Extend(Game.CursorPos, Q.Range));
            }
        }
        private static void AutoLevel()
        {
            int qL = Player.Spellbook.GetSpell(SpellSlot.Q).Level + q;
            int wL = Player.Spellbook.GetSpell(SpellSlot.W).Level + w;
            int eL = Player.Spellbook.GetSpell(SpellSlot.E).Level + e;
            int rL = Player.Spellbook.GetSpell(SpellSlot.R).Level + r;
            if (qL + wL + eL + rL < ObjectManager.Player.Level)
            {
                int[] level = { 0, 0, 0, 0 };
                for (int i = 0; i < ObjectManager.Player.Level; i++)
                {
                    level[abilitySequence[i] - 1] = level[abilitySequence[i] - 1] + 1;
                }
                if (qL < level[0]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (wL < level[1]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (eL < level[2]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (rL < level[3]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);

            }
        }
        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0f;
            if (Q.IsReady())
            {
                damage += Q.GetDamage(enemy);
            }
            if (W.IsReady()
                && !Q.IsReady())
            {
                damage += W.GetDamage(enemy);
            }
            if (E.IsReady())
            {
                damage += E.GetDamage(enemy)*3;
            }
            return damage;
        }

        private static void AABlock()
        {
            var aaBlock = Z.Item("misc.aablock").GetValue<bool>();
            if (aaBlock)
            {
                Orbwalker.SetAttack(false);
            }
        }
    }
}
