#region References
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System;
#endregion

namespace Server.Services.Virtues
{
    public class SacrificeVirtue
    {
        private static readonly TimeSpan GainDelay = TimeSpan.FromDays(1.0);
        private static readonly TimeSpan LossDelay = TimeSpan.FromDays(7.0);

        private const int LossAmount = 500;

        public static void Initialize()
        {
            VirtueGump.Register(110, OnVirtueUsed);
        }

        public static void OnVirtueUsed(Mobile from)
        {
            if (!from.Hidden)
            {
                if (from.Alive)
                    from.Target = new InternalTarget();
                else
                    Resurrect(from);
            }
            else
                from.SendLocalizedMessage(1052015); // You cannot do that while hidden.
        }

        public static void CheckAtrophy(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return;

            try
            {
                if ((pm.LastSacrificeLoss + LossDelay) < DateTime.UtcNow)
                {
                    if (VirtueHelper.Atrophy(from, VirtueName.Sacrifice, LossAmount))
                        from.SendLocalizedMessage(1052041); // You have lost some Sacrifice.

                    VirtueLevel level = VirtueHelper.GetLevel(from, VirtueName.Sacrifice);

                    pm.AvailableResurrects = (int)level;
                    pm.LastSacrificeLoss = DateTime.UtcNow;
                }
            }
            catch (Exception e)
            {
                Diagnostics.ExceptionLogging.LogException(e);
            }
        }

        public static void Resurrect(Mobile from)
        {
            if (from.Alive)
                return;

            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return;

            if (from.Criminal)
            {
                from.SendLocalizedMessage(1052007); // You cannot use this ability while flagged as a criminal.
            }
            else if (!VirtueHelper.IsSeeker(from, VirtueName.Sacrifice))
            {
                from.SendLocalizedMessage(1052004); // You cannot use this ability.
            }
            else if (pm.AvailableResurrects <= 0)
            {
                from.SendLocalizedMessage(1052005); // You do not have any resurrections left.
            }
            else
            {
                /*
				* We need to wait for them to accept the gump or they can just use
				* Sacrifice and cancel to have items in their backpack for free.
				*/
                from.CloseGump(typeof(ResurrectGump));
                from.SendGump(new ResurrectGump(from, true));
            }
        }

        public static void Sacrifice(Mobile from, object targeted)
        {
            if (!from.CheckAlive())
                return;

            PlayerMobile pm = from as PlayerMobile;

            if (pm == null)
                return;

            Mobile targ = targeted as Mobile;

            if (targ == null)
                return;

            if (!ValidateCreature(targ))
            {
                from.SendLocalizedMessage(1052014); // You cannot sacrifice your fame for that creature.
            }
            else if (((targ.Hits * 100) / Math.Max(targ.HitsMax, 1)) < 90)
            {
                from.SendLocalizedMessage(1052013); // You cannot sacrifice for this monster because it is too damaged.
            }
            else if (from.Hidden)
            {
                from.SendLocalizedMessage(1052015); // You cannot do that while hidden.
            }
            else if (VirtueHelper.IsHighestPath(from, VirtueName.Sacrifice))
            {
                from.SendLocalizedMessage(1052068); // You have already attained the highest path in this virtue.
            }
            else if (from.Fame < 2500)
            {
                from.SendLocalizedMessage(1052017); // You do not have enough fame to sacrifice.
            }
            else if (DateTime.UtcNow < (pm.LastSacrificeGain + GainDelay))
            {
                from.SendLocalizedMessage(1052016); // You must wait approximately one day before sacrificing again.
            }
            else
            {
                int toGain;

                if (from.Fame < 5000)
                    toGain = 500;
                else if (from.Fame < 10000)
                    toGain = 1000;
                else
                    toGain = 2000;

                from.Fame = 0;

                // I have seen the error of my ways!
                targ.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1052009);

                from.SendLocalizedMessage(1052010); // You have set the creature free.

                Timer.DelayCall(TimeSpan.FromSeconds(1.0), targ.Delete);

                pm.LastSacrificeGain = DateTime.UtcNow;

                bool gainedPath = false;

                if (VirtueHelper.Award(from, VirtueName.Sacrifice, toGain, ref gainedPath))
                {
                    if (gainedPath)
                    {
                        from.SendLocalizedMessage(1052008); // You have gained a path in Sacrifice!

                        if (pm.AvailableResurrects < 3)
                            ++pm.AvailableResurrects;
                    }
                    else
                    {
                        from.SendLocalizedMessage(1054160); // You have gained in sacrifice.
                    }
                }

                from.SendLocalizedMessage(1052016); // You must wait approximately one day before sacrificing again.
            }
        }

        public static bool ValidateCreature(Mobile m)
        {
            if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned))
                return false;

            return (m is Lich || m is Succubus || m is Daemon || m is EvilMage || m is EnslavedGargoyle ||
                    m is GargoyleEnforcer);
        }

        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(8, false, TargetFlags.None)
            { }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Sacrifice(from, targeted);
            }
        }
    }
}
