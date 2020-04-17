using Server.Network;

namespace Server.Engines.Khaldun
{
    public class RolledParchment : Item
    {
        public override int LabelNumber => 1158578;  // rolled parchment

        public int Page { get; set; }

        public RolledParchment(int page)
            : base(0x2831)
        {
            Page = page;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(Page);
        }

        public override void OnDoubleClick(Mobile m)
        {
            m.CloseGump(typeof(GumshoeItemGump));
            m.SendGump(new GumshoeItemGump(m, ItemID, Hue, "rolled parchment", 1158580, "Copied From a Book Found in a Hidden Supply Cache"));

            m.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1157722, "its origin", m.NetState); // *Your proficiency in ~1_SKILL~ reveals more about the item*
            m.SendSound(m.Female ? 0x30B : 0x41A);
        }

        public RolledParchment(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
            writer.Write(Page);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            Page = reader.ReadInt();
        }
    }
}
