using System;

namespace Server.Items
{
    public class TunicOfGuarding : LeatherChest
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public TunicOfGuarding()
        {
            this.LootType = LootType.Blessed;

            this.Attributes.BonusHits = 2;
            this.Attributes.ReflectPhysical = 5;
        }

        public TunicOfGuarding(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1077693;
            }
        }// Tunic of Guarding
        public override int BasePhysicalResistance
        {
            get
            {
                return 6;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 6;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 5;
            }
        }
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