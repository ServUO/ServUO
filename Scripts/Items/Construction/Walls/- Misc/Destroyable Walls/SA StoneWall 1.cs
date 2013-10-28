using System;

namespace Server.Items
{
    public class SAStoneWall1South : DamageableItem
    {
        [Constructable]
        public SAStoneWall1South()
            : base(969, 631)
        {
            this.Name = "Stone Wall";
            this.Hue = 1110;

            this.Level = ItemLevel.VeryEasy;
            this.Movable = false;
        }

        public SAStoneWall1South(Serial serial)
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

    public class SAStoneWall1East : DamageableItem
    {
        [Constructable]
        public SAStoneWall1East()
            : base(968, 636)
        {
            this.Name = "Stone Wall";
            this.Hue = 1110;

            this.Level = ItemLevel.VeryEasy;
            this.Movable = false;
        }

        public SAStoneWall1East(Serial serial)
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