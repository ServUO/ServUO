using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    public class AbyssalHairDye : Item
    {
        [Constructable]
        public AbyssalHairDye()
            : base(0xEFE)
        {
            Hue = 2075;
            LootType = LootType.Blessed;
        }

        public AbyssalHairDye(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                BaseGump.SendGump(new HairDyeConfirmGump(m as PlayerMobile, Hue, this));
            }
            else
            {
                m.SendLocalizedMessage(1042010); //You must have the object in your backpack to use it.
            }
        }

        public override int LabelNumber => 1149822;  // Abyssal Hair Dye

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
