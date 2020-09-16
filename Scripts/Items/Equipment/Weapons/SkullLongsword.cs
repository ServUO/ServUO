using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishSkullLongsword))]
    [Flipable(41793, 41794)]
    public class SkullLongsword : Longsword
    {
        public override int LabelNumber => 1125817;  // skull longsword

        [Constructable]
        public SkullLongsword()
        {
            ItemID = 41793;
        }

        public SkullLongsword(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Flipable(41797, 41798)]
    public class GargishSkullLongsword : Longsword
    {
        public override int LabelNumber => 1125821;  // gargish skull longsword

        [Constructable]
        public GargishSkullLongsword()
        {
            ItemID = 41797;
        }

        public GargishSkullLongsword(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
