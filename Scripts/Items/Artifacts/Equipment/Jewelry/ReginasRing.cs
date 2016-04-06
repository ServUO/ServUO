using System;

namespace Server.Items
{
    public class ReginasRing : SilverRing
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ReginasRing()
            : base()
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1;
        }

        public ReginasRing(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075305;
            }
        }// Regina's Ring
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