using System;

namespace Server.Items
{
    public class ArmorOfFortune : StuddedChest
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ArmorOfFortune()
        {
            this.Hue = 0x501;
            this.Attributes.Luck = 200;
            this.Attributes.DefendChance = 15;
            this.Attributes.LowerRegCost = 40;
            this.ArmorAttributes.MageArmor = 1;
        }

        public ArmorOfFortune(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061098;
            }
        }// Armor of Fortune
        public override int ArtifactRarity
        {
            get
            {
                return 11;
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

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}