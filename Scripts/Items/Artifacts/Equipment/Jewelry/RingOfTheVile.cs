using System;

namespace Server.Items
{
    public class RingOfTheVile : GoldRing
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RingOfTheVile()
        {
            this.Hue = 0x4F7;
            this.Attributes.BonusDex = 8;
            this.Attributes.RegenStam = 6;
            this.Attributes.AttackChance = 15;
            this.Resistances.Poison = 20;
        }

        public RingOfTheVile(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061102;
            }
        }// Ring of the Vile
        public override int ArtifactRarity
        {
            get
            {
                return 11;
            }
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

            if (this.Hue == 0x4F4)
                this.Hue = 0x4F7;
        }
    }
}