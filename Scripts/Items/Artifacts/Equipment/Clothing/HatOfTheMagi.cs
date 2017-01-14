using System;

namespace Server.Items
{
    public class HatOfTheMagi : WizardsHat
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public HatOfTheMagi()
        {
            this.Hue = 0x481;

            this.Attributes.BonusInt = 8;
            this.Attributes.RegenMana = 4;
            this.Attributes.SpellDamage = 10;
        }

        public HatOfTheMagi(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061597;
            }
        }// Hat of the Magi
        public override int ArtifactRarity
        {
            get
            {
                return 11;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 20;
            }
        }
        public override int BaseEnergyResistance
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
                        this.Resistances.Poison = 0;
                        this.Resistances.Energy = 0;
                        break;
                    }
            }
        }
    }
}