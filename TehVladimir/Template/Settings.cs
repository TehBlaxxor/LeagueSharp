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
        public const string CHAMPION_NAME = "Vladimir";
        public const TargetSelector.DamageType DAMAGE_TYPE = TargetSelector.DamageType.Magical; //magic or physical
        public const string VERSION = "1.0.0.0";
        public const string AUTHOR_NAME = "TehBlaxxor";
        public const string ASSEMBLY_NAME = "TehVladimir";

        /// <summary>
        /// Target Selector Range
        /// </summary>
        public const float TARGETSELECTOR_RANGE = 750;

        /// <summary>
        /// Notification Settings
        /// </summary>
        public const bool SHOW_NOTIFICATION = true;
        public const int NOTIFICATION_DURATION = 3000;
        public static Color NOTIFICATION_COLOR = Color.FromArgb(113, 203, 247);
        public const bool NOTIFICATION_DISPOSE = true;

        /// <summary>
        /// Drawings Settings
        /// </summary>
        public const string MENUITEM_DRAWINGS_NAME = "drawOn";
        public const string Q_DRAWING_MENUITEM_NAME = "drawQ";
        public const string E_DRAWING_MENUITEM_NAME = "drawE";
        public const string W_DRAWING_MENUITEM_NAME = "drawW";
        public const string R_DRAWING_MENUITEM_NAME = "drawR";

    }
}
