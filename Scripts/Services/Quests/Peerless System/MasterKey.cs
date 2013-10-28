using System;
using Server.Gumps;

namespace Server.Items
{
    public class MasterKey : PeerlessKey
    { 
        private PeerlessAltar m_Altar;
        [Constructable]
        public MasterKey(int itemID)
            : base(itemID)
        {
            this.LootType = LootType.Blessed;
        }

        public MasterKey(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074348;
            }
        }// master key
        [CommandProperty(AccessLevel.GameMaster)]
        public PeerlessAltar Altar
        {
            get
            {
                return this.m_Altar;
            }
            set
            {
                this.m_Altar = value;
            }
        }
        public override void OnDoubleClick(Mobile from)
        { 
            if (this.CanOfferConfirmation(from))
            {
                from.CloseGump(typeof(ConfirmPartyGump));			
                from.SendGump(new ConfirmPartyGump(this));
            }
        }

        public override void OnAfterDelete()
        { 
            if (this.m_Altar == null)
                return;
				
            this.m_Altar.MasterKeys.Remove(this);
			
            if (this.m_Altar.MasterKeys.Count == 0 && this.m_Altar.Fighters.Count == 0)
                this.m_Altar.FinishSequence();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
			
            writer.Write((Item)this.m_Altar);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            this.m_Altar = reader.ReadItem() as PeerlessAltar;
        }

        public virtual bool CanOfferConfirmation(Mobile from)
        {
            if (this.m_Altar != null && this.m_Altar.Fighters.Contains(from))
            {
                from.SendLocalizedMessage(1063296); // You may not use that teleporter at this time.
                return false;				
            }
				
            return true;
        }
    }
}