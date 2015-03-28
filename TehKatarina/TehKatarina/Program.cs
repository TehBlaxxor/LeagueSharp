using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace TehKatarina
{
    class Program
    {
        // escape gapcloser with e - done
        // wardjump
        // give credit for the damage indicator
        // add stuff with qinair
        public const string Katarina = "Katarina";
        public static bool Ractive;
        public static bool QinAir;
        public static List<Spell> Spells = new List<Spell>();
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static SpellSlot IgniteSlot;
        public static Menu Config;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static int LastPlaced;
        public static Vector3 LastWardPos;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static float GetComboDamage(Obj_AI_Hero enemy)
        {
            double damage = 0d;
            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q) + Player.GetSpellDamage(enemy, SpellSlot.Q, 1);
            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);
            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);
            if (R.IsReady() || (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).State == SpellState.Surpressed && R.Level > 0))
                damage += Player.GetSpellDamage(enemy, SpellSlot.R) * 8;
            if (IgniteSlot.IsReady())
                damage += IgniteHandler.GetIgniteDamage(enemy);
            return (float)damage;
        }

        private static InventorySlot GetBestWardSlot()
        {
            InventorySlot slot = Items.GetWardSlot();
            if (slot == default(InventorySlot)) return null;
            return slot;
        }

        public static Obj_AI_Base GetFarthestMinion(Vector3 playerpos, Vector3 enemypos)
        {
            return ObjectManager.Get<Obj_AI_Base>().Where(x => x.Distance(playerpos) < x.Distance(enemypos) && !x.IsInvulnerable && x.IsValidTarget(E.Range)).OrderBy(m => m.Distance(playerpos)).LastOrDefault();

        }

        public static Obj_AI_Base GetClosestMinion(Vector3 playerpos, Vector3 enemypos)
        {
            return ObjectManager.Get<Obj_AI_Base>().Where(x => x.Distance(playerpos) > x.Distance(enemypos) && !x.IsInvulnerable && x.IsValidTarget(E.Range)).OrderBy(m => m.Distance(enemypos)).FirstOrDefault();

        }

        private static readonly Random RandomPos = new Random(DateTime.Now.Millisecond);
        public static void MoveTo(Vector3 pos)
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Player.ServerPosition.Extend(pos, (RandomPos.NextFloat(0.6f, 1) + 0.2f) * 300));
        }

        public static void Flee()
        {
            MoveTo(Game.CursorPos);
        }

        private static void PlayAnimation(GameObject sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Animation.ToLowerInvariant() == "spell4")
                {
                    Ractive = true;
                }
                else if (args.Animation.ToLowerInvariant() == "run" || args.Animation.ToLowerInvariant() == "idle1" || args.Animation.ToLowerInvariant() == "attack1" || args.Animation.ToLowerInvariant() == "attack2")
                {
                    Ractive = false;
                }
            }
        }

        private static void OnCreateObj(GameObject sender, EventArgs args)
        {
            //"Katarina_Base_daggered.troy"
            //"katarina_bouncingBlades_tar.troy"
            if (sender.Name.ToLowerInvariant().Contains("katarina_bouncingblades_tar.troy") && sender.IsMe)
            {
                QinAir = true;
            }
            else if (sender.Name.ToLowerInvariant().Contains("katarina_base_daggered.troy") && sender.IsMe)
            {
                QinAir = false;
            }

            if (Environment.TickCount < LastPlaced + 300)
            {
                var ward = (Obj_AI_Minion)sender;
                if (ward.Name.ToLower().Contains("ward") && ward.Distance(LastWardPos) < 500 && E.IsReady() && Config.Item("TK/escape/run").GetValue<KeyBind>().Active && Config.Item("TK/escape/e").GetValue<bool>())
                {
                    E.Cast(ward);
                }
            }

                
        }

        private static readonly Menu OrbwalkerMenu = new Menu("Orbwalker", "Orbwalker");

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != Katarina)
            { return; }

            Game.PrintChat("TehKatarina Loaded!");

            Q = new Spell(SpellSlot.Q, 675);
            W = new Spell(SpellSlot.W, 375);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 550);

            IgniteHandler.Run();

            Spells.Add(Q);
            Spells.Add(W);
            Spells.Add(E);
            Spells.Add(R);

            Config = new Menu("TehKatarina", "TehKatarina", true);

            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            //Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            Config.AddSubMenu(OrbwalkerMenu);
            xSLxOrbwalker.AddToMenu(OrbwalkerMenu);

            var targetselectormenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetselectormenu);
            Config.AddSubMenu(targetselectormenu);

            //done
            Config.AddSubMenu(new Menu("TehKatarina - Combo", "TK/combo"));
            Config.SubMenu("TK/combo").AddItem(new MenuItem("TK/combo/q", "Use Bouncing Blades (Q)").SetValue(true));
            Config.SubMenu("TK/combo").AddItem(new MenuItem("TK/combo/w", "Use Sinister Steel (W)").SetValue(true));
            Config.SubMenu("TK/combo").AddItem(new MenuItem("TK/combo/e", "Use Shunpo (E)").SetValue(true));
            Config.SubMenu("TK/combo").AddItem(new MenuItem("TK/combo/r", "Use Death Lotus (R)").SetValue(true));
            Config.SubMenu("TK/combo").AddItem(new MenuItem("TK/combo/spacer", " "));
            Config.SubMenu("TK/combo").AddItem(new MenuItem("TK/combo/w/mode", "Sinister Steel (W) Mode").SetValue(new StringList(new[] { "Normal", "Enemy Marked" })));
            Config.SubMenu("TK/combo").AddItem(new MenuItem("TK/combo/e/mode", "Shunpo (E) Mode").SetValue(new StringList(new[] { "Normal", "Safe Shunpo" }, 1)));
            Config.SubMenu("TK/combo").AddItem(new MenuItem("TK/combo/r/mode", "Min Enemies for Death Lotus (R)").SetValue(new Slider(1,1,5)));

            //done
            Config.AddSubMenu(new Menu("TehKatarina - Harass", "TK/harass"));
            Config.SubMenu("TK/harass").AddItem(new MenuItem("TK/harass/q", "Use Bouncing Blades (Q)").SetValue(true));
            Config.SubMenu("TK/harass").AddItem(new MenuItem("TK/harass/w", "Use Sinister Steel (W)").SetValue(true));
            Config.SubMenu("TK/harass").AddItem(new MenuItem("TK/harass/spacer", " "));
            Config.SubMenu("TK/harass").AddItem(new MenuItem("TK/harass/system", "Harass Active").SetValue(new KeyBind('I', KeyBindType.Toggle)));

            //done
            Config.AddSubMenu(new Menu("TehKatarina - Kill Steal", "TK/ks"));
            Config.SubMenu("TK/ks").AddItem(new MenuItem("TK/ks/q", "Use Bouncing Blades (Q)").SetValue(true));
            Config.SubMenu("TK/ks").AddItem(new MenuItem("TK/ks/w", "Use Sinister Steel (W)").SetValue(true));
            Config.SubMenu("TK/ks").AddItem(new MenuItem("TK/ks/e", "Use Shunpo (E)").SetValue(true));
            Config.SubMenu("TK/ks").AddItem(new MenuItem("TK/ks/r", "Use Death Lotus (R)").SetValue(true));
            Config.SubMenu("TK/ks").AddItem(new MenuItem("TK/ks/spacer", " "));
            Config.SubMenu("TK/ks").AddItem(new MenuItem("TK/ks/system", "Use Smart KS System").SetValue(true));
            Config.SubMenu("TK/ks").AddItem(new MenuItem("TK/ks/e/mode", "Shunpo (E) Mode").SetValue(new StringList(new[] { "Normal", "Safe Shunpo" }, 1)));
            Config.SubMenu("TK/ks").AddItem(new MenuItem("TK/ks/r/mode", "Death Lotus (R) Mode").SetValue(new StringList(new[] { "Full HP", "Half HP" }, 1)));

            //done
            if (IgniteSlot != SpellSlot.Unknown)
            {
                Config.AddSubMenu(new Menu("TehKatarina - Summoners", "TK/summoners"));
                Config.SubMenu("TK/summoners").AddItem(new MenuItem("TK/summoners/ignite", "Use Smart Ignite").SetValue(true));
            }

            //done
            Config.AddSubMenu(new Menu("TehKatarina - Last Hit", "TK/lasthit"));
            Config.SubMenu("TK/lasthit").AddItem(new MenuItem("TK/lasthit/q", "Use Bouncing Blades (Q)").SetValue(true));
            Config.SubMenu("TK/lasthit").AddItem(new MenuItem("TK/lasthit/w", "Use Sinister Steel (W)").SetValue(true));
            Config.SubMenu("TK/lasthit").AddItem(new MenuItem("TK/lasthit/spacer", " "));
            Config.SubMenu("TK/lasthit").AddItem(new MenuItem("TK/lasthit/w/#", "Sinister Steel (W) Min Minions").SetValue(new Slider(1, 1, 5)));

            //done
            Config.AddSubMenu(new Menu("TehKatarina - Drawings", "TK/drawings"));
            Config.SubMenu("TK/drawings").AddItem(new MenuItem("TK/drawings/toggle", "Drawings").SetValue(true));
            Config.SubMenu("TK/drawings").AddItem(new MenuItem("TK/drawings/q", "Draw Bouncing Blades (Q)").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("TK/drawings").AddItem(new MenuItem("TK/drawings/w", "Draw Sinister Steel (W)").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("TK/drawings").AddItem(new MenuItem("TK/drawings/e", "Draw Shunpo (E)").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("TK/drawings").AddItem(new MenuItem("TK/drawings/r", "Draw Death Lotus (R)").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("TK/drawings").AddItem(new MenuItem("TK/drawings/target", "Draw Target").SetValue(new Circle(true, System.Drawing.Color.FromArgb(255, 0, 255, 255))));
            MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
            MenuItem drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true, System.Drawing.Color.FromArgb(90, 255, 169, 4)));
            Config.SubMenu("TK/drawings").AddItem(drawComboDamageMenu);
            Config.SubMenu("TK/drawings").AddItem(drawFill);
            DamageIndicator.DamageToUnit = GetComboDamage;
            DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
            DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
            DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
            drawComboDamageMenu.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };
            drawFill.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };

            //done
            Config.AddSubMenu(new Menu("TehKatarina - Escape", "TK/escape"));
            Config.SubMenu("TK/escape").AddItem(new MenuItem("TK/escape/run", "Escape Active").SetValue(new KeyBind('G', KeyBindType.Press)));
            Config.SubMenu("TK/escape").AddItem(new MenuItem("TK/escape/e", "Use Shunpo (E)").SetValue(true));
            Config.SubMenu("TK/escape").AddItem(new MenuItem("TK/escape/ward", "Use Ward").SetValue(true));
            Config.SubMenu("TK/escape").AddItem(new MenuItem("TK/escape/e/antigapcloser", " Shunpo (E) Anti Gapcloser").SetValue(true));

            Config.AddSubMenu(new Menu("TehKatarina - Misc", "TK/misc"));
            Config.SubMenu("TK/misc").AddItem(new MenuItem("TK/misc/combo/mode", "Combo Mode").SetValue(new StringList(new[] { "Q E W", "E Q W" })));
            Config.SubMenu("TK/misc").AddItem(new MenuItem("TK/misc/e/humanizer", "E Humanizer").SetValue(new Slider(0, 0, 1000)));
            Config.SubMenu("TK/misc").AddItem(new MenuItem("TK/misc/e/mode/#", "Shunpo (E) Max Enemies").SetValue(new Slider(2, 1, 5)));

            Config.AddSubMenu(new Menu("Assembly Info", "TK/info"));
            Config.SubMenu("TK/info").AddItem(new MenuItem("TK/info/author", "Author: TehBlaxxor"));
            Config.SubMenu("TK/info").AddItem(new MenuItem("TK/info/edition", "Edition: BETA"));
            Config.SubMenu("TK/info").AddItem(new MenuItem("TK/info/version", "5.6.1.1"));

            Config.AddSubMenu(new Menu("Keybinds", "TK/keybinds"));
            Config.SubMenu("TK/info").AddItem(new MenuItem("keybind.combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            Config.SubMenu("TK/info").AddItem(new MenuItem("keybind.lasthit", "Lasthit").SetValue(new KeyBind('X', KeyBindType.Press)));
            Config.SubMenu("TK/info").AddItem(new MenuItem("keybind.harass", "Harass").SetValue(new KeyBind('C',KeyBindType.Press)));


            //Config.SubMenu("TK/combo").AddItem(new MenuItem("TK/combo/", "").SetValue(true));

            Config.AddToMainMenu();

            GameObject.OnCreate += OnCreateObj;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if ((Player.IsChannelingImportantSpell() || Player.HasBuff("katarinarsound", true)))
            {
                Game.PrintChat("Enemy attempted gapcloser during ultimate. Denying gapcloser!");
                return;
            }

            if (gapcloser.Sender.IsEnemy && E.IsReady() && Config.Item("TK/escape/e/antigapcloser").GetValue<bool>())
            {
                if (GetFarthestMinion(Player.Position, gapcloser.Sender.Position).IsValidTarget(E.Range))
                {
                    E.Cast(GetFarthestMinion(Player.Position, gapcloser.Sender.Position));
                }
            }
        }

        static void Game_OnGameUpdate(EventArgs args)
        {

            if (Config.Item("keybind.combo").GetValue<KeyBind>().Active)
                Combo();

            if (Config.Item("keybind.lasthit").GetValue<KeyBind>().Active)
                LastHit();

            if (Config.Item("keybind.harass").GetValue<KeyBind>().Active)
                Harass();

            if (Config.Item("TK/escape/run").GetValue<KeyBind>().Active)
            {
                Flee();
                Escape();
            }

            if (Config.Item("TK/harass/system").GetValue<KeyBind>().Active)
            { Harass(); }

            if (Config.Item("TK/ks/system").GetValue<bool>())
            {
                KillSteal();
            }

            /*if (Ractive)
            {
                Orbwalker.SetAttack(false);
                Orbwalker.SetMovement(false);
            }
            else if (!Ractive)
            {
                Orbwalker.SetAttack(true);
                Orbwalker.SetMovement(true);
            }*/

        }
        /*if (Environment.TickCount <= LastPlaced + 3000 || !E.IsReady()) return;

Config.AddSubMenu(new Menu("TehKatarina - Escape", "TK/escape"));
Config.SubMenu("TK/escape").AddItem(new MenuItem("TK/escape/run", "Escape Active").SetValue(new KeyBind('G', KeyBindType.Press)));
Config.SubMenu("TK/escape").AddItem(new MenuItem("TK/escape/e", "Use Shunpo (E)").SetValue(true));
Config.SubMenu("TK/escape").AddItem(new MenuItem("TK/escape/e/antigapcloser", " Shunpo (E) Anti Gapcloser").SetValue(true));

*/
        private static void Escape()
        {
            if (!Config.Item("TK/escape/e").GetValue<bool>()) return;
            var minions = MinionManager.GetMinions(Game.CursorPos, 200f, MinionTypes.All, MinionTeam.All);
            if (minions.Count() >= 1 && E.IsReady() && minions[0].Distance(Player.Position) <= E.Range)
            {
                
                E.Cast(minions[0]);
            }
            else if (minions.Count() < 1)
            {
                if (Config.Item("TK/escape/ward").GetValue<bool>() && E.IsReady())
                {

            if (Environment.TickCount <= LastPlaced + 3000 || !E.IsReady()) return;
            Vector3 cursorPos = Game.CursorPos;
            Vector3 myPos = Player.ServerPosition;
            Vector3 delta = cursorPos - myPos;
            delta.Normalize();
            Vector3 wardPosition = myPos + delta * (600 - 5);
            InventorySlot invSlot = GetBestWardSlot();
            if (invSlot == null) return;
            Items.UseItem((int)invSlot.Id, wardPosition);
            LastWardPos = wardPosition;
            LastPlaced = Environment.TickCount;

            E.Cast();
                }
            }
        }

        private static void KillSteal()
        {
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(p => p.IsEnemy && !p.IsDead && p.IsValidTarget(E.Range + Q.Range) && !p.IsInvulnerable))
            {

                if (enemy.Health + enemy.HPRegenRate < Q.GetDamage(enemy) && Q.IsReady() && Config.Item("TK/ks/q").GetValue<bool>())
                {
                    if (enemy.IsValidTarget(Q.Range))
                    {
                        Q.Cast(enemy);
                    }
                }

                else if ((Q.GetDamage(enemy)) > enemy.Health + enemy.HPRegenRate && Config.Item("TK/ks/q").GetValue<bool>() && !enemy.IsValidTarget(Q.Range) && enemy.IsValidTarget(Q.Range + Player.Distance(GetFarthestMinion(Player.Position, enemy.Position).Position)) && Q.IsReady() && E.IsReady())
                    {
                        if (Config.Item("TK/ks/e/mode").GetValue<StringList>().SelectedIndex == 0)
                        {
                            if (Config.Item("TK/ks/e").GetValue<bool>())
                            {
                                E.Cast(GetClosestMinion(Player.Position, enemy.Position)); //E.Cast(qtarget);
                            }
                        }
                        else if (Config.Item("TK/ks/e/mode").GetValue<StringList>().SelectedIndex == 1)
                        {
                            if (Config.Item("TK/ks/e").GetValue<bool>() && GetClosestMinion(Player.Position, enemy.Position).CountEnemiesInRange(1000f) <= Config.Item("TK/misc/e/mode/#").GetValue<Slider>().Value)
                            {
                                E.Cast(GetClosestMinion(Player.Position, enemy.Position));
                            }
                        }
                    }

                else if (enemy.Health + enemy.HPRegenRate < W.GetDamage(enemy) && W.IsReady() && Config.Item("TK/ks/w").GetValue<bool>())
                {
                    if (enemy.IsValidTarget(W.Range))
                    {
                        W.Cast();
                    }
                }

                else if ((W.GetDamage(enemy)) > enemy.Health + enemy.HPRegenRate && !enemy.IsValidTarget(W.Range) && enemy.IsValidTarget(W.Range + Player.Distance(GetFarthestMinion(Player.Position, enemy.Position).Position)) && W.IsReady() && E.IsReady() && Config.Item("TK/ks/w").GetValue<bool>())
                {
                    if (Config.Item("TK/ks/e/mode").GetValue<StringList>().SelectedIndex == 0)
                    {
                        if (Config.Item("TK/ks/e").GetValue<bool>())
                        {
                            E.Cast(GetClosestMinion(Player.Position, enemy.Position)); //E.Cast(qtarget);
                        }
                    }
                    else if (Config.Item("TK/ks/e/mode").GetValue<StringList>().SelectedIndex == 1)
                    {
                        if (Config.Item("TK/ks/e").GetValue<bool>() && GetClosestMinion(Player.Position, enemy.Position).CountEnemiesInRange(1000f) <= Config.Item("TK/misc/e/mode/#").GetValue<Slider>().Value)
                        {
                            E.Cast(GetClosestMinion(Player.Position, enemy.Position));
                        }
                    }
                }

                else if (Config.Item("TK/ks/r/mode").GetValue<StringList>().SelectedIndex == 1 && enemy.Health + enemy.HPRegenRate < (R.GetDamage(enemy) / 2) && R.IsReady() && Config.Item("TK/ks/r").GetValue<bool>())
                {
                    if (enemy.IsValidTarget(R.Range - 50))
                    {
                        Utility.DelayAction.Add(100, () => R.Cast());
                    }
                }

                else if (Config.Item("TK/ks/r/mode").GetValue<StringList>().SelectedIndex == 0 && enemy.Health + enemy.HPRegenRate < (R.GetDamage(enemy)) && R.IsReady() && Config.Item("TK/ks/r").GetValue<bool>())
                {
                    if (enemy.IsValidTarget(R.Range - 50))
                    {
                        Utility.DelayAction.Add(100, () => R.Cast());
                    }
                }


                else if (Config.Item("TK/ks/r/mode").GetValue<StringList>().SelectedIndex == 1 && (R.GetDamage(enemy) / 2) > enemy.Health + enemy.HPRegenRate && !enemy.IsValidTarget(R.Range - 50) && enemy.IsValidTarget((R.Range - 50) + Player.Distance(GetFarthestMinion(Player.Position, enemy.Position).Position)) && R.IsReady() && E.IsReady() && Config.Item("TK/ks/r").GetValue<bool>())
                {
                    if (Config.Item("TK/ks/e/mode").GetValue<StringList>().SelectedIndex == 0)
                    {
                        if (Config.Item("TK/ks/e").GetValue<bool>())
                        {
                            E.Cast(GetClosestMinion(Player.Position, enemy.Position)); //E.Cast(qtarget);
                        }
                    }
                else if (Config.Item("TK/ks/e/mode").GetValue<StringList>().SelectedIndex == 1)
                    {
                        if (Config.Item("TK/ks/e").GetValue<bool>() && GetClosestMinion(Player.Position, enemy.Position).CountEnemiesInRange(1000f) <= Config.Item("TK/misc/e/mode/#").GetValue<Slider>().Value)
                        {
                            E.Cast(GetClosestMinion(Player.Position, enemy.Position));
                        }
                    }
                }

                else if (Config.Item("TK/ks/r/mode").GetValue<StringList>().SelectedIndex == 0 && (R.GetDamage(enemy)) > enemy.Health + enemy.HPRegenRate && !enemy.IsValidTarget(R.Range - 50) && enemy.IsValidTarget((R.Range - 50) + Player.Distance(GetFarthestMinion(Player.Position, enemy.Position).Position)) && R.IsReady() && E.IsReady() && Config.Item("TK/ks/r").GetValue<bool>())
                {
                    if (Config.Item("TK/ks/e/mode").GetValue<StringList>().SelectedIndex == 0)
                    {
                        if (Config.Item("TK/ks/e").GetValue<bool>())
                        {
                            E.Cast(GetClosestMinion(Player.Position, enemy.Position)); //E.Cast(qtarget);
                        }
                    }
                    else if (Config.Item("TK/ks/e/mode").GetValue<StringList>().SelectedIndex == 1)
                    {
                        if (Config.Item("TK/ks/e").GetValue<bool>() && GetClosestMinion(Player.Position, enemy.Position).CountEnemiesInRange(1000f) <= Config.Item("TK/misc/e/mode/#").GetValue<Slider>().Value)
                        {
                            E.Cast(GetClosestMinion(Player.Position, enemy.Position));
                        }
                    }
                }
            
                else if (Config.Item("TK/summoners/ignite").GetValue<bool>() && enemy.Health + enemy.HPRegenRate < IgniteHandler.GetIgniteDamage(enemy))
                {
                    if (enemy.IsValidTarget(IgniteHandler.IgniteRange))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(IgniteSlot, enemy);
                    }
                }

                else if (!enemy.IsValidTarget(IgniteHandler.IgniteRange) && enemy.IsValidTarget(IgniteHandler.IgniteRange + Player.Distance(GetFarthestMinion(Player.Position, enemy.Position).Position)) && IgniteHandler.GetIgniteDamage(enemy) > enemy.Health + enemy.HPRegenRate && E.IsReady() && Config.Item("TK/summoners/ignite").GetValue<bool>())
                {
                    if (Config.Item("TK/ks/e/mode").GetValue<StringList>().SelectedIndex == 0)
                    {
                        if (Config.Item("TK/ks/e").GetValue<bool>())
                        {
                            E.Cast(GetClosestMinion(Player.Position, enemy.Position)); //E.Cast(qtarget);
                        }
                    }
                    else if (Config.Item("TK/ks/e/mode").GetValue<StringList>().SelectedIndex == 1)
                    {
                        if (Config.Item("TK/ks/e").GetValue<bool>() && GetClosestMinion(Player.Position, enemy.Position).CountEnemiesInRange(1000f) <= Config.Item("TK/misc/e/mode/#").GetValue<Slider>().Value)
                        {
                            E.Cast(GetClosestMinion(Player.Position, enemy.Position));
                        }
                    }
                }

            }
        }
            


        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target.IsValidTarget(Q.Range) && Q.IsReady() && Config.Item("TK/harass/q").GetValue<bool>())
            {
                Q.Cast(target);
            }
            if (target.IsValidTarget(W.Range) && W.IsReady() && Config.Item("TK/harass/w").GetValue<bool>())
            {
                W.Cast();
            }

        }


        private static void LastHit()
        {
            var qminions = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).Where(p => p.Health < Q.GetDamage(p) && !p.IsInvulnerable && p.IsValidTarget(Q.Range));
            var wminions = MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).Where(p => p.Health < W.GetDamage(p) && !p.IsInvulnerable && p.IsValidTarget(W.Range));

            if (qminions != null)
            {
                if (Config.Item("TK/lasthit/q").GetValue<bool>() && Q.IsReady())
                {
                    Q.Cast(qminions.FirstOrDefault());
                }
            }
            if (wminions != null)
                if (Config.Item("TK/lasthit/w").GetValue<bool>() && W.IsReady() && wminions.Count() >= Config.Item("TK/lasthit/w/#").GetValue<Slider>().Value)
                {
                    W.Cast();
                }
        }

        private static void Combo()
        {
            var etarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var qtarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            int selectedindex = Config.Item("TK/misc/combo/mode").GetValue<StringList>().SelectedIndex + 1;

            if ((Player.IsChannelingImportantSpell() || Player.HasBuff("katarinarsound", true)))
            {
                if (Player.CountEnemiesInRange(R.Range) < 1)
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, etarget.ServerPosition);
                }
                else return;
            }

            if (selectedindex == 1)
            {
                if (Q.IsReady() && qtarget.IsValidTarget(Q.Range) && Config.Item("TK/combo/q").GetValue<bool>() == true)
                {
                    Q.Cast(qtarget);
                }

                if (Config.Item("TK/combo/e/mode").GetValue<StringList>().SelectedIndex == 0)
                {
                    if (E.IsReady() && qtarget.IsValidTarget(Q.Range) && Config.Item("TK/combo/e").GetValue<bool>() == true)
                    {
                        Utility.DelayAction.Add(Config.Item("TK/misc/e/humanizer").GetValue<Slider>().Value , () => E.Cast(qtarget)); //E.Cast(qtarget);
                    }
                }
                else if (Config.Item("TK/combo/e/mode").GetValue<StringList>().SelectedIndex == 1)
                {
                    if (qtarget.CountEnemiesInRange(1000f) <= Config.Item("TK/misc/e/mode/#").GetValue<Slider>().Value && E.IsReady() && qtarget.IsValidTarget(Q.Range) && Config.Item("TK/combo/e").GetValue<bool>() == true)
                    {
                        Utility.DelayAction.Add(Config.Item("TK/misc/e/humanizer").GetValue<Slider>().Value, () => E.Cast(qtarget)); //E.Cast(qtarget);
                    }
                }

                if (Config.Item("TK/combo/w/mode").GetValue<StringList>().SelectedIndex == 0)
                {
                    if (W.IsReady() && qtarget.IsValidTarget(W.Range) && Config.Item("TK/combo/w").GetValue<bool>())
                    {
                        W.Cast();
                    }
                }
                else if (Config.Item("TK/combo/w/mode").GetValue<StringList>().SelectedIndex == 1)
                {
                    if (W.IsReady() && qtarget.IsValidTarget(W.Range) && Config.Item("TK/combo/w").GetValue<bool>() && qtarget.HasBuff("katarinaqmark"))
                    {
                        W.Cast();
                    }
                    else if (W.IsReady() && qtarget.IsValidTarget(W.Range) && Config.Item("TK/combo/w").GetValue<bool>() && !qtarget.HasBuff("katarinaqmark") && !QinAir && !Q.IsReady())
                    {
                        W.Cast();
                        
                    }
                }

                if (!Q.IsReady() && !W.IsReady() && !E.IsReady() && R.IsReady() && Config.Item("TK/combo/r/mode").GetValue<Slider>().Value >= Player.CountEnemiesInRange(R.Range) && Config.Item("TK/combo/r").GetValue<bool>())
                {
                    Utility.DelayAction.Add(100, () => R.Cast());
                }
                //katarinaqmark



            }

            else if (selectedindex == 2)
            {

                if (Config.Item("TK/combo/e/mode").GetValue<StringList>().SelectedIndex == 0)
                {
                    if (E.IsReady() && etarget.IsValidTarget(E.Range) && Config.Item("TK/combo/e").GetValue<bool>() == true)
                    {
                        Utility.DelayAction.Add(Config.Item("TK/misc/e/humanizer").GetValue<Slider>().Value, () => E.Cast(etarget)); //E.Cast(etarget);
                    }
                }
                else if (Config.Item("TK/combo/e/mode").GetValue<StringList>().SelectedIndex == 1)
                {
                    if (etarget.CountEnemiesInRange(1000f) <= Config.Item("TK/misc/e/mode/#").GetValue<Slider>().Value && E.IsReady() && etarget.IsValidTarget(E.Range) && Config.Item("TK/combo/e").GetValue<bool>() == true)
                    {
                        Utility.DelayAction.Add(Config.Item("TK/misc/e/humanizer").GetValue<Slider>().Value, () => E.Cast(etarget)); //E.Cast(etarget);
                    }
                }

                if (Q.IsReady() && etarget.IsValidTarget(Q.Range) && Config.Item("TK/combo/q").GetValue<bool>() == true)
                {
                    Q.Cast(etarget);
                }



                if (Config.Item("TK/combo/w/mode").GetValue<StringList>().SelectedIndex == 0)
                {
                    if (W.IsReady() && etarget.IsValidTarget(W.Range) && Config.Item("TK/combo/w").GetValue<bool>())
                    {
                        W.Cast();
                    }
                }
                else if (Config.Item("TK/combo/w/mode").GetValue<StringList>().SelectedIndex == 1)
                {
                    if (W.IsReady() && etarget.IsValidTarget(W.Range) && Config.Item("TK/combo/w").GetValue<bool>() && qtarget.HasBuff("katarinaqmark"))
                    {
                        W.Cast();
                    }
                }

                if (!Q.IsReady() && !W.IsReady() && !E.IsReady() && R.IsReady() && Config.Item("TK/combo/r/mode").GetValue<Slider>().Value >= Player.CountEnemiesInRange(R.Range) && Config.Item("TK/combo/r").GetValue<bool>())
                {
                    Utility.DelayAction.Add(100, () => R.Cast());
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {

            if (!Config.Item("TK/drawings/toggle").GetValue<bool>())
            {
                return;
            }

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (Config.Item("TK/drawings/target").GetValue<Circle>().Active && target != null)
            {
                Render.Circle.DrawCircle(target.Position, 75f, Config.Item("TK/drawings/target").GetValue<Circle>().Color);
            }

            foreach (var x in Spells.Where(y => Config.Item("TK/drawings/" + y.Slot.ToString().ToLowerInvariant()).GetValue<Circle>().Active))
            {
                Render.Circle.DrawCircle(Player.Position, x.Range, x.IsReady()
                ? System.Drawing.Color.Green
                : System.Drawing.Color.Red
                ); // sad face
            }

        }
    }
}
