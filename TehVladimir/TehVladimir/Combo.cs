using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Template
{
    class Combo
    {
        /*
root.SubMenu("Combo").AddItem(new MenuItem("comboQ", "Use Transfusion (Q)")).SetValue(true);
root.SubMenu("Combo").AddItem(new MenuItem("comboW", "Use Sanguine Pool (W)")).SetValue(true);
root.SubMenu("Combo").AddItem(new MenuItem("comboE", "Use Tides of Blood (E)")).SetValue(true);
root.SubMenu("Combo").AddItem(new MenuItem("comboR", "Use Hemoplague (R)")).SetValue(true);
root.SubMenu("Combo").AddItem(new MenuItem("modW", "Use W").SetValue(new StringList(new[] {"If Enemy Killable", "If Enemy In Range"})));
root.SubMenu("Combo").AddItem(new MenuItem("modE", "Min. Enemies for E")).SetValue(new Slider(1, 1, 5));
root.SubMenu("Combo").AddItem(new MenuItem("modR", "Use R").SetValue(new StringList(new[] {"If Enemy Killable", "If Enemy In Range"})));
        */
        public static void Run()
        {
            var target = TargetSelector.GetTarget(Settings.TARGETSELECTOR_RANGE, Settings.DAMAGE_TYPE);
            float modW = Menu.GetSelectedStringListIndex("modW") + 1;
            float modR = Menu.GetSelectedStringListIndex("modR") + 1;
            float modE = Menu.GetSlider("modE");
            bool Qbool = Menu.GetBool("comboQ");
            bool Wbool = Menu.GetBool("comboW");
            bool Ebool = Menu.GetBool("comboE");
            bool Rbool = Menu.GetBool("comboR");

            if (Spells.R.IsReady() && target.IsValidTarget(Spells.R.Range) && Rbool)
            {
                if (modR == 1)
                {
                    if (Spells.R.GetDamage(target) > target.Health + target.HPRegenRate)
                    {
                        Spells.R.Cast(target);
                    }
                }
                else if (modR == 2)
                {
                    Spells.R.Cast(target);
                }

            }
            if (Spells.Q.IsReady() && target.IsValidTarget(Spells.Q.Range) && Qbool)
            {
                Spells.Q.Cast(target);
            }
            if (Spells.W.IsReady() && target.IsValidTarget(Spells.W.Range) && Wbool)
            {
                if (modW == 1)
                {
                    if (Spells.W.GetDamage(target) > target.Health + target.HPRegenRate)
                    {
                        Spells.W.Cast();
                    }
                }
                else if (modW == 2)
                {
                    Spells.W.Cast();
                }
            }
            if (Spells.E.IsReady() && target.IsValidTarget(Spells.E.Range) && Ebool)
            {
                if (ObjectManager.Player.CountEnemiesInRange(Spells.E.Range) >= modE)
                {
                    Spells.E.Cast();
                }
            }

        }
    }
}
