using System;
using Server;

namespace Server.Items
{
	public class LetterOfIntroduction : Item
	{
        public override int LabelNumber { get { return 1113243; } } // Laifem's Letter of Introduction
				
		[Constructable]
        public LetterOfIntroduction()
            : base(0xEC0)
		{
			LootType = LootType.Blessed;
			Weight = 1.0;
		}
		
		public LetterOfIntroduction( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

    public class MasteringWeaving : Item
    {
        public override int LabelNumber { get { return 1113244; } } // Mastering the Art of Weaving

        [Constructable]
        public MasteringWeaving()
            : base(0x0FF0)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
        }

        public MasteringWeaving(Serial serial)
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
