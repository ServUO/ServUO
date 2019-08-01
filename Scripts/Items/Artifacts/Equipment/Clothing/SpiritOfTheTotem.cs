using System;

namespace Server.Items
{
    public class SpiritOfTheTotem : BearMask
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1061599; } }// Spirit of the Totem
        public override int ArtifactRarity { get { return 11; } }
        public override int BasePhysicalResistance { get { return 20; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
		
        [Constructable]
        public SpiritOfTheTotem()
        {
            Hue = 0x455;
            Attributes.BonusStr = 20;
            Attributes.ReflectPhysical = 15;
            Attributes.AttackChance = 15;
        }

        public SpiritOfTheTotem(Serial serial)
            : base(serial)
        {
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
        }
    }
}