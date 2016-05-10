using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server
{
    class HumilityVirtue
    {
        public static void Initialize()
        {
            VirtueGump.Register(108, OnVirtueUsed);
        }

        public static void OnVirtueUsed(Mobile from)
        {
            if (!@from.Alive) return;
            @from.SendMessage("Target the pet you wish to embrace with your Humility...");
            @from.Target = new InternalTarget();
        }

        public static void Humility(Mobile from, object targ)
        {
            if (!(targ is Mobile)) return;

            BaseCreature bc = targ as BaseCreature;

            VirtueLevel vl = VirtueHelper.GetLevel(from, VirtueName.Humility);
            if (bc != null && bc.ControlMaster == @from && vl >= VirtueLevel.Seeker)
            {
                switch (vl)
                {
                    case VirtueLevel.Seeker:
                        bc.HumilityBuff = 1;
                        break;
                    case VirtueLevel.Follower:
                        bc.HumilityBuff = 2;
                        break;
                    case VirtueLevel.Knight:
                        bc.HumilityBuff = 3;
                        break;
                }

                Timer mTimer = new HumilityTimer(bc);
                mTimer.Start();
                from.SendLocalizedMessage(1155819);
            }
        }

        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(14, false, TargetFlags.Beneficial)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Humility(from, targeted);
            }
        }

        public class HumilityTimer : Timer
        {
            private BaseCreature pet;

            public HumilityTimer(BaseCreature m)
                : base(TimeSpan.FromMinutes(20))
            {
                pet = m;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (pet != null)
                {
                    pet.HumilityBuff = 0;
                }
                Stop();
            }
        }
    }


}