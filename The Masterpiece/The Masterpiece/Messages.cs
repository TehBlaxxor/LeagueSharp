using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace The_Masterpiece
{
    internal class Messages
    {
        public static Notification Create(string message)
        {
            return new Notification("THEMP: " + message, 400, true);
        }

        public static void OnAssemblyLoad()
        {
            Notifications.AddNotification(Create(ObjectManager.Player.BaseSkinName + " loaded!"));
        }

        public static void OnSurrenderInitiated()
        {
            Notifications.AddNotification(Create("Surrender vote initialized!"));
        }

        public static void OnLossExpected()
        {
            Notifications.AddNotification(Create("Loss possibility is very high! Surrendering is suggested!"));
        }
    }
}
