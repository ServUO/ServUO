using System;

namespace Server.Items
{
    public class StaffLightEditor : BaseRing
    {
        [Constructable]
        public StaffLightEditor()
            : base(0x108a)
        {
            this.Weight = 0.0;
            this.Name = "The Staff Light Editor";
            this.LootType = LootType.Blessed;
        }

        public StaffLightEditor(Serial serial)
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