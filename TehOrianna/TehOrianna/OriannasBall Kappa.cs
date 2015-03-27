using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;


namespace TehOrianna
{
    class OriannasBall
    {
        internal static Vector3 BallPos;
        internal static Vector3 BallPosDrawSpot;
        internal static bool IsMoving;
        static OriannasBall()
        {
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || Orianna.Player.GetSpellSlot(args.SData.Name) != SpellSlot.Q)
                return;

            IsMoving = true;
            Utility.DelayAction.Add((int)Math.Max(1, 1000 * (args.End.Distance(BallPos) - (Game.Ping / 2.0 + 0.1)) / 1200), () =>
            {
                BallPos = args.End;
                BallPosDrawSpot = args.End;
                IsMoving = false;
            });
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Orianna.Player.HasBuff("orianaghostself", true))
            {
                    BallPos = ObjectManager.Player.ServerPosition;
                    BallPosDrawSpot = ObjectManager.Player.Position;
                    IsMoving = false;
                    return;
            }
            foreach (var ally in ObjectManager.Get<Obj_AI_Hero>().Where(ally => ally.IsAlly && !ally.IsDead && ally.HasBuff("orianaghost", true)))
            {
                    BallPos = ally.ServerPosition;
                    BallPosDrawSpot = ally.Position;
                    IsMoving = false;
                    return;
            }
        }
    }
}
