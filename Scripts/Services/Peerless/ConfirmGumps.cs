using System;
using Server.Items;

namespace Server.Gumps
{
    public class ConfirmPartyGump : BaseConfirmGump
    {
        private readonly MasterKey m_Key;
        public ConfirmPartyGump(MasterKey key)
            : base()
        {
            this.m_Key = key;
        }

        public override int LabelNumber
        {
            get
            {
                return 1072525;
            }
        }// <CENTER>Are you sure you want to teleport <BR>your party to an unknown area?</CENTER>
        public override void Confirm(Mobile from)
        { 
            if (this.m_Key == null)
                return;				
				
            if (this.m_Key.Altar == null)
                return;
				
            this.m_Key.Altar.SendConfirmations(from);
            this.m_Key.Delete();		
        }
    }

    public class ConfirmExitGump : BaseConfirmGump
    {
        private readonly PeerlessAltar m_Altar;
        public ConfirmExitGump(PeerlessAltar altar)
            : base()
        {
            this.m_Altar = altar;
        }

        public override int LabelNumber
        {
            get
            {
                return 1075026;
            }
        }// Are you sure you wish to teleport?
        public override void Confirm(Mobile from)
        { 
            if (this.m_Altar == null)
                return;
				
            this.m_Altar.Exit(from);
        }
    }

    public class ConfirmEntranceGump : BaseConfirmGump
    {
        private readonly PeerlessAltar m_Altar;
        public ConfirmEntranceGump(PeerlessAltar altar)
            : base()
        {
            this.m_Altar = altar;
        }

        public override int LabelNumber
        {
            get
            {
                return 1072526;
            }
        }// <CENTER>Your party is teleporting to an unknown area.<BR>Do you wish to go?</CENTER>
        public override void Confirm(Mobile from)
        { 
            if (this.m_Altar == null)
                return;
				
            this.m_Altar.AddFighter(from, true);
        }

        public override void Refuse(Mobile from)
        { 
            if (this.m_Altar == null)
                return;
				
            this.m_Altar.AddFighter(from, false);
        }
    }
}