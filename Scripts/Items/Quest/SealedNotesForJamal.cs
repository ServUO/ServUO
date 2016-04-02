using System;

namespace Server.Items
{
    public class SealedNotesForJamal : Item
    {
        [Constructable]
        public SealedNotesForJamal()
            : base(0xEF9)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 5;
        }

        public SealedNotesForJamal(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074998;
            }
        }// sealed notes for Jamal
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