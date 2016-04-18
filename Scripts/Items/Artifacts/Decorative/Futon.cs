using System;

namespace Server.Items
{
    [Flipable]
    public class Futon : Item
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public Futon()
            : base(Utility.RandomDouble() > 0.5 ? 0x295C : 0x295E)
        {
        }

        public Futon(Serial serial)
            : base(serial)
        {
        }

        public void Flip()
        {
            switch ( this.ItemID )
            {
                case 0x295C:
                    this.ItemID = 0x295D;
                    break;
                case 0x295E:
                    this.ItemID = 0x295F;
                    break;
                case 0x295D:
                    this.ItemID = 0x295C;
                    break;
                case 0x295F:
                    this.ItemID = 0x295E;
                    break;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}