using System;

namespace Server.Items
{
    public class UndamagedUndeadGargoyleHorns : Item
    {
        public override int LabelNumber { get { return 1112903; } } // Undamaged Undead Gargoyle Horns

        [Constructable]
        public UndamagedUndeadGargoyleHorns()
            : base(0x315C)
        {
            this.Weight = 1.0;
            this.Hue = 61;
        }

        public UndamagedUndeadGargoyleHorns(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}