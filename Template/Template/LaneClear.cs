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
    class LaneClear
    {
        public static void Run()
        {
            var target = TargetSelector.GetTarget(Settings.TARGETSELECTOR_RANGE, Settings.DAMAGE_TYPE);

            // add stuff here
        }
    }
}
