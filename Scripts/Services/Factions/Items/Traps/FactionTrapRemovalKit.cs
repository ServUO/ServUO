using System;

namespace Server.Factions
{
    public class FactionTrapRemovalKit : Item
    {
        private int m_Charges;
        [Constructable]
        public FactionTrapRemovalKit()
            : base(7867)
        {
            this.LootType = LootType.Blessed;
            this.m_Charges = 25;
        }

        public FactionTrapRemovalKit(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get
            {
                return this.m_Charges;
            }
            set
            {
                this.m_Charges = value;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1041508;
            }
        }// a faction trap removal kit
        public void ConsumeCharge(Mobile consumer)
        {
            --this.m_Charges;

            if (this.m_Charges <= 0)
            {
                this.Delete();

                if (consumer != null)
                    consumer.SendLocalizedMessage(1042531); // You have used all of the parts in your trap removal kit.
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            // NOTE: OSI does not list uses remaining; intentional difference
            list.Add(1060584, this.m_Charges.ToString()); // uses remaining: ~1_val~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.WriteEncodedInt((int)this.m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Charges = reader.ReadEncodedInt();
                        break;
                    }
                case 0:
                    {
                        this.m_Charges = 25;
                        break;
                    }
            }
        }
    }
}