using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishSkullLongsword))]
    [FlipableAttribute(41793, 41794)]
    public class SkullLongsword : Longsword
    {
        public override void LabelNumber { get { return 1125817; } } // skull longsword

        [Constructable]
        public SkullLongsword()
        {
            ItemID = 41793;
        }

        public Longsword(Serial serial)
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

    [FlipableAttribute(41797, 41798)]
    public class GargishSkullLongsword : Longsword
    {
        public override void LabelNumber { get { return 1125821; } } // gargish skull longsword
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public GargishSkullLongsword()
        {
            ItemID = 41797;
        }

        public Longsword(Serial serial)
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
