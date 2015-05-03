using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Item = LeagueSharp.Common.Items.Item;

namespace The_Masterpiece.Plugins
{
    internal class Ryze
    {
        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static Spellbook SpellManager { get { return ObjectManager.Player.Spellbook; } }
        public static Spell Q, W, E, R;
        public static List<Spell> SpellList = new List<Spell>();
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Hero Target;
        public static int[] LevelOrder;

        public Ryze()
        {
            /*Synthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Senior);
            PromptBuilder PB = new PromptBuilder();
            string rate = "<prosody rate=\"medium\"> This is the slow speaking rate. </prosody>";
            PB.AppendSsmlMarkup(rate);
            PB.AppendText("Greetings, summoner! Today, the dark mage shall assist you in your affairs!");
            Synthesizer.Speak(PB);*/

            Q = new Spell(SpellSlot.Q, 900f);
            Q.SetSkillshot(0.25f, 50f, 1800f, true, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 580f);
            E = new Spell(SpellSlot.E, 580f);
            E.SetTargetted(0.2f, float.MaxValue);
            R = new Spell(SpellSlot.R);

            SpellList.Add(Q); SpellList.Add(W); SpellList.Add(E); SpellList.Add(R);

            LevelOrder = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };



            #region - Menu -
            Config = new Menu("The Masterpiece - Ryze", "ryzeroot", true);
            var TSMenu = new Menu("Target Selector", "ts");
            TargetSelector.AddToMenu(TSMenu);
            Config.AddSubMenu(TSMenu);

            var OrbMenu = new Menu("Orbwalker", "orb");
            Orbwalker = new Orbwalking.Orbwalker(OrbMenu);
            Config.AddSubMenu(OrbMenu);

            var MCombo = new Menu("Combo", "combo");
            MCombo.AddItem(new MenuItem("combo.q", "Use Overload (Q)").SetValue(true));
            MCombo.AddItem(new MenuItem("combo.w", "Use Rune Prison (W)").SetValue(true));
            MCombo.AddItem(new MenuItem("combo.e", "Use Spell Flux (E)").SetValue(true));
            MCombo.AddItem(new MenuItem("combo.r", "Use Desperate Power (R)").SetValue(true));
            MCombo.AddItem(new MenuItem("combo.r.count", "Min. Enemies for R").SetValue(new Slider(1, 1, 5)));
            Config.AddSubMenu(MCombo);

            var MHarass = new Menu("Harass", "harass");
            MHarass.AddItem(new MenuItem("harass.q", "Use Overload (Q)").SetValue(true));
            MHarass.AddItem(new MenuItem("harass.w", "Use Rune Prison (W)").SetValue(true));
            MHarass.AddItem(new MenuItem("harass.e", "Use Spell Flux (E)").SetValue(true));
            Config.AddSubMenu(MHarass);

            var MLaneClear = new Menu("Lane Clear", "lane");
            MLaneClear.AddItem(new MenuItem("lane.q", "Use Overload (Q)").SetValue(true));
            MLaneClear.AddItem(new MenuItem("lane.e", "Use Spell Flux (E)").SetValue(true));
            Config.AddSubMenu(MLaneClear);

            var MLastHit = new Menu("Last Hit", "last");
            MLastHit.AddItem(new MenuItem("last.q", "Use Overload (Q)").SetValue(true));
            Config.AddSubMenu(MLastHit);

            var MEscape = new Menu("Escape", "escape");
            MEscape.AddItem(new MenuItem("escape.w", "Use Rune Prison (W)").SetValue(true));
            MEscape.AddItem(new MenuItem("escape.r", "Use Desperate Power (R)").SetValue(true));
            Config.AddSubMenu(MEscape);

            var MExtra = new Menu("Extra", "extra");
            MExtra.AddItem(new MenuItem("extra.autolevel", "Auto Level Spells").SetValue(true));
            MExtra.AddItem(new MenuItem("extra.ks", "Attemp to Steal Kills").SetValue(true));
            MExtra.AddItem(new MenuItem("extra.mm", "Save % Mana For Combo").SetValue(new Slider(30, 0, 100)));
            MExtra.AddItem(new MenuItem("extra.counter", "Counter Enemy Harass").SetValue(true));
            MExtra.AddItem(new MenuItem("extra.gc", "Anti Gap Closer").SetValue(true));
            MExtra.AddItem(new MenuItem("extra.stack", "Stack Items").SetValue(true));
            MExtra.AddSubMenu(new Menu("Hitchances", "extra.hc"));
            MExtra.SubMenu("extra.hc").AddItem(new MenuItem("extra.hc.q", "Overload (Q)").SetValue(new StringList(new[] { "Normal", "High" })));
            MExtra.AddSubMenu(new Menu("Keybinds", "extra.kb"));
            MExtra.SubMenu("extra.kb").AddItem(new MenuItem("extra.kb.combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            MExtra.SubMenu("extra.kb").AddItem(new MenuItem("extra.kb.harass", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));
            MExtra.SubMenu("extra.kb").AddItem(new MenuItem("extra.kb.escape", "Escape").SetValue(new KeyBind('G', KeyBindType.Press)));
            MExtra.SubMenu("extra.kb").AddItem(new MenuItem("extra.kb.lane", "LaneClear").SetValue(new KeyBind('V', KeyBindType.Press)));
            MExtra.SubMenu("extra.kb").AddItem(new MenuItem("extra.kb.last", "LastHit").SetValue(new KeyBind('X', KeyBindType.Press)));
            Config.AddSubMenu(MExtra);

            var MDrawings = new Menu("Drawings", "drawings");
            MDrawings.AddItem(new MenuItem("draw", "Use Drawings").SetValue(true));
            MDrawings.AddItem(new MenuItem("draw.q", "Draw Overload (Q)").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            MDrawings.AddItem(new MenuItem("draw.w", "Draw Rune Prison (W)").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            MDrawings.AddItem(new MenuItem("draw.e", "Draw Spell Flux (E)").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            MDrawings.AddItem(new MenuItem("draw.target", "Draw Target").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            MDrawings.AddItem(new MenuItem("draw.dmg", "Draw Damage").SetValue(true));
            Config.AddSubMenu(MDrawings);

            Config.AddToMainMenu();
            #endregion

            Utility.HpBarDamageIndicator.DamageToUnit = GetDamageTo;
            Utility.HpBarDamageIndicator.Enabled = Config.Item("draw.dmg").GetValue<bool>();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;


        }

        public static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var noob = gapcloser.Sender;
            if (Config.Item("extra.gc").GetValue<bool>())
            {
                if (Q.IsReady()
                    && Q.GetDamage(noob) > noob.Health + 50
                    && noob.IsValidTarget(Q.Range))
                {
                    Q.CastIfHitchanceEquals(noob, HitChance.High);
                    Q.CastIfHitchanceEquals(noob, HitChance.Immobile);
                }

                if (W.IsReady())
                {
                    if ((!noob.HasBuffOfType(BuffType.SpellShield)
                    && !noob.HasBuffOfType(BuffType.SpellImmunity))
                        || W.GetDamage(noob) > noob.Health + 50)
                        W.Cast(noob);
                }
            }
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            if (!Config.Item("draw").GetValue<bool>())
                return;

            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (Config.Item("draw.target").GetValue<Circle>().Active && target != null)
            {
                Render.Circle.DrawCircle(target.Position, 75f, Config.Item("draw.target").GetValue<Circle>().Color);
            }

            foreach (var x in SpellList.Where(y => Config.Item("draw." + y.Slot.ToString().ToLowerInvariant()) != null && Config.Item("draw." + y.Slot.ToString().ToLowerInvariant()).GetValue<Circle>().Active))
            {
                Render.Circle.DrawCircle(Player.Position, x.Range, x.IsReady()
                ? System.Drawing.Color.Green
                : System.Drawing.Color.Red
                );
            }
        }

        public static void Game_OnUpdate(EventArgs args)
        {
            LevelSpells();
            SetBestTarget();
            if (Config.Item("extra.stack").GetValue<bool>())
                Stackerino();
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

        public static void KS()
        {
            var tg = Target;
            if (tg != null)
            {
                var realhp = tg.Health + 50;
                if (W.IsReady()
                    && W.IsInRange(tg)
                    && W.GetDamage(tg) > realhp)
                    W.Cast(tg);
                else if (E.IsReady()
                    && E.IsInRange(tg)
                    && E.GetDamage(tg) > realhp)
                    E.CastOnUnit(tg);
                else if (Q.IsReady()
                    && Q.IsInRange(tg)
                    && Q.GetDamage(tg) > realhp)
                    Q.CastIfHitchanceEquals(tg, HitChance.High);
            }
        }

        public static void Harass()
        {
            if (Target != null || Config.Item("extra.mm").GetValue<Slider>().Value < Player.ManaPercent)
            {

                if (Config.Item("harass.q").GetValue<bool>()
                    && Q.IsReady())
                    Q.CastIfHitchanceEquals(Target, QHC);
                if (Config.Item("harass.w").GetValue<bool>()
                    && W.IsReady()
                    && W.IsInRange(Target))
                    W.Cast(Target);
                if (Config.Item("harass.e").GetValue<bool>()
                    && E.IsReady()
                    && E.IsInRange(Target))
                    E.CastOnUnit(Target);
            }
        }

        public static void Escape()
        {
            var closestmfer = HeroManager.Enemies.OrderBy(x => x.Distance(Player.Position)).FirstOrDefault();
            if (closestmfer != null)
            {
                if (W.IsReady()
                    && closestmfer.IsValidTarget(W.Range)
                    && Config.Item("escape.w").GetValue<bool>())
                    W.Cast(closestmfer);
                if (R.IsReady()
                    && Player.CountEnemiesInRange(Q.Range + 100) >= 1
                    && Config.Item("escape.r").GetValue<bool>()
                    && closestmfer.MoveSpeed > Player.MoveSpeed)
                    R.Cast();
            }
        }

        public static void Lane()
        {
            if (Config.Item("extra.mm").GetValue<Slider>().Value >= Player.ManaPercent)
                return;

            if (UltActive)
            {
                foreach (var m in MinionManager.GetMinions(E.Range).Where(x => x.IsEnemy && !x.IsDead))
                {
                    if (CountMinionsInRange(m, 200) >= 3
                        && Config.Item("lane.e").GetValue<bool>())
                        E.CastOnUnit(m);
                }
            }
            else
            {
                foreach (var m in MinionManager.GetMinions(Q.Range).Where(x => x.IsEnemy && !x.IsDead))
                {
                    if (m.Health < Q.GetDamage(m)
                        && Q.GetPrediction(m).Hitchance == HitChance.Medium
                        && Config.Item("lane.q").GetValue<bool>())
                        Q.Cast(m);
                    else if (E.IsReady()
                        && Config.Item("lane.e").GetValue<bool>())
                        E.CastOnUnit(m);
                }
            }
        }

        public static bool UltActive
        {
            get
            {
                return Player.HasBuff("RyzeR");
            }
        }

        public static bool PassiveActive
        {
            get
            {
                return Player.HasBuff("RyzePassiveCharged");
            }
        }

        public static int PassiveStacks
        {
            get
            {
                if (Player.Buffs.FirstOrDefault(b => b.DisplayName == "RyzePassiveStack") != null)
                    return Player.Buffs.FirstOrDefault(b => b.DisplayName == "RyzePassiveStack").Count;
                return 0;
            }
        }

        public static int CountMinionsInRange(Obj_AI_Base unit, float range)
        {
            var minionsinrange = MinionManager.GetMinions(unit.Position, range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
            return (int)minionsinrange.Count;
        }

        public static void Last()
        {
            if (Config.Item("extra.mm").GetValue<Slider>().Value < Player.ManaPercent)
            {

                foreach (var mforq in MinionManager.GetMinions(Q.Range).OrderBy(x => x.Health))
                {
                    if (mforq != null
                        && mforq.Health + 10 < Q.GetDamage(mforq)
                        && Q.IsReady()
                        && Config.Item("last.q").GetValue<bool>()
                        && Q.GetPrediction(mforq).Hitchance == HitChance.Medium)
                        Q.Cast(mforq);
                }
            }
        }

        public static void Stackerino()
        {
            if (StackingItemOwned(Player)
                && Config.Item("extra.stack").GetValue<bool>()
                && Q.IsReady()
                && Player.InFountain())
                Q.Cast(Game.CursorPos);
        }

        public static Item Tear = new Item(3070);
        public static Item ScarTear = new Item(3073);
        public static Item Archangel = new Item(3003);
        public static Item ScarArchangel = new Item(3007);
        public static Item Manamune = new Item(3004);
        public static Item ScarManamune = new Item(3008);
        public static bool StackingItemOwned(Obj_AI_Hero Hero)
        {
            return Tear.IsOwned(Hero) || Archangel.IsOwned(Hero) || Manamune.IsOwned(Hero)
                    || ScarTear.IsOwned(Hero) || ScarArchangel.IsOwned(Hero) || ScarManamune.IsOwned(Hero);
        }


        public static void Combo()
        {
            if (!CanKillTarget() && Target != null)
            {

                if (Q.IsReady() && W.IsReady() && E.IsReady() && R.IsReady()
                    && Config.Item("combo.r").GetValue<bool>()
                    && Target != null
                    && Player.CountEnemiesInRange(Q.Range) >= Config.Item("combo.r.count").GetValue<Slider>().Value)
                    R.Cast();

                if (W.IsReady()
                    && W.IsInRange(Target)
                    && Config.Item("combo.w").GetValue<bool>())
                {
                    if (W.GetDamage(Target) > Target.Health + 50
                        || !Target.IsFacing(Player)
                        || (Q.IsReady() && E.IsReady()))
                        W.Cast(Target);
                }

                if (Q.IsReady()
                    && Target != null
                    && Config.Item("combo.q").GetValue<bool>())
                {
                    Q.CastIfHitchanceEquals(Target, QHC);
                    Q.CastIfHitchanceEquals(Target, HitChance.Immobile);
                }


                if (E.IsReady()
                    && E.IsInRange(Target)
                    && Config.Item("combo.e").GetValue<bool>())
                    E.CastOnUnit(Target);
            }
        }

        public static float GetDamageTo(Obj_AI_Hero hero)
        {
            double damage = 0d;
            if (Q.IsReady() && Config.Item("combo.q").GetValue<bool>())
                damage += Q.GetDamage(hero);
            if (W.IsReady() && Config.Item("combo.w").GetValue<bool>())
                damage += W.GetDamage(hero);
            if (E.IsReady() && Config.Item("combo.e").GetValue<bool>())
                damage += E.GetDamage(hero);

            return (float)damage;
        }

        public static bool CanKillTarget()
        {
            var realhp = Target.Health + 50;
            return (Q.IsReady() && Q.GetDamage(Target) > realhp)
                || (W.IsReady() && W.GetDamage(Target) > realhp)
                || (E.IsReady() && E.GetDamage(Target) > realhp);
        }

        public static void SetBestTarget()
        {
            var tstarg = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            foreach (var Hero in HeroManager.Enemies.Where(x => !x.IsDead && x.Distance(Player.Position) < Q.Range))
            {
                var calchp = Hero.Health + 50;
                var qpredoutput = Q.GetPrediction(Hero);
                if (calchp < Q.GetDamage(Hero)
                    && R.IsReady()
                    && Config.Item("combo.q").GetValue<bool>()
                    )
                {
                    if ((QHC == HitChance.High && qpredoutput.Hitchance == HitChance.High)
                        || (QHC == HitChance.Medium && qpredoutput.Hitchance == HitChance.Medium))
                        Target = Hero;
                }

                else if (calchp < W.GetDamage(Hero)
                    && W.IsReady()
                    && Config.Item("combo.w").GetValue<bool>()
                    && Target.IsValidTarget(W.Range))
                    Target = Hero;

                else if (calchp < E.GetDamage(Hero)
                    && E.IsReady()
                    && Config.Item("combo.e").GetValue<bool>()
                    && Target.IsValidTarget(E.Range))
                    Target = Hero;
            }

            if (Target == null && tstarg != null)
                Target = tstarg;
        }

        public static HitChance QHC
        {
            get
            {
                if (Config.Item("extra.hc.q").GetValue<StringList>().SelectedIndex == 0)
                    return HitChance.Medium;
                else return HitChance.High;
            }
        }

        public static void LevelSpells()
        {
            int q = Q.Level, w = W.Level, e = E.Level, r = R.Level;

            if (q + w + e + r < Player.Level)
            {
                int[] Levels = new int[] { 0, 0, 0, 0 };
                for (int i = 1; i < Player.Level; i++)
                    (Levels[LevelOrder[i] - 1]) = (Levels[LevelOrder[i] - 1] + 1);
                if (q < Levels[0])
                    SpellManager.LevelSpell(Q.Slot);
                if (w < Levels[1])
                    SpellManager.LevelSpell(W.Slot);
                if (e < Levels[2])
                    SpellManager.LevelSpell(E.Slot);
                if (r < Levels[3])
                    SpellManager.LevelSpell(R.Slot);
            }
        }



    }
}
