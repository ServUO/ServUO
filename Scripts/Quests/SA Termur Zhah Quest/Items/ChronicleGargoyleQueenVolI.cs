
using System;
using Server;
using Server.Targeting;
using Server.Misc;
using Server.Mobiles;
using Server.Items;

namespace Server.Items
{
    public class ChronicleGargoyleQueenVolI : Item
    {

        public override int LabelNumber { get { return 1150914; } } //Chronicle of the Gargoyle Queen Vol. I

        [Constructable]
        public ChronicleGargoyleQueenVolI()
            : base(0xFF2)
        {
            Weight = 1.0;
        }
        public ChronicleGargoyleQueenVolI(Serial serial)
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
