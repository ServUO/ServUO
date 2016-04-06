using System;

namespace Server.Items
{
    public class DeBoorShield : Item
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DeBoorShield()
            : base(0x1B74)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 7.0;
            this.Movable = false;
        }

        public override int LabelNumber
        {
            get
            {
                return 1075308; // Ancestral Shield
            }
        }

        public DeBoorShield(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 5.0)
                this.Weight = 7.0;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version
        }
    }
}