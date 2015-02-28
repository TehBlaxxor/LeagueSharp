using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Template
{
    class Settings
    {
        /// <summary>
        /// Main Assembly Settings
        /// </summary>
        public static string CHAMPION_NAME = "Vladimir";
        public static TargetSelector.DamageType DAMAGE_TYPE = TargetSelector.DamageType.Magical; //magic or physical
        public static string VERSION = "1.0.0.0";
        public static string AUTHOR_NAME = "TehBlaxxor";
        public static string ASSEMBLY_NAME = "TehVladimir";

        /// <summary>
        /// Target Selector Range
        /// </summary>
        public static float TARGETSELECTOR_RANGE = 750;

        /// <summary>
        /// Notification Settings
        /// </summary>
        public static bool SHOW_NOTIFICATION = true;
        public static int NOTIFICATION_DURATION = 3000;
        public static Color NOTIFICATION_COLOR = Color.FromArgb(113, 203, 247);
        public static bool NOTIFICATION_DISPOSE = true;

        /// <summary>
        /// Drawings Settings
        /// </summary>
        public static string MENUITEM_DRAWINGS_NAME = "drawOn";
        public static string Q_DRAWING_MENUITEM_NAME = "drawQ";
        public static string E_DRAWING_MENUITEM_NAME = "drawE";
        public static string W_DRAWING_MENUITEM_NAME = "drawW";
        public static string R_DRAWING_MENUITEM_NAME = "drawR";

    }
}
