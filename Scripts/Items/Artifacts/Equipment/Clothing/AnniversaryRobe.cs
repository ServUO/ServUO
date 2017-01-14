using System;

namespace Server.Items
{
    [Flipable(0x4B9D, 0x4B9E)]
    public class AnniversaryRobe : BaseOuterTorso
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public AnniversaryRobe() : this(0x455)
        {
        }

        [Constructable]
        public AnniversaryRobe(int hue) : base(0x4B9D, hue)
        {
            this.Name = "15th Anniversary Commemorative Robe";
            this.LootType = LootType.Blessed;
            this.Weight = 1.0;
        }

        public AnniversaryRobe(Serial serial)
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