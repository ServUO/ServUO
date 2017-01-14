// Unknown mechanical properties
using System;

namespace Server.Items
{
    public class VollemHeldInCrystal : Item
    {
        [Constructable]
        public VollemHeldInCrystal()
            : base(0x1f19)
        {
            this.Hue = 1154;
            this.LootType = LootType.Blessed;
            this.Weight = 1;
        }

        public VollemHeldInCrystal(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113629;
            }
        }// A Vollem Held in Crystal
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