using System;
using Server;

namespace Server.Items
{
	public class LetterOfIntroduction : Item
	{
        public override int LabelNumber { get { return 1113243; } } // Laifem's Letter of Introduction
				
		[Constructable]
        public LetterOfIntroduction()
            : base(0x1F23)
		{
            this.Hue = 1167;
			this.Weight = 2.0;
            this.QuestItem = true;
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
            : base(0x1E20)
        {
            this.Hue = 744;
            this.Weight = 2.0;
            this.QuestItem = true;
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
