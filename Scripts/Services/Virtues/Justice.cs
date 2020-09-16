#region References
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System;
#endregion

namespace Server.Services.Virtues
{
    public class JusticeVirtue
    {
        private static readonly TimeSpan LossDelay = TimeSpan.FromDays(7.0);

        private const int LossAmount = 950;

        public static void Initialize()
        {
            VirtueGump.Register(109, OnVirtueUsed);
        }

        public static bool CheckMapRegion(Mobile first, Mobile second)
        {
            Map map = first.Map;

            if (second.Map != map)
                return false;

            return GetMapRegion(map, first.Location) == GetMapRegion(map, second.Location);
        }

        public static int GetMapRegion(Map map, Point3D loc)
        {
            if (map == null || map.MapID >= 2)
                return 0;

            if (loc.X < 5120)
                return 0;

            if (loc.Y < 2304)
                return 1;

            return 2;
        }

        public static void OnVirtueUsed(Mobile from)
        {
            if (!from.CheckAlive())
                return;

            PlayerMobile protector = from as PlayerMobile;

            if (protector == null)
                return;

            if (!VirtueHelper.IsSeeker(protector, VirtueName.Justice))
            {
                protector.SendLocalizedMessage(1049610); // You must reach the first path in this virtue to invoke it.
            }
            else if (!protector.CanBeginAction(typeof(JusticeVirtue)))
            {
                protector.SendLocalizedMessage(1049370); // You must wait a while before offering your protection again.
            }
            else if (protector.JusticeProtectors.Count > 0)
            {
                protector.SendLocalizedMessage(1049542); // You cannot protect someone while being protected.
            }
            else if (protector.Map != Map.Felucca)
            {
                protector.SendLocalizedMessage(1049372); // You cannot use this ability here.
            }
            else
            {
                protector.BeginTarget(14, false, TargetFlags.None, OnVirtueTargeted);
                protector.SendLocalizedMessage(1049366); // Choose the player you wish to protect.
            }
        }

        public static void OnVirtueTargeted(Mobile from, object obj)
        {
            PlayerMobile protector = from as PlayerMobile;
            PlayerMobile pm = obj as PlayerMobile;

            if (protector == null)
                return;

            if (!VirtueHelper.IsSeeker(protector, VirtueName.Justice))
                protector.SendLocalizedMessage(1049610); // You must reach the first path in this virtue to invoke it.
            else if (!protector.CanBeginAction(typeof(JusticeVirtue)))
                protector.SendLocalizedMessage(1049370); // You must wait a while before offering your protection again.
            else if (protector.JusticeProtectors.Count > 0)
                protector.SendLocalizedMessage(1049542); // You cannot protect someone while being protected.
            else if (protector.Map != Map.Felucca)
                protector.SendLocalizedMessage(1049372); // You cannot use this ability here.
            else if (pm == null)
                protector.SendLocalizedMessage(1049678); // Only players can be protected.
            else if (pm.Map != Map.Felucca)
                protector.SendLocalizedMessage(1049372); // You cannot use this ability here.
            else if (pm == protector || pm.Criminal || pm.Murderer)
                protector.SendLocalizedMessage(1049436); // That player cannot be protected.
            else if (pm.JusticeProtectors.Count > 0)
                protector.SendLocalizedMessage(1049369); // You cannot protect that player right now.
            else if (pm.HasGump(typeof(AcceptProtectorGump)))
                protector.SendLocalizedMessage(1049369); // You cannot protect that player right now.
            else
                pm.SendGump(new AcceptProtectorGump(protector, pm));
        }

        public static void OnVirtueAccepted(PlayerMobile protector, PlayerMobile protectee)
        {
            if (!VirtueHelper.IsSeeker(protector, VirtueName.Justice))
            {
                protector.SendLocalizedMessage(1049610); // You must reach the first path in this virtue to invoke it.
            }
            else if (!protector.CanBeginAction(typeof(JusticeVirtue)))
            {
                protector.SendLocalizedMessage(1049370); // You must wait a while before offering your protection again.
            }
            else if (protector.JusticeProtectors.Count > 0)
            {
                protector.SendLocalizedMessage(1049542); // You cannot protect someone while being protected.
            }
            else if (protector.Map != Map.Felucca)
            {
                protector.SendLocalizedMessage(1049372); // You cannot use this ability here.
            }
            else if (protectee.Map != Map.Felucca)
            {
                protector.SendLocalizedMessage(1049372); // You cannot use this ability here.
            }
            else if (protectee == protector || protectee.Criminal || protectee.Murderer)
            {
                protector.SendLocalizedMessage(1049436); // That player cannot be protected.
            }
            else if (protectee.JusticeProtectors.Count > 0)
            {
                protector.SendLocalizedMessage(1049369); // You cannot protect that player right now.
            }
            else
            {
                protectee.JusticeProtectors.Add(protector);

                string args = string.Format("{0}\t{1}", protector.Name, protectee.Name);

                protectee.SendLocalizedMessage(1049451, args); // You are now being protected by ~1_NAME~.
                protector.SendLocalizedMessage(1049452, args); // You are now protecting ~2_NAME~.
            }
        }

        public static void OnVirtueRejected(PlayerMobile protector, PlayerMobile protectee)
        {
            string args = string.Format("{0}\t{1}", protector.Name, protectee.Name);

            protectee.SendLocalizedMessage(1049453, args); // You have declined protection from ~1_NAME~.
            protector.SendLocalizedMessage(1049454, args); // ~2_NAME~ has declined your protection.

            if (protector.BeginAction(typeof(JusticeVirtue)))
                Timer.DelayCall(TimeSpan.FromMinutes(15.0), RejectDelay_Callback, protector);
        }

        public static void RejectDelay_Callback(PlayerMobile state)
        {
            if (state != null)
                state.EndAction(typeof(JusticeVirtue));
        }

        public static void CheckAtrophy(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return;

            try
            {
                if ((pm.LastJusticeLoss + LossDelay) < DateTime.UtcNow)
                {
                    if (VirtueHelper.Atrophy(from, VirtueName.Justice, LossAmount))
                        from.SendLocalizedMessage(1049373); // You have lost some Justice.

                    pm.LastJusticeLoss = DateTime.UtcNow;
                }
            }
            catch (Exception e)
            {
                Diagnostics.ExceptionLogging.LogException(e);
            }
        }
    }

    public class AcceptProtectorGump : Gump
    {
        private readonly PlayerMobile m_Protector;
        private readonly PlayerMobile m_Protectee;

        public AcceptProtectorGump(PlayerMobile protector, PlayerMobile protectee)
            : base(150, 50)
        {
            m_Protector = protector;
            m_Protectee = protectee;

            Closable = false;

            AddPage(0);

            AddBackground(0, 0, 396, 218, 3600);

            AddImageTiled(15, 15, 365, 190, 2624);
            AddAlphaRegion(15, 15, 365, 190);

            AddHtmlLocalized(
                30,
                20,
                360,
                25,
                1049365,
                0x7FFF,
                false,
                false); // Another player is offering you their <a href="?ForceTopic88">protection</a>: 
            AddLabel(90, 55, 1153, protector.Name);

            AddImage(50, 45, 9005);
            AddImageTiled(80, 80, 200, 1, 9107);
            AddImageTiled(95, 82, 200, 1, 9157);

            AddRadio(30, 110, 9727, 9730, true, 1);
            AddHtmlLocalized(65, 115, 300, 25, 1049444, 0x7FFF, false, false); // Yes, I would like their protection.

            AddRadio(30, 145, 9727, 9730, false, 0);
            AddHtmlLocalized(65, 148, 300, 25, 1049445, 0x7FFF, false, false); // No thanks, I can take care of myself.

            AddButton(160, 175, 247, 248, 2, GumpButtonType.Reply, 0);

            AddImage(215, 0, 50581);

            AddImageTiled(15, 14, 365, 1, 9107);
            AddImageTiled(380, 14, 1, 190, 9105);
            AddImageTiled(15, 205, 365, 1, 9107);
            AddImageTiled(15, 14, 1, 190, 9105);
            AddImageTiled(0, 0, 395, 1, 9157);
            AddImageTiled(394, 0, 1, 217, 9155);
            AddImageTiled(0, 216, 395, 1, 9157);
            AddImageTiled(0, 0, 1, 217, 9155);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 2)
            {
                bool okay = info.IsSwitched(1);

                if (okay)
                    JusticeVirtue.OnVirtueAccepted(m_Protector, m_Protectee);
                else
                    JusticeVirtue.OnVirtueRejected(m_Protector, m_Protectee);
            }
        }
    }
}
