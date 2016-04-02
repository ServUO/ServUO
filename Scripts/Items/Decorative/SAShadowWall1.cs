using System;

namespace Server.Items
{
    public class SAShadowWall1South : DamageableItem
    {
        [Constructable]
        public SAShadowWall1South()
            : base(13883, 631)
        {
            this.Name = "Shadow Wall";

            this.Level = ItemLevel.VeryEasy;
            this.Movable = false;
        }

        public SAShadowWall1South(Serial serial)
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

    public class SAShadowWall1East : DamageableItem
    {
        [Constructable]
        public SAShadowWall1East()
            : base(13882, 636)
        {
            this.Name = "Shadow Wall";

            this.Level = ItemLevel.VeryEasy;
            this.Movable = false;
        }

        public SAShadowWall1East(Serial serial)
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