using System;

namespace Server.Items
{
    public class ChocolateFountain : Item
    {
        int m_charges;

        [Constructable]
        public ChocolateFountain()
            : base(40639)
        {
            this.Name = "Chocolate Fountain";
			LootType = LootType.Blessed;
            this.Weight = 10.0;
            this.Stackable = false;
            this.m_charges = 10;
            //this.Hue = Utility.RandomList(0x135, 0xcd, 0x38, 0x3b, 0x42, 0x4f, 0x11e, 0x60, 0x317, 0x10, 0x136, 0x1f9, 0x1a, 0xeb, 0x86, 0x2e);
        }

        public ChocolateFountain(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.AddToBackpack(new ChocolateCoveredStrawberry());
            
			m_charges -= 1;
            this.InvalidateProperties();
            if (m_charges < 1)
                this.Delete();
        }


		/*
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060741, this.m_charges.ToString()); // charges: ~1_val~
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            this.LabelTo(from, 1060741, this.m_charges.ToString()); // charges: ~1_val~
        }
		*/
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_charges = reader.ReadInt();
        }
    }

    public class ChocolateCoveredStrawberry : Food
    {
        [Constructable]
        public ChocolateCoveredStrawberry()
            : this(1)
        {
        }

        [Constructable]
        public ChocolateCoveredStrawberry(int amount)
            : base(amount, 40640)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
            this.Name = "Chocolate Covered Strawberry";
        }

        public ChocolateCoveredStrawberry(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}