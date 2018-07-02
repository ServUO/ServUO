using System;

namespace Server.Items
{
    public class LibrariansKey : PeerlessKey
    {
        [Constructable]
        public LibrariansKey()
            : base(0xFF3)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public LibrariansKey(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074347;
            }
        }// librarian's key
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
