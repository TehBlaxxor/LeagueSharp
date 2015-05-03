using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace The_Masterpiece.Plugins
{
    internal class CustomUtility
    {
        #region Cicle Void
        public static void Cicle(bool AutoSurrender, bool SurrenderSuggestion, bool TeamfightSimulator, float TFSimRange, bool oneVSone)
        {
            if (AutoSurrender)
                Module1();
            if (SurrenderSuggestion)
                Module2();
        }

        #endregion
        #region Auto Surrender & Surrender Suggestion
        public static float LastVote = 0;
        public static float LastSurrenderProposal = 0;

        public static bool ShouldSurrender()
        {
            float EnemyGoldEarned = 0;
            float AllyGoldEarned = 0;
            foreach (var hero in HeroManager.Enemies.Where(x => x.GoldTotal > 0))
            {
                EnemyGoldEarned += hero.GoldTotal;
            }
            foreach (var hero in HeroManager.Allies.Where(x => x.GoldTotal > 0))
            {
                AllyGoldEarned += hero.GoldTotal;
            }
            if (EnemyGoldEarned + 10000 > AllyGoldEarned)
                return true;

            int EnemyKills = 0;
            int AllyKills = 0;
            foreach (var hero in HeroManager.Enemies.Where(x => x.ChampionsKilled > 0))
            {
                EnemyKills += hero.ChampionsKilled;
            }
            foreach (var hero in HeroManager.Allies.Where(x => x.ChampionsKilled > 0))
            {
                AllyKills += hero.ChampionsKilled;
            }
            if (EnemyKills + 20 > AllyKills)
                return true;

            int EnemyTurretKills = 0;
            int AllyTurretKills = 0;
            foreach (var hero in HeroManager.Enemies.Where(x => x.TurretsKilled > 0))
            {
                EnemyTurretKills += hero.TurretsKilled;
            }
            foreach (var hero in HeroManager.Allies.Where(x => x.TurretsKilled > 0))
            {
                AllyTurretKills += hero.TurretsKilled;
            }
            if (EnemyTurretKills + 5 > AllyTurretKills)
                return true;

            return false;

        }

        public static void Module1()
        {
            if (ShouldSurrender() && Game.Time > LastVote + 2000)
            {
                LastVote = Game.Time;
                Game.Say("/ff");
                Messages.OnSurrenderInitiated();
            }
        }
        public static void Module2()
        {
            if (ShouldSurrender() && Game.Time > LastSurrenderProposal + 2000)
            {
                LastSurrenderProposal = Game.Time;
                Messages.OnLossExpected();
            }
        }
        #endregion
    }
}
