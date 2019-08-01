using System;

namespace Server.Items
{
    public class AnthropomorphistGlasses : ElvenGlasses
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1073379; } }//Anthropomorphist Reading Glasses
        public override int BasePhysicalResistance { get { return 5; } }
        public override int BaseFireResistance { get { return 5; } }
        public override int BaseColdResistance { get { return 10; } }
        public override int BasePoisonResistance { get { return 20; } }
        public override int BaseEnergyResistance { get { return 20; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
		
        [Constructable]
        public AnthropomorphistGlasses()
        {
            Attributes.BonusHits = 5;
            Attributes.RegenMana = 3;
            Attributes.ReflectPhysical = 20;
            Hue = 0x80;
        }

        public AnthropomorphistGlasses(Serial serial)
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