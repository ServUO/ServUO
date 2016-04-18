using System;

namespace Server.Items
{
    public class TongueoftheBeast : WoodenKiteShield//, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public TongueoftheBeast()
        {
            this.ItemID = 0x1B78;
            this.Hue = 0x556;

            this.Attributes.SpellChanneling = 1;
            this.Attributes.RegenStam = 3;
            this.Attributes.RegenMana = 3;
        }

        public TongueoftheBeast(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112405;
            }
        }// Tongue of the Beast 
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
                return 10;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 10;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
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

            if (this.Attributes.NightSight == 0)
                this.Attributes.NightSight = 1;
        }
    }
}