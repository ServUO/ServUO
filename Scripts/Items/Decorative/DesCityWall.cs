namespace Server.Items
{
    public class DesCityWallSouth : DamageableItem
    {
        [Constructable]
        public DesCityWallSouth()
            : base(641, 631)
        {
            Name = "Damaged Wall";

            Level = ItemLevel.VeryEasy;
            Movable = false;
        }

        public DesCityWallSouth(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); //version
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
            Name = "Damaged Wall";

            Level = ItemLevel.VeryEasy;
            Movable = false;
        }

        public DesCityWallEast(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}