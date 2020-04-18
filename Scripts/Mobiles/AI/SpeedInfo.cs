#region References
using Server.Mobiles;
using System;
#endregion

namespace Server
{
    public static class SpeedInfo
    {
        public static readonly double MinDelay = 0.1;
        public static readonly double MaxDelay = 0.5;
        public static readonly double MinDelayWild = 0.4;
        public static readonly double MaxDelayWild = 0.8;

        public static bool GetSpeeds(BaseCreature bc, ref double activeSpeed, ref double passiveSpeed)
        {
            int maxDex = GetMaxMovementDex(bc);
            int dex = Math.Min(maxDex, Math.Max(25, bc.Dex));

            double min = bc.IsMonster || InActivePVPCombat(bc) ? MinDelayWild : MinDelay;
            double max = bc.IsMonster || InActivePVPCombat(bc) ? MaxDelayWild : MaxDelay;

            if (bc.IsParagon)
            {
                min /= 2;
                max = min + .4;
            }

            activeSpeed = max - ((max - min) * ((double)dex / maxDex));

            if (activeSpeed < min)
            {
                activeSpeed = min;
            }

            passiveSpeed = activeSpeed * 2;

            return true;
        }

        private static int GetMaxMovementDex(BaseCreature bc)
        {
            return bc.IsMonster ? 150 : 190;
        }

        public static bool InActivePVPCombat(BaseCreature bc)
        {
            return bc.Combatant != null && bc.ControlOrder != OrderType.Follow && bc.Combatant is PlayerMobile;
        }

        public static double TransformMoveDelay(BaseCreature bc, double delay)
        {
            double adjusted = bc.IsMonster ? MaxDelayWild : MaxDelay;

            if (!bc.IsDeadPet && (bc.ReduceSpeedWithDamage || bc.IsSubdued))
            {
                double offset = bc.Stam / (double)bc.StamMax;

                if (offset < 1.0)
                {
                    delay = delay + ((adjusted - delay) * (1.0 - offset));
                }
            }

            if (delay > adjusted)
            {
                delay = adjusted;
            }

            return delay;
        }
    }
}
