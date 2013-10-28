using System;

namespace Server.Items
{
    [Furniture]
    [Flipable(0x2D07, 0x2D08)]
    public class FancyElvenArmoire : BaseContainer
    {
        [Constructable]
        public FancyElvenArmoire()
            : base(0x2D07)
        {
            this.Weight = 1.0;
        }

        public FancyElvenArmoire(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultGumpID
        {
            get
            {
                return 0x4E;
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
                return 1031527;
            }
        }// fancy elven armoire
        public override Rectangle2D Bounds
        {
            get
            {
                return new Rectangle2D(30, 30, 90, 150);
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