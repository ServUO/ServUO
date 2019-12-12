using System;

namespace Server.Items
{
    [TypeAlias("Server.Items.TongueoftheBeast")]
    public class TongueOfTheBeast : WoodenKiteShield
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1112405; } } // Tongue of the Beast [Replica]
		
        [Constructable]
        public TongueOfTheBeast()
        {
            Hue = 153;
            Attributes.SpellChanneling = 1;
            Attributes.RegenStam = 3;
            Attributes.RegenMana = 3;
        }

        public TongueOfTheBeast(Serial serial)
            : base(serial)
        {
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
                return 5;
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
		public override bool CanFortify
        {
            get
            {
                return false;
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