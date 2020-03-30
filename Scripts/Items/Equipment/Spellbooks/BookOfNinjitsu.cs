using System;

namespace Server.Items
{
    public class BookOfNinjitsu : Spellbook
    {
        [Constructable]
        public BookOfNinjitsu()
            : this((ulong)0xFF)
        {
        }

        [Constructable]
        public BookOfNinjitsu(ulong content)
            : base(content, 0x23A0)
        {
            Layer = Layer.OneHanded;
        }

        public BookOfNinjitsu(Serial serial)
            : base(serial)
        {
        }

        public override SpellbookType SpellbookType
        {
            get
            {
                return SpellbookType.Ninja;
            }
        }
        public override int BookOffset
        {
            get
            {
                return 500;
            }
        }
        public override int BookCount
        {
            get
            {
                return 8;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
