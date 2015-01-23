﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using TopOrAFK.Champions;

namespace TopOrAFK
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string ChampionSwitch = ObjectManager.Player.ChampionName.ToLowerInvariant();

            switch (ChampionSwitch)
            {
                case "aatrox":
                    new Aatrox();
                    Game.PrintChat("<font color='#FF00BF'>[TOPORAFK] : Aatrox | By TehBlaxxor</font>", ChampionSwitch);
                    break;

                default:
                    Game.PrintChat("[TOPORAFK] Failed to load assembly for champion : NOT SUPPORTED!", ChampionSwitch);
                    break;
            }


        }
    }
}