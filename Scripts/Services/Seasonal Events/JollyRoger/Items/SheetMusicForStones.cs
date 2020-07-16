using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class SheetMusicForStones : Item
    {
        public override int LabelNumber => 1159343; // Sheet Music for Stones

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active { get; set; }

        [Constructable]
        public SheetMusicForStones()
            : base(0xEBF)
        {
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            Gump g = new Gump(100, 100);
            g.AddBackground(0, 0, 454, 400, 0x24A4);
            g.AddItem(75, 120, 0xEBF);
            g.AddHtmlLocalized(177, 50, 250, 18, 1114513, "#1159343", 0x3442, false, false); // Sheet Music for Stones
            g.AddHtmlLocalized(177, 77, 250, 36, 1114513, "#1159344", 0x3442, false, false); // Recovered from a Strongbox in Castle British with a Musical Lock
            g.AddHtmlLocalized(177, 122, 250, 228, 1159345, 0xC63, true, true); // Standard sheet music for the seminal <i>Stones</i> by Iolo the Bard. Written, unsurprisingly, for the lute.

            from.SendGump(g);

            from.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1157722, "its origin", from.NetState); // *Your proficiency in ~1_SKILL~ reveals more about the item*
            from.SendSound(from.Female ? 0x30B : 0x41A);
        }

        public SheetMusicForStones(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(Active);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            Active = reader.ReadBool();
        }
    }
}
