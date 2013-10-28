using System;

namespace Server.Items
{
    public class DecoCrystalBall : Item
    {
        [Constructable]
        public DecoCrystalBall()
            : base(0xE2E)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoCrystalBall(Serial serial)
            : base(serial)
        {
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