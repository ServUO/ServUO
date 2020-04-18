namespace Server.Items
{
    public class SAStoneWall1South : DamageableItem
    {
        [Constructable]
        public SAStoneWall1South()
            : base(969, 631)
        {
            Name = "Stone Wall";
            Hue = 1110;

            Level = ItemLevel.VeryEasy;
            Movable = false;
        }

        public SAStoneWall1South(Serial serial)
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

    public class SAStoneWall1East : DamageableItem
    {
        [Constructable]
        public SAStoneWall1East()
            : base(968, 636)
        {
            Name = "Stone Wall";
            Hue = 1110;

            Level = ItemLevel.VeryEasy;
            Movable = false;
        }

        public SAStoneWall1East(Serial serial)
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