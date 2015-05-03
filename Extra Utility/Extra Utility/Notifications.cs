using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using Notif = LeagueSharp.Common.Notifications;

namespace Extra_Utility
{
    public static class Notifications
    {
        

        public static void OnModLoaded(string Name)
        {
            Notif.AddNotification("EU - " + Name + " loaded!", 1000, true);
        }

        public static void OnSkinChanged(string NewSkin)
        {
            Notif.AddNotification("New Skin: " + NewSkin, 500, true);
        }

        public static void OnModelChanged(string NewModel)
        {
            Notif.AddNotification("New Model: " + NewModel, 500, true);
        }

        public static void OnFakeInput()
        {
            Notif.AddNotification("Command is invalid!", 500, true);
        }

        public static void OnFailedModel()
        {
            Notif.AddNotification("Invalid model specified!", 500, true);

        }
    }
}
