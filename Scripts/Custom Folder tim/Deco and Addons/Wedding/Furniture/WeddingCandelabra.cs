using System;

namespace Server.Items
{
    [Furniture]
    public class WeddingCandelabra : BaseLight
    {
        public override int LitItemID
        {
            get
            {
                return 40689;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                return 40692;
            }
        }
		
        [Constructable]
        public WeddingCandelabra()
            : base(40689)
        {
            Name = "Wedding Candelabra";
			this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = true;
            this.Light = LightType.Circle225;
			this.Weight = 20.0;
			LootType = LootType.Blessed;
        }

        public WeddingCandelabra(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 6.0)
                this.Weight = 20.0;
        }
    }
	
	[Furniture]
    public class WeddingRoundCandelabra : BaseLight
    {
        public override int LitItemID
        {
            get
            {
                return 40595;
            }
        }
        public override int UnlitItemID
        {
            get
            {
                return 40598;
            }
        }
		
        [Constructable]
        public WeddingRoundCandelabra()
            : base(40595)
        {
            Name = "Wedding Candelabra";
			this.Duration = TimeSpan.Zero; // Never burnt out
            this.Burning = true;
            this.Light = LightType.Circle225;
			this.Weight = 20.0;
			LootType = LootType.Blessed;
        }

        public WeddingRoundCandelabra(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 6.0)
                this.Weight = 20.0;
        }
    }
}