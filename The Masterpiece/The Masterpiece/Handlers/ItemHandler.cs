using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using GameItem = LeagueSharp.Common.Items.Item;
using SharpDX;

namespace The_Masterpiece.Handlers
{

    internal class CustomItem
    {
        public GameItem Instance { get; set; }
        public float Range { get; set; }
    }

    static class ItemHandler
    {
        #region Items
        //Damages nearby enemies
        public static CustomItem Tiamat = new CustomItem
        {
            Instance = ItemData.Tiamat_Melee_Only.GetItem(),
            Range = Tiamat.Instance.Range
        };

        //Damages nearby enemies
        public static CustomItem Hydra = new CustomItem
        {
            Instance = ItemData.Ravenous_Hydra_Melee_Only.GetItem(),
            Range = Hydra.Instance.Range
        };

        //Damages nearby target
        public static CustomItem Cutlass = new CustomItem
        {
            Instance = ItemData.Bilgewater_Cutlass.GetItem(),
            Range = Cutlass.Instance.Range
        };

        //Damages nearby target
        public static CustomItem BotRK = new CustomItem
        {
            Instance = ItemData.Blade_of_the_Ruined_King.GetItem(),
            Range = BotRK.Instance.Range
        };

        //Slows nearby enemies
        public static CustomItem Randuin = new CustomItem
        {
            Instance = ItemData.Randuins_Omen.GetItem(),
            Range = Randuin.Instance.Range
        };

        //Temporary Invincibility
        public static CustomItem Zhonya = new CustomItem
        {
            Instance = ItemData.Zhonyas_Hourglass.GetItem()
        };

        //Damages & slows nearby target
        public static CustomItem Gunblade = new CustomItem
        {
            Instance = ItemData.Hextech_Gunblade.GetItem(),
            Range = Gunblade.Instance.Range
        };


        //Damages & slows nearby target
        public static CustomItem Revolver = new CustomItem
        {
            Instance = ItemData.Hextech_Revolver.GetItem(),
            Range = Revolver.Instance.Range
        };

        //Cleanse effect
        public static CustomItem Scimitar = new CustomItem
        {
            Instance = ItemData.Mercurial_Scimitar.GetItem()
        };

        //Cleanse effect
        public static CustomItem QSS = new CustomItem
        {
            Instance = ItemData.Quicksilver_Sash.GetItem()
        };

        //Cleanse + heal effect
        public static CustomItem Mikael = new CustomItem
        {
            Instance = ItemData.Mikaels_Crucible.GetItem(),
            Range = Mikael.Instance.Range
        };

        //Shields self for mana
        public static CustomItem Seraph = new CustomItem
        {
            Instance = ItemData.Seraphs_Embrace2.GetItem()
        };

        //Slows 2 enemies
        public static CustomItem Shadows = new CustomItem
        {
            Instance = ItemData.Zhonyas_Hourglass.GetItem(),
            Range = Shadows.Instance.Range
        };

        //Speed + attack speed self
        public static CustomItem Youmuu = new CustomItem
        {
            Instance = ItemData.Youmuus_Ghostblade.GetItem()
        };

        //AoE movement bonus + slowness in the end
        public static CustomItem Righteous = new CustomItem
        {
            Instance = ItemData.Righteous_Glory.GetItem(),
            Range = Righteous.Instance.Range
        };
        #endregion

        #region Application Programming Interface [API]
        public static bool CanCast(this CustomItem item)
        {
            return item.Instance.IsReady() && item.Instance.IsOwned();
        }

        public static bool CanCast(this CustomItem item, Obj_AI_Hero player)
        {
            return item.Instance.IsOwned() && item.Instance.IsReady() && item.Instance.IsInRange(player);
        }
        #endregion
    }
}
