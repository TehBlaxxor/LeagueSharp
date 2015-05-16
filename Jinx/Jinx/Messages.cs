using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Jinx
{
    public static class Messages
    {
        public static void OnLoad()
        {
            Notifications.AddNotification("JINX: Loading finished! Enjoy!", 500);
        }
        public static void OnSpellInitializationSequence(int phaseNum)
        {
            Notifications.AddNotification("JINX: Spells initialized! (phase " + phaseNum + ")", 500);
        }

        public static void OnMenuCreation()
        {
            Notifications.AddNotification("JINX: Menu initialized successfully!", 500);
        }
    }
}
