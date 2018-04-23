using System;

namespace Server.Items
{
    [Flipable(0x4CE3, 0x4C3F)]	
    public class Cello : Item
    {
        public Cello()
            : base(0x4CE3)
        {
            this.Weight = 35.0;
        }

        public Cello(Serial serial)
            : base(serial)
        {
        }

		public override void OnDoubleClick(Mobile from)
		{
			from.PlaySound( 0x582 );
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

            if (this.Weight == 3.0)
                this.Weight = 35.0;
        }
    }
}