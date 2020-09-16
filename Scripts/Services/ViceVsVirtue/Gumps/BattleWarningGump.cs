using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;
using System.Linq;

namespace Server.Engines.VvV
{
    public class BattleWarningGump : Gump
    {
        public PlayerMobile User { get; set; }
        public Timer Timer { get; set; }

        public BattleWarningGump(PlayerMobile pm)
            : base(50, 50)
        {
            User = pm;

            AddBackground(0, 0, 500, 200, 83);

            AddHtmlLocalized(0, 25, 500, 20, 1154645, "#1155582", Quests.BaseQuestGump.C32216(0xFF0000), false, false);
            AddHtmlLocalized(10, 55, 480, 100, 1154645, "#1155583", 0xFFFF, false, false); // You are in an active Vice vs Virtue battle region!  If you do not leave the City you will be open to attack!

            AddButton(463, 168, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(250, 171, 250, 20, 1155647, 0xFFFF, false, false); // Teleport to nearest Moongate?

            Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), () =>
                {
                    User.CloseGump(typeof(BattleWarningGump));
                    ViceVsVirtueSystem.AddTempParticipant(User, null);
                });
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }

            if (info.ButtonID == 1)
            {
                PublicMoongate closestGate = null;
                double closestDist = 0;

                foreach (PublicMoongate gate in PublicMoongate.Moongates.Where(mg => mg.Map == User.Map))
                {
                    double dist = User.GetDistanceToSqrt(gate);

                    if (closestGate == null || dist < closestDist)
                    {
                        closestDist = dist;
                        closestGate = gate;
                    }
                }

                if (closestGate != null && User.Map != null)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        Point3D p = User.Map.GetRandomSpawnPoint(new Rectangle2D(closestGate.X - 5, closestGate.Y - 5, 10, 10));

                        if (closestGate.Map.CanFit(p.X, p.Y, p.Z, 16, false, true, true))
                        {
                            BaseCreature.TeleportPets(User, p, closestGate.Map);
                            User.MoveToWorld(p, closestGate.Map);

                            return;
                        }
                    }
                }
                else
                {
                    User.SendLocalizedMessage(1155584); // You are now open to attack!
                    ViceVsVirtueSystem.AddTempParticipant(User, null);
                }
            }
            else
            {
                User.SendLocalizedMessage(1155584); // You are now open to attack!
                ViceVsVirtueSystem.AddTempParticipant(User, null);
            }
        }
    }
}