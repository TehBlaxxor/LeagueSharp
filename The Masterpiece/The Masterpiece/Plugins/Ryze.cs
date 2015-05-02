using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;

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
        public static List<Spell> SpellList;
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Hero Target = null;
        public static int[] LevelOrder;
        public static SpeechSynthesizer Synthesizer = new SpeechSynthesizer();

        public Ryze()
        {
            Synthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Senior);
            PromptBuilder PB = new PromptBuilder();
            string rate = "<prosody rate=\"medium\"> This is the slow speaking rate. </prosody>";
            PB.AppendSsmlMarkup(rate);
            PB.AppendText("Greetings, summoner! Today, the dark mage shall assist you in your affairs!");
            Synthesizer.Speak(PB);

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
            MDrawings.AddItem(new MenuItem("draw.q", "Draw Overload (Q)").SetValue(true));
            MDrawings.AddItem(new MenuItem("draw.w", "Draw Rune Prison (W)").SetValue(true));
            MDrawings.AddItem(new MenuItem("draw.e", "Draw Spell Flux (E)").SetValue(true));
            MDrawings.AddItem(new MenuItem("draw.target", "Draw Target").SetValue(true));
            MDrawings.AddItem(new MenuItem("draw.dmg", "Draw Damage").SetValue(true));
            Config.AddSubMenu(MDrawings);
            #endregion

            Utility.HpBarDamageIndicator.DamageToUnit = GetDamageTo;
            Utility.HpBarDamageIndicator.Enabled = Config.Item("draw.dmg").GetValue<bool>();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

        }

        void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
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
                    
            }
        }

        void Drawing_OnDraw(EventArgs args)
        {
            throw new NotImplementedException();
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

        public static void KS()
        {

        }

        public static void Harass()
        {

        }

        public static void Escape()
        {

        }

        public static void Lane()
        {

        }

        public static void Last()
        {

        }

        public static void Combo()
        {
            if (CanKillTarget()) return;


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
            foreach (var Hero in HeroManager.Enemies.Where(x => !x.IsDead && x.Distance(Player.Position) < Q.Range))
            {
                var calchp = Hero.Health + 50;
                var qpredoutput = Q.GetPrediction(Hero);
                var tstarg = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
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

                else if (tstarg != null) Target = tstarg;

                else Target = null;
            }
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
