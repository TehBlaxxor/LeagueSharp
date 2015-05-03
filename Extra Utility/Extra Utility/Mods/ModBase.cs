using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Extra_Utility.Mods
{
    public abstract class ModBase
    {
        #region Variables & Stuff
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public virtual string ModName { get; internal set; }
        public virtual string ModVersion { get; internal set; }
        public static Menu Config;
        #endregion

        #region Events
        protected ModBase()
        {
            Config = new Menu("EU - " + ModName, "eu." + ModName.Replace(" ", string.Empty).ToLowerInvariant(), true);
            LoadSettings(Config);
            Config.AddItem(new MenuItem("version", "Version: " + ModVersion));
            Config.AddItem(new MenuItem("site", "Visit joduska.me!"));
            Config.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Game.OnInput += Game_OnInput;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        public virtual void Drawing_OnDraw(EventArgs args) { }

        public virtual void Game_OnInput(GameInputEventArgs args) { }

        public virtual void Game_OnUpdate(EventArgs args) { }


        #endregion

        #region Settings Menu
        public virtual void LoadSettings(Menu menu) { }
        #endregion

    }
}
