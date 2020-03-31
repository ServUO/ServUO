using System;

namespace Server.Items
{
    public class NecromancerSpellbook : Spellbook
    {
        [Constructable]
        public NecromancerSpellbook()
            : this((ulong)0)
        {
        }

        [Constructable]
        public NecromancerSpellbook(ulong content)
            : base(content, 0x2253)
        {
            this.Layer = Layer.OneHanded;
        }

        public NecromancerSpellbook(Serial serial)
            : base(serial)
        {
        }

        public override SpellbookType SpellbookType
        {
            get
            {
                return SpellbookType.Necromancer;
            }
        }
        public override int BookOffset
        {
            get
            {
                return 100;
            }
        }
        public override int BookCount
        {
            get
            {
                return 17;
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

    public class CompleteNecromancerSpellbook : NecromancerSpellbook
    {
        [Constructable]
        public CompleteNecromancerSpellbook()
            : base((ulong)0x1FFFF)
        {
        }

        public CompleteNecromancerSpellbook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}
