using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Items = LeagueSharp.Common.Items;
using The_Masterpiece.Handlers;

namespace The_Masterpiece.Plugins
{
    internal class Vayne : BaseChampion
    {
        public static Spell Q, W, E, R;

        public static List<Spell> Spells = new List<Spell>();
        private AttackableUnit lastObjectAttacked = null;
        private int stackCount = 0;

        //Bonus de dano no 3 ataque
        public int[] danoVerdadeiro = { 20, 30, 40, 50, 60 };
        public int[] bonusPorcentagemHP = { 4, 5, 6, 7, 8 };


        public Vayne()
        {
            Q = new Spell(SpellSlot.Q, 300f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 550f);
            E.SetTargetted(0.25f, 1600f);
            R = new Spell(SpellSlot.R);


            Spells.Add(Q);
            Spells.Add(W);
            Spells.Add(E);
            Spells.Add(R);

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

        }

        public float danoDardoPrata(Obj_AI_Base target)
        {
            return (float)danoVerdadeiro[Player.Spellbook.GetSpell(SpellSlot.W).Level] + target.MaxHealth * (bonusPorcentagemHP[Player.Spellbook.GetSpell(SpellSlot.W).Level] / 100);
        }

        public int enemiesInRange(Obj_AI_Hero obj, float range)
        {
            return obj.CountEnemiesInRange(range);
        }

        public float calcularDanoAtaque(Obj_AI_Base target, bool autoAttack)
        {
            double dmg = 0;

            if (autoAttack && stackCount == 2 && target == lastObjectAttacked)
            {
                dmg = Player.GetAutoAttackDamage(target, false) + danoDardoPrata(target);
            }
            else if (!autoAttack && stackCount == 2 && target == lastObjectAttacked)
            {
                dmg = danoDardoPrata(target);
            }

            return (float)dmg;
        }

        public void UseItem(int id, Obj_AI_Hero target = null)
        {
            if (Items.HasItem(id) && Items.CanUseItem(id))
            {
                Items.UseItem(id, target);
            }
        }

        public bool CanUseItem(int id)
        {
            return Items.HasItem(id) && Items.CanUseItem(id);
        }

        public Vector3 getCondemnPosition(Obj_AI_Hero target)
        {
            if (CondemnCheck(Player.Position, out target))
                return Vector3.Zero;

            for (var I = 0; I <= 360; I += 65)
            {
                var F1 = new Vector2(Player.Position.X + (float)(300 * Math.Cos(I * (Math.PI / 180))), Player.Position.Y + (float)(300 * Math.Sin(I * (Math.PI / 180)))).To3D();
                Obj_AI_Hero targ;
                if (CondemnCheck(F1, out targ))
                {
                    return F1;
                }
            }
            return Vector3.Zero;
        }

        bool isUnderEnemyTurret(Vector3 Position)
        {
            return Position.UnderTurret(true);
        }

        bool CondemnCheck(Vector3 Position, out Obj_AI_Hero target)
        {
            if (isUnderEnemyTurret(Player.Position) && !GetValue<bool>("NoEEnT"))
            {
                target = null;
                return false;
            }

            foreach (var En in HeroManager.Enemies.Where(hero => hero.IsValidTarget() && !GetValue<bool>("nC" + hero.ChampionName) && hero.Distance(Player.Position) <= E.Range))
            {
                var EPred = E.GetPrediction(En);
                int pushDist = Menu.Item("PushDistance").GetValue<Slider>().Value;
                var FinalPosition = EPred.UnitPosition.To2D().Extend(Position.To2D(), -pushDist).To3D();
                for (int i = 1; i < pushDist; i += (int)En.BoundingRadius)
                {
                    Vector3 loc3 = EPred.UnitPosition.To2D().Extend(Position.To2D(), -i).To3D();
                    var OrTurret = GetValue<bool>("CondemnTurret") && isUnderTurret(FinalPosition);
                    var OrFountain = GetValue<bool>("CondemnTurret") && isFountain(FinalPosition);
                    if (isWall(loc3) || OrTurret || OrFountain)
                    {
                        target = En;
                        return true;
                    }
                }
            }
            target = null;
            return false;
        }

        bool isWall(Vector3 Pos)
        {
            return Pos.IsWall();
        }

        bool isUnderTurret(Vector3 Position)
        {
            foreach (var tur in ObjectManager.Get<Obj_AI_Turret>().Where(turr => turr.IsAlly && (turr.Health != 0)))
            {
                if (tur.Distance(Position) <= 975f) return true;
            }
            return false;
        }
        bool isFountain(Vector3 Position)
        {
            float fountainRange = 750;
            var map = Utility.Map.GetMap();
            if (map != null && map.Type == Utility.Map.MapType.SummonersRift)
            {
                fountainRange = 1050;
            }
            return
                ObjectManager.Get<GameObject>()
                    .Where(spawnPoint => spawnPoint is Obj_SpawnPoint && spawnPoint.IsAlly)
                    .Any(
                        spawnPoint =>
                            Vector2.Distance(Position.To2D(), spawnPoint.Position.To2D()) <
                            fountainRange);
        }

        public bool ManaManager()
        {
            return Player.Mana >= Player.MaxMana * (GetValue<Slider>("themp.manamanager.percentage").Value / 100);
        }

        void Game_OnUpdate(EventArgs args)
        {
            if (Menu.Item("themp.kb.combo").GetValue<bool>())
                DoCombo();
            if (Menu.Item("themp.kb.harass").GetValue<bool>())
                DoHarass();
            if (Menu.Item("themp.kb.escape").GetValue<bool>())
            {
                GlobalMethods.Flee();
                DoEscape();
            }

        }

        void Drawing_OnDraw(EventArgs args)
        {
            if (!Menu.Item("themp.drawings.draw").GetValue<bool>())
                return;

            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);

            var wts = Drawing.WorldToScreen(Player.Position);

            var p = Player.Position;

            if (GetValue<bool>("themp.drawings.condemn"))
            {
                var position = getCondemnPosition(target);
                Render.Circle.DrawCircle(position, 80, System.Drawing.Color.White);
            }
            
            if (Menu.Item("themp.drawings.target").GetValue<Circle>().Active && target != null)
            {
                Render.Circle.DrawCircle(target.Position, 75f, Menu.Item("themp.drawings.target").GetValue<Circle>().Color);
            }

            foreach (var x in Spells.Where(y => Menu.Item("themp.drawings." + y.Slot.ToString().ToLowerInvariant()).GetValue<Circle>().Active))
            {
                Render.Circle.DrawCircle(Player.Position, x.Range, x.IsReady()
                ? System.Drawing.Color.Green
                : System.Drawing.Color.Red
                );
            }
        }

        void Interrupter2_OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!GetValue<bool>("themp.misc.interrupte") || args.DangerLevel != Interrupter2.DangerLevel.High || sender.Distance(ObjectManager.Player.Position) <= E.Range)
                return;

            E.Cast();
        }

        void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && GetValue<bool>("themp.misc.gapclosere"))
                E.Cast(Game.CursorPos);
        }

        public override void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe)
            {
                if (!target.IsDead)
                {
                    if (lastObjectAttacked == target)
                    {
                        stackCount++;
                        if (stackCount > 2)
                        {
                            stackCount = 0;
                        }
                    }
                    else
                    {
                        stackCount = 0;
                    }
                    lastObjectAttacked = target;
                }
            }
            else
            {
                return;
            }

            if (Menu.Item("themp.kb.harass").GetValue<bool>())
            {
                if (GetValue<bool>("themp.harass.q") && Q.IsReady() && ManaManager())
                {
                    Q.Cast(Game.CursorPos);
                }
            }
            else if (Menu.Item("themp.kb.laneclear").GetValue<bool>())
            {
                if (GetValue<bool>("themp.laneclear.q") && Q.IsReady() && ManaManager())
                {
                    Q.Cast(Game.CursorPos);
                }
            }
        }

        private void DoCombo()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
            UseItems();
            UseSummoners();
            if (target == null)
                return;

            if (R.IsReady() && GetValue<bool>("themp.combo.r") && Player.Distance(target.Position) < Player.AttackRange && GetValue<Slider>("themp.combo.minenemiesr").Value >= enemiesInRange(Player, Player.AttackRange))
            {
                R.Cast();
            }

            if (CanUseItem(3142) && Player.Distance(target.Position) < Player.AttackRange && GetValue<bool>("YoumuuC"))
                UseItem(3142);

            if ((Player.Health / Player.MaxHealth) * 100 < (target.Health / target.MaxHealth) * 100 && (CanUseItem(3153) || CanUseItem(3144)) && GetValue<bool>("BotrkC"))
            {
                UseItem(3144, target);

                UseItem(3153, target);
            }

            if (stackCount == 2 && !lastObjectAttacked.IsDead && lastObjectAttacked.Type == GameObjectType.obj_AI_Hero)
            {
                if (E.IsReady() && calcularDanoAtaque(target, false) > target.Health && E.IsInRange(target.Position) && GetValue<bool>("themp.combo.e"))
                {
                    E.Cast(target);
                }
                else if (E.IsReady() && Q.IsReady() && calcularDanoAtaque(target, false) > target.Health && Player.Distance(target.Position) < E.Range + Q.Range && GetValue<bool>("themp.combo.e") && GetValue<bool>("themp.combo.e"))
                {
                    Q.Cast(target.Position);
                    E.Cast(target);
                }
            }

            if (Q.IsReady() &&
                stackCount == 2 &&
                !lastObjectAttacked.IsDead &&
                lastObjectAttacked.Type == GameObjectType.obj_AI_Hero &&
                target == lastObjectAttacked &&
                Q.IsInRange(target.Position) && GetValue<bool>("themp.combo.q"))
            {
                Q.Cast(target.Position);
            }

            if (E.IsReady() && GetValue<bool>("themp.combo.e"))
            {
                if (CondemnCheck(Player.Position, out target))
                {
                    E.Cast(target);
                }
                else
                {
                    var postition = getCondemnPosition(target);
                    if (postition == Vector3.Zero)
                        return;

                    if (Player.Distance(postition) <= Q.Range && Q.IsReady() && GetValue<bool>("themp.combo.q"))
                    {
                        Q.Cast(postition);
                    }
                }
            }

        }

        private void DoHarass()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);

            if (E.IsReady() && GetValue<bool>("themp.harass.e") && CondemnCheck(Player.Position, out target) && ManaManager())
            {
                E.Cast(target);
            }
        }

        private void DoEscape()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
            if (Q.IsReady()
                && target != null
                && GetValue<bool>("themp.escape.q"))
                Q.Cast(Game.CursorPos);

            if (E.IsReady()
                && target.IsValidTarget(E.Range)
                && GetValue<bool>("themp.escape.e"))
                E.Cast(target);
        }

        private void UseSummoners()
        {
            var inNeedOfHealthAlly = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly && x.Distance(Player.Position) < SpellHandler.Heal.Range).FirstOrDefault();
            var inNeedOfManaAlly = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly && x.Distance(Player.Position) < SpellHandler.Clarity.Range).FirstOrDefault();
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
            if (GetValue<bool>("themp.ghost"))
            {
                if (SpellHandler.Flash.SSpellSlot != SpellSlot.Unknown
                    && !SpellHandler.Flash.IsReady()
                    || SpellHandler.Flash.SSpellSlot == SpellSlot.Unknown
                    || SpellHandler.Flash.SSpellSlot == SpellSlot.Unknown
                    && Player.Distance(target.Position) > Player.AttackRange + 425f)
                {
                    if (target.Health < Player.GetAutoAttackDamage(target) * 2 && SpellHandler.Ghost.IsReady())
                        SpellHandler.Ghost.Cast();
                }
            }

            if (GetValue<bool>("themp.barrier"))
            {
                if (target != null
                    && Player.HealthPercent < 20
                    && SpellHandler.Barrier.IsReady())
                    SpellHandler.Barrier.Cast();
            }

            if (GetValue<bool>("themp.clarity"))
            {
                if (Player.ManaPercent < 20
                    && target != null
                    && SpellHandler.Clarity.IsReady())
                    SpellHandler.Clarity.Cast();

                else if (inNeedOfManaAlly.ManaPercent < 20
                    && target != null
                    && SpellHandler.Clarity.IsReady())
                    SpellHandler.Clarity.Cast(inNeedOfManaAlly);
            }

            if (GetValue<bool>("themp.exhaust"))
            {
                if (target.IsValidTarget(SpellHandler.Exhaust.Range)
                    && SpellHandler.Exhaust.IsReady()
                    && lastObjectAttacked == target)
                    SpellHandler.Exhaust.Cast(target);

            }

            if (GetValue<bool>("themp.flash"))
            {
                if (target.Health < Player.GetAutoAttackDamage(target)
                    && target.IsValidTarget(Player.AttackRange + 400f)
                    && !target.IsValidTarget(Player.AttackRange)
                    && SpellHandler.Flash.IsReady())
                    SpellHandler.Flash.Cast(target.Position);
            }

            if (GetValue<bool>("themp.heal"))
            {
                if (Player.HealthPercent < 20
                    && target != null
                    && SpellHandler.Heal.IsReady())
                    SpellHandler.Heal.Cast();

                else if (inNeedOfHealthAlly.HealthPercent < 20
                    && target != null
                    && SpellHandler.Heal.IsReady())
                    SpellHandler.Heal.Cast(inNeedOfHealthAlly);
            }

            if (GetValue<bool>("themp.ignite"))
            {
                if ((target.Health / 4) * 3 < Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)
                    && target.IsValidTarget(SpellHandler.Ignite.Range)
                    && SpellHandler.Ignite.IsReady())
                    SpellHandler.Ignite.Cast(target);
            }
        }

        private void UseItems()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
            if (ItemHandler.Randuin.CanCast(target)
                && Menu.Item("RanduinC").GetValue<bool>())
            {
                if (target.IsFacing(Player)
                    && !Player.IsFacing(target))
                    ItemHandler.Randuin.Instance.Cast();
                else if (!target.IsFacing(Player)
                    && Player.IsFacing(target))
                    ItemHandler.Randuin.Instance.Cast();
            }

            if (ItemHandler.Righteous.CanCast()
                && Menu.Item("RighteousC").GetValue<bool>())
            {
                if (Player.CountAlliesInRange(ItemHandler.Righteous.Range) >= 2
                    && target != null
                    && !target.IsFacing(Player))
                    ItemHandler.Righteous.Instance.Cast();
            }

            if (ItemHandler.Mikael.CanCast()
                && Menu.Item("MikaelC").GetValue<bool>())
            {
                if (Player.HasBuffOfType(BuffType.Charm)
                    || Player.HasBuffOfType(BuffType.Stun)
                    || Player.HasBuffOfType(BuffType.Suppression)
                    || Player.HasBuffOfType(BuffType.Taunt)
                    || Player.HasBuffOfType(BuffType.Fear)
                    || Player.HasBuffOfType(BuffType.Blind)
                    || Player.HasBuffOfType(BuffType.Snare))
                    ItemHandler.Mikael.Instance.Cast();
            }

            if (ItemHandler.QSS.CanCast()
                && Menu.Item("QSSC").GetValue<bool>())
            {
                if (Player.HasBuffOfType(BuffType.Charm)
                    || Player.HasBuffOfType(BuffType.Stun)
                    || Player.HasBuffOfType(BuffType.Suppression)
                    || Player.HasBuffOfType(BuffType.Taunt)
                    || Player.HasBuffOfType(BuffType.Fear)
                    || Player.HasBuffOfType(BuffType.Blind)
                    || Player.HasBuffOfType(BuffType.Snare))
                    ItemHandler.QSS.Instance.Cast();

            }
            
            if (ItemHandler.Scimitar.CanCast()
                 && Menu.Item("ScimitarC").GetValue<bool>())
            {
                if (Player.HasBuffOfType(BuffType.Charm)
                    || Player.HasBuffOfType(BuffType.Stun)
                    || Player.HasBuffOfType(BuffType.Suppression)
                    || Player.HasBuffOfType(BuffType.Taunt)
                    || Player.HasBuffOfType(BuffType.Fear)
                    || Player.HasBuffOfType(BuffType.Blind)
                    || Player.HasBuffOfType(BuffType.Snare))
                    ItemHandler.Scimitar.Instance.Cast();

            }
        }

        public override void Combo(Menu config)
        {
            config.AddItem(new MenuItem("themp.combo.q", "Use Tumble (Q)").SetValue(true));
            config.AddItem(new MenuItem("themp.combo.e", "Use Condemn (E)").SetValue(true));
            config.AddItem(new MenuItem("themp.combo.r", "Use Final Hour (R)").SetValue(true));
            config.AddItem(new MenuItem("themp.combo.minenemiesr", "Min. enemies in range to cast Ultimate").SetValue(new Slider(2, 1, 5)));
        }

        public override void Harass(Menu config)
        {
            config.AddItem(new MenuItem("themp.harass.q", "Use Tumble (Q)").SetValue(true));
            config.AddItem(new MenuItem("themp.harass.e", "Use Condemn (E)").SetValue(false));
        }

        public override void Laneclear(Menu config)
        {
            config.AddItem(new MenuItem("themp.laneclear.q", "Use Tumble (Q)").SetValue(true));
        }

        public override void Misc(Menu config)
        {
            config.AddItem(new MenuItem("themp.misc.interrupte", "Use E to interrupt").SetValue(true));
            config.AddItem(new MenuItem("themp.misc.gapclosere", "Anti Gap Closer with E").SetValue(true));
        }

        public override void Escape(Menu menu)
        {
            menu.AddItem(new MenuItem("themp.escape.q", "Use Tumble (Q)").SetValue(true));
            menu.AddItem(new MenuItem("themp.escape.e", "Use Condemn (E)").SetValue(true));
        }

        public override void ItemMenu(Menu menu)
        {
            menu.AddItem(new MenuItem("YoumuuC", "Use Youmuu's Ghostblade").SetValue(true));
            menu.AddItem(new MenuItem("BotrkC", "Use Blade of the Ruined King").SetValue(true));
            menu.AddItem(new MenuItem("RanduinC", "Use Randuin's Omen").SetValue(true));
            menu.AddItem(new MenuItem("RighteousC", "Use Righteous Glory").SetValue(true));
            menu.AddItem(new MenuItem("MikaelC", "Use Mikael's Crucible").SetValue(true));
            menu.AddItem(new MenuItem("QSSC", "Use Quicksilver Sash").SetValue(true));
            menu.AddItem(new MenuItem("ScimitarC", "Use Mercurial Scimitar").SetValue(true));
        }

        public override void Extra(Menu config)
        {
            var MiscManaSubMenu = new Menu("Misc - Mana Manager", "themp.manamanager");
            {
                MiscManaSubMenu.AddItem(new MenuItem("themp.manamanager.percentage", "% safe for Combo").SetValue(new Slider(50, 0, 100)));
            }
            config.AddSubMenu(MiscManaSubMenu);

            var MiscKeybindsSubMenu = new Menu("Misc - Keybinds", "themp.kb");
            {
                MiscKeybindsSubMenu.AddItem(new MenuItem("themp.kb.combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
                MiscKeybindsSubMenu.AddItem(new MenuItem("themp.kb.harass", "Harass").SetValue(new KeyBind('C', KeyBindType.Press)));
                MiscKeybindsSubMenu.AddItem(new MenuItem("themp.kb.laneclear", "LaneClear").SetValue(new KeyBind('V', KeyBindType.Press)));
                MiscKeybindsSubMenu.AddItem(new MenuItem("themp.kb.escape", "Escape").SetValue(new KeyBind('G', KeyBindType.Press)));
            }
            config.AddSubMenu(MiscKeybindsSubMenu);

            var MiscCSubMenu = new Menu("Misc - Condemn", "MiscC");
            {
                MiscCSubMenu.AddItem(new MenuItem("PushDistance", "E Push Dist").SetValue(new Slider(425, 400, 500)));
                MiscCSubMenu.AddItem(new MenuItem("CondemnTurret", "Try to Condemn to turret").SetValue(false));
                MiscCSubMenu.AddItem(new MenuItem("NoEEnT", "No E Under enemy turret").SetValue(true));
            }

            config.AddSubMenu(MiscCSubMenu);

            var MiscNoCondemn = new Menu("Don't Condemn", "NoCondemn");
            {
                foreach (var hero in HeroManager.Enemies)
                {
                    MiscNoCondemn.AddItem(new MenuItem("nC" + hero.ChampionName, hero.ChampionName).SetValue(false));
                }
            }

            config.AddSubMenu(MiscNoCondemn);
        }

        public override void Drawings(Menu config)
        {
            config.AddItem(new MenuItem("themp.drawings.q", "Draw Tumble (Q)").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            config.AddItem(new MenuItem("themp.drawings.e", "Draw Condemn (E)").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            config.AddItem(new MenuItem("themp.drawings.target", "Draw Target").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            config.AddItem(new MenuItem("themp.drawings.condemn", "Show Best Condemn position").SetValue(true));

        }
    }
}
