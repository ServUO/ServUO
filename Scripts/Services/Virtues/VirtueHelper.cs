using System;

namespace Server
{
    public enum VirtueLevel
    {
        None,
        Seeker,
        Follower,
        Knight
    }

    public enum VirtueName
    {
        Humility,
        Sacrifice,
        Compassion,
        Spirituality,
        Valor,
        Honor,
        Justice,
        Honesty
    }

    public class VirtueHelper
    {
        public static bool HasAny(Mobile from, VirtueName virtue)
        {
            return (from.Virtues.GetValue((int)virtue) > 0);
        }

        public static bool IsHighestPath(Mobile from, VirtueName virtue)
        {
            return (from.Virtues.GetValue((int)virtue) >= GetMaxAmount(virtue));
        }

        public static VirtueLevel GetLevel(Mobile from, VirtueName virtue)
        {
            int v = from.Virtues.GetValue((int)virtue);
            int vl;
            int vmax = GetMaxAmount(virtue);

            if (v < 4000)
                vl = 0;
            else if (v >= vmax)
                vl = 3;
            else
                vl = (v + 10000) / 10000;

            return (VirtueLevel)vl;
        }

        public static int GetMaxAmount(VirtueName virtue)
        {
            if (virtue == VirtueName.Honor)
                return 20000;

            if (virtue == VirtueName.Sacrifice)
                return 22000;

            return 21000;
        }

        public static bool Award(Mobile from, VirtueName virtue, int amount, ref bool gainedPath)
        {
            int current = from.Virtues.GetValue((int)virtue);

            int maxAmount = GetMaxAmount(virtue);

            if (current >= maxAmount)
                return false;

            if (from.FindItemOnLayer(Layer.TwoHanded) is Server.Items.VirtueShield)
                amount = amount + (int)((double)amount * 1.5);

            if ((current + amount) >= maxAmount)
                amount = maxAmount - current;

            VirtueLevel oldLevel = GetLevel(from, virtue);

            from.Virtues.SetValue((int)virtue, current + amount);

            gainedPath = (GetLevel(from, virtue) != oldLevel);

            return true;
        }

        public static bool Atrophy(Mobile from, VirtueName virtue)
        {
            return Atrophy(from, virtue, 1);
        }

        public static bool Atrophy(Mobile from, VirtueName virtue, int amount)
        {
            int current = from.Virtues.GetValue((int)virtue);

            if ((current - amount) >= 0)
                from.Virtues.SetValue((int)virtue, current - amount);
            else
                from.Virtues.SetValue((int)virtue, 0);

            return (current > 0);
        }

        public static bool IsSeeker(Mobile from, VirtueName virtue)
        {
            return (GetLevel(from, virtue) >= VirtueLevel.Seeker);
        }

        public static bool IsFollower(Mobile from, VirtueName virtue)
        {
            return (GetLevel(from, virtue) >= VirtueLevel.Follower);
        }

        public static bool IsKnight(Mobile from, VirtueName virtue)
        {
            return (GetLevel(from, virtue) >= VirtueLevel.Knight);
        }
    }
}