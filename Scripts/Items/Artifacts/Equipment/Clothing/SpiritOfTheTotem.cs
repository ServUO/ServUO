using System;

namespace Server.Items
{
    public class SpiritOfTheTotem : BearMask
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public SpiritOfTheTotem()
        {
            this.Hue = 0x455;

            this.Attributes.BonusStr = 20;
            this.Attributes.ReflectPhysical = 15;
            this.Attributes.AttackChance = 15;
        }

        public SpiritOfTheTotem(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061599;
            }
        }// Spirit of the Totem
        public override int ArtifactRarity
        {
            get
            {
                return 11;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 20;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.Resistances.Physical = 0;
                        break;
                    }
            }
        }
    }
}