using System;

namespace Server.Items
{
    public class DesCityWallSouth : DamageableItem
    {
        [Constructable]
        public DesCityWallSouth()
            : base(641, 631)
        {
            this.Name = "Damaged Wall";

            this.Level = ItemLevel.VeryEasy;
            this.Movable = false;
        }

        public DesCityWallSouth(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DesCityWallEast : DamageableItem
    {
        [Constructable]
        public DesCityWallEast()
            : base(642, 636)
        {
            this.Name = "Damaged Wall";

            this.Level = ItemLevel.VeryEasy;
            this.Movable = false;
        }

        public DesCityWallEast(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}