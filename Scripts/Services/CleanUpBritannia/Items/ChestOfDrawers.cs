using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    [Furniture]
    [Flipable(0x0A2C, 0x0A34)]
    public class ChestOfDrawers : LockableContainer
    {
        [Constructable]
        public ChestOfDrawers()
            : base(0x0A2C)
        {
            this.Weight = 25.0;
        }

        public ChestOfDrawers(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultGumpID
        {
            get
            {
                return 0x51;
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
                return 1022604;
            }
        }// chest of drawers

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