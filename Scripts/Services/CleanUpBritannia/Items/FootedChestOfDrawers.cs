using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    [Furniture]
    [Flipable(0x0A30, 0x0A38)]
    public class FootedChestOfDrawers : LockableContainer
    {
        [Constructable]
        public FootedChestOfDrawers()
            : base(0x0A30)
        {
            this.Weight = 25.0;
        }

        public FootedChestOfDrawers(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultGumpID
        {
            get
            {
                return 0x48;
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
                return 1151221;
            }
        }// footed chest of drawers

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