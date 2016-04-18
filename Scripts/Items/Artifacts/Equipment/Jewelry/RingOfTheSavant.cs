using System;

namespace Server.Items
{
    public class RingOfTheSavant : GoldRing
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RingOfTheSavant()
        {
            this.LootType = LootType.Blessed;

            this.Attributes.BonusInt = 3;
            this.Attributes.CastRecovery = 1;
            this.Attributes.CastSpeed = 1;
        }

        public RingOfTheSavant(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1077608;
            }
        }// Ring of the Savant
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}