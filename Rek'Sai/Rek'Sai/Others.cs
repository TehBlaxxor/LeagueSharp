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
    public static class Others
    {
        public enum PrintType
        {
            Message,
            Error,
            Warning,
            Custom
        }

        public static void Print(string message, PrintType type)
        {
            switch (type)
            {
                case PrintType.Custom:
                    Game.PrintChat(message);
                    break;
                case PrintType.Message:
                    Game.PrintChat("<font color = \"#00E5EE\">" + message + "</font>");
                    break;
                case PrintType.Warning:
                    Game.PrintChat("<font color = \"#FFA500\">" + message + "</font>");
                    break;
                case PrintType.Error:
                    Game.PrintChat("<font color = \"#FF0000\">" + message + "</font>");
                    break;
            }
        }

        public static bool IsBurrowed(this Obj_AI_Hero Hero)
        {
            return Hero.Buffs.Any(buff => buff.IsValidBuff() 
                && buff.DisplayName.ToLowerInvariant() == "reksaiw" 
                && Hero.NetworkId == buff.Caster.NetworkId);
        }

        public static bool QActive(this Obj_AI_Hero Hero)
        {
            return Hero.Buffs.Any(buff => buff.IsValidBuff() 
                && buff.DisplayName.ToLowerInvariant() == "reksaiq" 
                && Hero.NetworkId == buff.Caster.NetworkId);
        }

        public static bool IsUnder(this Obj_AI_Hero player, Obj_AI_Base target)
        {
            return player.IsBurrowed() 
                && target.Distance(player.Position) < Program.W2.Range;
        }

        public static bool ShouldBeCasted(this Spell spell)
        {
            if (!ObjectManager.Player.IsBurrowed())
            {
                return spell.IsReady()
                    && AssemblyMenu.Menu.Item("reksai.combo." + spell.Slot.ToString().ToLowerInvariant() + "1").GetValue<bool>();
            }
            else
            {
                return spell.IsReady() 
                    && AssemblyMenu.Menu.Item("reksai.combo." + spell.Slot.ToString().ToLowerInvariant() + "2").GetValue<bool>();

            }
        }

        public static float GetDamageOn(Obj_AI_Hero target)
        {
            double damage = 0;
            var player = ObjectManager.Player;
            if (player.IsBurrowed())
            {
                if (Program.Q2.IsReady())
                    damage += Program.Q2.GetDamage(target);
                if (Program.W2.IsReady())
                    damage += Program.W2.GetDamage(target);
            }
            if (!player.IsBurrowed())
            {
                if (Program.Q1.IsReady())
                    damage += Program.Q1.GetDamage(target) + player.GetAutoAttackDamage(target);
                if (Program.E1.IsReady())
                    damage += Program.E1.GetDamage(target);
                damage += player.GetAutoAttackDamage(target);
            }

            return (float)damage;
        }

        private static readonly Random RandomPos = new Random(DateTime.Now.Millisecond);
        public static void MoveTo(Vector3 pos)
        {
            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, ObjectManager.Player.ServerPosition.Extend(pos, (RandomPos.NextFloat(0.6f, 1) + 0.2f) * 300));
        }
    }
}
