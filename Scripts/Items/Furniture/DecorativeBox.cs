using System;

namespace Server.Items
{
    [Furniture]
    [Flipable(0x2DF3, 0x2DF4)]
    public class DecorativeBox : LockableContainer
    {
        [Constructable]
        public DecorativeBox()
            : base(0x2DF3)
        {
            this.Weight = 1.0;
        }

        public DecorativeBox(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultGumpID
        {
            get
            {
                return 0x43;
            }
        }
        public override int DefaultDropSound
        {
            get
            {
                return 0x42;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1073403;
            }
        }// decorative box
        public override Rectangle2D Bounds
        {
            get
            {
                return new Rectangle2D(16, 51, 168, 73);
            }
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