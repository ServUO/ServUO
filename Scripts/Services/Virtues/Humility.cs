using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
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
            @from.SendMessage("Target the pet you wish to embrace with your Humility..."); // Get cliloc
            @from.Target = new InternalTarget();
        }

        public static void Humility(Mobile from, object targ)
        {
            if (!(targ is Mobile)) return;

            BaseCreature bc = targ as BaseCreature;

            VirtueLevel vl = VirtueHelper.GetLevel(from, VirtueName.Humility);
            if (bc != null && bc.ControlMaster == @from && vl >= VirtueLevel.Seeker)
            {
                int usedPoints;
                if (from.Virtues.Humility < 4399)
                    usedPoints = 400;
                else if (from.Virtues.Humility < 10599)
                    usedPoints = 600;
                else
                    usedPoints = 1000;

                VirtueHelper.Atrophy(from, VirtueName.Humility, usedPoints);

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
            else
                from.SendMessage("You can only embrace your Humility on a pet.");//get cliloc
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

    public class HumilityGump : Gump
    {
        Mobile caller;

        public HumilityGump(Mobile from) : this()
        {
            caller = from;
        }

        public HumilityGump() : base(0, 0)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);
            AddBackground(147, 63, 548, 276, 9380);
            AddHtmlLocalized(187, 102, 477, 109, 1155858, false, false);
            AddImage(379, 215, 108);
        }



        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    {

                        break;
                    }

            }
        }
    }

}