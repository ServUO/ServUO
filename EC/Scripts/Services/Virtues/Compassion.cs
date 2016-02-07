using System;
using Server.Mobiles;

namespace Server
{
    public class CompassionVirtue
    {
        private static readonly TimeSpan LossDelay = TimeSpan.FromDays(7.0);
        private const int LossAmount = 500;
        public static void Initialize()
        {
            VirtueGump.Register(105, new OnVirtueUsed(OnVirtueUsed));
        }

        public static void OnVirtueUsed(Mobile from)
        {
            from.SendLocalizedMessage(1053001); // This virtue is not activated through the virtue menu.
        }

        public static void CheckAtrophy(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return;

            try
            {
                if ((pm.LastCompassionLoss + LossDelay) < DateTime.UtcNow)
                {
                    VirtueHelper.Atrophy(from, VirtueName.Compassion, LossAmount);
                    //OSI has no cliloc message for losing compassion.  Weird.
                    pm.LastCompassionLoss = DateTime.UtcNow;
                }
            }
            catch
            {
            }
        }
    }
}