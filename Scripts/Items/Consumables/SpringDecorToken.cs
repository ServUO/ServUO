using Server.Gumps;

namespace Server.Items
{
    public class SpringDecorToken : Item
    {
        public override int LabelNumber => 1070997; // a promotional token

        [Constructable]
        public SpringDecorToken()
            : base(0x2AAA)
        {
            LootType = LootType.Blessed;
            Weight = 5.0;
        }

        public SpringDecorToken(Serial serial)
            : base(serial)
        {
        }
        
        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(SpringDecorTokenGump));
                from.SendGump(new SpringDecorTokenGump(this, from));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1070998, string.Format("#{0}", 1075553));  // Spring Decor Collection Item
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
