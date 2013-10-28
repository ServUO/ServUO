using System;

namespace Server.Items
{
    public class UndeadGargoyleMedallions : Item
    {
        [Constructable]
        public UndeadGargoyleMedallions()
            : base(0x1088)
        {
            this.Name = "Undead Gargoyle Medallions";
            this.LootType = LootType.Blessed;
            this.Weight = 1.0;
            this.Hue = 0x47F; // TODO check
        }

        public UndeadGargoyleMedallions(Serial serial)
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