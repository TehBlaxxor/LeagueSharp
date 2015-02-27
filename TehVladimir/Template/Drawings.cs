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
    class Drawings
    {
        public static void Initialize()
        {
            if (!Menu.GetBool(Settings.MENUITEM_DRAWINGS_NAME))
            {
                return;
            }

            if (Menu.GetBool(Settings.Q_DRAWING_MENUITEM_NAME))
            {
                Drawing.DrawCircle(Program.Player.Position, Spells.Q.Range, Color.Blue);
            }

            if (Menu.GetBool(Settings.E_DRAWING_MENUITEM_NAME))
            {
                Drawing.DrawCircle(Program.Player.Position, Spells.E.Range, Color.Blue);
            }

            if (Menu.GetBool(Settings.W_DRAWING_MENUITEM_NAME))
            {
                Drawing.DrawCircle(Program.Player.Position, Spells.W.Range, Color.Blue);
            }

            if (Menu.GetBool(Settings.R_DRAWING_MENUITEM_NAME))
            {
                Drawing.DrawCircle(Program.Player.Position, Spells.R.Range, Color.Red);
            }

        }
    }
}
