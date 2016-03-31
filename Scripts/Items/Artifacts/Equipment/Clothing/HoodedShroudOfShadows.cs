using System;

namespace Server.Items
{
    [Flipable(0x2684, 0x2683)]
    public class HoodedShroudOfShadows : BaseOuterTorso
    {
        [Constructable]
        public HoodedShroudOfShadows()
            : this(0x455)
        {
        }

        [Constructable]
        public HoodedShroudOfShadows(int hue)
            : base(0x2684, hue)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 3.0;
        }

        public HoodedShroudOfShadows(Serial serial)
            : base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
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