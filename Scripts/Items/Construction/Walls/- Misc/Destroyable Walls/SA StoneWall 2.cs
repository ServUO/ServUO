using System;

namespace Server.Items
{
    public class SAStoneWall2South : DamageableItem
    {
        [Constructable]
        public SAStoneWall2South()
            : base(88, 631)
        {
            this.Name = "Stone Wall";

            this.Level = ItemLevel.VeryEasy;
            this.Movable = false;
        }

        public SAStoneWall2South(Serial serial)
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

    public class SAStoneWall2East : DamageableItem
    {
        [Constructable]
        public SAStoneWall2East()
            : base(87, 636)
        {
            this.Name = "Stone Wall";

            this.Level = ItemLevel.VeryEasy;
            this.Movable = false;
        }

        public SAStoneWall2East(Serial serial)
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